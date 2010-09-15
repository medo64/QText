Friend Class App

    Friend Shared _form As MainForm

    Friend Shared Hotkey As New Medo.Windows.Forms.Hotkey
    Private Shared _setupMutex As System.Threading.Mutex

    Friend Shared Sub Main()
        _setupMutex = New System.Threading.Mutex(False, "Global\JosipMedved_QText")

        If (Not QTextAux.Settings.LegacySettingsCopied) Then
            Dim currProcess As Process = Process.GetCurrentProcess()
            Dim currProcessId As Integer = currProcess.Id
            Dim currProcessName As String = currProcess.ProcessName
            Dim currProcessFileName As String = currProcess.MainModule.FileName
            For Each iProcess As Process In Process.GetProcesses()
                Debug.WriteLine(iProcess.ProcessName)
                Try
                    If (iProcess.ProcessName = "QText") Then
                        'If (iProcess.Id <> currProcessId) AndAlso (iProcess.MainModule.FileName = currProcessFileName) Then
                        If (iProcess.Id <> currProcessId) Then
                            iProcess.Kill()
                        End If
                    End If
                Catch ex As System.ComponentModel.Win32Exception
                End Try
            Next
        End If


        System.Windows.Forms.Application.EnableVisualStyles()
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(False)
        System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(Global.Medo.Configuration.Settings.Read("CultureName", "en-US"))

        AddHandler Global.Medo.Application.UnhandledCatch.ThreadException, AddressOf UnhandledException
        Global.Medo.Application.UnhandledCatch.Attach()

        AddHandler System.Windows.Forms.Application.ApplicationExit, AddressOf ApplicationExit


        LegacySettingsCopy.CopyIfNeeded()

        _form = New MainForm()

        AddHandler Global.Medo.Application.SingleInstance.NewInstanceDetected, AddressOf NewInstanceDetected
        Global.Medo.Application.SingleInstance.Attach()


        Try
            QTextAux.Helper.Path.Create(QTextAux.Settings.FilesLocation)
        Catch ex As Exception
            Select Case Global.Medo.MessageBox.ShowQuestion(Nothing, ex.Message + Environment.NewLine + "Do you wish to try using default location instead?", MessageBoxButtons.YesNo)
                Case DialogResult.Yes
                    Try
                        Dim defaultPath As String = Settings.DefaultFilesLocation
                        QTextAux.Helper.Path.Create(defaultPath)
                        QTextAux.Settings.FilesLocation = defaultPath
                    Catch ex2 As Exception
                        Global.Medo.MessageBox.ShowError(Nothing, ex2.Message, MessageBoxButtons.OK)
                        Application.Exit()
                        End
                    End Try
                Case DialogResult.No
            End Select
        End Try

        Tray.Init()
        If (Settings.StartupShow = False) Then
            Tray.Show()
        End If

        AddHandler Hotkey.HotkeyActivated, AddressOf Hotkey_HotkeyActivated
        If (Settings.ActivationHotkey <> Keys.None) Then
            Try
                App.Hotkey.Register(Settings.ActivationHotkey)
            Catch ex As InvalidOperationException
                Medo.MessageBox.ShowWarning(Nothing, "Hotkey is already in use.")
            End Try
        End If


        If (Settings.StartupShow) Then App._form.Show()



        Application.Run()


        System.GC.KeepAlive(_setupMutex)
    End Sub


    Private Shared Sub UnhandledException(ByVal sender As Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
        Global.Medo.Diagnostics.ErrorReport.SaveToTemp(e.Exception)
#If CONFIG = "Release" Then
        Medo.Diagnostics.ErrorReport.ShowDialog(Nothing, e.Exception, New Uri("http://jmedved.com/ErrorReport/"))
#Else
        Throw e.Exception
#End If
    End Sub

    Private Shared Sub ApplicationExit(ByVal sender As Object, ByVal e As EventArgs)
        Tray.Hide()
        System.Environment.Exit(0)
    End Sub

    Private Shared Sub NewInstanceDetected(ByVal sender As Object, ByVal e As Global.Medo.Application.NewInstanceEventArgs)
        Try
            _form.CreateControl()
            App._form.Handle.GetType()

            Dim method As New NewInstanceDetectedProcDelegate(AddressOf NewInstanceDetectedProc)
            _form.Invoke(method)
        Catch ex As Exception
        End Try
    End Sub


    Private Delegate Sub NewInstanceDetectedProcDelegate()

    Private Shared Sub NewInstanceDetectedProc()
        If (Settings.StartupShow = False) Then
            Tray.Init()
            Tray.Show()
        End If

        If (_form.WindowState = FormWindowState.Minimized) Then _form.WindowState = FormWindowState.Normal
        _form.Show()
        _form.Activate()
    End Sub

    Private Shared Sub Hotkey_HotkeyActivated(ByVal sender As Object, ByVal e As System.EventArgs)
        If (Settings.StartupShow = False) Then
            Tray.Init()
            Tray.Show()
        End If

        If (_form.WindowState = FormWindowState.Minimized) Then _form.WindowState = FormWindowState.Normal
        _form.Show()
        _form.Activate()
    End Sub


    Private NotInheritable Class NativeMethods
        Private Sub New()
        End Sub

        <System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint:="AllowSetForegroundWindow")> _
        Public Shared Function AllowSetForegroundWindow(ByVal dwProcessId As UInteger) As <System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)> Boolean
        End Function

    End Class

End Class