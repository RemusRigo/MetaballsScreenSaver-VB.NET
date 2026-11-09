'--------------------------------------------------------------------------------------------------
' MetaballsSS: LavaField.vb
'   © 2026 Remus Rigo
'       v1.0.20261109
'--------------------------------------------------------------------------------------------------

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

''' <summary>
''' Owns the blobs and turns their positions into a colorized metaball field
''' each frame. The field is computed on a coarse grid (a handful of screen
''' pixels per cell) and then stretched to full size with bicubic
''' interpolation - that upscaling blur is what gives the wax its soft,
''' melting edges instead of hard circles.
'''
''' TUTORIAL: this is the heart of the whole program. Everything else
''' (the form, the timer, the config dialog) exists just to call Update()
''' and Render() on an instance of this class every frame. If you only
''' read one file to understand "how does the lava lamp actually work",
''' read this one. See TUTORIAL.md in the project root for the full
''' walkthrough with diagrams; the comments below are the short version
''' inline with the code they explain.
''' </summary>
Public Class LavaField

   ' How many real screen pixels each low-res grid cell stands for. We never
   ' compute the metaball field at full screen resolution - that would mean
   ' redoing the math below for every one of a few million pixels, 30 times
   ' a second. Instead we compute it on a grid that's CellSize times smaller
   ' in each dimension (so 36x fewer cells than pixels), then let the GPU/GDI+
   ' image scaler blow it back up to full size. Bigger CellSize = faster but
   ' blockier; smaller = smoother but slower. 6 is a sweet spot for 1080p-ish
   ' screens at 30fps on the CPU.
   Private Const CellSize As Integer = 6

   Private ReadOnly _blobs As Blob()
   Private ReadOnly _palette As Color()
   Private ReadOnly _rand As New Random()

   Private _gridWidth As Integer
   Private _gridHeight As Integer
   Private _screenWidth As Integer
   Private _screenHeight As Integer
   Private _fieldValues As Single()
   Private _lowResBitmap As Bitmap
   Private ReadOnly _speedMultiplier As Double
   Private ReadOnly _radiusMultiplier As Double
   Private ReadOnly _glowIntensity As Double

   '-----------------------------------------------------------------------------------------------
   ' Constructor: Creates a LavaField with the given screen size, blob count, and palette.
   Public Sub New(screenWidth As Integer, screenHeight As Integer, blobCount As Integer, palette As Color(),
                   Optional speedMultiplier As Double = 1.0,
                   Optional radiusMultiplier As Double = 1.0,
                   Optional glowIntensity As Double = 1.0)
      _palette = palette
      _speedMultiplier = speedMultiplier
      _radiusMultiplier = radiusMultiplier
      _glowIntensity = glowIntensity
      Dim count = Math.Max(2, Math.Min(3, blobCount))
      _blobs = New Blob(count - 1) {}

      Resize(screenWidth, screenHeight)
      InitBlobs(count)
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' InitBlobs: Creates the given number of blobs at random positions and radii.
   Private Sub InitBlobs(blobCount As Integer)
      For i As Integer = 0 To blobCount - 1
         Dim radius = _screenHeight * (0.16 + _rand.NextDouble() * 0.09) * _radiusMultiplier
         Dim x = _rand.NextDouble() * _screenWidth
         Dim y = _rand.NextDouble() * _screenHeight
         _blobs(i) = New Blob(x, y, radius, _screenWidth, _screenHeight, _rand, _speedMultiplier)
      Next
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Resize: Updates the screen size and rebuilds the low-res grid and bitmap.
   Public Sub Resize(screenWidth As Integer, screenHeight As Integer)
      _screenWidth = Math.Max(screenWidth, 1)
      _screenHeight = Math.Max(screenHeight, 1)
      _gridWidth = Math.Max(_screenWidth \ CellSize, 2)
      _gridHeight = Math.Max(_screenHeight \ CellSize, 2)
      _fieldValues = New Single(_gridWidth * _gridHeight - 1) {}

      If _lowResBitmap IsNot Nothing Then
         _lowResBitmap.Dispose()
      End If
      _lowResBitmap = New Bitmap(_gridWidth, _gridHeight, PixelFormat.Format32bppArgb)

      If _blobs IsNot Nothing Then
         For Each b In _blobs
            If b IsNot Nothing Then
               b.UpdateBounds(_screenWidth, _screenHeight)
            End If
         Next
      End If
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Update: Advances all blobs by the given elapsed time in seconds.
   Public Sub Update(dt As Double)
      For Each b In _blobs
         b.Update(dt)
      Next
   End Sub

   ''' <summary>
   ''' Draws one frame: recompute the field, colorize it into the low-res bitmap, then let GDI+ stretch that bitmap up to fill the real window/screen.
   ''' Called from frmSS.OnPaint every time the form repaints (which we force every tick - see frmSS.OnTick).
   ''' </summary>
   '-----------------------------------------------------------------------------------------------
   ' Render: Draws the current frame into the given Graphics surface, scaling to the target size.
   Public Sub Render(g As Graphics, targetSize As Size)
      If targetSize.Width <= 0 OrElse targetSize.Height <= 0 Then Return

      ComputeField()
      PaintLowResBitmap()

      ' HighQualityBicubic is what turns the blocky low-res grid into smooth
      ' melting blobs: instead of just enlarging each grid cell into a flat
      ' colored square (which is what InterpolationMode.NearestNeighbor would
      ' do), bicubic sampling blends each output pixel from a neighborhood of
      ' surrounding low-res pixels, producing soft gradients between cells.
      ' This blur is doing a lot of the "wax" look - try switching this to
      ' NearestNeighbor for a second to see the raw grid and appreciate it.
      g.InterpolationMode = InterpolationMode.HighQualityBicubic
      g.PixelOffsetMode = PixelOffsetMode.Half
      g.SmoothingMode = SmoothingMode.None
      g.DrawImage(_lowResBitmap, New Rectangle(0, 0, targetSize.Width, targetSize.Height))
   End Sub

   ''' <summary>
   ''' This is the "metaball" or "plasma field" math. For every cell in the
   ''' low-res grid, we ask: "how much total influence do all the blobs have
   ''' at this point?" and store that single number. Rendering (below) then
   ''' turns that number into a color.
   '''
   ''' Each blob contributes influence = radius^2 / distance^2 - an inverse-
   ''' square falloff, the same shape as how light or gravity fades with
   ''' distance in real life. That means:
   '''   - Right on top of a blob's center, distance ~ 0, so influence is huge.
   '''   - Far away, influence drops toward 0 quickly (squared, not linear).
   '''   - A bigger blob (radius) has a proportionally bigger influence everywhere.
   ''' We SUM every blob's contribution at each point rather than taking the
   ''' max. That summing is what makes two nearby blobs visually merge into
   ''' one shape as they approach each other - their fields add up and the
   ''' combined value exceeds any single blob's contribution, so the "hot"
   ''' region between them fills in. This additive-field idea is exactly what
   ''' "metaballs" means in computer graphics.
   ''' </summary>
   '-----------------------------------------------------------------------------------------------
   ' ComputeField: Computes the metaball field values for each cell in the low-res grid.
   Private Sub ComputeField()
      Dim scaleX = _screenWidth / CDbl(_gridWidth)
      Dim scaleY = _screenHeight / CDbl(_gridHeight)

      For gy As Integer = 0 To _gridHeight - 1
         ' Center of this grid cell, converted back to screen-pixel coordinates
         ' so we can measure real distances to the blobs (which store their
         ' positions in screen pixels, not grid cells).
         Dim py = (gy + 0.5) * scaleY
         Dim rowOffset = gy * _gridWidth

         For gx As Integer = 0 To _gridWidth - 1
            Dim px = (gx + 0.5) * scaleX
            Dim total As Double = 0

            For Each b In _blobs
               Dim dx = px - b.X
               Dim dy = py - b.Y
               ' Pythagorean distance, squared (we never need the actual
               ' square root - working with distSq avoids a slow Math.Sqrt
               ' call per blob per cell, tens of thousands of times a frame).
               ' The "+ 1.0" is just there to stop a division by (near) zero
               ' when a grid point sits exactly on a blob's center.
               Dim distSq = dx * dx + dy * dy + 1.0
               total += (b.Radius * b.Radius) / distSq
            Next

            _fieldValues(rowOffset + gx) = CSng(total)
         Next
      Next
   End Sub

   ''' <summary>
   ''' Converts the raw field numbers computed above into actual pixel colors and writes them straight into the low-res bitmap's memory.
   ''' </summary>
   Private Sub PaintLowResBitmap()
      ' LockBits hands us a direct pointer (bmpData.Scan0) into the bitmap's
      ' raw pixel memory instead of making us call SetPixel() one pixel at a
      ' time (SetPixel is notoriously slow - each call has overhead to look
      ' up the pixel format, do bounds checks, etc). We build a plain byte
      ' array ourselves and then Marshal.Copy the whole thing into that
      ' memory in one shot, which is dramatically faster for a full-frame
      ' update like this.
      Dim bmpData As BitmapData = _lowResBitmap.LockBits(New Rectangle(0, 0, _gridWidth, _gridHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

      Try
         ' "Stride" is the number of bytes from the start of one row of
         ' pixels to the start of the next. You might expect it to always
         ' equal gridWidth * 4 bytes-per-pixel, and it usually does here,
         ' but Windows is allowed to pad each row to a 4-byte boundary for
         ' performance reasons, so the safe/correct way to walk the buffer
         ' is always via bmpData.Stride, never a hand-computed width * 4.
         Dim stride = bmpData.Stride
         Dim bytes(stride * _gridHeight - 1) As Byte
         Dim paletteMax As Integer = _palette.Length - 1

         For gy As Integer = 0 To _gridHeight - 1
            Dim rowOffset = gy * _gridWidth
            Dim byteRow = gy * stride

            For gx As Integer = 0 To _gridWidth - 1
               ' _glowIntensity (from the config dialog) simply scales the
               ' raw field value before it's compressed - turning it up
               ' makes more of the screen read as "hot" (near the palette's
               ' bright end) without changing the blobs' actual size/shape.
               Dim v = _fieldValues(rowOffset + gx) * CSng(_glowIntensity)

               ' The raw field value (v) is unbounded - right at a blob's
               ' center it can be huge, and it trails off to 0 far away.
               ' We need a number in [0, 1] to pick a palette entry. The
               ' naive approach would be to clamp v directly, but that
               ' makes every "hot" pixel slam into pure white with a harsh
               ' edge. Instead v / (v + 1) is a curve that smoothly
               ' approaches 1 as v grows (0 -> 0, 1 -> 0.5, 9 -> 0.9, 99 ->
               ' 0.99, ...) without ever quite reaching it - this is the
               ' same shape as a camera's tone-mapping curve, and it's why
               ' blob cores glow instead of flat-out clip to white.
               Dim intensity As Single = v / (v + 1.0F)
               If intensity < 0.0F Then intensity = 0.0F
               If intensity > 1.0F Then intensity = 1.0F

               ' The palette (built once in ColorPalette.vb) is a 256-color
               ' gradient lookup table, dark-to-bright. intensity in [0,1]
               ' picks which rung of that gradient this pixel gets - this
               ' is exactly how a thermal/heatmap image turns "temperature"
               ' into color, just with a custom color ramp instead of the
               ' usual blue-to-red one.
               Dim idx As Integer = CInt(intensity * paletteMax)
               Dim c = _palette(idx)

               ' Pixel format is 32bppArgb, but in memory the byte order is B, G, R, A (little-endian) rather than the "ARGB" the name
               ' suggests - a classic GDI+ gotcha. Get this order wrong and you'll get a very blue (or very red) screensaver instead
               ' of a crash, which makes the bug easy to miss.
               Dim bi = byteRow + gx * 4
               bytes(bi + 0) = c.B
               bytes(bi + 1) = c.G
               bytes(bi + 2) = c.R
               bytes(bi + 3) = 255 ' fully opaque
            Next
         Next

         Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length)
      Finally
         ' Always unlock, even if something above throws - otherwise the bitmap stays locked and every future LockBits/DrawImage call on it fails.
         ' That's the whole reason this is wrapped in Try/Finally.
         _lowResBitmap.UnlockBits(bmpData)
      End Try
    End Sub

End Class
