<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OptionsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.tab = New System.Windows.Forms.TabControl()
        Me.tab_pagAppearance = New System.Windows.Forms.TabPage()
        Me.lblColorExample = New System.Windows.Forms.Label()
        Me.btnColorBackground = New System.Windows.Forms.Button()
        Me.btnColorForeground = New System.Windows.Forms.Button()
        Me.lblColor = New System.Windows.Forms.Label()
        Me.lblFont = New System.Windows.Forms.Label()
        Me.btnFont = New System.Windows.Forms.Button()
        Me.nudTabWidth = New System.Windows.Forms.NumericUpDown()
        Me.lblTabWidth = New System.Windows.Forms.Label()
        Me.chkFollowURLs = New System.Windows.Forms.CheckBox()
        Me.chkDisplayURLs = New System.Windows.Forms.CheckBox()
        Me.txtFont = New System.Windows.Forms.TextBox()
        Me.tab_pagDisplay = New System.Windows.Forms.TabPage()
        Me.chbZoomToolbarWithDpi = New System.Windows.Forms.CheckBox()
        Me.chbVerticalScrollbar = New System.Windows.Forms.CheckBox()
        Me.chbHorizontalScrollbar = New System.Windows.Forms.CheckBox()
        Me.chbShowMinimizeMaximizeButtons = New System.Windows.Forms.CheckBox()
        Me.chbMultilineTabHeaders = New System.Windows.Forms.CheckBox()
        Me.chkShowMenu = New System.Windows.Forms.CheckBox()
        Me.chkShowToolbar = New System.Windows.Forms.CheckBox()
        Me.chkShowInTaskbar = New System.Windows.Forms.CheckBox()
        Me.tab_pagFiles = New System.Windows.Forms.TabPage()
        Me.lblQuickAutoSaveSeconds = New System.Windows.Forms.Label()
        Me.nudQuickAutoSaveSeconds = New System.Windows.Forms.NumericUpDown()
        Me.chbEnableQuickAutoSave = New System.Windows.Forms.CheckBox()
        Me.btnOpenLocationFolder = New System.Windows.Forms.Button()
        Me.chbFilesSaveOnHide = New System.Windows.Forms.CheckBox()
        Me.lblFilesAutoSaveIntervalSeconds = New System.Windows.Forms.Label()
        Me.nudFilesAutoSaveInterval = New System.Windows.Forms.NumericUpDown()
        Me.lblFilesAutoSaveInterval = New System.Windows.Forms.Label()
        Me.chkDeleteToRecycleBin = New System.Windows.Forms.CheckBox()
        Me.chkPreloadFilesOnStartup = New System.Windows.Forms.CheckBox()
        Me.chkRememberSelectedFile = New System.Windows.Forms.CheckBox()
        Me.btnChangeLocation = New System.Windows.Forms.Button()
        Me.tab_pagBehavior = New System.Windows.Forms.TabPage()
        Me.txtHotkey = New System.Windows.Forms.TextBox()
        Me.lblHotkey = New System.Windows.Forms.Label()
        Me.chkShowOnStartup = New System.Windows.Forms.CheckBox()
        Me.chkRunAtStartup = New System.Windows.Forms.CheckBox()
        Me.chkSingleClickTrayActivation = New System.Windows.Forms.CheckBox()
        Me.chkMinimizeToTray = New System.Windows.Forms.CheckBox()
        Me.tab_pagBackup = New System.Windows.Forms.TabPage()
        Me.btnBackupFolderSelect = New System.Windows.Forms.Button()
        Me.chbBackupIgnoreCopyErrors = New System.Windows.Forms.CheckBox()
        Me.chbUseBackup = New System.Windows.Forms.CheckBox()
        Me.chbBackupCreateFolder = New System.Windows.Forms.CheckBox()
        Me.txtBackupFolder = New System.Windows.Forms.TextBox()
        Me.lblBackupFolder = New System.Windows.Forms.Label()
        Me.btnBackupOpenFolder = New System.Windows.Forms.Button()
        Me.btnBackupRestore = New System.Windows.Forms.Button()
        Me.tab_pagCarbonCopy = New System.Windows.Forms.TabPage()
        Me.btnCarbonCopyOpenFolder = New System.Windows.Forms.Button()
        Me.btnCarbonCopyFolderSelect = New System.Windows.Forms.Button()
        Me.chbCarbonCopyIgnoreCopyErrors = New System.Windows.Forms.CheckBox()
        Me.chbUseCarbonCopy = New System.Windows.Forms.CheckBox()
        Me.chkCarbonCopyFolderCreate = New System.Windows.Forms.CheckBox()
        Me.txtCarbonCopyFolder = New System.Windows.Forms.TextBox()
        Me.lblCarbonCopyFolder = New System.Windows.Forms.Label()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tab.SuspendLayout()
        Me.tab_pagAppearance.SuspendLayout()
        CType(Me.nudTabWidth, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tab_pagDisplay.SuspendLayout()
        Me.tab_pagFiles.SuspendLayout()
        CType(Me.nudQuickAutoSaveSeconds, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudFilesAutoSaveInterval, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tab_pagBehavior.SuspendLayout()
        Me.tab_pagBackup.SuspendLayout()
        Me.tab_pagCarbonCopy.SuspendLayout()
        Me.SuspendLayout()
        '
        'tab
        '
        Me.tab.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tab.Controls.Add(Me.tab_pagAppearance)
        Me.tab.Controls.Add(Me.tab_pagDisplay)
        Me.tab.Controls.Add(Me.tab_pagFiles)
        Me.tab.Controls.Add(Me.tab_pagBehavior)
        Me.tab.Controls.Add(Me.tab_pagBackup)
        Me.tab.Controls.Add(Me.tab_pagCarbonCopy)
        Me.tab.Location = New System.Drawing.Point(12, 12)
        Me.tab.Name = "tab"
        Me.tab.SelectedIndex = 0
        Me.tab.Size = New System.Drawing.Size(510, 405)
        Me.tab.TabIndex = 0
        '
        'tab_pagAppearance
        '
        Me.tab_pagAppearance.Controls.Add(Me.lblColorExample)
        Me.tab_pagAppearance.Controls.Add(Me.btnColorBackground)
        Me.tab_pagAppearance.Controls.Add(Me.btnColorForeground)
        Me.tab_pagAppearance.Controls.Add(Me.lblColor)
        Me.tab_pagAppearance.Controls.Add(Me.lblFont)
        Me.tab_pagAppearance.Controls.Add(Me.btnFont)
        Me.tab_pagAppearance.Controls.Add(Me.nudTabWidth)
        Me.tab_pagAppearance.Controls.Add(Me.lblTabWidth)
        Me.tab_pagAppearance.Controls.Add(Me.chkFollowURLs)
        Me.tab_pagAppearance.Controls.Add(Me.chkDisplayURLs)
        Me.tab_pagAppearance.Controls.Add(Me.txtFont)
        Me.tab_pagAppearance.Location = New System.Drawing.Point(4, 25)
        Me.tab_pagAppearance.Name = "tab_pagAppearance"
        Me.tab_pagAppearance.Size = New System.Drawing.Size(502, 376)
        Me.tab_pagAppearance.TabIndex = 4
        Me.tab_pagAppearance.Text = "Appearance"
        Me.tab_pagAppearance.UseVisualStyleBackColor = True
        '
        'lblColorExample
        '
        Me.lblColorExample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblColorExample.Location = New System.Drawing.Point(123, 119)
        Me.lblColorExample.Name = "lblColorExample"
        Me.lblColorExample.Size = New System.Drawing.Size(29, 29)
        Me.lblColorExample.TabIndex = 9
        Me.lblColorExample.Text = "C"
        Me.lblColorExample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnColorBackground
        '
        Me.btnColorBackground.Location = New System.Drawing.Point(158, 119)
        Me.btnColorBackground.Name = "btnColorBackground"
        Me.btnColorBackground.Size = New System.Drawing.Size(98, 29)
        Me.btnColorBackground.TabIndex = 10
        Me.btnColorBackground.Text = "Background"
        Me.btnColorBackground.UseVisualStyleBackColor = True
        '
        'btnColorForeground
        '
        Me.btnColorForeground.Location = New System.Drawing.Point(262, 119)
        Me.btnColorForeground.Name = "btnColorForeground"
        Me.btnColorForeground.Size = New System.Drawing.Size(98, 29)
        Me.btnColorForeground.TabIndex = 11
        Me.btnColorForeground.Text = "Foreground"
        Me.btnColorForeground.UseVisualStyleBackColor = True
        '
        'lblColor
        '
        Me.lblColor.AutoSize = True
        Me.lblColor.Location = New System.Drawing.Point(3, 125)
        Me.lblColor.Name = "lblColor"
        Me.lblColor.Size = New System.Drawing.Size(52, 17)
        Me.lblColor.TabIndex = 8
        Me.lblColor.Text = "Colors:"
        '
        'lblFont
        '
        Me.lblFont.AutoSize = True
        Me.lblFont.Location = New System.Drawing.Point(3, 94)
        Me.lblFont.Name = "lblFont"
        Me.lblFont.Size = New System.Drawing.Size(40, 17)
        Me.lblFont.TabIndex = 5
        Me.lblFont.Text = "Font:"
        '
        'btnFont
        '
        Me.btnFont.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFont.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.btnFont.Location = New System.Drawing.Point(472, 91)
        Me.btnFont.Margin = New System.Windows.Forms.Padding(0, 3, 3, 3)
        Me.btnFont.Name = "btnFont"
        Me.btnFont.Size = New System.Drawing.Size(22, 22)
        Me.btnFont.TabIndex = 7
        Me.btnFont.Text = "..."
        Me.btnFont.UseVisualStyleBackColor = True
        '
        'nudTabWidth
        '
        Me.nudTabWidth.Location = New System.Drawing.Point(123, 63)
        Me.nudTabWidth.Maximum = New Decimal(New Integer() {16, 0, 0, 0})
        Me.nudTabWidth.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudTabWidth.Name = "nudTabWidth"
        Me.nudTabWidth.Size = New System.Drawing.Size(53, 22)
        Me.nudTabWidth.TabIndex = 4
        Me.nudTabWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nudTabWidth.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'lblTabWidth
        '
        Me.lblTabWidth.AutoSize = True
        Me.lblTabWidth.Location = New System.Drawing.Point(3, 65)
        Me.lblTabWidth.Name = "lblTabWidth"
        Me.lblTabWidth.Size = New System.Drawing.Size(105, 17)
        Me.lblTabWidth.TabIndex = 3
        Me.lblTabWidth.Text = "Tab char width:"
        '
        'chkFollowURLs
        '
        Me.chkFollowURLs.AutoSize = True
        Me.chkFollowURLs.Location = New System.Drawing.Point(23, 30)
        Me.chkFollowURLs.Name = "chkFollowURLs"
        Me.chkFollowURLs.Size = New System.Drawing.Size(108, 21)
        Me.chkFollowURLs.TabIndex = 2
        Me.chkFollowURLs.Text = "Follow URLs"
        Me.chkFollowURLs.UseVisualStyleBackColor = True
        '
        'chkDisplayURLs
        '
        Me.chkDisplayURLs.AutoSize = True
        Me.chkDisplayURLs.Location = New System.Drawing.Point(3, 3)
        Me.chkDisplayURLs.Name = "chkDisplayURLs"
        Me.chkDisplayURLs.Size = New System.Drawing.Size(115, 21)
        Me.chkDisplayURLs.TabIndex = 1
        Me.chkDisplayURLs.Text = "Display URLs"
        Me.chkDisplayURLs.UseVisualStyleBackColor = True
        '
        'txtFont
        '
        Me.txtFont.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFont.Location = New System.Drawing.Point(123, 91)
        Me.txtFont.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
        Me.txtFont.Name = "txtFont"
        Me.txtFont.ReadOnly = True
        Me.txtFont.Size = New System.Drawing.Size(349, 22)
        Me.txtFont.TabIndex = 6
        '
        'tab_pagDisplay
        '
        Me.tab_pagDisplay.Controls.Add(Me.chbZoomToolbarWithDpi)
        Me.tab_pagDisplay.Controls.Add(Me.chbVerticalScrollbar)
        Me.tab_pagDisplay.Controls.Add(Me.chbHorizontalScrollbar)
        Me.tab_pagDisplay.Controls.Add(Me.chbShowMinimizeMaximizeButtons)
        Me.tab_pagDisplay.Controls.Add(Me.chbMultilineTabHeaders)
        Me.tab_pagDisplay.Controls.Add(Me.chkShowMenu)
        Me.tab_pagDisplay.Controls.Add(Me.chkShowToolbar)
        Me.tab_pagDisplay.Controls.Add(Me.chkShowInTaskbar)
        Me.tab_pagDisplay.Location = New System.Drawing.Point(4, 25)
        Me.tab_pagDisplay.Name = "tab_pagDisplay"
        Me.tab_pagDisplay.Size = New System.Drawing.Size(502, 376)
        Me.tab_pagDisplay.TabIndex = 1
        Me.tab_pagDisplay.Text = "Display"
        Me.tab_pagDisplay.UseVisualStyleBackColor = True
        '
        'chbZoomToolbarWithDpi
        '
        Me.chbZoomToolbarWithDpi.AutoSize = True
        Me.chbZoomToolbarWithDpi.Location = New System.Drawing.Point(3, 84)
        Me.chbZoomToolbarWithDpi.Name = "chbZoomToolbarWithDpi"
        Me.chbZoomToolbarWithDpi.Size = New System.Drawing.Size(219, 21)
        Me.chbZoomToolbarWithDpi.TabIndex = 5
        Me.chbZoomToolbarWithDpi.Text = "Zoom toolbar with DPI change"
        Me.chbZoomToolbarWithDpi.UseVisualStyleBackColor = True
        '
        'chbVerticalScrollbar
        '
        Me.chbVerticalScrollbar.AutoSize = True
        Me.chbVerticalScrollbar.Location = New System.Drawing.Point(3, 192)
        Me.chbVerticalScrollbar.Name = "chbVerticalScrollbar"
        Me.chbVerticalScrollbar.Size = New System.Drawing.Size(135, 21)
        Me.chbVerticalScrollbar.TabIndex = 10
        Me.chbVerticalScrollbar.Text = "Vertical scrollbar"
        Me.chbVerticalScrollbar.UseVisualStyleBackColor = True
        '
        'chbHorizontalScrollbar
        '
        Me.chbHorizontalScrollbar.AutoSize = True
        Me.chbHorizontalScrollbar.Location = New System.Drawing.Point(3, 165)
        Me.chbHorizontalScrollbar.Name = "chbHorizontalScrollbar"
        Me.chbHorizontalScrollbar.Size = New System.Drawing.Size(152, 21)
        Me.chbHorizontalScrollbar.TabIndex = 9
        Me.chbHorizontalScrollbar.Text = "Horizontal scrollbar"
        Me.chbHorizontalScrollbar.UseVisualStyleBackColor = True
        '
        'chbShowMinimizeMaximizeButtons
        '
        Me.chbShowMinimizeMaximizeButtons.AutoSize = True
        Me.chbShowMinimizeMaximizeButtons.Location = New System.Drawing.Point(3, 138)
        Me.chbShowMinimizeMaximizeButtons.Name = "chbShowMinimizeMaximizeButtons"
        Me.chbShowMinimizeMaximizeButtons.Size = New System.Drawing.Size(234, 21)
        Me.chbShowMinimizeMaximizeButtons.TabIndex = 7
        Me.chbShowMinimizeMaximizeButtons.Text = "Show minimize/maximize buttons"
        Me.chbShowMinimizeMaximizeButtons.UseVisualStyleBackColor = True
        '
        'chbMultilineTabHeaders
        '
        Me.chbMultilineTabHeaders.AutoSize = True
        Me.chbMultilineTabHeaders.Location = New System.Drawing.Point(3, 111)
        Me.chbMultilineTabHeaders.Name = "chbMultilineTabHeaders"
        Me.chbMultilineTabHeaders.Size = New System.Drawing.Size(161, 21)
        Me.chbMultilineTabHeaders.TabIndex = 6
        Me.chbMultilineTabHeaders.Text = "Multiline tab headers"
        Me.chbMultilineTabHeaders.UseVisualStyleBackColor = True
        '
        'chkShowMenu
        '
        Me.chkShowMenu.AutoSize = True
        Me.chkShowMenu.Location = New System.Drawing.Point(3, 30)
        Me.chkShowMenu.Name = "chkShowMenu"
        Me.chkShowMenu.Size = New System.Drawing.Size(103, 21)
        Me.chkShowMenu.TabIndex = 2
        Me.chkShowMenu.Text = "Show menu"
        Me.chkShowMenu.UseVisualStyleBackColor = True
        '
        'chkShowToolbar
        '
        Me.chkShowToolbar.AutoSize = True
        Me.chkShowToolbar.Location = New System.Drawing.Point(3, 57)
        Me.chkShowToolbar.Name = "chkShowToolbar"
        Me.chkShowToolbar.Size = New System.Drawing.Size(112, 21)
        Me.chkShowToolbar.TabIndex = 4
        Me.chkShowToolbar.Text = "Show toolbar"
        Me.chkShowToolbar.UseVisualStyleBackColor = True
        '
        'chkShowInTaskbar
        '
        Me.chkShowInTaskbar.AutoSize = True
        Me.chkShowInTaskbar.Location = New System.Drawing.Point(3, 3)
        Me.chkShowInTaskbar.Name = "chkShowInTaskbar"
        Me.chkShowInTaskbar.Size = New System.Drawing.Size(130, 21)
        Me.chkShowInTaskbar.TabIndex = 1
        Me.chkShowInTaskbar.Text = "Show in taskbar"
        Me.chkShowInTaskbar.UseVisualStyleBackColor = True
        '
        'tab_pagFiles
        '
        Me.tab_pagFiles.Controls.Add(Me.lblQuickAutoSaveSeconds)
        Me.tab_pagFiles.Controls.Add(Me.nudQuickAutoSaveSeconds)
        Me.tab_pagFiles.Controls.Add(Me.chbEnableQuickAutoSave)
        Me.tab_pagFiles.Controls.Add(Me.btnOpenLocationFolder)
        Me.tab_pagFiles.Controls.Add(Me.chbFilesSaveOnHide)
        Me.tab_pagFiles.Controls.Add(Me.lblFilesAutoSaveIntervalSeconds)
        Me.tab_pagFiles.Controls.Add(Me.nudFilesAutoSaveInterval)
        Me.tab_pagFiles.Controls.Add(Me.lblFilesAutoSaveInterval)
        Me.tab_pagFiles.Controls.Add(Me.chkDeleteToRecycleBin)
        Me.tab_pagFiles.Controls.Add(Me.chkPreloadFilesOnStartup)
        Me.tab_pagFiles.Controls.Add(Me.chkRememberSelectedFile)
        Me.tab_pagFiles.Controls.Add(Me.btnChangeLocation)
        Me.tab_pagFiles.Location = New System.Drawing.Point(4, 25)
        Me.tab_pagFiles.Name = "tab_pagFiles"
        Me.tab_pagFiles.Padding = New System.Windows.Forms.Padding(4)
        Me.tab_pagFiles.Size = New System.Drawing.Size(502, 376)
        Me.tab_pagFiles.TabIndex = 0
        Me.tab_pagFiles.Text = "Files"
        Me.tab_pagFiles.UseVisualStyleBackColor = True
        '
        'lblQuickAutoSaveSeconds
        '
        Me.lblQuickAutoSaveSeconds.AutoSize = True
        Me.lblQuickAutoSaveSeconds.Location = New System.Drawing.Point(211, 33)
        Me.lblQuickAutoSaveSeconds.Name = "lblQuickAutoSaveSeconds"
        Me.lblQuickAutoSaveSeconds.Size = New System.Drawing.Size(61, 17)
        Me.lblQuickAutoSaveSeconds.TabIndex = 5
        Me.lblQuickAutoSaveSeconds.Text = "seconds"
        '
        'nudQuickAutoSaveSeconds
        '
        Me.nudQuickAutoSaveSeconds.Location = New System.Drawing.Point(155, 31)
        Me.nudQuickAutoSaveSeconds.Maximum = New Decimal(New Integer() {300, 0, 0, 0})
        Me.nudQuickAutoSaveSeconds.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudQuickAutoSaveSeconds.Name = "nudQuickAutoSaveSeconds"
        Me.nudQuickAutoSaveSeconds.Size = New System.Drawing.Size(50, 22)
        Me.nudQuickAutoSaveSeconds.TabIndex = 4
        Me.nudQuickAutoSaveSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nudQuickAutoSaveSeconds.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'chbEnableQuickAutoSave
        '
        Me.chbEnableQuickAutoSave.AutoSize = True
        Me.chbEnableQuickAutoSave.Location = New System.Drawing.Point(3, 32)
        Me.chbEnableQuickAutoSave.Name = "chbEnableQuickAutoSave"
        Me.chbEnableQuickAutoSave.Size = New System.Drawing.Size(132, 21)
        Me.chbEnableQuickAutoSave.TabIndex = 3
        Me.chbEnableQuickAutoSave.Text = "Quick auto save"
        Me.chbEnableQuickAutoSave.UseVisualStyleBackColor = True
        '
        'btnOpenLocationFolder
        '
        Me.btnOpenLocationFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnOpenLocationFolder.Location = New System.Drawing.Point(3, 344)
        Me.btnOpenLocationFolder.Name = "btnOpenLocationFolder"
        Me.btnOpenLocationFolder.Size = New System.Drawing.Size(196, 29)
        Me.btnOpenLocationFolder.TabIndex = 10
        Me.btnOpenLocationFolder.Text = "Open folder"
        Me.btnOpenLocationFolder.UseVisualStyleBackColor = True
        '
        'chbFilesSaveOnHide
        '
        Me.chbFilesSaveOnHide.AutoSize = True
        Me.chbFilesSaveOnHide.Location = New System.Drawing.Point(3, 59)
        Me.chbFilesSaveOnHide.Name = "chbFilesSaveOnHide"
        Me.chbFilesSaveOnHide.Size = New System.Drawing.Size(113, 21)
        Me.chbFilesSaveOnHide.TabIndex = 6
        Me.chbFilesSaveOnHide.Text = "Save on hide"
        Me.chbFilesSaveOnHide.UseVisualStyleBackColor = True
        '
        'lblFilesAutoSaveIntervalSeconds
        '
        Me.lblFilesAutoSaveIntervalSeconds.AutoSize = True
        Me.lblFilesAutoSaveIntervalSeconds.Location = New System.Drawing.Point(211, 5)
        Me.lblFilesAutoSaveIntervalSeconds.Name = "lblFilesAutoSaveIntervalSeconds"
        Me.lblFilesAutoSaveIntervalSeconds.Size = New System.Drawing.Size(61, 17)
        Me.lblFilesAutoSaveIntervalSeconds.TabIndex = 2
        Me.lblFilesAutoSaveIntervalSeconds.Text = "seconds"
        '
        'nudFilesAutoSaveInterval
        '
        Me.nudFilesAutoSaveInterval.Location = New System.Drawing.Point(155, 3)
        Me.nudFilesAutoSaveInterval.Maximum = New Decimal(New Integer() {300, 0, 0, 0})
        Me.nudFilesAutoSaveInterval.Minimum = New Decimal(New Integer() {15, 0, 0, 0})
        Me.nudFilesAutoSaveInterval.Name = "nudFilesAutoSaveInterval"
        Me.nudFilesAutoSaveInterval.Size = New System.Drawing.Size(50, 22)
        Me.nudFilesAutoSaveInterval.TabIndex = 1
        Me.nudFilesAutoSaveInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nudFilesAutoSaveInterval.Value = New Decimal(New Integer() {60, 0, 0, 0})
        '
        'lblFilesAutoSaveInterval
        '
        Me.lblFilesAutoSaveInterval.AutoSize = True
        Me.lblFilesAutoSaveInterval.Location = New System.Drawing.Point(3, 5)
        Me.lblFilesAutoSaveInterval.Name = "lblFilesAutoSaveInterval"
        Me.lblFilesAutoSaveInterval.Size = New System.Drawing.Size(126, 17)
        Me.lblFilesAutoSaveInterval.TabIndex = 0
        Me.lblFilesAutoSaveInterval.Text = "Auto-save interval:"
        '
        'chkDeleteToRecycleBin
        '
        Me.chkDeleteToRecycleBin.AutoSize = True
        Me.chkDeleteToRecycleBin.Location = New System.Drawing.Point(3, 119)
        Me.chkDeleteToRecycleBin.Name = "chkDeleteToRecycleBin"
        Me.chkDeleteToRecycleBin.Size = New System.Drawing.Size(159, 21)
        Me.chkDeleteToRecycleBin.TabIndex = 8
        Me.chkDeleteToRecycleBin.Text = "Delete to recycle bin"
        Me.chkDeleteToRecycleBin.UseVisualStyleBackColor = True
        '
        'chkPreloadFilesOnStartup
        '
        Me.chkPreloadFilesOnStartup.AutoSize = True
        Me.chkPreloadFilesOnStartup.Location = New System.Drawing.Point(3, 89)
        Me.chkPreloadFilesOnStartup.Name = "chkPreloadFilesOnStartup"
        Me.chkPreloadFilesOnStartup.Size = New System.Drawing.Size(176, 21)
        Me.chkPreloadFilesOnStartup.TabIndex = 7
        Me.chkPreloadFilesOnStartup.Text = "Preload files on startup"
        Me.chkPreloadFilesOnStartup.UseVisualStyleBackColor = True
        '
        'chkRememberSelectedFile
        '
        Me.chkRememberSelectedFile.AutoSize = True
        Me.chkRememberSelectedFile.Location = New System.Drawing.Point(3, 149)
        Me.chkRememberSelectedFile.Name = "chkRememberSelectedFile"
        Me.chkRememberSelectedFile.Size = New System.Drawing.Size(178, 21)
        Me.chkRememberSelectedFile.TabIndex = 9
        Me.chkRememberSelectedFile.Text = "Remember selected file"
        Me.chkRememberSelectedFile.UseVisualStyleBackColor = True
        '
        'btnChangeLocation
        '
        Me.btnChangeLocation.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnChangeLocation.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnChangeLocation.Location = New System.Drawing.Point(303, 344)
        Me.btnChangeLocation.Name = "btnChangeLocation"
        Me.btnChangeLocation.Size = New System.Drawing.Size(196, 29)
        Me.btnChangeLocation.TabIndex = 11
        Me.btnChangeLocation.Text = "Change location"
        Me.btnChangeLocation.UseVisualStyleBackColor = True
        '
        'tab_pagBehavior
        '
        Me.tab_pagBehavior.Controls.Add(Me.txtHotkey)
        Me.tab_pagBehavior.Controls.Add(Me.lblHotkey)
        Me.tab_pagBehavior.Controls.Add(Me.chkShowOnStartup)
        Me.tab_pagBehavior.Controls.Add(Me.chkRunAtStartup)
        Me.tab_pagBehavior.Controls.Add(Me.chkSingleClickTrayActivation)
        Me.tab_pagBehavior.Controls.Add(Me.chkMinimizeToTray)
        Me.tab_pagBehavior.Location = New System.Drawing.Point(4, 25)
        Me.tab_pagBehavior.Name = "tab_pagBehavior"
        Me.tab_pagBehavior.Size = New System.Drawing.Size(502, 376)
        Me.tab_pagBehavior.TabIndex = 2
        Me.tab_pagBehavior.Text = "Behavior"
        Me.tab_pagBehavior.UseVisualStyleBackColor = True
        '
        'txtHotkey
        '
        Me.txtHotkey.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtHotkey.Location = New System.Drawing.Point(153, 3)
        Me.txtHotkey.Name = "txtHotkey"
        Me.txtHotkey.ReadOnly = True
        Me.txtHotkey.Size = New System.Drawing.Size(346, 22)
        Me.txtHotkey.TabIndex = 7
        '
        'lblHotkey
        '
        Me.lblHotkey.AutoSize = True
        Me.lblHotkey.Location = New System.Drawing.Point(3, 6)
        Me.lblHotkey.Name = "lblHotkey"
        Me.lblHotkey.Size = New System.Drawing.Size(56, 17)
        Me.lblHotkey.TabIndex = 6
        Me.lblHotkey.Text = "Hotkey:"
        '
        'chkShowOnStartup
        '
        Me.chkShowOnStartup.AutoSize = True
        Me.chkShowOnStartup.Location = New System.Drawing.Point(3, 122)
        Me.chkShowOnStartup.Name = "chkShowOnStartup"
        Me.chkShowOnStartup.Size = New System.Drawing.Size(132, 21)
        Me.chkShowOnStartup.TabIndex = 4
        Me.chkShowOnStartup.Text = "Show on startup"
        Me.chkShowOnStartup.UseVisualStyleBackColor = True
        '
        'chkRunAtStartup
        '
        Me.chkRunAtStartup.AutoSize = True
        Me.chkRunAtStartup.Location = New System.Drawing.Point(3, 95)
        Me.chkRunAtStartup.Name = "chkRunAtStartup"
        Me.chkRunAtStartup.Size = New System.Drawing.Size(184, 21)
        Me.chkRunAtStartup.TabIndex = 3
        Me.chkRunAtStartup.Text = "Run on Windows startup"
        Me.chkRunAtStartup.UseVisualStyleBackColor = True
        '
        'chkSingleClickTrayActivation
        '
        Me.chkSingleClickTrayActivation.AutoSize = True
        Me.chkSingleClickTrayActivation.Location = New System.Drawing.Point(3, 65)
        Me.chkSingleClickTrayActivation.Name = "chkSingleClickTrayActivation"
        Me.chkSingleClickTrayActivation.Size = New System.Drawing.Size(223, 21)
        Me.chkSingleClickTrayActivation.TabIndex = 2
        Me.chkSingleClickTrayActivation.Text = "Tray activation with single click"
        Me.chkSingleClickTrayActivation.UseVisualStyleBackColor = True
        '
        'chkMinimizeToTray
        '
        Me.chkMinimizeToTray.AutoSize = True
        Me.chkMinimizeToTray.Location = New System.Drawing.Point(3, 38)
        Me.chkMinimizeToTray.Name = "chkMinimizeToTray"
        Me.chkMinimizeToTray.Size = New System.Drawing.Size(128, 21)
        Me.chkMinimizeToTray.TabIndex = 1
        Me.chkMinimizeToTray.Text = "Minimize to tray"
        Me.chkMinimizeToTray.UseVisualStyleBackColor = True
        '
        'tab_pagBackup
        '
        Me.tab_pagBackup.Controls.Add(Me.btnBackupFolderSelect)
        Me.tab_pagBackup.Controls.Add(Me.chbBackupIgnoreCopyErrors)
        Me.tab_pagBackup.Controls.Add(Me.chbUseBackup)
        Me.tab_pagBackup.Controls.Add(Me.chbBackupCreateFolder)
        Me.tab_pagBackup.Controls.Add(Me.txtBackupFolder)
        Me.tab_pagBackup.Controls.Add(Me.lblBackupFolder)
        Me.tab_pagBackup.Controls.Add(Me.btnBackupOpenFolder)
        Me.tab_pagBackup.Controls.Add(Me.btnBackupRestore)
        Me.tab_pagBackup.Location = New System.Drawing.Point(4, 25)
        Me.tab_pagBackup.Name = "tab_pagBackup"
        Me.tab_pagBackup.Size = New System.Drawing.Size(502, 376)
        Me.tab_pagBackup.TabIndex = 5
        Me.tab_pagBackup.Text = "Backup"
        Me.tab_pagBackup.UseVisualStyleBackColor = True
        '
        'btnBackupFolderSelect
        '
        Me.btnBackupFolderSelect.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBackupFolderSelect.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.btnBackupFolderSelect.Location = New System.Drawing.Point(477, 30)
        Me.btnBackupFolderSelect.Margin = New System.Windows.Forms.Padding(0, 3, 3, 3)
        Me.btnBackupFolderSelect.Name = "btnBackupFolderSelect"
        Me.btnBackupFolderSelect.Size = New System.Drawing.Size(22, 22)
        Me.btnBackupFolderSelect.TabIndex = 3
        Me.btnBackupFolderSelect.Text = "..."
        Me.btnBackupFolderSelect.UseVisualStyleBackColor = True
        '
        'chbBackupIgnoreCopyErrors
        '
        Me.chbBackupIgnoreCopyErrors.AutoSize = True
        Me.chbBackupIgnoreCopyErrors.Location = New System.Drawing.Point(23, 85)
        Me.chbBackupIgnoreCopyErrors.Name = "chbBackupIgnoreCopyErrors"
        Me.chbBackupIgnoreCopyErrors.Size = New System.Drawing.Size(162, 21)
        Me.chbBackupIgnoreCopyErrors.TabIndex = 5
        Me.chbBackupIgnoreCopyErrors.Text = "Ignore backup errors"
        Me.chbBackupIgnoreCopyErrors.UseVisualStyleBackColor = True
        '
        'chbUseBackup
        '
        Me.chbUseBackup.AutoSize = True
        Me.chbUseBackup.Location = New System.Drawing.Point(3, 3)
        Me.chbUseBackup.Name = "chbUseBackup"
        Me.chbUseBackup.Size = New System.Drawing.Size(130, 21)
        Me.chbUseBackup.TabIndex = 0
        Me.chbUseBackup.Text = "Activate backup"
        Me.chbUseBackup.UseVisualStyleBackColor = True
        '
        'chbBackupCreateFolder
        '
        Me.chbBackupCreateFolder.AutoSize = True
        Me.chbBackupCreateFolder.Location = New System.Drawing.Point(23, 58)
        Me.chbBackupCreateFolder.Name = "chbBackupCreateFolder"
        Me.chbBackupCreateFolder.Size = New System.Drawing.Size(193, 21)
        Me.chbBackupCreateFolder.TabIndex = 4
        Me.chbBackupCreateFolder.Text = "Create if one doesn't exist"
        Me.chbBackupCreateFolder.UseVisualStyleBackColor = True
        '
        'txtBackupFolder
        '
        Me.txtBackupFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtBackupFolder.Location = New System.Drawing.Point(153, 30)
        Me.txtBackupFolder.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
        Me.txtBackupFolder.Name = "txtBackupFolder"
        Me.txtBackupFolder.Size = New System.Drawing.Size(324, 22)
        Me.txtBackupFolder.TabIndex = 2
        '
        'lblBackupFolder
        '
        Me.lblBackupFolder.AutoSize = True
        Me.lblBackupFolder.Location = New System.Drawing.Point(23, 33)
        Me.lblBackupFolder.Name = "lblBackupFolder"
        Me.lblBackupFolder.Size = New System.Drawing.Size(52, 17)
        Me.lblBackupFolder.TabIndex = 1
        Me.lblBackupFolder.Text = "Folder:"
        '
        'btnBackupOpenFolder
        '
        Me.btnBackupOpenFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnBackupOpenFolder.Location = New System.Drawing.Point(3, 344)
        Me.btnBackupOpenFolder.Name = "btnBackupOpenFolder"
        Me.btnBackupOpenFolder.Size = New System.Drawing.Size(196, 29)
        Me.btnBackupOpenFolder.TabIndex = 6
        Me.btnBackupOpenFolder.Text = "Open folder"
        Me.btnBackupOpenFolder.UseVisualStyleBackColor = True
        '
        'btnBackupRestore
        '
        Me.btnBackupRestore.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBackupRestore.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnBackupRestore.Location = New System.Drawing.Point(303, 344)
        Me.btnBackupRestore.Name = "btnBackupRestore"
        Me.btnBackupRestore.Size = New System.Drawing.Size(196, 29)
        Me.btnBackupRestore.TabIndex = 7
        Me.btnBackupRestore.Text = "Restore from backup"
        Me.btnBackupRestore.UseVisualStyleBackColor = True
        Me.btnBackupRestore.Visible = False
        '
        'tab_pagCarbonCopy
        '
        Me.tab_pagCarbonCopy.Controls.Add(Me.btnCarbonCopyOpenFolder)
        Me.tab_pagCarbonCopy.Controls.Add(Me.btnCarbonCopyFolderSelect)
        Me.tab_pagCarbonCopy.Controls.Add(Me.chbCarbonCopyIgnoreCopyErrors)
        Me.tab_pagCarbonCopy.Controls.Add(Me.chbUseCarbonCopy)
        Me.tab_pagCarbonCopy.Controls.Add(Me.chkCarbonCopyFolderCreate)
        Me.tab_pagCarbonCopy.Controls.Add(Me.txtCarbonCopyFolder)
        Me.tab_pagCarbonCopy.Controls.Add(Me.lblCarbonCopyFolder)
        Me.tab_pagCarbonCopy.Location = New System.Drawing.Point(4, 25)
        Me.tab_pagCarbonCopy.Name = "tab_pagCarbonCopy"
        Me.tab_pagCarbonCopy.Size = New System.Drawing.Size(502, 376)
        Me.tab_pagCarbonCopy.TabIndex = 3
        Me.tab_pagCarbonCopy.Text = "Carbon copy"
        Me.tab_pagCarbonCopy.UseVisualStyleBackColor = True
        '
        'btnCarbonCopyOpenFolder
        '
        Me.btnCarbonCopyOpenFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCarbonCopyOpenFolder.Location = New System.Drawing.Point(3, 344)
        Me.btnCarbonCopyOpenFolder.Name = "btnCarbonCopyOpenFolder"
        Me.btnCarbonCopyOpenFolder.Size = New System.Drawing.Size(196, 29)
        Me.btnCarbonCopyOpenFolder.TabIndex = 6
        Me.btnCarbonCopyOpenFolder.Text = "Open folder"
        Me.btnCarbonCopyOpenFolder.UseVisualStyleBackColor = True
        '
        'btnCarbonCopyFolderSelect
        '
        Me.btnCarbonCopyFolderSelect.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.btnCarbonCopyFolderSelect.Location = New System.Drawing.Point(477, 32)
        Me.btnCarbonCopyFolderSelect.Margin = New System.Windows.Forms.Padding(0, 3, 3, 3)
        Me.btnCarbonCopyFolderSelect.Name = "btnCarbonCopyFolderSelect"
        Me.btnCarbonCopyFolderSelect.Size = New System.Drawing.Size(22, 22)
        Me.btnCarbonCopyFolderSelect.TabIndex = 3
        Me.btnCarbonCopyFolderSelect.Text = "..."
        Me.btnCarbonCopyFolderSelect.UseVisualStyleBackColor = True
        '
        'chbCarbonCopyIgnoreCopyErrors
        '
        Me.chbCarbonCopyIgnoreCopyErrors.AutoSize = True
        Me.chbCarbonCopyIgnoreCopyErrors.Location = New System.Drawing.Point(23, 87)
        Me.chbCarbonCopyIgnoreCopyErrors.Name = "chbCarbonCopyIgnoreCopyErrors"
        Me.chbCarbonCopyIgnoreCopyErrors.Size = New System.Drawing.Size(146, 21)
        Me.chbCarbonCopyIgnoreCopyErrors.TabIndex = 5
        Me.chbCarbonCopyIgnoreCopyErrors.Text = "Ignore copy errors"
        Me.chbCarbonCopyIgnoreCopyErrors.UseVisualStyleBackColor = True
        '
        'chbUseCarbonCopy
        '
        Me.chbUseCarbonCopy.AutoSize = True
        Me.chbUseCarbonCopy.Location = New System.Drawing.Point(3, 3)
        Me.chbUseCarbonCopy.Name = "chbUseCarbonCopy"
        Me.chbUseCarbonCopy.Size = New System.Drawing.Size(162, 21)
        Me.chbUseCarbonCopy.TabIndex = 0
        Me.chbUseCarbonCopy.Text = "Activate carbon copy"
        Me.chbUseCarbonCopy.UseVisualStyleBackColor = True
        '
        'chkCarbonCopyFolderCreate
        '
        Me.chkCarbonCopyFolderCreate.AutoSize = True
        Me.chkCarbonCopyFolderCreate.Location = New System.Drawing.Point(23, 60)
        Me.chkCarbonCopyFolderCreate.Name = "chkCarbonCopyFolderCreate"
        Me.chkCarbonCopyFolderCreate.Size = New System.Drawing.Size(193, 21)
        Me.chkCarbonCopyFolderCreate.TabIndex = 4
        Me.chkCarbonCopyFolderCreate.Text = "Create if one doesn't exist"
        Me.chkCarbonCopyFolderCreate.UseVisualStyleBackColor = True
        '
        'txtCarbonCopyFolder
        '
        Me.txtCarbonCopyFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCarbonCopyFolder.Location = New System.Drawing.Point(153, 32)
        Me.txtCarbonCopyFolder.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
        Me.txtCarbonCopyFolder.Name = "txtCarbonCopyFolder"
        Me.txtCarbonCopyFolder.Size = New System.Drawing.Size(324, 22)
        Me.txtCarbonCopyFolder.TabIndex = 2
        '
        'lblCarbonCopyFolder
        '
        Me.lblCarbonCopyFolder.AutoSize = True
        Me.lblCarbonCopyFolder.Location = New System.Drawing.Point(23, 35)
        Me.lblCarbonCopyFolder.Name = "lblCarbonCopyFolder"
        Me.lblCarbonCopyFolder.Size = New System.Drawing.Size(52, 17)
        Me.lblCarbonCopyFolder.TabIndex = 1
        Me.lblCarbonCopyFolder.Text = "Folder:"
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOk.Location = New System.Drawing.Point(320, 426)
        Me.btnOk.Margin = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(98, 29)
        Me.btnOk.TabIndex = 1
        Me.btnOk.Text = "OK"
        Me.btnOk.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(424, 426)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(98, 29)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'OptionsForm
        '
        Me.AcceptButton = Me.btnOk
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(534, 467)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.tab)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "OptionsForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Options"
        Me.tab.ResumeLayout(False)
        Me.tab_pagAppearance.ResumeLayout(False)
        Me.tab_pagAppearance.PerformLayout()
        CType(Me.nudTabWidth, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tab_pagDisplay.ResumeLayout(False)
        Me.tab_pagDisplay.PerformLayout()
        Me.tab_pagFiles.ResumeLayout(False)
        Me.tab_pagFiles.PerformLayout()
        CType(Me.nudQuickAutoSaveSeconds, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudFilesAutoSaveInterval, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tab_pagBehavior.ResumeLayout(False)
        Me.tab_pagBehavior.PerformLayout()
        Me.tab_pagBackup.ResumeLayout(False)
        Me.tab_pagBackup.PerformLayout()
        Me.tab_pagCarbonCopy.ResumeLayout(False)
        Me.tab_pagCarbonCopy.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
	Friend WithEvents tab As System.Windows.Forms.TabControl
	Friend WithEvents tab_pagFiles As System.Windows.Forms.TabPage
	Friend WithEvents tab_pagDisplay As System.Windows.Forms.TabPage
	Friend WithEvents btnOk As System.Windows.Forms.Button
	Friend WithEvents btnCancel As System.Windows.Forms.Button
	Friend WithEvents btnChangeLocation As System.Windows.Forms.Button
	Friend WithEvents tab_pagBehavior As System.Windows.Forms.TabPage
	Friend WithEvents chkMinimizeToTray As System.Windows.Forms.CheckBox
	Friend WithEvents chkSingleClickTrayActivation As System.Windows.Forms.CheckBox
	Friend WithEvents chkRunAtStartup As System.Windows.Forms.CheckBox
	Friend WithEvents chkShowOnStartup As System.Windows.Forms.CheckBox
    Friend WithEvents chkRememberSelectedFile As System.Windows.Forms.CheckBox
    Friend WithEvents chkPreloadFilesOnStartup As System.Windows.Forms.CheckBox
	Friend WithEvents chkDeleteToRecycleBin As System.Windows.Forms.CheckBox
	Friend WithEvents chkShowMenu As System.Windows.Forms.CheckBox
	Friend WithEvents chkShowToolbar As System.Windows.Forms.CheckBox
	Friend WithEvents chkShowInTaskbar As System.Windows.Forms.CheckBox
	Friend WithEvents tab_pagCarbonCopy As System.Windows.Forms.TabPage
	Friend WithEvents chbMultilineTabHeaders As System.Windows.Forms.CheckBox
	Friend WithEvents chbShowMinimizeMaximizeButtons As System.Windows.Forms.CheckBox
    Friend WithEvents txtHotkey As System.Windows.Forms.TextBox
	Friend WithEvents lblHotkey As System.Windows.Forms.Label
	Friend WithEvents txtCarbonCopyFolder As System.Windows.Forms.TextBox
	Friend WithEvents lblCarbonCopyFolder As System.Windows.Forms.Label
	Friend WithEvents chkCarbonCopyFolderCreate As System.Windows.Forms.CheckBox
	Friend WithEvents chbUseCarbonCopy As System.Windows.Forms.CheckBox
	Friend WithEvents chbCarbonCopyIgnoreCopyErrors As System.Windows.Forms.CheckBox
	Friend WithEvents btnCarbonCopyFolderSelect As System.Windows.Forms.Button
	Friend WithEvents nudFilesAutoSaveInterval As System.Windows.Forms.NumericUpDown
	Friend WithEvents lblFilesAutoSaveInterval As System.Windows.Forms.Label
	Friend WithEvents lblFilesAutoSaveIntervalSeconds As System.Windows.Forms.Label
	Friend WithEvents chbFilesSaveOnHide As System.Windows.Forms.CheckBox
	Friend WithEvents tab_pagAppearance As System.Windows.Forms.TabPage
	Friend WithEvents chkFollowURLs As System.Windows.Forms.CheckBox
	Friend WithEvents chkDisplayURLs As System.Windows.Forms.CheckBox
    Friend WithEvents lblColorExample As System.Windows.Forms.Label
	Friend WithEvents btnColorBackground As System.Windows.Forms.Button
	Friend WithEvents btnColorForeground As System.Windows.Forms.Button
	Friend WithEvents lblColor As System.Windows.Forms.Label
	Friend WithEvents txtFont As System.Windows.Forms.TextBox
	Friend WithEvents lblFont As System.Windows.Forms.Label
	Friend WithEvents btnFont As System.Windows.Forms.Button
	Friend WithEvents nudTabWidth As System.Windows.Forms.NumericUpDown
	Friend WithEvents lblTabWidth As System.Windows.Forms.Label
	Friend WithEvents chbVerticalScrollbar As System.Windows.Forms.CheckBox
	Friend WithEvents chbHorizontalScrollbar As System.Windows.Forms.CheckBox
	Friend WithEvents btnOpenLocationFolder As System.Windows.Forms.Button
	Friend WithEvents lblQuickAutoSaveSeconds As System.Windows.Forms.Label
	Friend WithEvents nudQuickAutoSaveSeconds As System.Windows.Forms.NumericUpDown
	Friend WithEvents chbEnableQuickAutoSave As System.Windows.Forms.CheckBox
	Friend WithEvents btnCarbonCopyOpenFolder As System.Windows.Forms.Button
	Friend WithEvents tab_pagBackup As System.Windows.Forms.TabPage
	Friend WithEvents btnBackupFolderSelect As System.Windows.Forms.Button
	Friend WithEvents chbBackupIgnoreCopyErrors As System.Windows.Forms.CheckBox
	Friend WithEvents chbUseBackup As System.Windows.Forms.CheckBox
	Friend WithEvents chbBackupCreateFolder As System.Windows.Forms.CheckBox
	Friend WithEvents txtBackupFolder As System.Windows.Forms.TextBox
	Friend WithEvents lblBackupFolder As System.Windows.Forms.Label
	Friend WithEvents btnBackupOpenFolder As System.Windows.Forms.Button
    Friend WithEvents btnBackupRestore As System.Windows.Forms.Button
    Friend WithEvents chbZoomToolbarWithDpi As System.Windows.Forms.CheckBox
End Class
