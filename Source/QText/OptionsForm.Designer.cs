namespace QText {
    partial class OptionsForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.tab = new System.Windows.Forms.TabControl();
            this.tab_pagAppearance = new System.Windows.Forms.TabPage();
            this.lblColorExample = new System.Windows.Forms.Label();
            this.btnColorBackground = new System.Windows.Forms.Button();
            this.btnColorForeground = new System.Windows.Forms.Button();
            this.lblColor = new System.Windows.Forms.Label();
            this.lblFont = new System.Windows.Forms.Label();
            this.btnFont = new System.Windows.Forms.Button();
            this.nudTabWidth = new System.Windows.Forms.NumericUpDown();
            this.lblTabWidth = new System.Windows.Forms.Label();
            this.chkFollowURLs = new System.Windows.Forms.CheckBox();
            this.chkDisplayURLs = new System.Windows.Forms.CheckBox();
            this.txtFont = new System.Windows.Forms.TextBox();
            this.tab_pagDisplay = new System.Windows.Forms.TabPage();
            this.cmbSelectionDelimiters = new System.Windows.Forms.ComboBox();
            this.lblSelectionDelimiters = new System.Windows.Forms.Label();
            this.chbMultilineTabs = new System.Windows.Forms.CheckBox();
            this.chbVerticalScrollbar = new System.Windows.Forms.CheckBox();
            this.chbHorizontalScrollbar = new System.Windows.Forms.CheckBox();
            this.chbShowMinimizeMaximizeButtons = new System.Windows.Forms.CheckBox();
            this.chkShowToolbar = new System.Windows.Forms.CheckBox();
            this.chkShowInTaskbar = new System.Windows.Forms.CheckBox();
            this.tab_pagFiles = new System.Windows.Forms.TabPage();
            this.chbSavePlainWithLF = new System.Windows.Forms.CheckBox();
            this.nudQuickSaveIntervalInSeconds = new System.Windows.Forms.NumericUpDown();
            this.btnOpenLocationFolder = new System.Windows.Forms.Button();
            this.lblQuickSaveIntervalSeconds = new System.Windows.Forms.Label();
            this.lblQuickSaveInterval = new System.Windows.Forms.Label();
            this.chkDeleteToRecycleBin = new System.Windows.Forms.CheckBox();
            this.chkPreloadFilesOnStartup = new System.Windows.Forms.CheckBox();
            this.btnChangeLocation = new System.Windows.Forms.Button();
            this.tab_pagBehavior = new System.Windows.Forms.TabPage();
            this.txtHotkey = new System.Windows.Forms.TextBox();
            this.lblHotkey = new System.Windows.Forms.Label();
            this.chkRunAtStartup = new System.Windows.Forms.CheckBox();
            this.chkSingleClickTrayActivation = new System.Windows.Forms.CheckBox();
            this.chkMinimizeToTray = new System.Windows.Forms.CheckBox();
            this.tab_pagCarbonCopy = new System.Windows.Forms.TabPage();
            this.btnCarbonCopyOpenFolder = new System.Windows.Forms.Button();
            this.btnCarbonCopyFolderSelect = new System.Windows.Forms.Button();
            this.chbCarbonCopyIgnoreCopyErrors = new System.Windows.Forms.CheckBox();
            this.chbUseCarbonCopy = new System.Windows.Forms.CheckBox();
            this.txtCarbonCopyFolder = new System.Windows.Forms.TextBox();
            this.lblCarbonCopyFolder = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnAllowSave = new System.Windows.Forms.Button();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.tab.SuspendLayout();
            this.tab_pagAppearance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTabWidth)).BeginInit();
            this.tab_pagDisplay.SuspendLayout();
            this.tab_pagFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuickSaveIntervalInSeconds)).BeginInit();
            this.tab_pagBehavior.SuspendLayout();
            this.tab_pagCarbonCopy.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab
            // 
            this.tab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tab.Controls.Add(this.tab_pagAppearance);
            this.tab.Controls.Add(this.tab_pagDisplay);
            this.tab.Controls.Add(this.tab_pagFiles);
            this.tab.Controls.Add(this.tab_pagBehavior);
            this.tab.Controls.Add(this.tab_pagCarbonCopy);
            this.tab.Location = new System.Drawing.Point(12, 12);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(498, 251);
            this.tab.TabIndex = 1;
            // 
            // tab_pagAppearance
            // 
            this.tab_pagAppearance.Controls.Add(this.lblColorExample);
            this.tab_pagAppearance.Controls.Add(this.btnColorBackground);
            this.tab_pagAppearance.Controls.Add(this.btnColorForeground);
            this.tab_pagAppearance.Controls.Add(this.lblColor);
            this.tab_pagAppearance.Controls.Add(this.lblFont);
            this.tab_pagAppearance.Controls.Add(this.btnFont);
            this.tab_pagAppearance.Controls.Add(this.nudTabWidth);
            this.tab_pagAppearance.Controls.Add(this.lblTabWidth);
            this.tab_pagAppearance.Controls.Add(this.chkFollowURLs);
            this.tab_pagAppearance.Controls.Add(this.chkDisplayURLs);
            this.tab_pagAppearance.Controls.Add(this.txtFont);
            this.tab_pagAppearance.Location = new System.Drawing.Point(4, 25);
            this.tab_pagAppearance.Name = "tab_pagAppearance";
            this.tab_pagAppearance.Size = new System.Drawing.Size(490, 222);
            this.tab_pagAppearance.TabIndex = 4;
            this.tab_pagAppearance.Text = "Appearance";
            this.tab_pagAppearance.UseVisualStyleBackColor = true;
            // 
            // lblColorExample
            // 
            this.lblColorExample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColorExample.Location = new System.Drawing.Point(123, 119);
            this.lblColorExample.Name = "lblColorExample";
            this.lblColorExample.Size = new System.Drawing.Size(29, 29);
            this.lblColorExample.TabIndex = 8;
            this.lblColorExample.Text = "C";
            this.lblColorExample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tooltip.SetToolTip(this.lblColorExample, "Background and foreground color for plain text.");
            // 
            // btnColorBackground
            // 
            this.btnColorBackground.Location = new System.Drawing.Point(158, 119);
            this.btnColorBackground.Name = "btnColorBackground";
            this.btnColorBackground.Size = new System.Drawing.Size(98, 29);
            this.btnColorBackground.TabIndex = 9;
            this.btnColorBackground.Text = "Background";
            this.tooltip.SetToolTip(this.btnColorBackground, "Browse background color.");
            this.btnColorBackground.UseVisualStyleBackColor = true;
            this.btnColorBackground.Click += new System.EventHandler(this.btnColorBackground_Click);
            // 
            // btnColorForeground
            // 
            this.btnColorForeground.Location = new System.Drawing.Point(262, 119);
            this.btnColorForeground.Name = "btnColorForeground";
            this.btnColorForeground.Size = new System.Drawing.Size(98, 29);
            this.btnColorForeground.TabIndex = 10;
            this.btnColorForeground.Text = "Foreground";
            this.tooltip.SetToolTip(this.btnColorForeground, "Browse foreground color.");
            this.btnColorForeground.UseVisualStyleBackColor = true;
            this.btnColorForeground.Click += new System.EventHandler(this.btnColorForeground_Click);
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(3, 125);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(52, 17);
            this.lblColor.TabIndex = 7;
            this.lblColor.Text = "Colors:";
            // 
            // lblFont
            // 
            this.lblFont.AutoSize = true;
            this.lblFont.Location = new System.Drawing.Point(3, 94);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(40, 17);
            this.lblFont.TabIndex = 4;
            this.lblFont.Text = "Font:";
            // 
            // btnFont
            // 
            this.btnFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFont.Font = new System.Drawing.Font("Microsoft Sans Serif", 4.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFont.Location = new System.Drawing.Point(460, 91);
            this.btnFont.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(22, 22);
            this.btnFont.TabIndex = 6;
            this.btnFont.Text = "...";
            this.tooltip.SetToolTip(this.btnFont, "Browse font selection.");
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // nudTabWidth
            // 
            this.nudTabWidth.Location = new System.Drawing.Point(123, 63);
            this.nudTabWidth.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudTabWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTabWidth.Name = "nudTabWidth";
            this.nudTabWidth.Size = new System.Drawing.Size(53, 22);
            this.nudTabWidth.TabIndex = 3;
            this.nudTabWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tooltip.SetToolTip(this.nudTabWidth, "Number of characters each tab will use for alignment.");
            this.nudTabWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblTabWidth
            // 
            this.lblTabWidth.AutoSize = true;
            this.lblTabWidth.Location = new System.Drawing.Point(3, 65);
            this.lblTabWidth.Name = "lblTabWidth";
            this.lblTabWidth.Size = new System.Drawing.Size(105, 17);
            this.lblTabWidth.TabIndex = 2;
            this.lblTabWidth.Text = "Tab char width:";
            // 
            // chkFollowURLs
            // 
            this.chkFollowURLs.AutoSize = true;
            this.chkFollowURLs.Location = new System.Drawing.Point(23, 30);
            this.chkFollowURLs.Name = "chkFollowURLs";
            this.chkFollowURLs.Size = new System.Drawing.Size(108, 21);
            this.chkFollowURLs.TabIndex = 1;
            this.chkFollowURLs.Text = "Follow URLs";
            this.tooltip.SetToolTip(this.chkFollowURLs, "Controls whether clicking the URL will ");
            this.chkFollowURLs.UseVisualStyleBackColor = true;
            // 
            // chkDisplayURLs
            // 
            this.chkDisplayURLs.AutoSize = true;
            this.chkDisplayURLs.Location = new System.Drawing.Point(3, 3);
            this.chkDisplayURLs.Name = "chkDisplayURLs";
            this.chkDisplayURLs.Size = new System.Drawing.Size(115, 21);
            this.chkDisplayURLs.TabIndex = 0;
            this.chkDisplayURLs.Text = "Display URLs";
            this.tooltip.SetToolTip(this.chkDisplayURLs, "If checked, URLs in text will be detected and displayed as such.");
            this.chkDisplayURLs.UseVisualStyleBackColor = true;
            this.chkDisplayURLs.CheckedChanged += new System.EventHandler(this.chkDisplayURLs_CheckedChanged);
            // 
            // txtFont
            // 
            this.txtFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFont.Location = new System.Drawing.Point(123, 91);
            this.txtFont.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtFont.Name = "txtFont";
            this.txtFont.ReadOnly = true;
            this.txtFont.Size = new System.Drawing.Size(337, 22);
            this.txtFont.TabIndex = 5;
            this.tooltip.SetToolTip(this.txtFont, "Font to use as a default. Rich text will use different fonts too if set.");
            // 
            // tab_pagDisplay
            // 
            this.tab_pagDisplay.Controls.Add(this.cmbSelectionDelimiters);
            this.tab_pagDisplay.Controls.Add(this.lblSelectionDelimiters);
            this.tab_pagDisplay.Controls.Add(this.chbMultilineTabs);
            this.tab_pagDisplay.Controls.Add(this.chbVerticalScrollbar);
            this.tab_pagDisplay.Controls.Add(this.chbHorizontalScrollbar);
            this.tab_pagDisplay.Controls.Add(this.chbShowMinimizeMaximizeButtons);
            this.tab_pagDisplay.Controls.Add(this.chkShowToolbar);
            this.tab_pagDisplay.Controls.Add(this.chkShowInTaskbar);
            this.tab_pagDisplay.Location = new System.Drawing.Point(4, 25);
            this.tab_pagDisplay.Name = "tab_pagDisplay";
            this.tab_pagDisplay.Size = new System.Drawing.Size(490, 222);
            this.tab_pagDisplay.TabIndex = 1;
            this.tab_pagDisplay.Text = "Display";
            this.tab_pagDisplay.UseVisualStyleBackColor = true;
            // 
            // cmbSelectionDelimiters
            // 
            this.cmbSelectionDelimiters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSelectionDelimiters.FormattingEnabled = true;
            this.cmbSelectionDelimiters.Items.AddRange(new object[] {
            "\"/()[]{}<>",
            "\".,:;!?/|\\()[]{}<>"});
            this.cmbSelectionDelimiters.Location = new System.Drawing.Point(167, 180);
            this.cmbSelectionDelimiters.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.cmbSelectionDelimiters.Name = "cmbSelectionDelimiters";
            this.cmbSelectionDelimiters.Size = new System.Drawing.Size(320, 24);
            this.cmbSelectionDelimiters.TabIndex = 7;
            this.tooltip.SetToolTip(this.cmbSelectionDelimiters, "Delimiters that will be used for double-click selection.");
            // 
            // lblSelectionDelimiters
            // 
            this.lblSelectionDelimiters.AutoSize = true;
            this.lblSelectionDelimiters.Location = new System.Drawing.Point(3, 183);
            this.lblSelectionDelimiters.Name = "lblSelectionDelimiters";
            this.lblSelectionDelimiters.Size = new System.Drawing.Size(134, 17);
            this.lblSelectionDelimiters.TabIndex = 6;
            this.lblSelectionDelimiters.Text = "Selection delimiters:";
            // 
            // chbMultilineTabs
            // 
            this.chbMultilineTabs.AutoSize = true;
            this.chbMultilineTabs.Location = new System.Drawing.Point(3, 87);
            this.chbMultilineTabs.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.chbMultilineTabs.Name = "chbMultilineTabs";
            this.chbMultilineTabs.Size = new System.Drawing.Size(161, 21);
            this.chbMultilineTabs.TabIndex = 3;
            this.chbMultilineTabs.Text = "Multiline tab headers";
            this.tooltip.SetToolTip(this.chbMultilineTabs, "Controls if tabs are to be shown in multiple lines when they cannot fit on screen" +
        ".");
            this.chbMultilineTabs.UseVisualStyleBackColor = true;
            // 
            // chbVerticalScrollbar
            // 
            this.chbVerticalScrollbar.AutoSize = true;
            this.chbVerticalScrollbar.Location = new System.Drawing.Point(3, 144);
            this.chbVerticalScrollbar.Name = "chbVerticalScrollbar";
            this.chbVerticalScrollbar.Size = new System.Drawing.Size(135, 21);
            this.chbVerticalScrollbar.TabIndex = 5;
            this.chbVerticalScrollbar.Text = "Vertical scrollbar";
            this.tooltip.SetToolTip(this.chbVerticalScrollbar, "If checked, vertical scrollbar will be shown.");
            this.chbVerticalScrollbar.UseVisualStyleBackColor = true;
            // 
            // chbHorizontalScrollbar
            // 
            this.chbHorizontalScrollbar.AutoSize = true;
            this.chbHorizontalScrollbar.Location = new System.Drawing.Point(3, 117);
            this.chbHorizontalScrollbar.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.chbHorizontalScrollbar.Name = "chbHorizontalScrollbar";
            this.chbHorizontalScrollbar.Size = new System.Drawing.Size(152, 21);
            this.chbHorizontalScrollbar.TabIndex = 4;
            this.chbHorizontalScrollbar.Text = "Horizontal scrollbar";
            this.tooltip.SetToolTip(this.chbHorizontalScrollbar, "Controls if horizontal scrolling is to be allowed. If not checked, text will wrap" +
        " on right edge.");
            this.chbHorizontalScrollbar.UseVisualStyleBackColor = true;
            // 
            // chbShowMinimizeMaximizeButtons
            // 
            this.chbShowMinimizeMaximizeButtons.AutoSize = true;
            this.chbShowMinimizeMaximizeButtons.Location = new System.Drawing.Point(3, 30);
            this.chbShowMinimizeMaximizeButtons.Name = "chbShowMinimizeMaximizeButtons";
            this.chbShowMinimizeMaximizeButtons.Size = new System.Drawing.Size(234, 21);
            this.chbShowMinimizeMaximizeButtons.TabIndex = 1;
            this.chbShowMinimizeMaximizeButtons.Text = "Show minimize/maximize buttons";
            this.tooltip.SetToolTip(this.chbShowMinimizeMaximizeButtons, "If checked, both minimize and maximize buttons will be shown.");
            this.chbShowMinimizeMaximizeButtons.UseVisualStyleBackColor = true;
            this.chbShowMinimizeMaximizeButtons.CheckedChanged += new System.EventHandler(this.chbShowMinimizeMaximizeButtons_CheckedChanged);
            // 
            // chkShowToolbar
            // 
            this.chkShowToolbar.AutoSize = true;
            this.chkShowToolbar.Location = new System.Drawing.Point(3, 57);
            this.chkShowToolbar.Name = "chkShowToolbar";
            this.chkShowToolbar.Size = new System.Drawing.Size(112, 21);
            this.chkShowToolbar.TabIndex = 2;
            this.chkShowToolbar.Text = "Show toolbar";
            this.tooltip.SetToolTip(this.chkShowToolbar, "If true, toolbar will be shown.");
            this.chkShowToolbar.UseVisualStyleBackColor = true;
            // 
            // chkShowInTaskbar
            // 
            this.chkShowInTaskbar.AutoSize = true;
            this.chkShowInTaskbar.Location = new System.Drawing.Point(3, 3);
            this.chkShowInTaskbar.Name = "chkShowInTaskbar";
            this.chkShowInTaskbar.Size = new System.Drawing.Size(130, 21);
            this.chkShowInTaskbar.TabIndex = 0;
            this.chkShowInTaskbar.Text = "Show in taskbar";
            this.tooltip.SetToolTip(this.chkShowInTaskbar, "If checked, QText window will be shown in taskbar.");
            this.chkShowInTaskbar.UseVisualStyleBackColor = true;
            this.chkShowInTaskbar.CheckedChanged += new System.EventHandler(this.chkShowInTaskbar_CheckedChanged);
            // 
            // tab_pagFiles
            // 
            this.tab_pagFiles.Controls.Add(this.chbSavePlainWithLF);
            this.tab_pagFiles.Controls.Add(this.nudQuickSaveIntervalInSeconds);
            this.tab_pagFiles.Controls.Add(this.btnOpenLocationFolder);
            this.tab_pagFiles.Controls.Add(this.lblQuickSaveIntervalSeconds);
            this.tab_pagFiles.Controls.Add(this.lblQuickSaveInterval);
            this.tab_pagFiles.Controls.Add(this.chkDeleteToRecycleBin);
            this.tab_pagFiles.Controls.Add(this.chkPreloadFilesOnStartup);
            this.tab_pagFiles.Controls.Add(this.btnChangeLocation);
            this.tab_pagFiles.Location = new System.Drawing.Point(4, 25);
            this.tab_pagFiles.Name = "tab_pagFiles";
            this.tab_pagFiles.Padding = new System.Windows.Forms.Padding(4);
            this.tab_pagFiles.Size = new System.Drawing.Size(490, 222);
            this.tab_pagFiles.TabIndex = 0;
            this.tab_pagFiles.Text = "Files";
            this.tab_pagFiles.UseVisualStyleBackColor = true;
            // 
            // chbSavePlainWithLF
            // 
            this.chbSavePlainWithLF.AutoSize = true;
            this.chbSavePlainWithLF.Location = new System.Drawing.Point(7, 60);
            this.chbSavePlainWithLF.Name = "chbSavePlainWithLF";
            this.chbSavePlainWithLF.Size = new System.Drawing.Size(243, 21);
            this.chbSavePlainWithLF.TabIndex = 4;
            this.chbSavePlainWithLF.Text = "Save plain text with LF line ending";
            this.tooltip.SetToolTip(this.chbSavePlainWithLF, "Controls if plain text will use CRLF (Windows) or LF (Linux) line endings.");
            this.chbSavePlainWithLF.UseVisualStyleBackColor = true;
            // 
            // nudQuickSaveIntervalInSeconds
            // 
            this.nudQuickSaveIntervalInSeconds.DecimalPlaces = 1;
            this.nudQuickSaveIntervalInSeconds.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudQuickSaveIntervalInSeconds.Location = new System.Drawing.Point(155, 5);
            this.nudQuickSaveIntervalInSeconds.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudQuickSaveIntervalInSeconds.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudQuickSaveIntervalInSeconds.Name = "nudQuickSaveIntervalInSeconds";
            this.nudQuickSaveIntervalInSeconds.Size = new System.Drawing.Size(50, 22);
            this.nudQuickSaveIntervalInSeconds.TabIndex = 1;
            this.nudQuickSaveIntervalInSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tooltip.SetToolTip(this.nudQuickSaveIntervalInSeconds, "Number of seconds to wait before saving a file. Each key press restarts timer.");
            this.nudQuickSaveIntervalInSeconds.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // btnOpenLocationFolder
            // 
            this.btnOpenLocationFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenLocationFolder.Location = new System.Drawing.Point(3, 190);
            this.btnOpenLocationFolder.Name = "btnOpenLocationFolder";
            this.btnOpenLocationFolder.Size = new System.Drawing.Size(196, 29);
            this.btnOpenLocationFolder.TabIndex = 6;
            this.btnOpenLocationFolder.Text = "Open folder";
            this.tooltip.SetToolTip(this.btnOpenLocationFolder, "Show all files in Explorer.");
            this.btnOpenLocationFolder.UseVisualStyleBackColor = true;
            this.btnOpenLocationFolder.Click += new System.EventHandler(this.btnOpenLocationFolder_Click);
            // 
            // lblQuickSaveIntervalSeconds
            // 
            this.lblQuickSaveIntervalSeconds.AutoSize = true;
            this.lblQuickSaveIntervalSeconds.Location = new System.Drawing.Point(211, 5);
            this.lblQuickSaveIntervalSeconds.Name = "lblQuickSaveIntervalSeconds";
            this.lblQuickSaveIntervalSeconds.Size = new System.Drawing.Size(61, 17);
            this.lblQuickSaveIntervalSeconds.TabIndex = 2;
            this.lblQuickSaveIntervalSeconds.Text = "seconds";
            // 
            // lblQuickSaveInterval
            // 
            this.lblQuickSaveInterval.AutoSize = true;
            this.lblQuickSaveInterval.Location = new System.Drawing.Point(3, 5);
            this.lblQuickSaveInterval.Name = "lblQuickSaveInterval";
            this.lblQuickSaveInterval.Size = new System.Drawing.Size(133, 17);
            this.lblQuickSaveInterval.TabIndex = 0;
            this.lblQuickSaveInterval.Text = "Quick-save interval:";
            // 
            // chkDeleteToRecycleBin
            // 
            this.chkDeleteToRecycleBin.AutoSize = true;
            this.chkDeleteToRecycleBin.Location = new System.Drawing.Point(7, 87);
            this.chkDeleteToRecycleBin.Name = "chkDeleteToRecycleBin";
            this.chkDeleteToRecycleBin.Size = new System.Drawing.Size(159, 21);
            this.chkDeleteToRecycleBin.TabIndex = 5;
            this.chkDeleteToRecycleBin.Text = "Delete to recycle bin";
            this.tooltip.SetToolTip(this.chkDeleteToRecycleBin, "If checked, all delete will be to recycle bin.");
            this.chkDeleteToRecycleBin.UseVisualStyleBackColor = true;
            // 
            // chkPreloadFilesOnStartup
            // 
            this.chkPreloadFilesOnStartup.AutoSize = true;
            this.chkPreloadFilesOnStartup.Location = new System.Drawing.Point(7, 33);
            this.chkPreloadFilesOnStartup.Name = "chkPreloadFilesOnStartup";
            this.chkPreloadFilesOnStartup.Size = new System.Drawing.Size(176, 21);
            this.chkPreloadFilesOnStartup.TabIndex = 3;
            this.chkPreloadFilesOnStartup.Text = "Preload files on startup";
            this.tooltip.SetToolTip(this.chkPreloadFilesOnStartup, "All files will be loaded in advance.");
            this.chkPreloadFilesOnStartup.UseVisualStyleBackColor = true;
            // 
            // btnChangeLocation
            // 
            this.btnChangeLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeLocation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnChangeLocation.Location = new System.Drawing.Point(291, 190);
            this.btnChangeLocation.Name = "btnChangeLocation";
            this.btnChangeLocation.Size = new System.Drawing.Size(196, 29);
            this.btnChangeLocation.TabIndex = 7;
            this.btnChangeLocation.Text = "Change location";
            this.tooltip.SetToolTip(this.btnChangeLocation, "Changes location of files.");
            this.btnChangeLocation.UseVisualStyleBackColor = true;
            this.btnChangeLocation.Click += new System.EventHandler(this.btnChangeLocation_Click);
            // 
            // tab_pagBehavior
            // 
            this.tab_pagBehavior.Controls.Add(this.txtHotkey);
            this.tab_pagBehavior.Controls.Add(this.lblHotkey);
            this.tab_pagBehavior.Controls.Add(this.chkRunAtStartup);
            this.tab_pagBehavior.Controls.Add(this.chkSingleClickTrayActivation);
            this.tab_pagBehavior.Controls.Add(this.chkMinimizeToTray);
            this.tab_pagBehavior.Location = new System.Drawing.Point(4, 25);
            this.tab_pagBehavior.Name = "tab_pagBehavior";
            this.tab_pagBehavior.Size = new System.Drawing.Size(490, 222);
            this.tab_pagBehavior.TabIndex = 2;
            this.tab_pagBehavior.Text = "Behavior";
            this.tab_pagBehavior.UseVisualStyleBackColor = true;
            // 
            // txtHotkey
            // 
            this.txtHotkey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHotkey.Location = new System.Drawing.Point(153, 3);
            this.txtHotkey.Name = "txtHotkey";
            this.txtHotkey.ReadOnly = true;
            this.txtHotkey.Size = new System.Drawing.Size(334, 22);
            this.txtHotkey.TabIndex = 1;
            this.tooltip.SetToolTip(this.txtHotkey, "Hotkey to use for bringing up the window.");
            this.txtHotkey.Enter += new System.EventHandler(this.txtHotkey_Enter);
            this.txtHotkey.Leave += new System.EventHandler(this.txtHotkey_Leave);
            this.txtHotkey.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHotkey_PreviewKeyDown);
            // 
            // lblHotkey
            // 
            this.lblHotkey.AutoSize = true;
            this.lblHotkey.Location = new System.Drawing.Point(3, 6);
            this.lblHotkey.Name = "lblHotkey";
            this.lblHotkey.Size = new System.Drawing.Size(56, 17);
            this.lblHotkey.TabIndex = 0;
            this.lblHotkey.Text = "Hotkey:";
            // 
            // chkRunAtStartup
            // 
            this.chkRunAtStartup.AutoSize = true;
            this.chkRunAtStartup.Location = new System.Drawing.Point(3, 95);
            this.chkRunAtStartup.Name = "chkRunAtStartup";
            this.chkRunAtStartup.Size = new System.Drawing.Size(184, 21);
            this.chkRunAtStartup.TabIndex = 4;
            this.chkRunAtStartup.Tag = "If checked, QText will be started in tray on Windows startup.";
            this.chkRunAtStartup.Text = "Run on Windows startup";
            this.chkRunAtStartup.UseVisualStyleBackColor = true;
            // 
            // chkSingleClickTrayActivation
            // 
            this.chkSingleClickTrayActivation.AutoSize = true;
            this.chkSingleClickTrayActivation.Location = new System.Drawing.Point(3, 65);
            this.chkSingleClickTrayActivation.Name = "chkSingleClickTrayActivation";
            this.chkSingleClickTrayActivation.Size = new System.Drawing.Size(223, 21);
            this.chkSingleClickTrayActivation.TabIndex = 3;
            this.chkSingleClickTrayActivation.Text = "Tray activation with single click";
            this.tooltip.SetToolTip(this.chkSingleClickTrayActivation, "Controls if tray icon will show window upon single or double click.");
            this.chkSingleClickTrayActivation.UseVisualStyleBackColor = true;
            // 
            // chkMinimizeToTray
            // 
            this.chkMinimizeToTray.AutoSize = true;
            this.chkMinimizeToTray.Location = new System.Drawing.Point(3, 38);
            this.chkMinimizeToTray.Name = "chkMinimizeToTray";
            this.chkMinimizeToTray.Size = new System.Drawing.Size(128, 21);
            this.chkMinimizeToTray.TabIndex = 2;
            this.chkMinimizeToTray.Text = "Minimize to tray";
            this.tooltip.SetToolTip(this.chkMinimizeToTray, "If checked, QText will minimize to tray.");
            this.chkMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // tab_pagCarbonCopy
            // 
            this.tab_pagCarbonCopy.Controls.Add(this.btnCarbonCopyOpenFolder);
            this.tab_pagCarbonCopy.Controls.Add(this.btnCarbonCopyFolderSelect);
            this.tab_pagCarbonCopy.Controls.Add(this.chbCarbonCopyIgnoreCopyErrors);
            this.tab_pagCarbonCopy.Controls.Add(this.chbUseCarbonCopy);
            this.tab_pagCarbonCopy.Controls.Add(this.txtCarbonCopyFolder);
            this.tab_pagCarbonCopy.Controls.Add(this.lblCarbonCopyFolder);
            this.tab_pagCarbonCopy.Location = new System.Drawing.Point(4, 25);
            this.tab_pagCarbonCopy.Name = "tab_pagCarbonCopy";
            this.tab_pagCarbonCopy.Size = new System.Drawing.Size(490, 222);
            this.tab_pagCarbonCopy.TabIndex = 3;
            this.tab_pagCarbonCopy.Text = "Carbon copy";
            this.tab_pagCarbonCopy.UseVisualStyleBackColor = true;
            // 
            // btnCarbonCopyOpenFolder
            // 
            this.btnCarbonCopyOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCarbonCopyOpenFolder.Location = new System.Drawing.Point(3, 190);
            this.btnCarbonCopyOpenFolder.Name = "btnCarbonCopyOpenFolder";
            this.btnCarbonCopyOpenFolder.Size = new System.Drawing.Size(196, 29);
            this.btnCarbonCopyOpenFolder.TabIndex = 6;
            this.btnCarbonCopyOpenFolder.Text = "Open folder";
            this.tooltip.SetToolTip(this.btnCarbonCopyOpenFolder, "Open backup folder in Explorer.");
            this.btnCarbonCopyOpenFolder.UseVisualStyleBackColor = true;
            this.btnCarbonCopyOpenFolder.Click += new System.EventHandler(this.btnCarbonCopyOpenFolder_Click);
            // 
            // btnCarbonCopyFolderSelect
            // 
            this.btnCarbonCopyFolderSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCarbonCopyFolderSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 5F);
            this.btnCarbonCopyFolderSelect.Location = new System.Drawing.Point(465, 32);
            this.btnCarbonCopyFolderSelect.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnCarbonCopyFolderSelect.Name = "btnCarbonCopyFolderSelect";
            this.btnCarbonCopyFolderSelect.Size = new System.Drawing.Size(22, 22);
            this.btnCarbonCopyFolderSelect.TabIndex = 3;
            this.btnCarbonCopyFolderSelect.Text = "...";
            this.tooltip.SetToolTip(this.btnCarbonCopyFolderSelect, "Browse for folder.");
            this.btnCarbonCopyFolderSelect.UseVisualStyleBackColor = true;
            this.btnCarbonCopyFolderSelect.Click += new System.EventHandler(this.btnCarbonCopyFolderSelect_Click);
            // 
            // chbCarbonCopyIgnoreCopyErrors
            // 
            this.chbCarbonCopyIgnoreCopyErrors.AutoSize = true;
            this.chbCarbonCopyIgnoreCopyErrors.Location = new System.Drawing.Point(19, 60);
            this.chbCarbonCopyIgnoreCopyErrors.Name = "chbCarbonCopyIgnoreCopyErrors";
            this.chbCarbonCopyIgnoreCopyErrors.Size = new System.Drawing.Size(146, 21);
            this.chbCarbonCopyIgnoreCopyErrors.TabIndex = 5;
            this.chbCarbonCopyIgnoreCopyErrors.Text = "Ignore copy errors";
            this.tooltip.SetToolTip(this.chbCarbonCopyIgnoreCopyErrors, "If true, backup copy errors are ignored.");
            this.chbCarbonCopyIgnoreCopyErrors.UseVisualStyleBackColor = true;
            // 
            // chbUseCarbonCopy
            // 
            this.chbUseCarbonCopy.AutoSize = true;
            this.chbUseCarbonCopy.Location = new System.Drawing.Point(3, 3);
            this.chbUseCarbonCopy.Name = "chbUseCarbonCopy";
            this.chbUseCarbonCopy.Size = new System.Drawing.Size(162, 21);
            this.chbUseCarbonCopy.TabIndex = 0;
            this.chbUseCarbonCopy.Text = "Activate carbon copy";
            this.tooltip.SetToolTip(this.chbUseCarbonCopy, "If true, all changes will be copied to another folder.");
            this.chbUseCarbonCopy.UseVisualStyleBackColor = true;
            this.chbUseCarbonCopy.CheckedChanged += new System.EventHandler(this.chbUseCarbonCopy_CheckedChanged);
            // 
            // txtCarbonCopyFolder
            // 
            this.txtCarbonCopyFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCarbonCopyFolder.Location = new System.Drawing.Point(153, 32);
            this.txtCarbonCopyFolder.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtCarbonCopyFolder.Name = "txtCarbonCopyFolder";
            this.txtCarbonCopyFolder.Size = new System.Drawing.Size(312, 22);
            this.txtCarbonCopyFolder.TabIndex = 2;
            this.tooltip.SetToolTip(this.txtCarbonCopyFolder, "Location where copies are stored.");
            this.txtCarbonCopyFolder.TextChanged += new System.EventHandler(this.txtCarbonCopyFolder_TextChanged);
            // 
            // lblCarbonCopyFolder
            // 
            this.lblCarbonCopyFolder.AutoSize = true;
            this.lblCarbonCopyFolder.Location = new System.Drawing.Point(23, 35);
            this.lblCarbonCopyFolder.Name = "lblCarbonCopyFolder";
            this.lblCarbonCopyFolder.Size = new System.Drawing.Size(52, 17);
            this.lblCarbonCopyFolder.TabIndex = 1;
            this.lblCarbonCopyFolder.Text = "Folder:";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(308, 272);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(98, 29);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.tooltip.SetToolTip(this.btnOk, "Apply all settings and close window.");
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(412, 272);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 29);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.tooltip.SetToolTip(this.btnCancel, "Cancel changes.");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExit.Location = new System.Drawing.Point(12, 272);
            this.btnExit.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(98, 29);
            this.btnExit.TabIndex = 2;
            this.btnExit.TabStop = false;
            this.btnExit.Text = "Exit";
            this.tooltip.SetToolTip(this.btnExit, "Exit the application.");
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnAllowSave
            // 
            this.btnAllowSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAllowSave.Location = new System.Drawing.Point(128, 272);
            this.btnAllowSave.Name = "btnAllowSave";
            this.btnAllowSave.Size = new System.Drawing.Size(98, 29);
            this.btnAllowSave.TabIndex = 3;
            this.btnAllowSave.TabStop = false;
            this.btnAllowSave.Text = "Allow save";
            this.tooltip.SetToolTip(this.btnAllowSave, "Allow save of settings in registry.");
            this.btnAllowSave.UseVisualStyleBackColor = true;
            this.btnAllowSave.Click += new System.EventHandler(this.btnAllowSave_Click);
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(522, 313);
            this.Controls.Add(this.btnAllowSave);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.tab);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsForm_FormClosing);
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.tab.ResumeLayout(false);
            this.tab_pagAppearance.ResumeLayout(false);
            this.tab_pagAppearance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTabWidth)).EndInit();
            this.tab_pagDisplay.ResumeLayout(false);
            this.tab_pagDisplay.PerformLayout();
            this.tab_pagFiles.ResumeLayout(false);
            this.tab_pagFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuickSaveIntervalInSeconds)).EndInit();
            this.tab_pagBehavior.ResumeLayout(false);
            this.tab_pagBehavior.PerformLayout();
            this.tab_pagCarbonCopy.ResumeLayout(false);
            this.tab_pagCarbonCopy.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TabControl tab;
        internal System.Windows.Forms.TabPage tab_pagAppearance;
        internal System.Windows.Forms.Label lblColorExample;
        internal System.Windows.Forms.Button btnColorBackground;
        internal System.Windows.Forms.Button btnColorForeground;
        internal System.Windows.Forms.Label lblColor;
        internal System.Windows.Forms.Label lblFont;
        internal System.Windows.Forms.Button btnFont;
        internal System.Windows.Forms.NumericUpDown nudTabWidth;
        internal System.Windows.Forms.Label lblTabWidth;
        internal System.Windows.Forms.CheckBox chkFollowURLs;
        internal System.Windows.Forms.CheckBox chkDisplayURLs;
        internal System.Windows.Forms.TextBox txtFont;
        internal System.Windows.Forms.TabPage tab_pagDisplay;
        internal System.Windows.Forms.CheckBox chbVerticalScrollbar;
        internal System.Windows.Forms.CheckBox chbHorizontalScrollbar;
        internal System.Windows.Forms.CheckBox chbShowMinimizeMaximizeButtons;
        internal System.Windows.Forms.CheckBox chkShowToolbar;
        internal System.Windows.Forms.CheckBox chkShowInTaskbar;
        internal System.Windows.Forms.TabPage tab_pagFiles;
        internal System.Windows.Forms.NumericUpDown nudQuickSaveIntervalInSeconds;
        internal System.Windows.Forms.Button btnOpenLocationFolder;
        internal System.Windows.Forms.Label lblQuickSaveIntervalSeconds;
        internal System.Windows.Forms.Label lblQuickSaveInterval;
        internal System.Windows.Forms.CheckBox chkDeleteToRecycleBin;
        internal System.Windows.Forms.CheckBox chkPreloadFilesOnStartup;
        internal System.Windows.Forms.Button btnChangeLocation;
        internal System.Windows.Forms.TabPage tab_pagBehavior;
        internal System.Windows.Forms.TextBox txtHotkey;
        internal System.Windows.Forms.Label lblHotkey;
        internal System.Windows.Forms.CheckBox chkRunAtStartup;
        internal System.Windows.Forms.CheckBox chkSingleClickTrayActivation;
        internal System.Windows.Forms.CheckBox chkMinimizeToTray;
        internal System.Windows.Forms.TabPage tab_pagCarbonCopy;
        internal System.Windows.Forms.Button btnCarbonCopyOpenFolder;
        internal System.Windows.Forms.Button btnCarbonCopyFolderSelect;
        internal System.Windows.Forms.CheckBox chbCarbonCopyIgnoreCopyErrors;
        internal System.Windows.Forms.CheckBox chbUseCarbonCopy;
        internal System.Windows.Forms.TextBox txtCarbonCopyFolder;
        internal System.Windows.Forms.Label lblCarbonCopyFolder;
        internal System.Windows.Forms.Button btnOk;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.CheckBox chbMultilineTabs;
        private System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Label lblSelectionDelimiters;
        private System.Windows.Forms.ComboBox cmbSelectionDelimiters;
        private System.Windows.Forms.Button btnAllowSave;
        internal System.Windows.Forms.CheckBox chbSavePlainWithLF;
        private System.Windows.Forms.ToolTip tooltip;
    }
}