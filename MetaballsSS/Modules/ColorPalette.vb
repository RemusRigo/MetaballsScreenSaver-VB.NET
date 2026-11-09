'--------------------------------------------------------------------------------------------------
' MetaballsSS: ColorPalette.vb
'   © 2026 Remus Rigo
'       v1.0.20261109
'--------------------------------------------------------------------------------------------------

''' <summary>
''' Builds 256-entry gradient lookup tables used to colorize the metaball
''' field. Field intensity in [0,1] maps directly to an index here.
'''
''' TUTORIAL: this is a "color ramp" or "LUT" (lookup table) - a very common
''' graphics trick. Rather than computing a color formula for every pixel
''' every frame, you precompute 256 colors ONCE (at startup) that represent
''' "coldest" to "hottest", and then each frame you just look one up by
''' index. LavaField.PaintLowResBitmap does exactly that: it turns a field
''' intensity into an index 0-255 and reads palette(index). This is the same
''' idea behind indexed/palette image formats (like old 256-color GIFs) and
''' behind heatmap/thermal-camera colorings.
''' </summary>
Public Module ColorPalette

   Public Enum PaletteKind
      Blue
      Red
   End Enum

   Public Function BuildPalette(kind As PaletteKind) As Color()
      Select Case kind
         Case PaletteKind.Red
            Return BuildRedPalette()
         Case Else
            Return BuildBluePalette()
      End Select
   End Function

   Public Function BuildBluePalette() As Color()
      Dim stops As (Position As Single, Tint As Color)() = {
            (0.0F, Color.FromArgb(255, 2, 4, 12)),
            (0.35F, Color.FromArgb(255, 8, 20, 60)),
            (0.55F, Color.FromArgb(255, 20, 60, 140)),
            (0.75F, Color.FromArgb(255, 40, 130, 220)),
            (0.9F, Color.FromArgb(255, 120, 210, 250)),
            (1.0F, Color.FromArgb(255, 235, 250, 255))
        }
      Return BuildGradient(stops)
   End Function

   Public Function BuildRedPalette() As Color()
      Dim stops As (Position As Single, Tint As Color)() = {
         (0.0F, Color.FromArgb(255, 6, 2, 2)),
         (0.35F, Color.FromArgb(255, 50, 8, 8)),
         (0.55F, Color.FromArgb(255, 150, 20, 10)),
         (0.75F, Color.FromArgb(255, 230, 90, 15)),
         (0.9F, Color.FromArgb(255, 250, 170, 40)),
         (1.0F, Color.FromArgb(255, 255, 250, 220))
      }
      Return BuildGradient(stops)
   End Function

   ''' <summary>
   ''' Turns a handful of "stops" (a position 0-1 and a color at that
   ''' position, e.g. "at 35% along the ramp, use this shade of blue") into
   ''' a full smooth 256-color gradient by linearly interpolating between
   ''' each pair of neighboring stops. This is the exact same concept as a
   ''' gradient tool in Photoshop/GIMP/CSS `linear-gradient` - you place a
   ''' few color stops and the software fills in everything between them.
   ''' </summary>
   Private Function BuildGradient(stops As (Position As Single, Tint As Color)()) As Color()
      Dim palette(255) As Color

      For i As Integer = 0 To 255
         Dim t As Single = i / 255.0F ' this entry's position along the ramp, 0..1
         Dim lower = stops(0)
         Dim upper = stops(stops.Length - 1)

         ' Find which pair of stops "t" falls between, e.g. for t = 0.6 with
         ' stops at 0.55 and 0.75, lower=0.55's color and upper=0.75's color.
         For s As Integer = 0 To stops.Length - 2
            If t >= stops(s).Position AndAlso t <= stops(s + 1).Position Then
               lower = stops(s)
               upper = stops(s + 1)
               Exit For
            End If
         Next

         ' Re-express t as a fraction of the way *between those two specific
         ' stops* (not the whole 0-1 ramp) so LerpColor blends correctly
         ' regardless of how far apart the stops are.
         Dim span As Single = upper.Position - lower.Position
         Dim localT As Single = If(span > 0, (t - lower.Position) / span, 0.0F)
         palette(i) = LerpColor(lower.Tint, upper.Tint, localT)
      Next

      Return palette
   End Function

   ''' <summary>
   ''' Linear interpolation ("lerp") between two colors: blend each of the
   ''' R, G, B channels independently by the same fraction t. t=0 gives
   ''' color a, t=1 gives color b, t=0.5 gives the color exactly halfway
   ''' between them. This is one of the most-used building blocks in all of
   ''' graphics programming - worth committing to memory: result = a + (b -
   ''' a) * t.
   ''' </summary>
   Private Function LerpColor(a As Color, b As Color, t As Single) As Color
      Dim r = Clamp(CInt(a.R + (b.R - a.R) * t))
      Dim g = Clamp(CInt(a.G + (b.G - a.G) * t))
      Dim bl = Clamp(CInt(a.B + (b.B - a.B) * t))
      Return Color.FromArgb(255, r, g, bl)
   End Function

   Private Function Clamp(v As Integer) As Integer
      Return Math.Min(255, Math.Max(0, v))
   End Function

End Module
