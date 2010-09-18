Friend Class App

    Friend Shared Sub Main()
        QTextAux.App.Main()


        QTextAux.App.Form = New MainForm()

        AddHandler Medo.Application.SingleInstance.NewInstanceDetected, AddressOf NewInstanceDetected
        Medo.Application.SingleInstance.Attach()


        Try
            QTextAux.Helper.Path.CreatePath(QTextAux.Settings.FilesLocation)
        Catch ex As Exception
            Select Case Global.Medo.MessageBox.ShowQuestion(Nothing, ex.Message + Environment.NewLine + "Do you wish to try using default location instead?", MessageBoxButtons.YesNo)
                Case DialogResult.Yes
                    Try
                        Dim defaultPath As String = QTextAux.Settings.DefaultFilesLocation
                        QTextAux.Helper.Path.CreatePath(defaultPath)
                        QTextAux.Settings.FilesLocation = defaultPath
                    Catch ex2 As Exception
                        Global.Medo.MessageBox.ShowError(Nothing, ex2.Message, MessageBoxButtons.OK)
                        Application.Exit()
                        End
                    End Try
                Case DialogResult.No
            End Select
        End Try

        QTextAux.App.Tray = New QTextAux.Tray(QTextAux.App.Form)
        If (Settings.StartupShow = False) Then
            QTextAux.App.Tray.Show()
        End If

        AddHandler QTextAux.App.Hotkey.HotkeyActivated, AddressOf Hotkey_HotkeyActivated
        If (Settings.ActivationHotkey <> Keys.None) Then
            Try
                QTextAux.App.Hotkey.Register(Settings.ActivationHotkey)
            Catch ex As InvalidOperationException
                Medo.MessageBox.ShowWarning(Nothing, "Hotkey is already in use.")
            End Try
        End If


        If (Settings.StartupShow) Then QTextAux.App.Form.Show()



        Application.Run()


        System.GC.KeepAlive(QTextAux.App.SetupMutex)
    End Sub


    Private Shared Sub NewInstanceDetected(ByVal sender As Object, ByVal e As Global.Medo.Application.NewInstanceEventArgs)
        Try
            QTextAux.App.Form.CreateControl()
            QTextAux.App.Form.Handle.GetType()

            Dim method As New NewInstanceDetectedProcDelegate(AddressOf NewInstanceDetectedProc)
            QTextAux.App.Form.Invoke(method)
        Catch ex As Exception
        End Try
    End Sub


    Private Delegate Sub NewInstanceDetectedProcDelegate()

    Private Shared Sub NewInstanceDetectedProc()
        If (Settings.StartupShow = False) Then
            QTextAux.App.Tray.Show()
        End If

        If (QTextAux.App.Form.WindowState = FormWindowState.Minimized) Then QTextAux.App.Form.WindowState = FormWindowState.Normal
        QTextAux.App.Form.Show()
        QTextAux.App.Form.Activate()
    End Sub

    Private Shared Sub Hotkey_HotkeyActivated(ByVal sender As Object, ByVal e As System.EventArgs)
        If (Settings.StartupShow = False) Then
            QTextAux.App.Tray.Show()
        End If

        If (QTextAux.App.Form.WindowState = FormWindowState.Minimized) Then QTextAux.App.Form.WindowState = FormWindowState.Normal
        QTextAux.App.Form.Show()
        QTextAux.App.Form.Activate()
    End Sub


    Private NotInheritable Class NativeMethods
        Private Sub New()
        End Sub

        <System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint:="AllowSetForegroundWindow")> _
        Public Shared Function AllowSetForegroundWindow(ByVal dwProcessId As UInteger) As <System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)> Boolean
        End Function

    End Class

End Class