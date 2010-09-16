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
            If (App.Hotkey.IsRegistered) Then App.Hotkey.Unregister()
            If (Settings.ActivationHotkey <> Keys.None) Then App.Hotkey.Register(Settings.ActivationHotkey)
        End Set
    End Property


    '<System.ComponentModel.Category("Carbon copy")> _
    '<System.ComponentModel.DisplayName("Create folder")> _
    '<System.ComponentModel.Description("If True, creates folder if possible.")> _
    '<System.ComponentModel.DefaultValue(True)> _
    'Public Shared Property CarbonCopyCreateFolder() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("CarbonCopyCreateFolder", True)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("CarbonCopyCreateFolder", value)
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Carbon copy")> _
    '<System.ComponentModel.DisplayName("Folder")> _
    '<System.ComponentModel.Description("Folder in which to save copies of all files.")> _
    '<System.ComponentModel.DefaultValue("")> _
    'Public Shared Property CarbonCopyFolder() As String
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("CarbonCopyFolder", "")
    '    End Get
    '    Set(ByVal value As String)
    '        If FilesLocation Is Nothing Then Exit Property
    '        If (String.Compare(value.Trim, FilesLocation.Trim, True) = 0) Then Exit Property
    '        Global.Medo.Configuration.Settings.Write("CarbonCopyFolder", value)
    '        If (String.IsNullOrEmpty(value)) Then CarbonCopyUse = False
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Carbon copy")> _
    '<System.ComponentModel.DisplayName("Ignore errors")> _
    '<System.ComponentModel.Description("If True, errors during copy will be ignored.")> _
    '<System.ComponentModel.DefaultValue(True)> _
    'Public Shared Property CarbonCopyIgnoreErrors() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("CarbonCopyIgnoreErrors", True)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("CarbonCopyIgnoreErrors", value)
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Carbon copy")> _
    '<System.ComponentModel.DisplayName("Use carbon copy")> _
    '<System.ComponentModel.Description("If True, carbon copy will be used.")> _
    '<System.ComponentModel.DefaultValue(False)> _
    'Public Shared Property CarbonCopyUse() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("CarbonCopyUse", False)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("CarbonCopyUse", value)
    '    End Set
    'End Property


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

    '<System.ComponentModel.Category("Display")> _
    '<System.ComponentModel.DisplayName("Background color")> _
    '<System.ComponentModel.Description("Background of text.")> _
    '<System.ComponentModel.DefaultValue(GetType(System.Drawing.Color), "Window")> _
    'Public Shared Property DisplayBackgroundColor() As System.Drawing.Color
    '    Get
    '        Try
    '            Return System.Drawing.Color.FromArgb(Global.Medo.Configuration.Settings.Read("DisplayBackgroundColor", System.Drawing.SystemColors.Window.ToArgb))
    '        Catch
    '            Return System.Drawing.SystemColors.Window
    '        End Try
    '    End Get
    '    Set(ByVal value As System.Drawing.Color)
    '        Global.Medo.Configuration.Settings.Write("DisplayBackgroundColor", value.ToArgb)
    '    End Set
    'End Property

    'Public Shared Property DisplayFont() As Drawing.Font
    '    Get
    '        Dim tmpFamilyName As String = Global.Medo.Configuration.Settings.Read("DisplayFont_FamilyName", System.Drawing.SystemFonts.MessageBoxFont.Name)
    '        Dim tmpSize As Double = Global.Medo.Configuration.Settings.Read("DisplayFont_Size", System.Drawing.SystemFonts.MessageBoxFont.Size)
    '        Dim tmpStyle As Integer = Global.Medo.Configuration.Settings.Read("DisplayFont_Style", DirectCast(System.Drawing.SystemFonts.MessageBoxFont.Style, Int32))
    '        Try
    '            Return New Drawing.Font(tmpFamilyName, CSng(tmpSize), DirectCast(tmpStyle, Drawing.FontStyle))
    '        Catch ex As Exception
    '            Return System.Drawing.SystemFonts.MessageBoxFont
    '        End Try
    '    End Get
    '    Set(ByVal value As Drawing.Font)
    '        Global.Medo.Configuration.Settings.Write("DisplayFont_FamilyName", value.Name)
    '        Global.Medo.Configuration.Settings.Write("DisplayFont_Size", value.Size)
    '        Global.Medo.Configuration.Settings.Write("DisplayFont_Style", value.Style)
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Display")> _
    '<System.ComponentModel.DisplayName("Foreground color")> _
    '<System.ComponentModel.Description("Foreground of text.")> _
    '<System.ComponentModel.DefaultValue(GetType(System.Drawing.Color), "WindowText")> _
    'Public Shared Property DisplayForegroundColor() As System.Drawing.Color
    '    Get
    '        Try
    '            Return System.Drawing.Color.FromArgb(Global.Medo.Configuration.Settings.Read("DisplayForegroundColor", System.Drawing.SystemColors.WindowText.ToArgb))
    '        Catch ex As Exception
    '            Return System.Drawing.SystemColors.WindowText
    '        End Try
    '    End Get
    '    Set(ByVal value As System.Drawing.Color)
    '        Global.Medo.Configuration.Settings.Write("DisplayForegroundColor", value.ToArgb)
    '    End Set
    'End Property

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

    '<System.ComponentModel.Category("Display")> _
    '<System.ComponentModel.DisplayName("Scroll bars")> _
    '<System.ComponentModel.Description("Indicates which scroll bars will be shown.")> _
    '<System.ComponentModel.DefaultValue(GetType(System.Windows.Forms.ScrollBars), "Vertical")> _
    'Public Shared Property DisplayScrollbars() As System.Windows.Forms.ScrollBars
    '    Get
    '        Try
    '            Return CType(Global.Medo.Configuration.Settings.Read("DisplayScrollbars", CInt(System.Windows.Forms.ScrollBars.Vertical)), System.Windows.Forms.ScrollBars)
    '        Catch ex As Exception
    '            Return System.Windows.Forms.ScrollBars.Vertical
    '        End Try
    '    End Get
    '    Set(ByVal value As System.Windows.Forms.ScrollBars)
    '        Global.Medo.Configuration.Settings.Write("DisplayScrollbars", CInt(value))
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Display")> _
    '<System.ComponentModel.DisplayName("Select all text")> _
    '<System.ComponentModel.Description("If True, text will be selected upon opening.")> _
    '<System.ComponentModel.DefaultValue(False)> _
    'Public Shared Property DisplaySelectAll() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("DisplaySelectAll", False)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("DisplaySelectAll", value)
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Display")> _
    '<System.ComponentModel.DisplayName("Underline URLs")> _
    '<System.ComponentModel.Description("If True, URLs will be underlined and blue. Valid only if RichTextBox is used.")> _
    '<System.ComponentModel.DefaultValue(True)> _
    'Public Shared Property DisplayUnderlineURLs() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("DisplayUnderlineURLs", True)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("DisplayUnderlineURLs", value)
    '    End Set
    'End Property

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

    '<System.ComponentModel.Category("Display")> _
    '<System.ComponentModel.DisplayName("Tab width")> _
    '<System.ComponentModel.Description("Number of spaces that will form tab character.")> _
    '<System.ComponentModel.DefaultValue(4)> _
    'Public Shared Property DisplayTabWidth() As Integer
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("DisplayTabWidth", 4)
    '    End Get
    '    Set(ByVal value As Integer)
    '        If (value >= 1) AndAlso (value <= 16) Then
    '            Global.Medo.Configuration.Settings.Write("DisplayTabWidth", value)
    '        End If
    '    End Set
    'End Property

    '<System.ComponentModel.Browsable(False)> _
    'Public Shared ReadOnly Property DisplayWordWrap() As Boolean
    '    Get
    '        Return (DisplayScrollbars = ScrollBars.Vertical)
    '    End Get
    'End Property


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

    '<System.ComponentModel.Category("Files")> _
    '<System.ComponentModel.DisplayName("Use RichTextBox")> _
    '<System.ComponentModel.Description("If true, RichTextBox control will be used. This brings advantage of multiple undo and redo operations at const of slightly larger memory consumption.")> _
    '<System.ComponentModel.DefaultValue(True)> _
    'Public Shared Property FilesUseRichTextBox() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("UseRichTextBox", True)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("UseRichTextBox", value)
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Files")> _
    '<System.ComponentModel.DisplayName("Delete to recycle bin")> _
    '<System.ComponentModel.Description("If True, file will be deleted to recycle bin.")> _
    '<System.ComponentModel.Browsable(False)> _
    '<System.ComponentModel.DefaultValue(True)> _
    'Public Shared Property FilesDeleteToRecycleBin() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("FilesDeleteToRecycleBin", True)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("FilesDeleteToRecycleBin", value)
    '    End Set
    'End Property

    'Public Shared Property FilesLocation() As String
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("DataPath", System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Global.Medo.Reflection.EntryAssembly.Company + "\" + Global.Medo.Reflection.EntryAssembly.Name))
    '    End Get
    '    Set(ByVal value As String)
    '        Global.Medo.Configuration.Settings.Write("DataPath", value)
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Files")> _
    '<System.ComponentModel.DisplayName("Preload")> _
    '<System.ComponentModel.Description("If True, all files will be opened when program starts.")> _
    '<System.ComponentModel.DefaultValue(False)> _
    'Public Shared Property FilesPreload() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("FilesPreload", False)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("FilesPreload", value)
    '    End Set
    'End Property

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


    '<System.ComponentModel.Category("Tray")> _
    '<System.ComponentModel.DisplayName("Close to tray")> _
    '<System.ComponentModel.Description("If true, program will go to tray area instead of exiting when user clicks on close button.")> _
    '<System.ComponentModel.DefaultValue(True)> _
    'Public Shared Property TrayOnClose() As Boolean
    '    Get
    '        Return Not Global.Medo.Configuration.Settings.Read("ExitOnClose", Not True)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("ExitOnClose", Not value)
    '    End Set
    'End Property

    '<System.ComponentModel.Category("Tray")> _
    '<System.ComponentModel.DisplayName("One-click activation")> _
    '<System.ComponentModel.Description("If true, program will be activated when user does click on tray icon. If false, double-click is needed.")> _
    '<System.ComponentModel.DefaultValue(True)> _
    'Public Shared Property TrayOneClickActivation() As Boolean
    '    Get
    '        Return Global.Medo.Configuration.Settings.Read("TrayOneClickActivation", True)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Global.Medo.Configuration.Settings.Write("TrayOneClickActivation", value)
    '    End Set
    'End Property

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

    Public Shared ReadOnly DefaultFilesLocation As String = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Global.Medo.Reflection.EntryAssembly.Company + "\" + Global.Medo.Reflection.EntryAssembly.Name)


End Class
