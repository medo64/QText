Friend Class Settings

    Private Sub New()
    End Sub


    <System.ComponentModel.Category("Activation")> _
    <System.ComponentModel.DisplayName("Hotkey")> _
    <System.ComponentModel.Description("System-wide hotkey used for activation.")> _
    <System.ComponentModel.DefaultValue(GetType(Keys), "Ctrl+Shift+Q")> _
    Public Shared Property ActivationHotkey() As Keys
        Get
            Return CType(Global.Medo.Configuration.Settings.Read("ActivationHotkey", CInt(Keys.Control Or Keys.Shift Or Keys.Q)), Keys)
        End Get
        Set(ByVal value As Keys)
            Global.Medo.Configuration.Settings.Write("ActivationHotkey", value)
            If (QTextAux.App.Hotkey.IsRegistered) Then QTextAux.App.Hotkey.Unregister()
            If (Settings.ActivationHotkey <> Keys.None) Then QTextAux.App.Hotkey.Register(Settings.ActivationHotkey)
        End Set
    End Property


    <System.ComponentModel.Category("Display")> _
    <System.ComponentModel.DisplayName("Always on Top")> _
    <System.ComponentModel.Description("If True, main window will be always on top.")> _
    <System.ComponentModel.Browsable(False)> _
    <System.ComponentModel.DefaultValue(False)> _
    Public Shared Property DisplayAlwaysOnTop() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("DisplayAlwaysOnTop", False)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("DisplayAlwaysOnTop", value)
        End Set
    End Property

    <System.ComponentModel.Category("Display")> _
    <System.ComponentModel.DisplayName("Minimize/Maximize buttons")> _
    <System.ComponentModel.Description("If True, minimize/maximize buttons will appear on main form.")> _
    <System.ComponentModel.DefaultValue(False)> _
    Public Shared Property DisplayMinimizeMaximizeButtons() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("DisplayMinimizeMaximizeButtons", True)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("DisplayMinimizeMaximizeButtons", value)
        End Set
    End Property

    <System.ComponentModel.Category("Display")> _
    <System.ComponentModel.DisplayName("Multiline tab headers")> _
    <System.ComponentModel.Description("If True, tab headers will go in multiline mode when not all can fit on screen.")> _
    <System.ComponentModel.DefaultValue(False)> _
    Public Shared Property DisplayMultilineTabHeader() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("DisplayMultilineTabHeader", False)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("DisplayMultilineTabHeader", value)
        End Set
    End Property



    <System.ComponentModel.Category("Display"), _
      System.ComponentModel.DisplayName("Show in taskbar"), _
      System.ComponentModel.Description("If True, main window will shown on taskbar when open."), _
      System.ComponentModel.DefaultValue(True)> _
    Public Shared Property DisplayShowInTaskbar() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("DisplayShowInTaskbar", False)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("DisplayShowInTaskbar", value)
        End Set
    End Property

    <System.ComponentModel.Category("Display")> _
    <System.ComponentModel.DisplayName("Toolbar")> _
    <System.ComponentModel.Description("If True, toolbar will be displayed.")> _
    <System.ComponentModel.DefaultValue(True)> _
    Public Shared Property DisplayShowToolbar() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("DisplayShowToolbar", True)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("DisplayShowToolbar", value)
        End Set
    End Property

    Public Shared Property ShowMenu() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("ShowMenu", False)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("ShowMenu", value)
        End Set
    End Property

    <System.ComponentModel.Category("Files")> _
    <System.ComponentModel.DisplayName("Auto-save")> _
    <System.ComponentModel.Description("Interval in seconds between autosaves. Valid values are from 15 (15 seconds) to 600 (ten minutes). If value is 0, there is no auto-save.")> _
    <System.ComponentModel.DefaultValue(60)> _
    Public Shared Property FilesAutoSaveInterval() As Integer
        Get
            Dim value As Integer = Global.Medo.Configuration.Settings.Read("FilesAutoSaveInterval", 60)
            If value < 15 Then Return 15
            If value > 300 Then Return 300
            Return value
        End Get
        Set(ByVal value As Integer)
            If value < 15 Then value = 15
            If value > 300 Then value = 300
            Global.Medo.Configuration.Settings.Write("FilesAutoSaveInterval", value)
        End Set
    End Property

    Public Shared Property EnableQuickAutoSave() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("EnableQuickAutoSave", True)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("EnableQuickAutoSave", value)
        End Set
    End Property

    Public Shared Property QuickAutoSaveSeconds() As Integer
        Get
            Dim value As Integer = Global.Medo.Configuration.Settings.Read("QuickAutoSaveSeconds", 3)
            If value < 1 Then Return 1
            If value > 15 Then Return 15
            Return value
        End Get
        Set(ByVal value As Integer)
            If value < 1 Then value = 1
            If value > 15 Then value = 15
            Global.Medo.Configuration.Settings.Write("QuickAutoSaveSeconds", value)
        End Set
    End Property

    <System.ComponentModel.Category("Files")> _
    <System.ComponentModel.DisplayName("Save on close")> _
    <System.ComponentModel.Description("If True, files will be saved when window is closed.")> _
    <System.ComponentModel.DefaultValue(True)> _
    Public Shared Property SaveOnHide() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("FilesSaveOnClose", True)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("FilesSaveOnClose", value)
        End Set
    End Property

    <System.ComponentModel.Category("Startup")> _
    <System.ComponentModel.DisplayName("Remember selected file")> _
    <System.ComponentModel.Description("If True, program will remember which file was opened and reopen it when restarted.")> _
    <System.ComponentModel.DefaultValue(True)> _
    Public Shared Property StartupRememberSelectedFile() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("StartupRememberSelectedFile", True)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("StartupRememberSelectedFile", value)
        End Set
    End Property

    <System.ComponentModel.Category("Startup")> _
    <System.ComponentModel.DisplayName("Run at startup")> _
    <System.ComponentModel.Description("If true, program will run at startup.")> _
    <System.ComponentModel.DefaultValue(True)> _
    Public Shared Property StartupRun() As Boolean
        Get
            Return Global.Medo.Configuration.RunOnStartup.Current.RunForCurrentUser
        End Get
        Set(ByVal value As Boolean)
            Try
                Global.Medo.Configuration.RunOnStartup.Current.RunForCurrentUser = value
            Catch ex As Exception
            End Try
        End Set
    End Property

    <System.ComponentModel.Category("Startup")> _
    <System.ComponentModel.DisplayName("Show on startup")> _
    <System.ComponentModel.Description("If True, program will be shown when started.")> _
    <System.ComponentModel.DefaultValue(False)> _
    Public Shared Property StartupShow() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("StartupShow", False)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("StartupShow", value)
        End Set
    End Property

    <System.ComponentModel.Category("Tray")> _
    <System.ComponentModel.DisplayName("Minimize to tray")> _
    <System.ComponentModel.Description("If true, program will go to tray area when user clicks on minimize button.")> _
    <System.ComponentModel.DefaultValue(False)> _
    Public Shared Property TrayOnMinimize() As Boolean
        Get
            Return Global.Medo.Configuration.Settings.Read("TrayOnMinimize", False)
        End Get
        Set(ByVal value As Boolean)
            Global.Medo.Configuration.Settings.Write("TrayOnMinimize", value)
        End Set
    End Property

End Class
