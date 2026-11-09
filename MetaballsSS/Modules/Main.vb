'--------------------------------------------------------------------------------------------------
' MetaballsSS: Main.vb: Main()
'   © 2026 Remus Rigo
'       v1.0.20261109
'--------------------------------------------------------------------------------------------------

''' <summary>
''' Standard Windows screensaver command-line contract:
'''   /c or /c:hwnd   -> show the settings dialog
'''   /p hwnd (or /p:hwnd) -> render embedded in the picker's preview thumbnail
'''   /s              -> run full-screen across every monitor
'''   (no arguments)  -> also runs full-screen, so double-clicking the exe while developing shows the effect immediately
''' </summary>
Module Main

   <STAThread>
   Sub Main(args As String())
#If Not NETFRAMEWORK Then
      Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)
#End If
      Application.EnableVisualStyles()
      Application.SetCompatibleTextRenderingDefault(False)

      Dim mode As String = "/s" ' default: start screensaver
      If args.Length > 0 AndAlso args(0).Length >= 2 Then
         mode = args(0).Substring(0, 2).ToLower()
      End If

      Select Case mode
         Case "/s" : RunScreensaver()
         Case "/c" : ShowConfig()
         Case "/p"
            Dim h As IntPtr = ParsePreviewHandle(args)
            If h <> IntPtr.Zero Then RunPreview(h)
      End Select
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' ParsePreviewHandle: Extracts the preview window handle from the command-line arguments.
   Private Function ParsePreviewHandle(args As String()) As IntPtr
      Dim arg0 As String = args(0).Trim()
      Dim handleText As String = ""

      Dim colonIndex As Integer = arg0.IndexOf(":"c)
      If colonIndex >= 0 Then
         handleText = arg0.Substring(colonIndex + 1)
      ElseIf args.Length > 1 Then
         handleText = args(1)
      End If

      Dim handleValue As Long
      If Long.TryParse(handleText, handleValue) Then Return New IntPtr(handleValue)
      Return IntPtr.Zero
   End Function

   '-----------------------------------------------------------------------------------------------
   ' RunScreensaver: Creates a frmSS for each monitor and runs the application message loop.
   Private Sub RunScreensaver()
      For Each monitor As Screen In Screen.AllScreens
         Dim frm As New frmSS(monitor)
         AddHandler frm.ExitRequested, Sub() Application.Exit()
         frm.Show()
      Next
      Application.Run()
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' ShowConfig: Creates a frmCfg and runs the application message loop.
   Private Sub ShowConfig()
      Application.Run(New frmCfg())
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' RunPreview: Creates a frmSS for the given preview window handle and runs the application message loop.
   Private Sub RunPreview(previewHandle As IntPtr)
      Application.Run(New frmSS(previewHandle))
   End Sub

End Module
