﻿namespace QText {
    partial class MainForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.tls = new System.Windows.Forms.ToolStrip();
            this.tls_btnNew = new System.Windows.Forms.ToolStripButton();
            this.tls_btnSaveNow = new System.Windows.Forms.ToolStripButton();
            this.tls_btnRename = new System.Windows.Forms.ToolStripButton();
            this.tls_0 = new System.Windows.Forms.ToolStripSeparator();
            this.tls_btnPrintPreview = new System.Windows.Forms.ToolStripButton();
            this.tls_btnPrint = new System.Windows.Forms.ToolStripButton();
            this.tls_1 = new System.Windows.Forms.ToolStripSeparator();
            this.tls_btnCut = new System.Windows.Forms.ToolStripButton();
            this.tls_btnCopy = new System.Windows.Forms.ToolStripButton();
            this.tls_btnPaste = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tls_btnFont = new System.Windows.Forms.ToolStripButton();
            this.tls_btnBold = new System.Windows.Forms.ToolStripButton();
            this.tls_btnItalic = new System.Windows.Forms.ToolStripButton();
            this.tls_btnUnderline = new System.Windows.Forms.ToolStripButton();
            this.tls_btnStrikeout = new System.Windows.Forms.ToolStripButton();
            this.tls_RtfSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tls_btnUndo = new System.Windows.Forms.ToolStripButton();
            this.tls_btnRedo = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tls_btnFind = new System.Windows.Forms.ToolStripButton();
            this.tls_btnAlwaysOnTop = new System.Windows.Forms.ToolStripButton();
            this.tlbHelpAbout = new System.Windows.Forms.ToolStripButton();
            this.tlbHelpReportABug = new System.Windows.Forms.ToolStripButton();
            this.tls_btnOptions = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tmrUpdateToolbar = new System.Windows.Forms.Timer(this.components);
            this.tmrQuickAutoSave = new System.Windows.Forms.Timer(this.components);
            this.mnxTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnxTextBoxUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBox0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTextBoxCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxCutCopyPasteAsTextSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTextBoxCutAsText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxCopyAsText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxPasteAsText = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem16 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTextBoxSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTextBoxFont = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxBold = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxItalic = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxUnderline = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxStrikeout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxRtfSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTextBoxFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxSortAZ = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxSortZA = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTextBoxConvertCaseToUpper = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxConvertCaseToLower = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxConvertCaseToTitleCapitalizeAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTextBoxConvertCaseToTitleDrGrammar = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrAutoSave = new System.Windows.Forms.Timer(this.components);
            this.fswLocationTxt = new System.IO.FileSystemWatcher();
            this.mnxTab = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnxTabNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTab0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTabReopen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTabSaveNow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTab1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTabDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTabRename = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTab2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTabPrintPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTabPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxConvertTo = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTabConvertToPlainText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxTabConvertToRichText = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxTabOpenContainingFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu = new QText.MenuStripExOnMainForm();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileReopen = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileConvertToPlainText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileConvertToRichText = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem15 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSaveNow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileRename = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFilePrintPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditFind = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditFindNext = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewAlwaysOnTop = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewToolbar = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem17 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatFont = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatBold = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatItalic = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatUnderline = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatStrikeout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatRtfSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFormatSortAscending = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatSortDescending = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFormatConvertToLower = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatConvertToUpper = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatConvertToTitleCase = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormatConvertToTitleCaseDrGrammar = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpReportABug = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabFiles = new TabControlDnD();
            this.tls.SuspendLayout();
            this.mnxTextBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fswLocationTxt)).BeginInit();
            this.mnxTab.SuspendLayout();
            this.mnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tls
            // 
            this.tls.CanOverflow = false;
            this.tls.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tls.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tls_btnNew,
            this.tls_btnSaveNow,
            this.tls_btnRename,
            this.tls_0,
            this.tls_btnPrintPreview,
            this.tls_btnPrint,
            this.tls_1,
            this.tls_btnCut,
            this.tls_btnCopy,
            this.tls_btnPaste,
            this.ToolStripSeparator1,
            this.tls_btnFont,
            this.tls_btnBold,
            this.tls_btnItalic,
            this.tls_btnUnderline,
            this.tls_btnStrikeout,
            this.tls_RtfSeparator,
            this.tls_btnUndo,
            this.tls_btnRedo,
            this.ToolStripSeparator3,
            this.tls_btnFind,
            this.tls_btnAlwaysOnTop,
            this.tlbHelpAbout,
            this.tlbHelpReportABug,
            this.tls_btnOptions,
            this.ToolStripSeparator2});
            this.tls.Location = new System.Drawing.Point(0, 0);
            this.tls.Name = "tls";
            this.tls.Size = new System.Drawing.Size(542, 25);
            this.tls.TabIndex = 1;
            // 
            // tls_btnNew
            // 
            this.tls_btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnNew.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnNew.Image")));
            this.tls_btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnNew.Name = "tls_btnNew";
            this.tls_btnNew.Size = new System.Drawing.Size(23, 22);
            this.tls_btnNew.Text = "New...";
            this.tls_btnNew.ToolTipText = "New tab (Ctrl+N)";
            // 
            // tls_btnSaveNow
            // 
            this.tls_btnSaveNow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnSaveNow.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnSaveNow.Image")));
            this.tls_btnSaveNow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnSaveNow.Name = "tls_btnSaveNow";
            this.tls_btnSaveNow.Size = new System.Drawing.Size(23, 22);
            this.tls_btnSaveNow.Text = "Save now";
            this.tls_btnSaveNow.ToolTipText = "Save now (Ctrl+S)";
            // 
            // tls_btnRename
            // 
            this.tls_btnRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnRename.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnRename.Image")));
            this.tls_btnRename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnRename.Name = "tls_btnRename";
            this.tls_btnRename.Size = new System.Drawing.Size(23, 22);
            this.tls_btnRename.Text = "Rename";
            this.tls_btnRename.ToolTipText = "Rename tab (F2)";
            // 
            // tls_0
            // 
            this.tls_0.Name = "tls_0";
            this.tls_0.Size = new System.Drawing.Size(6, 25);
            // 
            // tls_btnPrintPreview
            // 
            this.tls_btnPrintPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnPrintPreview.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnPrintPreview.Image")));
            this.tls_btnPrintPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnPrintPreview.Name = "tls_btnPrintPreview";
            this.tls_btnPrintPreview.Size = new System.Drawing.Size(23, 22);
            this.tls_btnPrintPreview.Text = "Print preview";
            this.tls_btnPrintPreview.ToolTipText = "Print preview";
            // 
            // tls_btnPrint
            // 
            this.tls_btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnPrint.Image")));
            this.tls_btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnPrint.Name = "tls_btnPrint";
            this.tls_btnPrint.Size = new System.Drawing.Size(23, 22);
            this.tls_btnPrint.Text = "Print";
            this.tls_btnPrint.ToolTipText = "Print (Ctrl+P)";
            // 
            // tls_1
            // 
            this.tls_1.Name = "tls_1";
            this.tls_1.Size = new System.Drawing.Size(6, 25);
            // 
            // tls_btnCut
            // 
            this.tls_btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnCut.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnCut.Image")));
            this.tls_btnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnCut.Name = "tls_btnCut";
            this.tls_btnCut.Size = new System.Drawing.Size(23, 22);
            this.tls_btnCut.Text = "Cut";
            this.tls_btnCut.ToolTipText = "Cut (Ctrl+X)";
            // 
            // tls_btnCopy
            // 
            this.tls_btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnCopy.Image")));
            this.tls_btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnCopy.Name = "tls_btnCopy";
            this.tls_btnCopy.Size = new System.Drawing.Size(23, 22);
            this.tls_btnCopy.Text = "Copy";
            this.tls_btnCopy.ToolTipText = "Copy (Ctrl+C)";
            // 
            // tls_btnPaste
            // 
            this.tls_btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnPaste.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnPaste.Image")));
            this.tls_btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnPaste.Name = "tls_btnPaste";
            this.tls_btnPaste.Size = new System.Drawing.Size(23, 22);
            this.tls_btnPaste.Text = "Paste";
            this.tls_btnPaste.ToolTipText = "Paste (Ctrl+V)";
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tls_btnFont
            // 
            this.tls_btnFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnFont.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnFont.Image")));
            this.tls_btnFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnFont.Name = "tls_btnFont";
            this.tls_btnFont.Size = new System.Drawing.Size(23, 22);
            this.tls_btnFont.Text = "Font";
            // 
            // tls_btnBold
            // 
            this.tls_btnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnBold.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnBold.Image")));
            this.tls_btnBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnBold.Name = "tls_btnBold";
            this.tls_btnBold.Size = new System.Drawing.Size(23, 22);
            this.tls_btnBold.Text = "Bold";
            this.tls_btnBold.ToolTipText = "Bold (Ctrl+B)";
            // 
            // tls_btnItalic
            // 
            this.tls_btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnItalic.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnItalic.Image")));
            this.tls_btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnItalic.Name = "tls_btnItalic";
            this.tls_btnItalic.Size = new System.Drawing.Size(23, 22);
            this.tls_btnItalic.Text = "Italic";
            this.tls_btnItalic.ToolTipText = "Italic (Ctrl+I)";
            // 
            // tls_btnUnderline
            // 
            this.tls_btnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnUnderline.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnUnderline.Image")));
            this.tls_btnUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnUnderline.Name = "tls_btnUnderline";
            this.tls_btnUnderline.Size = new System.Drawing.Size(23, 22);
            this.tls_btnUnderline.Text = "Underline";
            this.tls_btnUnderline.ToolTipText = "Underline (Ctrl+U)";
            // 
            // tls_btnStrikeout
            // 
            this.tls_btnStrikeout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnStrikeout.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnStrikeout.Image")));
            this.tls_btnStrikeout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnStrikeout.Name = "tls_btnStrikeout";
            this.tls_btnStrikeout.Size = new System.Drawing.Size(23, 22);
            this.tls_btnStrikeout.Text = "Strikeout";
            // 
            // tls_RtfSeparator
            // 
            this.tls_RtfSeparator.Name = "tls_RtfSeparator";
            this.tls_RtfSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // tls_btnUndo
            // 
            this.tls_btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnUndo.Image")));
            this.tls_btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnUndo.Name = "tls_btnUndo";
            this.tls_btnUndo.Size = new System.Drawing.Size(23, 22);
            this.tls_btnUndo.Text = "Undo";
            this.tls_btnUndo.ToolTipText = "Undo (Ctrl+Z)";
            // 
            // tls_btnRedo
            // 
            this.tls_btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnRedo.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnRedo.Image")));
            this.tls_btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnRedo.Name = "tls_btnRedo";
            this.tls_btnRedo.Size = new System.Drawing.Size(23, 22);
            this.tls_btnRedo.Text = "Redo";
            this.tls_btnRedo.ToolTipText = "Redo (Ctrl+Y)";
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tls_btnFind
            // 
            this.tls_btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnFind.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnFind.Image")));
            this.tls_btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnFind.Name = "tls_btnFind";
            this.tls_btnFind.Size = new System.Drawing.Size(23, 22);
            this.tls_btnFind.Text = "Find";
            this.tls_btnFind.ToolTipText = "Find (Ctrl+F)";
            // 
            // tls_btnAlwaysOnTop
            // 
            this.tls_btnAlwaysOnTop.CheckOnClick = true;
            this.tls_btnAlwaysOnTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnAlwaysOnTop.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnAlwaysOnTop.Image")));
            this.tls_btnAlwaysOnTop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnAlwaysOnTop.Name = "tls_btnAlwaysOnTop";
            this.tls_btnAlwaysOnTop.Size = new System.Drawing.Size(23, 22);
            this.tls_btnAlwaysOnTop.Text = "Always on top";
            this.tls_btnAlwaysOnTop.ToolTipText = "Always on top (Ctrl+T)";
            // 
            // tlbHelpAbout
            // 
            this.tlbHelpAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tlbHelpAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tlbHelpAbout.Image = ((System.Drawing.Image)(resources.GetObject("tlbHelpAbout.Image")));
            this.tlbHelpAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tlbHelpAbout.Name = "tlbHelpAbout";
            this.tlbHelpAbout.Size = new System.Drawing.Size(23, 22);
            this.tlbHelpAbout.Text = "About";
            // 
            // tlbHelpReportABug
            // 
            this.tlbHelpReportABug.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tlbHelpReportABug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tlbHelpReportABug.Image = ((System.Drawing.Image)(resources.GetObject("tlbHelpReportABug.Image")));
            this.tlbHelpReportABug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tlbHelpReportABug.Name = "tlbHelpReportABug";
            this.tlbHelpReportABug.Size = new System.Drawing.Size(23, 22);
            this.tlbHelpReportABug.Text = "Report a bug";
            // 
            // tls_btnOptions
            // 
            this.tls_btnOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tls_btnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tls_btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("tls_btnOptions.Image")));
            this.tls_btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tls_btnOptions.Name = "tls_btnOptions";
            this.tls_btnOptions.Size = new System.Drawing.Size(23, 22);
            this.tls_btnOptions.Text = "Options";
            this.tls_btnOptions.ToolTipText = "Options";
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // mnxTextBox
            // 
            this.mnxTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxTextBoxUndo,
            this.mnxTextBoxRedo,
            this.mnxTextBox0,
            this.mnxTextBoxCut,
            this.mnxTextBoxCopy,
            this.mnxTextBoxPaste,
            this.mnxTextBoxCutCopyPasteAsTextSeparator,
            this.mnxTextBoxCutAsText,
            this.mnxTextBoxCopyAsText,
            this.mnxTextBoxPasteAsText,
            this.ToolStripMenuItem16,
            this.mnxTextBoxSelectAll,
            this.ToolStripMenuItem4,
            this.mnxTextBoxFont,
            this.mnxTextBoxBold,
            this.mnxTextBoxItalic,
            this.mnxTextBoxUnderline,
            this.mnxTextBoxStrikeout,
            this.mnxTextBoxRtfSeparator,
            this.mnxTextBoxFormat});
            this.mnxTextBox.Name = "mnxTextBox";
            this.mnxTextBox.Size = new System.Drawing.Size(323, 394);
            // 
            // mnxTextBoxUndo
            // 
            this.mnxTextBoxUndo.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxUndo.Image")));
            this.mnxTextBoxUndo.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTextBoxUndo.Name = "mnxTextBoxUndo";
            this.mnxTextBoxUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.mnxTextBoxUndo.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxUndo.Text = "&Undo";
            // 
            // mnxTextBoxRedo
            // 
            this.mnxTextBoxRedo.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxRedo.Image")));
            this.mnxTextBoxRedo.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTextBoxRedo.Name = "mnxTextBoxRedo";
            this.mnxTextBoxRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.mnxTextBoxRedo.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxRedo.Text = "&Redo";
            // 
            // mnxTextBox0
            // 
            this.mnxTextBox0.Name = "mnxTextBox0";
            this.mnxTextBox0.Size = new System.Drawing.Size(319, 6);
            // 
            // mnxTextBoxCut
            // 
            this.mnxTextBoxCut.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxCut.Image")));
            this.mnxTextBoxCut.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTextBoxCut.Name = "mnxTextBoxCut";
            this.mnxTextBoxCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mnxTextBoxCut.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxCut.Text = "Cu&t";
            // 
            // mnxTextBoxCopy
            // 
            this.mnxTextBoxCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxCopy.Image")));
            this.mnxTextBoxCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTextBoxCopy.Name = "mnxTextBoxCopy";
            this.mnxTextBoxCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnxTextBoxCopy.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxCopy.Text = "&Copy";
            // 
            // mnxTextBoxPaste
            // 
            this.mnxTextBoxPaste.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxPaste.Image")));
            this.mnxTextBoxPaste.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTextBoxPaste.Name = "mnxTextBoxPaste";
            this.mnxTextBoxPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.mnxTextBoxPaste.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxPaste.Text = "&Paste";
            // 
            // mnxTextBoxCutCopyPasteAsTextSeparator
            // 
            this.mnxTextBoxCutCopyPasteAsTextSeparator.Name = "mnxTextBoxCutCopyPasteAsTextSeparator";
            this.mnxTextBoxCutCopyPasteAsTextSeparator.Size = new System.Drawing.Size(319, 6);
            // 
            // mnxTextBoxCutAsText
            // 
            this.mnxTextBoxCutAsText.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxCutAsText.Image")));
            this.mnxTextBoxCutAsText.Name = "mnxTextBoxCutAsText";
            this.mnxTextBoxCutAsText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.X)));
            this.mnxTextBoxCutAsText.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxCutAsText.Text = "Cut without formatting";
            // 
            // mnxTextBoxCopyAsText
            // 
            this.mnxTextBoxCopyAsText.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxCopyAsText.Image")));
            this.mnxTextBoxCopyAsText.Name = "mnxTextBoxCopyAsText";
            this.mnxTextBoxCopyAsText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.C)));
            this.mnxTextBoxCopyAsText.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxCopyAsText.Text = "Copy without formatting";
            // 
            // mnxTextBoxPasteAsText
            // 
            this.mnxTextBoxPasteAsText.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxPasteAsText.Image")));
            this.mnxTextBoxPasteAsText.Name = "mnxTextBoxPasteAsText";
            this.mnxTextBoxPasteAsText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.V)));
            this.mnxTextBoxPasteAsText.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxPasteAsText.Text = "Paste without formatting";
            // 
            // ToolStripMenuItem16
            // 
            this.ToolStripMenuItem16.Name = "ToolStripMenuItem16";
            this.ToolStripMenuItem16.Size = new System.Drawing.Size(319, 6);
            // 
            // mnxTextBoxSelectAll
            // 
            this.mnxTextBoxSelectAll.Name = "mnxTextBoxSelectAll";
            this.mnxTextBoxSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnxTextBoxSelectAll.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxSelectAll.Text = "Select &all";
            // 
            // ToolStripMenuItem4
            // 
            this.ToolStripMenuItem4.Name = "ToolStripMenuItem4";
            this.ToolStripMenuItem4.Size = new System.Drawing.Size(319, 6);
            // 
            // mnxTextBoxFont
            // 
            this.mnxTextBoxFont.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxFont.Image")));
            this.mnxTextBoxFont.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTextBoxFont.Name = "mnxTextBoxFont";
            this.mnxTextBoxFont.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxFont.Text = "&Font...";
            // 
            // mnxTextBoxBold
            // 
            this.mnxTextBoxBold.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxBold.Image")));
            this.mnxTextBoxBold.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTextBoxBold.Name = "mnxTextBoxBold";
            this.mnxTextBoxBold.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.mnxTextBoxBold.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxBold.Text = "&Bold";
            // 
            // mnxTextBoxItalic
            // 
            this.mnxTextBoxItalic.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxItalic.Image")));
            this.mnxTextBoxItalic.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTextBoxItalic.Name = "mnxTextBoxItalic";
            this.mnxTextBoxItalic.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.mnxTextBoxItalic.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxItalic.Text = "&Italic";
            // 
            // mnxTextBoxUnderline
            // 
            this.mnxTextBoxUnderline.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxUnderline.Image")));
            this.mnxTextBoxUnderline.Name = "mnxTextBoxUnderline";
            this.mnxTextBoxUnderline.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.mnxTextBoxUnderline.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxUnderline.Text = "&Underline";
            // 
            // mnxTextBoxStrikeout
            // 
            this.mnxTextBoxStrikeout.Image = ((System.Drawing.Image)(resources.GetObject("mnxTextBoxStrikeout.Image")));
            this.mnxTextBoxStrikeout.Name = "mnxTextBoxStrikeout";
            this.mnxTextBoxStrikeout.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxStrikeout.Text = "&Strikeout";
            // 
            // mnxTextBoxRtfSeparator
            // 
            this.mnxTextBoxRtfSeparator.Name = "mnxTextBoxRtfSeparator";
            this.mnxTextBoxRtfSeparator.Size = new System.Drawing.Size(319, 6);
            // 
            // mnxTextBoxFormat
            // 
            this.mnxTextBoxFormat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxTextBoxSortAZ,
            this.mnxTextBoxSortZA,
            this.ToolStripMenuItem9,
            this.mnxTextBoxConvertCaseToUpper,
            this.mnxTextBoxConvertCaseToLower,
            this.mnxTextBoxConvertCaseToTitleCapitalizeAll,
            this.mnxTextBoxConvertCaseToTitleDrGrammar});
            this.mnxTextBoxFormat.Name = "mnxTextBoxFormat";
            this.mnxTextBoxFormat.Size = new System.Drawing.Size(322, 24);
            this.mnxTextBoxFormat.Text = "&Format";
            // 
            // mnxTextBoxSortAZ
            // 
            this.mnxTextBoxSortAZ.Name = "mnxTextBoxSortAZ";
            this.mnxTextBoxSortAZ.Size = new System.Drawing.Size(344, 24);
            this.mnxTextBoxSortAZ.Text = "Sort &ascending";
            // 
            // mnxTextBoxSortZA
            // 
            this.mnxTextBoxSortZA.Name = "mnxTextBoxSortZA";
            this.mnxTextBoxSortZA.Size = new System.Drawing.Size(344, 24);
            this.mnxTextBoxSortZA.Text = "Sort &descending";
            // 
            // ToolStripMenuItem9
            // 
            this.ToolStripMenuItem9.Name = "ToolStripMenuItem9";
            this.ToolStripMenuItem9.Size = new System.Drawing.Size(341, 6);
            // 
            // mnxTextBoxConvertCaseToUpper
            // 
            this.mnxTextBoxConvertCaseToUpper.Name = "mnxTextBoxConvertCaseToUpper";
            this.mnxTextBoxConvertCaseToUpper.Size = new System.Drawing.Size(344, 24);
            this.mnxTextBoxConvertCaseToUpper.Text = "Convert to &upper case";
            // 
            // mnxTextBoxConvertCaseToLower
            // 
            this.mnxTextBoxConvertCaseToLower.Name = "mnxTextBoxConvertCaseToLower";
            this.mnxTextBoxConvertCaseToLower.Size = new System.Drawing.Size(344, 24);
            this.mnxTextBoxConvertCaseToLower.Text = "Convert to &lower case";
            // 
            // mnxTextBoxConvertCaseToTitleCapitalizeAll
            // 
            this.mnxTextBoxConvertCaseToTitleCapitalizeAll.Name = "mnxTextBoxConvertCaseToTitleCapitalizeAll";
            this.mnxTextBoxConvertCaseToTitleCapitalizeAll.Size = new System.Drawing.Size(344, 24);
            this.mnxTextBoxConvertCaseToTitleCapitalizeAll.Text = "Convert to &title case";
            // 
            // mnxTextBoxConvertCaseToTitleDrGrammar
            // 
            this.mnxTextBoxConvertCaseToTitleDrGrammar.Name = "mnxTextBoxConvertCaseToTitleDrGrammar";
            this.mnxTextBoxConvertCaseToTitleDrGrammar.Size = new System.Drawing.Size(344, 24);
            this.mnxTextBoxConvertCaseToTitleDrGrammar.Text = "Convert to title case (Dr. &Grammar rules)";
            // 
            // tmrAutoSave
            // 
            this.tmrAutoSave.Enabled = true;
            this.tmrAutoSave.Interval = 1000;
            // 
            // fswLocationTxt
            // 
            this.fswLocationTxt.EnableRaisingEvents = true;
            this.fswLocationTxt.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            this.fswLocationTxt.SynchronizingObject = this;
            // 
            // mnxTab
            // 
            this.mnxTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxTabNew,
            this.mnxTab0,
            this.mnxTabReopen,
            this.mnxTabSaveNow,
            this.mnxTab1,
            this.mnxTabDelete,
            this.mnxTabRename,
            this.mnxTab2,
            this.mnxTabPrintPreview,
            this.mnxTabPrint,
            this.mnxConvertTo,
            this.mnxTabConvertToPlainText,
            this.mnxTabConvertToRichText,
            this.ToolStripMenuItem14,
            this.mnxTabOpenContainingFolder});
            this.mnxTab.Name = "mnxTab";
            this.mnxTab.Size = new System.Drawing.Size(233, 274);
            // 
            // mnxTabNew
            // 
            this.mnxTabNew.Image = ((System.Drawing.Image)(resources.GetObject("mnxTabNew.Image")));
            this.mnxTabNew.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTabNew.Name = "mnxTabNew";
            this.mnxTabNew.Size = new System.Drawing.Size(232, 24);
            this.mnxTabNew.Text = "&New...";
            // 
            // mnxTab0
            // 
            this.mnxTab0.Name = "mnxTab0";
            this.mnxTab0.Size = new System.Drawing.Size(229, 6);
            // 
            // mnxTabReopen
            // 
            this.mnxTabReopen.Name = "mnxTabReopen";
            this.mnxTabReopen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.mnxTabReopen.Size = new System.Drawing.Size(232, 24);
            this.mnxTabReopen.Text = "Re&open";
            // 
            // mnxTabSaveNow
            // 
            this.mnxTabSaveNow.Image = ((System.Drawing.Image)(resources.GetObject("mnxTabSaveNow.Image")));
            this.mnxTabSaveNow.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTabSaveNow.Name = "mnxTabSaveNow";
            this.mnxTabSaveNow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnxTabSaveNow.Size = new System.Drawing.Size(232, 24);
            this.mnxTabSaveNow.Text = "&Save";
            // 
            // mnxTab1
            // 
            this.mnxTab1.Name = "mnxTab1";
            this.mnxTab1.Size = new System.Drawing.Size(229, 6);
            // 
            // mnxTabDelete
            // 
            this.mnxTabDelete.Image = ((System.Drawing.Image)(resources.GetObject("mnxTabDelete.Image")));
            this.mnxTabDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTabDelete.Name = "mnxTabDelete";
            this.mnxTabDelete.Size = new System.Drawing.Size(232, 24);
            this.mnxTabDelete.Text = "&Delete";
            // 
            // mnxTabRename
            // 
            this.mnxTabRename.Image = ((System.Drawing.Image)(resources.GetObject("mnxTabRename.Image")));
            this.mnxTabRename.Name = "mnxTabRename";
            this.mnxTabRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mnxTabRename.Size = new System.Drawing.Size(232, 24);
            this.mnxTabRename.Text = "&Rename...";
            // 
            // mnxTab2
            // 
            this.mnxTab2.Name = "mnxTab2";
            this.mnxTab2.Size = new System.Drawing.Size(229, 6);
            // 
            // mnxTabPrintPreview
            // 
            this.mnxTabPrintPreview.Image = ((System.Drawing.Image)(resources.GetObject("mnxTabPrintPreview.Image")));
            this.mnxTabPrintPreview.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTabPrintPreview.Name = "mnxTabPrintPreview";
            this.mnxTabPrintPreview.Size = new System.Drawing.Size(232, 24);
            this.mnxTabPrintPreview.Text = "Prin&t preview...";
            // 
            // mnxTabPrint
            // 
            this.mnxTabPrint.Image = ((System.Drawing.Image)(resources.GetObject("mnxTabPrint.Image")));
            this.mnxTabPrint.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnxTabPrint.Name = "mnxTabPrint";
            this.mnxTabPrint.Size = new System.Drawing.Size(232, 24);
            this.mnxTabPrint.Text = "&Print...";
            // 
            // mnxConvertTo
            // 
            this.mnxConvertTo.Name = "mnxConvertTo";
            this.mnxConvertTo.Size = new System.Drawing.Size(229, 6);
            // 
            // mnxTabConvertToPlainText
            // 
            this.mnxTabConvertToPlainText.Name = "mnxTabConvertToPlainText";
            this.mnxTabConvertToPlainText.Size = new System.Drawing.Size(232, 24);
            this.mnxTabConvertToPlainText.Text = "Convert to plain text";
            // 
            // mnxTabConvertToRichText
            // 
            this.mnxTabConvertToRichText.Name = "mnxTabConvertToRichText";
            this.mnxTabConvertToRichText.Size = new System.Drawing.Size(232, 24);
            this.mnxTabConvertToRichText.Text = "Convert to rich text";
            // 
            // ToolStripMenuItem14
            // 
            this.ToolStripMenuItem14.Name = "ToolStripMenuItem14";
            this.ToolStripMenuItem14.Size = new System.Drawing.Size(229, 6);
            // 
            // mnxTabOpenContainingFolder
            // 
            this.mnxTabOpenContainingFolder.Name = "mnxTabOpenContainingFolder";
            this.mnxTabOpenContainingFolder.Size = new System.Drawing.Size(232, 24);
            this.mnxTabOpenContainingFolder.Text = "Open containing folder";
            // 
            // mnu
            // 
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuView,
            this.mnuFormat,
            this.mnuTools,
            this.mnuHelp});
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.mnu.Size = new System.Drawing.Size(542, 28);
            this.mnu.TabIndex = 4;
            this.mnu.Visible = false;
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.ToolStripMenuItem1,
            this.mnuFileReopen,
            this.ToolStripMenuItem2,
            this.mnuFileConvertToPlainText,
            this.mnuFileConvertToRichText,
            this.ToolStripMenuItem15,
            this.mnuFileSaveNow,
            this.mnuFileSaveAll,
            this.ToolStripMenuItem3,
            this.mnuFileDelete,
            this.mnuFileRename,
            this.ToolStripMenuItem5,
            this.mnuFilePrintPreview,
            this.mnuFilePrint,
            this.ToolStripMenuItem6,
            this.mnuFileClose,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(44, 24);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileNew.Image")));
            this.mnuFileNew.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mnuFileNew.Size = new System.Drawing.Size(219, 24);
            this.mnuFileNew.Text = "&New";
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(216, 6);
            // 
            // mnuFileReopen
            // 
            this.mnuFileReopen.Name = "mnuFileReopen";
            this.mnuFileReopen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.mnuFileReopen.Size = new System.Drawing.Size(219, 24);
            this.mnuFileReopen.Text = "Re&open";
            // 
            // ToolStripMenuItem2
            // 
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(216, 6);
            // 
            // mnuFileConvertToPlainText
            // 
            this.mnuFileConvertToPlainText.Name = "mnuFileConvertToPlainText";
            this.mnuFileConvertToPlainText.Size = new System.Drawing.Size(219, 24);
            this.mnuFileConvertToPlainText.Text = "Convert to plain text";
            // 
            // mnuFileConvertToRichText
            // 
            this.mnuFileConvertToRichText.Name = "mnuFileConvertToRichText";
            this.mnuFileConvertToRichText.Size = new System.Drawing.Size(219, 24);
            this.mnuFileConvertToRichText.Text = "Convert to rich text";
            // 
            // ToolStripMenuItem15
            // 
            this.ToolStripMenuItem15.Name = "ToolStripMenuItem15";
            this.ToolStripMenuItem15.Size = new System.Drawing.Size(216, 6);
            // 
            // mnuFileSaveNow
            // 
            this.mnuFileSaveNow.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileSaveNow.Image")));
            this.mnuFileSaveNow.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFileSaveNow.Name = "mnuFileSaveNow";
            this.mnuFileSaveNow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuFileSaveNow.Size = new System.Drawing.Size(219, 24);
            this.mnuFileSaveNow.Text = "&Save";
            // 
            // mnuFileSaveAll
            // 
            this.mnuFileSaveAll.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileSaveAll.Image")));
            this.mnuFileSaveAll.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFileSaveAll.Name = "mnuFileSaveAll";
            this.mnuFileSaveAll.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.mnuFileSaveAll.Size = new System.Drawing.Size(219, 24);
            this.mnuFileSaveAll.Text = "Save &all";
            // 
            // ToolStripMenuItem3
            // 
            this.ToolStripMenuItem3.Name = "ToolStripMenuItem3";
            this.ToolStripMenuItem3.Size = new System.Drawing.Size(216, 6);
            // 
            // mnuFileDelete
            // 
            this.mnuFileDelete.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileDelete.Image")));
            this.mnuFileDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFileDelete.Name = "mnuFileDelete";
            this.mnuFileDelete.Size = new System.Drawing.Size(219, 24);
            this.mnuFileDelete.Text = "&Delete";
            // 
            // mnuFileRename
            // 
            this.mnuFileRename.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileRename.Image")));
            this.mnuFileRename.Name = "mnuFileRename";
            this.mnuFileRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mnuFileRename.Size = new System.Drawing.Size(219, 24);
            this.mnuFileRename.Text = "&Rename";
            // 
            // ToolStripMenuItem5
            // 
            this.ToolStripMenuItem5.Name = "ToolStripMenuItem5";
            this.ToolStripMenuItem5.Size = new System.Drawing.Size(216, 6);
            // 
            // mnuFilePrintPreview
            // 
            this.mnuFilePrintPreview.Image = ((System.Drawing.Image)(resources.GetObject("mnuFilePrintPreview.Image")));
            this.mnuFilePrintPreview.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFilePrintPreview.Name = "mnuFilePrintPreview";
            this.mnuFilePrintPreview.Size = new System.Drawing.Size(219, 24);
            this.mnuFilePrintPreview.Text = "Prin&t preview";
            // 
            // mnuFilePrint
            // 
            this.mnuFilePrint.Image = ((System.Drawing.Image)(resources.GetObject("mnuFilePrint.Image")));
            this.mnuFilePrint.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFilePrint.Name = "mnuFilePrint";
            this.mnuFilePrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mnuFilePrint.Size = new System.Drawing.Size(219, 24);
            this.mnuFilePrint.Text = "&Print";
            // 
            // ToolStripMenuItem6
            // 
            this.ToolStripMenuItem6.Name = "ToolStripMenuItem6";
            this.ToolStripMenuItem6.Size = new System.Drawing.Size(216, 6);
            // 
            // mnuFileClose
            // 
            this.mnuFileClose.Name = "mnuFileClose";
            this.mnuFileClose.ShortcutKeyDisplayString = "Escape";
            this.mnuFileClose.Size = new System.Drawing.Size(219, 24);
            this.mnuFileClose.Text = "&Close";
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(219, 24);
            this.mnuFileExit.Text = "E&xit";
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditUndo,
            this.mnuEditRedo,
            this.ToolStripMenuItem7,
            this.mnuEditCut,
            this.mnuEditCopy,
            this.mnuEditPaste,
            this.mnuEditDelete,
            this.ToolStripMenuItem8,
            this.mnuEditSelectAll,
            this.ToolStripMenuItem12,
            this.mnuEditFind,
            this.mnuEditFindNext});
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(47, 24);
            this.mnuEdit.Text = "&Edit";
            // 
            // mnuEditUndo
            // 
            this.mnuEditUndo.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditUndo.Image")));
            this.mnuEditUndo.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuEditUndo.Name = "mnuEditUndo";
            this.mnuEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.mnuEditUndo.Size = new System.Drawing.Size(190, 24);
            this.mnuEditUndo.Text = "&Undo";
            // 
            // mnuEditRedo
            // 
            this.mnuEditRedo.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditRedo.Image")));
            this.mnuEditRedo.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuEditRedo.Name = "mnuEditRedo";
            this.mnuEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.mnuEditRedo.Size = new System.Drawing.Size(190, 24);
            this.mnuEditRedo.Text = "&Redo";
            // 
            // ToolStripMenuItem7
            // 
            this.ToolStripMenuItem7.Name = "ToolStripMenuItem7";
            this.ToolStripMenuItem7.Size = new System.Drawing.Size(187, 6);
            // 
            // mnuEditCut
            // 
            this.mnuEditCut.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditCut.Image")));
            this.mnuEditCut.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuEditCut.Name = "mnuEditCut";
            this.mnuEditCut.Size = new System.Drawing.Size(190, 24);
            this.mnuEditCut.Text = "Cu&t";
            // 
            // mnuEditCopy
            // 
            this.mnuEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditCopy.Image")));
            this.mnuEditCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.Size = new System.Drawing.Size(190, 24);
            this.mnuEditCopy.Text = "&Copy";
            // 
            // mnuEditPaste
            // 
            this.mnuEditPaste.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditPaste.Image")));
            this.mnuEditPaste.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuEditPaste.Name = "mnuEditPaste";
            this.mnuEditPaste.Size = new System.Drawing.Size(190, 24);
            this.mnuEditPaste.Text = "&Paste";
            // 
            // mnuEditDelete
            // 
            this.mnuEditDelete.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditDelete.Image")));
            this.mnuEditDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuEditDelete.Name = "mnuEditDelete";
            this.mnuEditDelete.Size = new System.Drawing.Size(190, 24);
            this.mnuEditDelete.Text = "&Delete";
            // 
            // ToolStripMenuItem8
            // 
            this.ToolStripMenuItem8.Name = "ToolStripMenuItem8";
            this.ToolStripMenuItem8.Size = new System.Drawing.Size(187, 6);
            // 
            // mnuEditSelectAll
            // 
            this.mnuEditSelectAll.Name = "mnuEditSelectAll";
            this.mnuEditSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnuEditSelectAll.Size = new System.Drawing.Size(190, 24);
            this.mnuEditSelectAll.Text = "Select &all";
            // 
            // ToolStripMenuItem12
            // 
            this.ToolStripMenuItem12.Name = "ToolStripMenuItem12";
            this.ToolStripMenuItem12.Size = new System.Drawing.Size(187, 6);
            // 
            // mnuEditFind
            // 
            this.mnuEditFind.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditFind.Image")));
            this.mnuEditFind.Name = "mnuEditFind";
            this.mnuEditFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.mnuEditFind.Size = new System.Drawing.Size(190, 24);
            this.mnuEditFind.Text = "&Find";
            // 
            // mnuEditFindNext
            // 
            this.mnuEditFindNext.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditFindNext.Image")));
            this.mnuEditFindNext.Name = "mnuEditFindNext";
            this.mnuEditFindNext.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.mnuEditFindNext.Size = new System.Drawing.Size(190, 24);
            this.mnuEditFindNext.Text = "Find &next";
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewAlwaysOnTop,
            this.ToolStripMenuItem10,
            this.mnuViewMenu,
            this.mnuViewToolbar,
            this.ToolStripMenuItem17,
            this.mnuViewZoomIn,
            this.mnuViewZoomOut,
            this.ToolStripMenuItem11,
            this.mnuViewRefresh});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(53, 24);
            this.mnuView.Text = "&View";
            // 
            // mnuViewAlwaysOnTop
            // 
            this.mnuViewAlwaysOnTop.CheckOnClick = true;
            this.mnuViewAlwaysOnTop.Image = ((System.Drawing.Image)(resources.GetObject("mnuViewAlwaysOnTop.Image")));
            this.mnuViewAlwaysOnTop.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuViewAlwaysOnTop.Name = "mnuViewAlwaysOnTop";
            this.mnuViewAlwaysOnTop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.mnuViewAlwaysOnTop.Size = new System.Drawing.Size(222, 24);
            this.mnuViewAlwaysOnTop.Text = "Always on &top";
            // 
            // ToolStripMenuItem10
            // 
            this.ToolStripMenuItem10.Name = "ToolStripMenuItem10";
            this.ToolStripMenuItem10.Size = new System.Drawing.Size(219, 6);
            // 
            // mnuViewMenu
            // 
            this.mnuViewMenu.CheckOnClick = true;
            this.mnuViewMenu.Name = "mnuViewMenu";
            this.mnuViewMenu.Size = new System.Drawing.Size(222, 24);
            this.mnuViewMenu.Text = "&Menu";
            // 
            // mnuViewToolbar
            // 
            this.mnuViewToolbar.CheckOnClick = true;
            this.mnuViewToolbar.Name = "mnuViewToolbar";
            this.mnuViewToolbar.Size = new System.Drawing.Size(222, 24);
            this.mnuViewToolbar.Text = "&Toolbar";
            // 
            // ToolStripMenuItem17
            // 
            this.ToolStripMenuItem17.Name = "ToolStripMenuItem17";
            this.ToolStripMenuItem17.Size = new System.Drawing.Size(219, 6);
            // 
            // mnuViewZoomIn
            // 
            this.mnuViewZoomIn.Name = "mnuViewZoomIn";
            this.mnuViewZoomIn.Size = new System.Drawing.Size(222, 24);
            this.mnuViewZoomIn.Text = "Zoom in";
            // 
            // mnuViewZoomOut
            // 
            this.mnuViewZoomOut.Name = "mnuViewZoomOut";
            this.mnuViewZoomOut.Size = new System.Drawing.Size(222, 24);
            this.mnuViewZoomOut.Text = "Zoom out";
            // 
            // ToolStripMenuItem11
            // 
            this.ToolStripMenuItem11.Name = "ToolStripMenuItem11";
            this.ToolStripMenuItem11.Size = new System.Drawing.Size(219, 6);
            // 
            // mnuViewRefresh
            // 
            this.mnuViewRefresh.Image = ((System.Drawing.Image)(resources.GetObject("mnuViewRefresh.Image")));
            this.mnuViewRefresh.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuViewRefresh.Name = "mnuViewRefresh";
            this.mnuViewRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuViewRefresh.Size = new System.Drawing.Size(222, 24);
            this.mnuViewRefresh.Text = "&Refresh";
            // 
            // mnuFormat
            // 
            this.mnuFormat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFormatFont,
            this.mnuFormatBold,
            this.mnuFormatItalic,
            this.mnuFormatUnderline,
            this.mnuFormatStrikeout,
            this.mnuFormatRtfSeparator,
            this.mnuFormatSortAscending,
            this.mnuFormatSortDescending,
            this.ToolStripMenuItem13,
            this.mnuFormatConvertToLower,
            this.mnuFormatConvertToUpper,
            this.mnuFormatConvertToTitleCase,
            this.mnuFormatConvertToTitleCaseDrGrammar});
            this.mnuFormat.Name = "mnuFormat";
            this.mnuFormat.Size = new System.Drawing.Size(68, 24);
            this.mnuFormat.Text = "F&ormat";
            // 
            // mnuFormatFont
            // 
            this.mnuFormatFont.Image = ((System.Drawing.Image)(resources.GetObject("mnuFormatFont.Image")));
            this.mnuFormatFont.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFormatFont.Name = "mnuFormatFont";
            this.mnuFormatFont.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatFont.Text = "&Font";
            // 
            // mnuFormatBold
            // 
            this.mnuFormatBold.Image = ((System.Drawing.Image)(resources.GetObject("mnuFormatBold.Image")));
            this.mnuFormatBold.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFormatBold.Name = "mnuFormatBold";
            this.mnuFormatBold.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.mnuFormatBold.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatBold.Text = "&Bold";
            // 
            // mnuFormatItalic
            // 
            this.mnuFormatItalic.Image = ((System.Drawing.Image)(resources.GetObject("mnuFormatItalic.Image")));
            this.mnuFormatItalic.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuFormatItalic.Name = "mnuFormatItalic";
            this.mnuFormatItalic.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.mnuFormatItalic.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatItalic.Text = "&Italic";
            // 
            // mnuFormatUnderline
            // 
            this.mnuFormatUnderline.Image = ((System.Drawing.Image)(resources.GetObject("mnuFormatUnderline.Image")));
            this.mnuFormatUnderline.Name = "mnuFormatUnderline";
            this.mnuFormatUnderline.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.mnuFormatUnderline.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatUnderline.Text = "&Underline";
            // 
            // mnuFormatStrikeout
            // 
            this.mnuFormatStrikeout.Image = ((System.Drawing.Image)(resources.GetObject("mnuFormatStrikeout.Image")));
            this.mnuFormatStrikeout.Name = "mnuFormatStrikeout";
            this.mnuFormatStrikeout.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatStrikeout.Text = "&Strikeout";
            // 
            // mnuFormatRtfSeparator
            // 
            this.mnuFormatRtfSeparator.Name = "mnuFormatRtfSeparator";
            this.mnuFormatRtfSeparator.Size = new System.Drawing.Size(341, 6);
            // 
            // mnuFormatSortAscending
            // 
            this.mnuFormatSortAscending.Name = "mnuFormatSortAscending";
            this.mnuFormatSortAscending.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatSortAscending.Text = "Sort &ascending";
            // 
            // mnuFormatSortDescending
            // 
            this.mnuFormatSortDescending.Name = "mnuFormatSortDescending";
            this.mnuFormatSortDescending.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatSortDescending.Text = "Sort &descending";
            // 
            // ToolStripMenuItem13
            // 
            this.ToolStripMenuItem13.Name = "ToolStripMenuItem13";
            this.ToolStripMenuItem13.Size = new System.Drawing.Size(341, 6);
            // 
            // mnuFormatConvertToLower
            // 
            this.mnuFormatConvertToLower.Name = "mnuFormatConvertToLower";
            this.mnuFormatConvertToLower.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatConvertToLower.Text = "Convert to &lower case";
            // 
            // mnuFormatConvertToUpper
            // 
            this.mnuFormatConvertToUpper.Name = "mnuFormatConvertToUpper";
            this.mnuFormatConvertToUpper.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatConvertToUpper.Text = "Convert to &upper case";
            // 
            // mnuFormatConvertToTitleCase
            // 
            this.mnuFormatConvertToTitleCase.Name = "mnuFormatConvertToTitleCase";
            this.mnuFormatConvertToTitleCase.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatConvertToTitleCase.Text = "Convert to &title case";
            // 
            // mnuFormatConvertToTitleCaseDrGrammar
            // 
            this.mnuFormatConvertToTitleCaseDrGrammar.Name = "mnuFormatConvertToTitleCaseDrGrammar";
            this.mnuFormatConvertToTitleCaseDrGrammar.Size = new System.Drawing.Size(344, 24);
            this.mnuFormatConvertToTitleCaseDrGrammar.Text = "Convert to title case (Dr. &Grammar rules)";
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsOptions});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(57, 24);
            this.mnuTools.Text = "&Tools";
            // 
            // mnuToolsOptions
            // 
            this.mnuToolsOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnuToolsOptions.Image")));
            this.mnuToolsOptions.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuToolsOptions.Name = "mnuToolsOptions";
            this.mnuToolsOptions.Size = new System.Drawing.Size(130, 24);
            this.mnuToolsOptions.Text = "&Options";
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpReportABug,
            this.mnuHelp0,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(53, 24);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpReportABug
            // 
            this.mnuHelpReportABug.Image = ((System.Drawing.Image)(resources.GetObject("mnuHelpReportABug.Image")));
            this.mnuHelpReportABug.Name = "mnuHelpReportABug";
            this.mnuHelpReportABug.Size = new System.Drawing.Size(165, 24);
            this.mnuHelpReportABug.Text = "Report a bug";
            // 
            // mnuHelp0
            // 
            this.mnuHelp0.Name = "mnuHelp0";
            this.mnuHelp0.Size = new System.Drawing.Size(162, 6);
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Image = ((System.Drawing.Image)(resources.GetObject("mnuHelpAbout.Image")));
            this.mnuHelpAbout.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(165, 24);
            this.mnuHelpAbout.Text = "&About";
            // 
            // tabFiles
            // 
            this.tabFiles.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabFiles.Location = new System.Drawing.Point(94, 70);
            this.tabFiles.Margin = new System.Windows.Forms.Padding(0);
            this.tabFiles.Name = "tabFiles";
            this.tabFiles.SelectedIndex = 0;
            this.tabFiles.Size = new System.Drawing.Size(320, 197);
            this.tabFiles.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 320);
            this.Controls.Add(this.tabFiles);
            this.Controls.Add(this.tls);
            this.Controls.Add(this.mnu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnu;
            this.MinimumSize = new System.Drawing.Size(320, 200);
            this.Name = "MainForm";
            this.Text = "QText";
            this.tls.ResumeLayout(false);
            this.tls.PerformLayout();
            this.mnxTextBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fswLocationTxt)).EndInit();
            this.mnxTab.ResumeLayout(false);
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ToolTip tooltip;
        internal System.Windows.Forms.ToolStrip tls;
        internal System.Windows.Forms.ToolStripButton tls_btnNew;
        internal System.Windows.Forms.ToolStripButton tls_btnSaveNow;
        internal System.Windows.Forms.ToolStripButton tls_btnRename;
        internal System.Windows.Forms.ToolStripSeparator tls_0;
        internal System.Windows.Forms.ToolStripButton tls_btnPrintPreview;
        internal System.Windows.Forms.ToolStripButton tls_btnPrint;
        internal System.Windows.Forms.ToolStripSeparator tls_1;
        internal System.Windows.Forms.ToolStripButton tls_btnCut;
        internal System.Windows.Forms.ToolStripButton tls_btnCopy;
        internal System.Windows.Forms.ToolStripButton tls_btnPaste;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        internal System.Windows.Forms.ToolStripButton tls_btnFont;
        internal System.Windows.Forms.ToolStripButton tls_btnBold;
        internal System.Windows.Forms.ToolStripButton tls_btnItalic;
        internal System.Windows.Forms.ToolStripButton tls_btnUnderline;
        internal System.Windows.Forms.ToolStripButton tls_btnStrikeout;
        internal System.Windows.Forms.ToolStripSeparator tls_RtfSeparator;
        internal System.Windows.Forms.ToolStripButton tls_btnUndo;
        internal System.Windows.Forms.ToolStripButton tls_btnRedo;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
        internal System.Windows.Forms.ToolStripButton tls_btnFind;
        internal System.Windows.Forms.ToolStripButton tls_btnAlwaysOnTop;
        internal System.Windows.Forms.ToolStripButton tlbHelpAbout;
        internal System.Windows.Forms.ToolStripButton tlbHelpReportABug;
        internal System.Windows.Forms.ToolStripButton tls_btnOptions;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
        internal System.Windows.Forms.Timer tmrUpdateToolbar;
        internal System.Windows.Forms.Timer tmrQuickAutoSave;
        internal System.Windows.Forms.ContextMenuStrip mnxTextBox;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxUndo;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxRedo;
        internal System.Windows.Forms.ToolStripSeparator mnxTextBox0;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxCut;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxCopy;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxPaste;
        internal System.Windows.Forms.ToolStripSeparator mnxTextBoxCutCopyPasteAsTextSeparator;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxCutAsText;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxCopyAsText;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxPasteAsText;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem16;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxSelectAll;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem4;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxFont;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxBold;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxItalic;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxUnderline;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxStrikeout;
        internal System.Windows.Forms.ToolStripSeparator mnxTextBoxRtfSeparator;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxFormat;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxSortAZ;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxSortZA;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem9;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxConvertCaseToUpper;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxConvertCaseToLower;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxConvertCaseToTitleCapitalizeAll;
        internal System.Windows.Forms.ToolStripMenuItem mnxTextBoxConvertCaseToTitleDrGrammar;
        internal System.Windows.Forms.Timer tmrAutoSave;
        internal System.IO.FileSystemWatcher fswLocationTxt;
        internal System.Windows.Forms.ContextMenuStrip mnxTab;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabNew;
        internal System.Windows.Forms.ToolStripSeparator mnxTab0;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabReopen;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabSaveNow;
        internal System.Windows.Forms.ToolStripSeparator mnxTab1;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabDelete;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabRename;
        internal System.Windows.Forms.ToolStripSeparator mnxTab2;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabPrintPreview;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabPrint;
        internal System.Windows.Forms.ToolStripSeparator mnxConvertTo;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabConvertToPlainText;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabConvertToRichText;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem14;
        internal System.Windows.Forms.ToolStripMenuItem mnxTabOpenContainingFolder;
        internal MenuStripExOnMainForm mnu;
        internal System.Windows.Forms.ToolStripMenuItem mnuFile;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileReopen;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem2;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileConvertToPlainText;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileConvertToRichText;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem15;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileSaveNow;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileSaveAll;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem3;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileDelete;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileRename;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem5;
        internal System.Windows.Forms.ToolStripMenuItem mnuFilePrintPreview;
        internal System.Windows.Forms.ToolStripMenuItem mnuFilePrint;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem6;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileClose;
        internal System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        internal System.Windows.Forms.ToolStripMenuItem mnuEdit;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditUndo;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditRedo;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem7;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditCut;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditCopy;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditPaste;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditDelete;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem8;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditSelectAll;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem12;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditFind;
        internal System.Windows.Forms.ToolStripMenuItem mnuEditFindNext;
        internal System.Windows.Forms.ToolStripMenuItem mnuView;
        internal System.Windows.Forms.ToolStripMenuItem mnuViewAlwaysOnTop;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem10;
        internal System.Windows.Forms.ToolStripMenuItem mnuViewMenu;
        internal System.Windows.Forms.ToolStripMenuItem mnuViewToolbar;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem17;
        internal System.Windows.Forms.ToolStripMenuItem mnuViewZoomIn;
        internal System.Windows.Forms.ToolStripMenuItem mnuViewZoomOut;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem11;
        internal System.Windows.Forms.ToolStripMenuItem mnuViewRefresh;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormat;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatFont;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatBold;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatItalic;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatUnderline;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatStrikeout;
        internal System.Windows.Forms.ToolStripSeparator mnuFormatRtfSeparator;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatSortAscending;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatSortDescending;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem13;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatConvertToLower;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatConvertToUpper;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatConvertToTitleCase;
        internal System.Windows.Forms.ToolStripMenuItem mnuFormatConvertToTitleCaseDrGrammar;
        internal System.Windows.Forms.ToolStripMenuItem mnuTools;
        internal System.Windows.Forms.ToolStripMenuItem mnuToolsOptions;
        internal System.Windows.Forms.ToolStripMenuItem mnuHelp;
        internal System.Windows.Forms.ToolStripMenuItem mnuHelpReportABug;
        internal System.Windows.Forms.ToolStripSeparator mnuHelp0;
        internal System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        internal TabControlDnD tabFiles;
    }
}