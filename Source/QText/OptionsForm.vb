Friend Class OptionsForm

    Private Sub OptionsForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If (Settings.ActivationHotkey <> App.Hotkey.Key) Then
            If (App.Hotkey.IsRegistered) Then App.Hotkey.Unregister()
            If (Settings.ActivationHotkey <> Keys.None) Then
                Try
                    App.Hotkey.Register(Settings.ActivationHotkey)
                Catch ex As InvalidOperationException
                    Medo.MessageBox.ShowWarning(Nothing, "Hotkey is already in use.")
                End Try
            End If
        End If
    End Sub

    Private Sub OptionsForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Appearance
        chkDisplayURLs.Checked = QTextAux.Settings.DisplayUnderlineURLs
        chkFollowURLs.Checked = QTextAux.Settings.FollowURLs
        nudTabWidth.Value = QTextAux.Settings.DisplayTabWidth
        txtFont.Text = GetFontText(QTextAux.Settings.DisplayFont)
        txtFont.Tag = QTextAux.Settings.DisplayFont
        lblColorExample.BackColor = QTextAux.Settings.DisplayBackgroundColor
        lblColorExample.ForeColor = QTextAux.Settings.DisplayForegroundColor
        Call chkDisplayURLs_CheckedChanged(Nothing, Nothing)

        'Display
        chkShowMenu.Checked = Settings.ShowMenu
        chkShowToolbar.Checked = Settings.DisplayShowToolbar
        chkShowInTaskbar.Checked = Settings.DisplayShowInTaskbar
        chbMultilineTabHeaders.Checked = Settings.DisplayMultilineTabHeader
        chbShowMinimizeMaximizeButtons.Checked = Settings.DisplayMinimizeMaximizeButtons
        chbHorizontalScrollbar.Checked = (QTextAux.Settings.DisplayScrollbars = ScrollBars.Horizontal) OrElse (QTextAux.Settings.DisplayScrollbars = ScrollBars.Both)
        chbVerticalScrollbar.Checked = (QTextAux.Settings.DisplayScrollbars = ScrollBars.Vertical) OrElse (QTextAux.Settings.DisplayScrollbars = ScrollBars.Both)
        chbZoomToolbarWithDpi.Checked = QTextAux.Settings.ZoomToolbarWithDpiChange

        'Files
        chkRememberSelectedFile.Checked = Settings.StartupRememberSelectedFile
        chkPreloadFilesOnStartup.Checked = QTextAux.Settings.FilesPreload
        chkDeleteToRecycleBin.Checked = QTextAux.Settings.FilesDeleteToRecycleBin
        nudFilesAutoSaveInterval.Value = Settings.FilesAutoSaveInterval
        chbFilesSaveOnHide.Checked = Settings.SaveOnHide
        chbEnableQuickAutoSave.Checked = Settings.EnableQuickAutoSave
        nudQuickAutoSaveSeconds.Value = Settings.QuickAutoSaveSeconds
        Call chbEnableQuickAutoSave_CheckedChanged(Nothing, Nothing)

        'Behavior
        txtHotkey.Text = GetKeyString(Settings.ActivationHotkey)
        txtHotkey.Tag = Settings.ActivationHotkey
        chkMinimizeToTray.Checked = Settings.TrayOnMinimize
        chkSingleClickTrayActivation.Checked = QTextAux.Settings.TrayOneClickActivation
        chkRunAtStartup.Checked = Settings.StartupRun
        chkShowOnStartup.Checked = Settings.StartupShow

        'Carbon copy
        chbUseCarbonCopy.Checked = QTextAux.Settings.CarbonCopyUse
        txtCarbonCopyFolder.Text = QTextAux.Settings.CarbonCopyFolder
        chkCarbonCopyFolderCreate.Checked = QTextAux.Settings.CarbonCopyCreateFolder
        chbCarbonCopyIgnoreCopyErrors.Checked = QTextAux.Settings.CarbonCopyIgnoreErrors
        Call chbUseCarbonCopy_CheckedChanged(Nothing, Nothing)

        'Backup
        chbUseBackup.Enabled = False
        Call chbUseBackup_CheckedChanged(Nothing, Nothing)

        If (App.Hotkey.IsRegistered) Then App.Hotkey.Unregister()
        tab.TabPages.Remove(tab_pagBackup)
    End Sub

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        'Appearance
        QTextAux.Settings.DisplayUnderlineURLs = chkDisplayURLs.Checked
        QTextAux.Settings.FollowURLs = chkFollowURLs.Checked
        QTextAux.Settings.DisplayTabWidth = CInt(nudTabWidth.Value)
        QTextAux.Settings.DisplayFont = DirectCast(txtFont.Tag, System.Drawing.Font)
        QTextAux.Settings.DisplayBackgroundColor = lblColorExample.BackColor
        QTextAux.Settings.DisplayForegroundColor = lblColorExample.ForeColor

        'Display
        Settings.ShowMenu = chkShowMenu.Checked
        Settings.DisplayShowToolbar = chkShowToolbar.Checked
        Settings.DisplayShowInTaskbar = chkShowInTaskbar.Checked
        Settings.DisplayMultilineTabHeader = chbMultilineTabHeaders.Checked
        Settings.DisplayMinimizeMaximizeButtons = chbShowMinimizeMaximizeButtons.Checked
        If (chbHorizontalScrollbar.Checked AndAlso chbVerticalScrollbar.Checked) Then
            QTextAux.Settings.DisplayScrollbars = ScrollBars.Both
        ElseIf (chbHorizontalScrollbar.Checked) Then
            QTextAux.Settings.DisplayScrollbars = ScrollBars.Horizontal
        ElseIf (chbVerticalScrollbar.Checked) Then
            QTextAux.Settings.DisplayScrollbars = ScrollBars.Vertical
        Else
            QTextAux.Settings.DisplayScrollbars = ScrollBars.None
        End If
        QTextAux.Settings.ZoomToolbarWithDpiChange = chbZoomToolbarWithDpi.Checked

        'Files
        Settings.StartupRememberSelectedFile = chkRememberSelectedFile.Checked
        QTextAux.Settings.FilesPreload = chkPreloadFilesOnStartup.Checked
        QTextAux.Settings.FilesDeleteToRecycleBin = chkDeleteToRecycleBin.Checked
        Settings.FilesAutoSaveInterval = CInt(nudFilesAutoSaveInterval.Value)
        Settings.SaveOnHide = chbFilesSaveOnHide.Checked
        Settings.EnableQuickAutoSave = chbEnableQuickAutoSave.Checked
        Settings.QuickAutoSaveSeconds = CInt(nudQuickAutoSaveSeconds.Value)

        'Behavior
        Settings.ActivationHotkey = DirectCast(txtHotkey.Tag, Keys)
        Settings.TrayOnMinimize = chkMinimizeToTray.Checked
        QTextAux.Settings.TrayOneClickActivation = chkSingleClickTrayActivation.Checked
        Settings.StartupRun = chkRunAtStartup.Checked
        Settings.StartupShow = chkShowOnStartup.Checked

        'Carbon copy
        QTextAux.Settings.CarbonCopyUse = chbUseCarbonCopy.Checked
        QTextAux.Settings.CarbonCopyFolder = txtCarbonCopyFolder.Text
        QTextAux.Settings.CarbonCopyCreateFolder = chkCarbonCopyFolderCreate.Checked
        QTextAux.Settings.CarbonCopyIgnoreErrors = chbCarbonCopyIgnoreCopyErrors.Checked
    End Sub


    Private Sub chkDisplayURLs_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDisplayURLs.CheckedChanged
        chkFollowURLs.Enabled = chkDisplayURLs.Checked
    End Sub

    Private Sub chbUseCarbonCopy_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chbUseCarbonCopy.CheckedChanged
        lblCarbonCopyFolder.Enabled = chbUseCarbonCopy.Checked
        txtCarbonCopyFolder.Enabled = chbUseCarbonCopy.Checked
        btnCarbonCopyFolderSelect.Enabled = chbUseCarbonCopy.Checked
        chkCarbonCopyFolderCreate.Enabled = chbUseCarbonCopy.Checked
        chbCarbonCopyIgnoreCopyErrors.Enabled = chbUseCarbonCopy.Checked
        btnCarbonCopyOpenFolder.Enabled = txtCarbonCopyFolder.Text.Length > 0
    End Sub


    Private Sub txtHotkey_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles txtHotkey.PreviewKeyDown
        Dim str As String = GetKeyString(e.KeyData)
        If Not String.IsNullOrEmpty(str) Then
            txtHotkey.Text = str
            txtHotkey.Tag = e.KeyData
        Else
            txtHotkey.Text = "Use Ctrl+Alt, Ctrl+Shift or Alt+Shift"
            txtHotkey.Tag = Keys.None
        End If
    End Sub




    Private Shared Function GetKeyString(ByVal keyData As Keys) As String
        If ((keyData And Keys.LWin) = Keys.LWin) Then Return String.Empty
        If ((keyData And Keys.RWin) = Keys.RWin) Then Return String.Empty

        Dim sb As New System.Text.StringBuilder()
        Dim usesShift As Boolean, usesCtrl As Boolean, usesAlt As Boolean

        If ((keyData And Keys.Control) = Keys.Control) Then
            If (sb.Length > 0) Then sb.Append("+")
            sb.Append("Ctrl")
            keyData = keyData Xor Keys.Control
            usesCtrl = True
        End If

        If ((keyData And Keys.Alt) = Keys.Alt) Then
            If (sb.Length > 0) Then sb.Append("+")
            sb.Append("Alt")
            keyData = keyData Xor Keys.Alt
            usesAlt = True
        End If

        If ((keyData And Keys.Shift) = Keys.Shift) Then
            If (sb.Length > 0) Then sb.Append("+")
            sb.Append("Shift")
            keyData = keyData Xor Keys.Shift
            usesShift = True
        End If

        Select Case keyData
            Case 0 : Return String.Empty
            Case Keys.ControlKey : Return String.Empty
            Case Keys.Menu : Return String.Empty
            Case Keys.ShiftKey : Return String.Empty
            Case Else
                If Not ((usesCtrl AndAlso usesAlt) OrElse (usesCtrl AndAlso usesShift) OrElse (usesAlt AndAlso usesShift)) Then Return String.Empty
                If (sb.Length > 0) Then sb.Append("+")
                sb.Append(keyData.ToString())
                Return sb.ToString()
        End Select
    End Function

    Private Sub btnCarbonCopyFolderSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCarbonCopyFolderSelect.Click
        Using fd As New FolderBrowserDialog
            fd.RootFolder = Environment.SpecialFolder.Desktop
            fd.Description = "Please select folder for carbon copy. It cannot be same folder that is used for main storage."
            fd.ShowNewFolderButton = True
            If String.IsNullOrEmpty(QTextAux.Settings.CarbonCopyFolder) Then
                fd.SelectedPath = QTextAux.Settings.FilesLocation
            Else
                fd.SelectedPath = QTextAux.Settings.CarbonCopyFolder
            End If
            If fd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                If (String.Compare(fd.SelectedPath, QTextAux.Settings.FilesLocation, True) = 0) Then
                    Global.Medo.MessageBox.ShowWarning(Me, "Carbon copy folder cannot be same as one used for main program storage.")
                Else
                    txtCarbonCopyFolder.Text = fd.SelectedPath
                End If
            End If
        End Using
    End Sub

    Private Sub btnFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFont.Click
        Using f As New FontDialog
            f.AllowScriptChange = True
            f.AllowSimulations = True
            f.AllowVectorFonts = True
            f.AllowVerticalFonts = False
            f.FixedPitchOnly = False
            f.FontMustExist = True
            f.ShowApply = False
            f.ShowColor = False
            f.ShowEffects = False
            f.Font = DirectCast(txtFont.Tag, System.Drawing.Font)
            If f.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                txtFont.Text = GetFontText(f.Font)
                txtFont.Tag = f.Font
            End If
        End Using
    End Sub

    Private Sub btnColorBackground_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnColorBackground.Click
        Using f As New ColorDialog
            f.AllowFullOpen = True
            f.AnyColor = True
            f.FullOpen = True
            f.Color = lblColorExample.BackColor
            If f.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                lblColorExample.BackColor = f.Color
            End If
        End Using
    End Sub

    Private Sub btnColorForeground_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnColorForeground.Click
        Using f As New ColorDialog
            f.AllowFullOpen = True
            f.AnyColor = True
            f.FullOpen = True
            f.Color = lblColorExample.ForeColor
            If f.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                lblColorExample.ForeColor = f.Color
            End If
        End Using
    End Sub


    Private Shared Function GetFontText(ByVal font As System.Drawing.Font) As String
        Dim sb As New System.Text.StringBuilder
        sb.Append(font.Name)
        If (font.Bold) Then
            sb.Append(", bold")
        End If

        If (font.Italic) Then
            sb.Append(", italic")
        End If

        sb.Append(", " + font.SizeInPoints.ToString() + " pt")

        Return sb.ToString()
    End Function

    Private Sub btnOpenLocationFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenLocationFolder.Click
        Try
            System.Diagnostics.Process.Start(QTextAux.Settings.FilesLocation, Nothing)
        Catch ex As System.ComponentModel.Win32Exception
            Global.Medo.MessageBox.ShowWarning(Me, ex.Message)
        End Try
    End Sub

    Private Sub chbEnableQuickAutoSave_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chbEnableQuickAutoSave.CheckedChanged
        nudQuickAutoSaveSeconds.Enabled = chbEnableQuickAutoSave.Checked
        lblQuickAutoSaveSeconds.Enabled = chbEnableQuickAutoSave.Checked
    End Sub

    Private Sub btnCarbonCopyOpenFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCarbonCopyOpenFolder.Click
        Try
            System.Diagnostics.Process.Start(txtCarbonCopyFolder.Text, Nothing)
        Catch ex As System.ComponentModel.Win32Exception
            Global.Medo.MessageBox.ShowWarning(Me, ex.Message)
        End Try
    End Sub

    Private Sub btnBackupFolderSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBackupFolderSelect.Click
        Using fd As New FolderBrowserDialog
            fd.RootFolder = Environment.SpecialFolder.Desktop
            fd.Description = "Please select folder for backup."
            fd.ShowNewFolderButton = True
            'If String.IsNullOrEmpty(Options.CarbonCopyFolder) Then
            'TODO fd.SelectedPath = Options.FilesLocation
            'Else
            'TODO fd.SelectedPath = Options.CarbonCopyFolder
            'End If
            If fd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                txtBackupFolder.Text = fd.SelectedPath
            End If
        End Using
    End Sub

    Private Sub chbUseBackup_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chbUseBackup.CheckedChanged
        lblBackupFolder.Enabled = chbUseBackup.Checked
        txtBackupFolder.Enabled = chbUseBackup.Checked
        btnBackupFolderSelect.Enabled = chbUseBackup.Checked
        chbBackupCreateFolder.Enabled = chbUseBackup.Checked
        chbBackupIgnoreCopyErrors.Enabled = chbUseBackup.Checked
        btnBackupOpenFolder.Enabled = txtBackupFolder.Text.Length > 0
    End Sub

    Private Sub bntBackupOpenFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBackupOpenFolder.Click
        Try
            System.Diagnostics.Process.Start(txtBackupFolder.Text, Nothing)
        Catch ex As System.ComponentModel.Win32Exception
            Global.Medo.MessageBox.ShowWarning(Me, ex.Message)
        End Try
    End Sub

    Private Sub txtCarbonCopyFolder_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCarbonCopyFolder.TextChanged
        btnCarbonCopyOpenFolder.Enabled = txtCarbonCopyFolder.Text.Length > 0
    End Sub

    Private Sub txtBackupFolder_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBackupFolder.TextChanged
        btnBackupOpenFolder.Enabled = txtBackupFolder.Text.Length > 0
    End Sub

    Private Sub btnChangeLocation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChangeLocation.Click
        'Using fd As New FolderBrowserDialog
        '    fd.RootFolder = Environment.SpecialFolder.Desktop
        '    fd.Description = "Please select folder for storage of files. Current folder is """ + Settings.FilesLocation + """."
        '    fd.ShowNewFolderButton = True
        '    fd.SelectedPath = Settings.FilesLocation
        '    If fd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
        '        If (String.Compare(fd.SelectedPath, Settings.CarbonCopyFolder, True) = 0) Then
        '            Medo.MessageBox.ShowWarning(Me, "This folder is currenly used for carbon copy. Move will be aborted.")
        '        Else
        '            Dim newPath As String = fd.SelectedPath

        '            Dim isOK As Boolean = True
        '            Try
        '                Helper.Path.Create(newPath)
        '                Try
        '                    Dim oldFiles As String() = System.IO.Directory.GetFiles(Settings.FilesLocation, "*.txt")
        '                    If (oldFiles.Length > 0) Then
        '                        Select Case Medo.MessageBox.ShowQuestion("Do you want to copy existing files to new location?", MessageBoxButtons.YesNo)
        '                            Case DialogResult.Yes
        '                                Try
        '                                    System.IO.File.Copy(System.IO.Path.Combine(Settings.FilesLocation, "QText.xml"), System.IO.Path.Combine(newPath, "QText.xml"))
        '                                Catch ex As Exception
        '                                End Try
        '                                For i As Integer = 0 To oldFiles.Length - 1
        '                                    Try
        '                                        Dim oldFile As String = oldFiles(i)
        '                                        Dim oldFI As New System.IO.FileInfo(oldFile)
        '                                        Dim newFile As String = System.IO.Path.Combine(newPath, oldFI.Name)
        '                                        If System.IO.File.Exists(newFile) Then
        '                                            Select Case Medo.MessageBox.ShowQuestion(Me, "File """ + oldFI.Name + """ already exists at destination. Do you want to overwrite?", MessageBoxButtons.YesNoCancel)
        '                                                Case Windows.Forms.DialogResult.Yes
        '                                                    System.IO.File.Copy(oldFile, newFile, True)
        '                                                Case Windows.Forms.DialogResult.No
        '                                                    Continue For
        '                                                Case Windows.Forms.DialogResult.Cancel
        '                                                    Exit Sub
        '                                            End Select
        '                                        Else
        '                                            System.IO.File.Copy(oldFile, newFile)
        '                                        End If
        '                                    Catch ex As Exception
        '                                        Medo.MessageBox.ShowWarning(Me, "Error copying files." + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
        '                                        isOK = False
        '                                    End Try
        '                                Next
        '                        End Select
        '                    End If
        '                    Me.DialogResult = Windows.Forms.DialogResult.OK
        '                Catch ex As Exception
        '                    Medo.MessageBox.ShowWarning(Me, "Error retrieving old path." + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
        '                    isOK = False
        '                End Try
        '                If isOK Then
        '                    Settings.FilesLocation = newPath
        '                    Medo.MessageBox.ShowInformation(Me, "Data location transfer succeeded.")
        '                Else
        '                    Medo.MessageBox.ShowWarning(Me, "Data location transfer succeeded with some errors.")
        '                End If
        '            Catch ex As Exception
        '                Medo.MessageBox.ShowWarning(Me, ex.Message, MessageBoxButtons.OK)
        '            End Try

        '        End If
        '    End If
        'End Using
        Using fd As New SaveFileDialog
            fd.CheckFileExists = False
            fd.CheckPathExists = True
            fd.CreatePrompt = False
            fd.Filter = "All files|" + System.Guid.Empty.ToString()
            fd.FileName = "any"
            fd.InitialDirectory = QTextAux.Settings.FilesLocation
            fd.OverwritePrompt = False
            fd.Title = "Please select folder for storage of files"
            fd.ValidateNames = False
            If fd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                Dim selectedFile As New System.IO.FileInfo(fd.FileName)
                Dim selectedPath As String = selectedFile.DirectoryName
                If (String.Compare(selectedPath, QTextAux.Settings.CarbonCopyFolder, True) = 0) Then
                    Global.Medo.MessageBox.ShowWarning(Me, "This folder is currenly used for carbon copy. Move will be aborted.")
                ElseIf (String.Compare(selectedPath, QTextAux.Settings.FilesLocation, True) = 0) Then
                    Global.Medo.MessageBox.ShowWarning(Me, "This folder is already used for storage. Move will be aborted.")
                Else
                    Dim newPath As String = selectedPath

                    Dim isOK As Boolean = True
                    Try
                        QTextAux.Helper.Path.Create(newPath)
                        Try
                            Dim oldFiles As String() = System.IO.Directory.GetFiles(QTextAux.Settings.FilesLocation, "*.txt")
                            If (oldFiles.Length > 0) Then
                                Select Case Global.Medo.MessageBox.ShowQuestion(Me, "Do you want to copy existing files to new location?", MessageBoxButtons.YesNo)
                                    Case DialogResult.Yes
                                        Try
                                            System.IO.File.Copy(System.IO.Path.Combine(QTextAux.Settings.FilesLocation, "QText.xml"), System.IO.Path.Combine(newPath, "QText.xml"))
                                        Catch ex As Exception
                                        End Try
                                        For i As Integer = 0 To oldFiles.Length - 1
                                            Try
                                                Dim oldFile As String = oldFiles(i)
                                                Dim oldFI As New System.IO.FileInfo(oldFile)
                                                Dim newFile As String = System.IO.Path.Combine(newPath, oldFI.Name)
                                                If System.IO.File.Exists(newFile) Then
                                                    Select Case Global.Medo.MessageBox.ShowQuestion(Me, "File """ + oldFI.Name + """ already exists at destination. Do you want to overwrite?", MessageBoxButtons.YesNoCancel)
                                                        Case Windows.Forms.DialogResult.Yes
                                                            System.IO.File.Copy(oldFile, newFile, True)
                                                        Case Windows.Forms.DialogResult.No
                                                            Continue For
                                                        Case Windows.Forms.DialogResult.Cancel
                                                            Exit Sub
                                                    End Select
                                                Else
                                                    System.IO.File.Copy(oldFile, newFile)
                                                End If
                                            Catch ex As Exception
                                                Global.Medo.MessageBox.ShowWarning(Me, "Error copying files." + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
                                                isOK = False
                                            End Try
                                        Next
                                End Select
                            End If
                            Me.DialogResult = Windows.Forms.DialogResult.OK
                        Catch ex As Exception
                            Global.Medo.MessageBox.ShowWarning(Me, "Error retrieving old path." + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
                            isOK = False
                        End Try
                        If isOK Then
                            QTextAux.Settings.FilesLocation = newPath
                            Global.Medo.MessageBox.ShowInformation(Me, "Data location transfer succeeded.")
                        Else
                            Global.Medo.MessageBox.ShowWarning(Me, "Data location transfer succeeded with some errors.")
                        End If
                    Catch ex As Exception
                        Global.Medo.MessageBox.ShowWarning(Me, ex.Message, MessageBoxButtons.OK)
                    End Try

                End If
            End If
        End Using
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.Font = System.Drawing.SystemFonts.MessageBoxFont

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub tab_pagAppearance_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tab_pagAppearance.Click

    End Sub

    Private Sub chkShowInTaskbar_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkShowInTaskbar.CheckedChanged
        If (chkShowInTaskbar.Checked = False) Then
            chbShowMinimizeMaximizeButtons.Checked = False
        End If
    End Sub

    Private Sub chbShowMinimizeMaximizeButtons_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chbShowMinimizeMaximizeButtons.CheckedChanged
        If (chbShowMinimizeMaximizeButtons.Checked) Then
            chkShowInTaskbar.Checked = True
        End If
    End Sub

End Class