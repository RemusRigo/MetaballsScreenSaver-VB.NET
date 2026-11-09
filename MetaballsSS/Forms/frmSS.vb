'--------------------------------------------------------------------------------------------------
' MetaballsSS: frmSS.vb
'   © 2026 Remus Rigo
'       v1.0.20261109
'--------------------------------------------------------------------------------------------------

Imports System.Runtime.InteropServices
Imports MetaballsSS.API

''' <summary>
''' Renders the lava field full-screen on one monitor, or embedded inside the tiny thumbnail Windows gives us for /p preview mode.
''' Any key press, mouse click, or real mouse movement raises ExitRequested so Main.vb can tear every monitor's form down together.
''' </summary>
''' 
Partial Public Class frmSS

   Public Event ExitRequested As EventHandler

   Private ReadOnly _isPreview As Boolean
   Private ReadOnly _timer As New Timer()
   Private ReadOnly _stopwatch As New Stopwatch()
   Private _field As LavaField
   Private _lastTicks As Long
   Private _lastMousePos As Point
   Private _mouseInitialized As Boolean
   Private Const MouseMoveThreshold As Integer = 8

   Private Const GWL_STYLE As Integer = -16
   Private Const WS_CHILD As Integer = &H40000000
   Private Const WS_VISIBLE As Integer = &H10000000

   ''' <summary>Preview mode: embed as a child of the small dialog thumbnail.</summary>
   Public Sub New(previewHandle As IntPtr)
      InitializeComponent()
      _isPreview = True
      InitializeFormBase()

      Dim rect As New RECT
      GetClientRect(previewHandle, rect)
      Dim width = Math.Max(rect.Right - rect.Left, 1)
      Dim height = Math.Max(rect.Bottom - rect.Top, 1)

      Me.FormBorderStyle = FormBorderStyle.None
      Me.ClientSize = New Size(width, height)
      Me.Location = New Point(0, 0)

      Dim forceHandleCreation = Me.Handle ' ensure the window handle exists before reparenting
      SetParent(Me.Handle, previewHandle)
      SetWindowLong(Me.Handle, GWL_STYLE, WS_CHILD Or WS_VISIBLE)

      SetupField(width, height)

      _timer.Interval = 66 ' ~15 fps is plenty for a postage-stamp preview
      AddHandler _timer.Tick, AddressOf OnTick
      _timer.Start()
   End Sub

   ''' <summary>Screensaver mode: borderless, full-screen on the given monitor.</summary>
   Public Sub New(scr As Screen)
      InitializeComponent()
      _isPreview = False
      InitializeFormBase()

      Me.FormBorderStyle = FormBorderStyle.None
      Me.Bounds = scr.Bounds
      Me.TopMost = True
      Me.KeyPreview = True

      SetupField(scr.Bounds.Width, scr.Bounds.Height)

      _timer.Interval = 33 ' ~30 fps
      AddHandler _timer.Tick, AddressOf OnTick

      AddHandler Me.KeyDown, AddressOf OnAnyKey
      AddHandler Me.MouseMove, AddressOf OnAnyMouseMove
      AddHandler Me.MouseClick, AddressOf OnAnyMouseClick
      AddHandler Me.Shown, Sub()
                              Cursor.Hide()
                              _lastMousePos = Cursor.Position
                              _mouseInitialized = True
                              Me.Activate()
                           End Sub

      _timer.Start()
   End Sub

   Private Sub InitializeFormBase()
      ' These ControlStyles flags turn on "double buffering": instead of
      ' drawing straight to the visible window (which would flicker as each
      ' shape appears), Windows Forms draws the whole frame to an
      ' off-screen bitmap first and then copies that finished bitmap to the
      ' screen in one go. UserPaint says "I'll do my own drawing in
      ' OnPaint rather than relying on default control painting";
      ' AllPaintingInWmPaint avoids a redundant erase-background step;
      ' ResizeRedraw forces a repaint whenever the form is resized (relevant
      ' mainly for the config dialog's live preview panel).
      Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or
                   ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
      Me.BackColor = Color.Black
      Me.StartPosition = FormStartPosition.Manual
      Me.ShowInTaskbar = False
      Me.MaximizeBox = False
      Me.MinimizeBox = False
   End Sub

   Private Sub SetupField(width As Integer, height As Integer)
      AppSettings.LoadSettings()
      Dim palette = ColorPalette.BuildPalette(AppSettings.PaletteKind)
      _field = New LavaField(width, height, AppSettings.BlobCount, palette,
                             AppSettings.SpeedMultiplier, AppSettings.RadiusMultiplier,
                             AppSettings.GlowIntensity)
      _stopwatch.Start()
      _lastTicks = _stopwatch.ElapsedTicks
   End Sub

   ''' <summary>
   ''' Fires on every Timer tick (every 33ms / ~30fps in screensaver mode -
   ''' see the constructor). This is the whole "game loop": measure real
   ''' elapsed time, advance the simulation by that amount, then ask Windows
   ''' to repaint. Note this method does NOT draw anything itself - it just
   ''' updates blob positions and calls Invalidate(), which schedules
   ''' OnPaint to run soon after. Separating "update" from "paint" this way
   ''' is standard: it means resizing or a stray repaint request never
   ''' accidentally advances the simulation twice.
   ''' </summary>
   Private Sub OnTick(sender As Object, e As EventArgs)
      Dim now = _stopwatch.ElapsedTicks
      Dim dt = (now - _lastTicks) / CDbl(Stopwatch.Frequency)
      _lastTicks = now
      dt = Math.Min(dt, 0.1) ' guard against a huge jump after a stall

      _field.Update(dt)
      Me.Invalidate() ' marks the whole client area "dirty" -> Windows queues a WM_PAINT -> OnPaint runs
   End Sub

   ''' <summary>
   ''' Windows calls this whenever the form needs to (re)draw itself -
   ''' triggered here by our own Invalidate() call above, but it can also
   ''' fire on its own (e.g. the window was uncovered). e.Graphics is the
   ''' drawing surface for this frame; we just hand it straight to the field
   ''' to fill in.
   ''' </summary>
   Protected Overrides Sub OnPaint(e As PaintEventArgs)
      MyBase.OnPaint(e)
      If _field IsNot Nothing Then
         _field.Render(e.Graphics, Me.ClientSize)
      End If
   End Sub

   Protected Overrides Sub OnResize(e As EventArgs)
      MyBase.OnResize(e)
      If Not _isPreview AndAlso _field IsNot Nothing AndAlso Me.ClientSize.Width > 0 AndAlso Me.ClientSize.Height > 0 Then
         _field.Resize(Me.ClientSize.Width, Me.ClientSize.Height)
      End If
   End Sub

   Private Sub OnAnyKey(sender As Object, e As KeyEventArgs)
      RequestExit()
   End Sub

   Private Sub OnAnyMouseClick(sender As Object, e As MouseEventArgs)
      RequestExit()
   End Sub

   Private Sub OnAnyMouseMove(sender As Object, e As MouseEventArgs)
      If Not _mouseInitialized Then
         _lastMousePos = Cursor.Position
         _mouseInitialized = True
         Return
      End If

      Dim current = Cursor.Position
      Dim dx = Math.Abs(current.X - _lastMousePos.X)
      Dim dy = Math.Abs(current.Y - _lastMousePos.Y)
      If dx > MouseMoveThreshold OrElse dy > MouseMoveThreshold Then
         RequestExit()
      End If
   End Sub

   Private Sub RequestExit()
      If _isPreview Then Return
      RaiseEvent ExitRequested(Me, EventArgs.Empty)
   End Sub

   Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
      _timer.Stop()
      If Not _isPreview Then Cursor.Show()
      MyBase.OnFormClosed(e)
   End Sub

End Class
