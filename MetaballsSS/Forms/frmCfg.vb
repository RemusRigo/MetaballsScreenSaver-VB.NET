'--------------------------------------------------------------------------------------------------
' MetaballsSS: frmCfg.vb
'   © 2026 Remus Rigo
'       v1.0.20261109
'--------------------------------------------------------------------------------------------------

Imports MetaballsSS.API

Public Class frmCfg

   Private Const SYSMENU_ABOUT_ID As UInteger = 1000
   Private _previewField As LavaField
   Private ReadOnly _previewTimer As New Timer() 'Ready-only to avoid accidental reassignment; we want to keep the same timer instance for the lifetime of the dialog.
   Private ReadOnly Property SpeedMultiplier As Double
      Get
         Return tbSpeed.Value / 100.0
      End Get
   End Property

   Private ReadOnly Property RadiusMultiplier As Double
      Get
         Return tbRadius.Value / 100.0
      End Get
   End Property

   Private ReadOnly Property GlowIntensity As Double
      Get
         Return tbGlowIntensity.Value / 100.0
      End Get
   End Property

   '-----------------------------------------------------------------------------------------------
   ' create menu
   Protected Overrides Sub OnHandleCreated(e As EventArgs)
      MyBase.OnHandleCreated(e)
      Dim hSysMenu As IntPtr = GetSystemMenu(Me.Handle, False)
      ' Add a separator and then your custom item
      AppendMenu(hSysMenu, MF_SEPARATOR, 0, String.Empty)
      AppendMenu(hSysMenu, MF_STRING, SYSMENU_ABOUT_ID, "About...")
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' append menu to app system menu
   Protected Overrides Sub WndProc(ByRef m As Message)
      MyBase.WndProc(m)
      If m.Msg = WM_SYSCOMMAND Then
         If CUInt(m.WParam) = SYSMENU_ABOUT_ID Then
            Using frm As New frmAbout()
               frm.ShowDialog(Me) ' Passing Me makes frmCfg the dialog's owner, so the About box centers on it and stays on top of it
            End Using
         End If
      End If
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Constructor and initialization
   Public Sub New()
      InitializeComponent()

      'Me.AcceptButton = btnOk
      'Me.CancelButton = btnCancel

      cbBlobs.Items.AddRange({"2", "3"})

      AppSettings.LoadSettings()
      rBtnBlue.Checked = (AppSettings.PaletteKind = ColorPalette.PaletteKind.Blue)
      rBtnRed.Checked = (AppSettings.PaletteKind = ColorPalette.PaletteKind.Red)
      cbBlobs.SelectedItem = AppSettings.BlobCount.ToString()
      tbSpeed.Value = KeepInRange(CInt(Math.Round(AppSettings.SpeedMultiplier * 100)), tbSpeed.Minimum, tbSpeed.Maximum)
      tbRadius.Value = KeepInRange(CInt(Math.Round(AppSettings.RadiusMultiplier * 100)), tbRadius.Minimum, tbRadius.Maximum)
      tbGlowIntensity.Value = KeepInRange(CInt(Math.Round(AppSettings.GlowIntensity * 100)), tbGlowIntensity.Minimum, tbGlowIntensity.Maximum)
      UpdateSliderLabels()

      AddHandler _previewTimer.Tick, AddressOf _previewTimer_Tick

      _previewTimer.Interval = 66
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' KeepInRange: Keeps a value within the given range, inclusive.
   ' If the value is less than minValue, returns minValue; if greater than maxValue, returns maxValue; otherwise returns the value itself.
   Private Shared Function KeepInRange(value As Integer, minValue As Integer, maxValue As Integer) As Integer
      Return Math.Min(maxValue, Math.Max(minValue, value))
   End Function


   '-----------------------------------------------------------------------------------------------
   ' RebuildPreview: Rebuilds the preview field based on the current settings.
   Private Sub RebuildPreview()
      Dim kind = If(rBtnRed.Checked, ColorPalette.PaletteKind.Red, ColorPalette.PaletteKind.Blue)
      Dim palette = ColorPalette.BuildPalette(kind)
      Dim count = If(cbBlobs.SelectedItem IsNot Nothing, Integer.Parse(CStr(cbBlobs.SelectedItem)), 3)
      _previewField = New LavaField(pnlPreview.Width, pnlPreview.Height, count, palette, SpeedMultiplier, RadiusMultiplier, GlowIntensity)
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' UpdateSliderLabels: Updates the labels next to the sliders to show the current values.
   Private Sub UpdateSliderLabels()
      lblSpeed.Text = $"Speed: {SpeedMultiplier:F2}x"
      lblRadius.Text = $"Blob radius: {RadiusMultiplier:F2}x"
      lblGlowIntensity.Text = $"Glow intensity: {GlowIntensity:F2}x"
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' OnPreviewTick: Called on each timer tick to update the preview field and invalidate the preview panel for repainting.
   Private Sub _previewTimer_Tick(sender As Object, e As EventArgs) 'Handles _previewTimer.Tick
      If _previewField IsNot Nothing Then
         _previewField.Update(0.066)
         pnlPreview.Invalidate()
      End If
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' SaveSettings
   ''' <summary>Copies the dialog's controls into AppSettings and writes them all to the registry.</summary>
   Private Sub SaveSettings()
      AppSettings.PaletteKind = If(rBtnRed.Checked, ColorPalette.PaletteKind.Red, ColorPalette.PaletteKind.Blue)
      If cbBlobs.SelectedItem IsNot Nothing Then
         AppSettings.BlobCount = Integer.Parse(CStr(cbBlobs.SelectedItem))
      End If
      AppSettings.SpeedMultiplier = SpeedMultiplier
      AppSettings.RadiusMultiplier = RadiusMultiplier
      AppSettings.GlowIntensity = GlowIntensity
      AppSettings.SaveSettings()
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' frmCfg: Shown
   Private Sub OnDialogShown(sender As Object, e As EventArgs) Handles Me.Shown
      RebuildPreview()
      _previewTimer.Start()
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' frmCfg: FormClosing: Stop the preview timer and save settings if the dialog result is OK.
   Private Sub frmCfg_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
      _previewTimer.Stop()
      If Me.DialogResult = DialogResult.OK Then
         SaveSettings()
      End If
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' OnSettingChanged: Called when any of the settings controls change. Updates the slider labels and rebuilds the preview if the dialog is visible.
   Private Sub frmCfg_OnSettingChanged(sender As Object, e As EventArgs) Handles rBtnBlue.CheckedChanged, rBtnRed.CheckedChanged, cbBlobs.SelectedIndexChanged, tbSpeed.ValueChanged, tbRadius.ValueChanged, tbGlowIntensity.ValueChanged
      UpdateSliderLabels()
      If Me.Visible Then
         RebuildPreview()
      End If
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' pnlPreview: Paint: Called when the preview panel needs to be repainted. Renders the preview field onto the panel.
   Private Sub pnlPreview_Paint(sender As Object, e As PaintEventArgs) Handles pnlPreview.Paint
      If _previewField IsNot Nothing Then
         _previewField.Render(e.Graphics, pnlPreview.ClientSize)
      End If
   End Sub

   ' The dialog is shown via Application.Run (not ShowDialog), so a button's DialogResult property alone won't close it — close explicitly.

   '-----------------------------------------------------------------------------------------------
   ' OK button: save settings and close the dialog.
   Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
      Me.DialogResult = DialogResult.OK
      Me.Close()
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Cancel button: just close the dialog without saving settings.
   Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
      Me.DialogResult = DialogResult.Cancel
      Me.Close()
   End Sub

End Class
