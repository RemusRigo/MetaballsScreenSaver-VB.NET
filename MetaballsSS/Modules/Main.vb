Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

''' <summary>
''' Standard Windows screensaver command-line contract:
'''   /c or /c:hwnd   -> show the settings dialog
'''   /p hwnd (or /p:hwnd) -> render embedded in the picker's preview thumbnail
'''   /s              -> run full-screen across every monitor
'''   (no arguments)  -> also runs full-screen, so double-clicking the exe while
'''                      developing shows the effect immediately
''' </summary>
Module Main

    <STAThread>
    Sub Main(args As String())
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim previewHandle As IntPtr = IntPtr.Zero
        Dim mode As String = "screensaver"

        If args.Length > 0 Then
            Dim arg0 As String = args(0).Trim()

            If arg0.StartsWith("/c", StringComparison.OrdinalIgnoreCase) Then
                mode = "config"
            ElseIf arg0.StartsWith("/p", StringComparison.OrdinalIgnoreCase) Then
                mode = "preview"
                Dim handleText As String = ""
                Dim colonIndex = arg0.IndexOf(":"c)
                If colonIndex >= 0 Then
                    handleText = arg0.Substring(colonIndex + 1)
                ElseIf args.Length > 1 Then
                    handleText = args(1)
                End If

                Dim handleValue As Long
                If Long.TryParse(handleText, handleValue) Then
                    previewHandle = New IntPtr(handleValue)
                End If
            ElseIf arg0.StartsWith("/s", StringComparison.OrdinalIgnoreCase) Then
                mode = "screensaver"
            End If
        End If

        Select Case mode
            Case "config"
                Application.Run(New frmCfg())
            Case "preview"
                If previewHandle <> IntPtr.Zero Then
                    Application.Run(New frmSS(previewHandle))
                End If
            Case Else
                RunScreensaver()
        End Select
    End Sub

    Private Sub RunScreensaver()
        Dim context As New ApplicationContext()
        Dim forms As New List(Of frmSS)

        For Each scr As Screen In Screen.AllScreens
            Dim f As New frmSS(scr)
            AddHandler f.FormClosed, Sub(s, e)
                                          forms.Remove(f)
                                          If forms.Count = 0 Then
                                              context.ExitThread()
                                          End If
                                      End Sub
            forms.Add(f)
        Next

        For Each f In forms
            AddHandler f.ExitRequested, Sub()
                                             For Each other In forms.ToArray()
                                                 other.Close()
                                             Next
                                         End Sub
        Next

        For Each f In forms
            f.Show()
        Next

        Application.Run(context)
    End Sub

End Module
