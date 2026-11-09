'--------------------------------------------------------------------------------------------------
' About form
'    © 2026 Remus Rigo
'       v1.1.20261025
'--------------------------------------------------------------------------------------------------
Public Class frmAbout

   Private Shared Function LoadImage(name As String) As Image
      Using res = GetType(frmAbout).Assembly.GetManifestResourceStream(name)
         Return New Bitmap(Image.FromStream(res))
      End Using
   End Function

   Private Sub frmAbout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
      lblTitle.Text = AppData.appName
      lblVer.Text = AppData.appVersion
      lnkLblGitHub.Text = AppData.appLink
      lnkLblGitHub.Links.Add(0, lnkLblGitHub.Text.Length, "https://github.com/RemusRigo/")

      imgPayPal.Image = LoadImage("PayPal.png")
      imgRevolut.Image = LoadImage("Revolut.png")
   End Sub

   Private Sub lnkLblGitHub_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkLblGitHub.LinkClicked
      Process.Start(New ProcessStartInfo("https://github.com/RemusRigo/") With {.UseShellExecute = True})
   End Sub

   Private Sub imgPayPal_Click(sender As Object, e As EventArgs) Handles imgPayPal.Click
      Process.Start(New ProcessStartInfo("https://paypal.me/remusrigo") With {.UseShellExecute = True})
   End Sub

   Private Sub imgRevolut_Click(sender As Object, e As EventArgs) Handles imgRevolut.Click
      Process.Start(New ProcessStartInfo("https://revolut.me/remusrigo") With {.UseShellExecute = True})
   End Sub


End Class