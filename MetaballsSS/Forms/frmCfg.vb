Imports System
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' The dialog shown for the /c switch. Lets the user pick blue vs. red,
''' 2 vs. 3 blobs, and tune speed / radius / glow intensity, with a small
''' live preview. Settings are written to the registry via AppSettings only
''' when the user clicks OK; Cancel (or closing the window) discards changes.
''' </summary>
Public Class frmCfg

   Private _previewField As LavaField
   Private ReadOnly _previewTimer As New Timer()

   Public Sub New()
      InitializeComponent()

      Me.Text = "Plasma Lava Lamp Settings"
      Me.FormBorderStyle = FormBorderStyle.FixedDialog
      Me.MaximizeBox = False
      Me.MinimizeBox = False
      Me.ShowInTaskbar = True
      Me.StartPosition = FormStartPosition.CenterScreen

      cbBlobs.Items.AddRange({"2", "3"})

      Me.AcceptButton = btnOk
      Me.CancelButton = btnCancel

      AppSettings.LoadSettings()
      rBtnBlue.Checked = (AppSettings.PaletteKind = ColorPalette.PaletteKind.Blue)
      rBtnRed.Checked = (AppSettings.PaletteKind = ColorPalette.PaletteKind.Red)
      cbBlobs.SelectedItem = AppSettings.BlobCount.ToString()
      tbSpeed.Value = ClampToRange(CInt(Math.Round(AppSettings.SpeedMultiplier * 100)), tbSpeed.Minimum, tbSpeed.Maximum)
      tbRadius.Value = ClampToRange(CInt(Math.Round(AppSettings.RadiusMultiplier * 100)), tbRadius.Minimum, tbRadius.Maximum)
      tbGlowIntensity.Value = ClampToRange(CInt(Math.Round(AppSettings.GlowIntensity * 100)), tbGlowIntensity.Minimum, tbGlowIntensity.Maximum)
      UpdateSliderLabels()

      _previewTimer.Interval = 66
      AddHandler _previewTimer.Tick, AddressOf OnPreviewTick
   End Sub

   Private Shared Function ClampToRange(value As Integer, minValue As Integer, maxValue As Integer) As Integer
      Return Math.Min(maxValue, Math.Max(minValue, value))
   End Function

   Private Sub OnDialogShown(sender As Object, e As EventArgs) Handles Me.Shown
      RebuildPreview()
      _previewTimer.Start()
   End Sub

   Private Sub RebuildPreview()
      Dim kind = If(rBtnRed.Checked, ColorPalette.PaletteKind.Red, ColorPalette.PaletteKind.Blue)
      Dim palette = ColorPalette.BuildPalette(kind)
      Dim count = If(cbBlobs.SelectedItem IsNot Nothing, Integer.Parse(CStr(cbBlobs.SelectedItem)), 3)
      _previewField = New LavaField(pnlPreview.Width, pnlPreview.Height, count, palette,
                                       SpeedMultiplier, RadiusMultiplier, GlowIntensity)
   End Sub

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

   Private Sub UpdateSliderLabels()
      lblSpeed.Text = $"Speed: {SpeedMultiplier:F2}x"
      lblRadius.Text = $"Blob radius: {RadiusMultiplier:F2}x"
      lblGlowIntensity.Text = $"Glow intensity: {GlowIntensity:F2}x"
   End Sub

   Private Sub OnSettingChanged(sender As Object, e As EventArgs) Handles rBtnBlue.CheckedChanged, rBtnRed.CheckedChanged, cbBlobs.SelectedIndexChanged, tbSpeed.ValueChanged, tbRadius.ValueChanged, tbGlowIntensity.ValueChanged
      UpdateSliderLabels()
      If Me.Visible Then
         RebuildPreview()
      End If
   End Sub

   ' The dialog is shown via Application.Run (not ShowDialog), so a button's
   ' DialogResult property alone won't close it — close explicitly.
   Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
      Me.DialogResult = DialogResult.OK
      Me.Close()
   End Sub

   Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
      Me.DialogResult = DialogResult.Cancel
      Me.Close()
   End Sub

   Private Sub OnPreviewTick(sender As Object, e As EventArgs)
      If _previewField IsNot Nothing Then
         _previewField.Update(0.066)
         pnlPreview.Invalidate()
      End If
   End Sub

   Private Sub OnPreviewPaint(sender As Object, e As PaintEventArgs) Handles pnlPreview.Paint
      If _previewField IsNot Nothing Then
         _previewField.Render(e.Graphics, pnlPreview.ClientSize)
      End If
   End Sub

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

   Private Sub OnDialogClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
      _previewTimer.Stop()
      If Me.DialogResult = DialogResult.OK Then
         SaveSettings()
      End If
   End Sub

End Class
