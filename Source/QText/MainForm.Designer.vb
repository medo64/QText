<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.tmrAutoSave = New System.Windows.Forms.Timer(Me.components)
        Me.mnxTab = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnxTabNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTab0 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTabReopen = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTabSaveNow = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTab1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTabDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTabRename = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTab2 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTabPrintPreview = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTabPrint = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxConvertTo = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTabConvertToPlainText = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTabConvertToRichText = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem14 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTabOpenContainingFolder = New System.Windows.Forms.ToolStripMenuItem()
        Me.tls = New System.Windows.Forms.ToolStrip()
        Me.tls_btnNew = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnSaveNow = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnRename = New System.Windows.Forms.ToolStripButton()
        Me.tls_0 = New System.Windows.Forms.ToolStripSeparator()
        Me.tls_btnPrintPreview = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnPrint = New System.Windows.Forms.ToolStripButton()
        Me.tls_1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tls_btnCut = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnCopy = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnPaste = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tls_btnFont = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnBold = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnItalic = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnUnderline = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnStrikeout = New System.Windows.Forms.ToolStripButton()
        Me.tls_RtfSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.tls_btnUndo = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnRedo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.tls_btnFind = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnAlwaysOnTop = New System.Windows.Forms.ToolStripButton()
        Me.tlbHelpAbout = New System.Windows.Forms.ToolStripButton()
        Me.tlbHelpReportABug = New System.Windows.Forms.ToolStripButton()
        Me.tls_btnOptions = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.tmrUpdateToolbar = New System.Windows.Forms.Timer(Me.components)
        Me.mnxTextBox = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnxTextBoxUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxRedo = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBox0 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTextBoxCut = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxPaste = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxCutCopyPasteAsTextSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTextBoxCutAsText = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxCopyAsText = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxPasteAsText = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem16 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTextBoxSelectAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTextBoxFont = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxBold = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxItalic = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxUnderline = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxStrikeout = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxRtfSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTextBoxFormat = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxSortAZ = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxSortZA = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem9 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnxTextBoxConvertCaseToUpper = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxConvertCaseToLower = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxConvertCaseToTitleCapitalizeAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnxTextBoxConvertCaseToTitleDrGrammar = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnu = New QText.MenuStripExOnMainForm()
        Me.mnuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFileReopen = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFileConvertToPlainText = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileConvertToRichText = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem15 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFileSaveNow = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileSaveAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFileDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileRename = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFilePrintPreview = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFilePrint = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFileClose = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEdit = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditRedo = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem7 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuEditCut = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditPaste = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem8 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuEditSelectAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem12 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuEditFind = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditFindNext = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuView = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuViewAlwaysOnTop = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem10 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuViewMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuViewToolbar = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem17 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuViewZoomIn = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuViewZoomOut = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem11 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuViewRefresh = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormat = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatFont = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatBold = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatItalic = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatUnderline = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatStrikeout = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatRtfSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFormatSortAscending = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatSortDescending = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem13 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFormatConvertToLower = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatConvertToUpper = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatConvertToTitleCase = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFormatConvertToTitleCaseDrGrammar = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuTools = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuToolsOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelpReportABug = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelp0 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuHelpAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.tooltip = New System.Windows.Forms.ToolTip(Me.components)
        Me.tabFiles = New QTextAux.TabControlDnD()
        Me.tmrQuickAutoSave = New System.Windows.Forms.Timer(Me.components)
        Me.fswLocationTxt = New System.IO.FileSystemWatcher()
        Me.mnxTab.SuspendLayout()
        Me.tls.SuspendLayout()
        Me.mnxTextBox.SuspendLayout()
        Me.mnu.SuspendLayout()
        CType(Me.fswLocationTxt, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tmrAutoSave
        '
        Me.tmrAutoSave.Enabled = True
        Me.tmrAutoSave.Interval = 1000
        '
        'mnxTab
        '
        Me.mnxTab.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnxTabNew, Me.mnxTab0, Me.mnxTabReopen, Me.mnxTabSaveNow, Me.mnxTab1, Me.mnxTabDelete, Me.mnxTabRename, Me.mnxTab2, Me.mnxTabPrintPreview, Me.mnxTabPrint, Me.mnxConvertTo, Me.mnxTabConvertToPlainText, Me.mnxTabConvertToRichText, Me.ToolStripMenuItem14, Me.mnxTabOpenContainingFolder})
        Me.mnxTab.Name = "mnxTab"
        Me.mnxTab.Size = New System.Drawing.Size(233, 274)
        '
        'mnxTabNew
        '
        Me.mnxTabNew.Image = CType(resources.GetObject("mnxTabNew.Image"), System.Drawing.Image)
        Me.mnxTabNew.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTabNew.Name = "mnxTabNew"
        Me.mnxTabNew.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabNew.Text = "&New..."
        '
        'mnxTab0
        '
        Me.mnxTab0.Name = "mnxTab0"
        Me.mnxTab0.Size = New System.Drawing.Size(229, 6)
        '
        'mnxTabReopen
        '
        Me.mnxTabReopen.Name = "mnxTabReopen"
        Me.mnxTabReopen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.mnxTabReopen.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabReopen.Text = "Re&open"
        '
        'mnxTabSaveNow
        '
        Me.mnxTabSaveNow.Image = CType(resources.GetObject("mnxTabSaveNow.Image"), System.Drawing.Image)
        Me.mnxTabSaveNow.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTabSaveNow.Name = "mnxTabSaveNow"
        Me.mnxTabSaveNow.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.mnxTabSaveNow.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabSaveNow.Text = "&Save"
        '
        'mnxTab1
        '
        Me.mnxTab1.Name = "mnxTab1"
        Me.mnxTab1.Size = New System.Drawing.Size(229, 6)
        '
        'mnxTabDelete
        '
        Me.mnxTabDelete.Image = CType(resources.GetObject("mnxTabDelete.Image"), System.Drawing.Image)
        Me.mnxTabDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTabDelete.Name = "mnxTabDelete"
        Me.mnxTabDelete.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabDelete.Text = "&Delete"
        '
        'mnxTabRename
        '
        Me.mnxTabRename.Image = CType(resources.GetObject("mnxTabRename.Image"), System.Drawing.Image)
        Me.mnxTabRename.Name = "mnxTabRename"
        Me.mnxTabRename.ShortcutKeys = System.Windows.Forms.Keys.F2
        Me.mnxTabRename.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabRename.Text = "&Rename..."
        '
        'mnxTab2
        '
        Me.mnxTab2.Name = "mnxTab2"
        Me.mnxTab2.Size = New System.Drawing.Size(229, 6)
        '
        'mnxTabPrintPreview
        '
        Me.mnxTabPrintPreview.Image = CType(resources.GetObject("mnxTabPrintPreview.Image"), System.Drawing.Image)
        Me.mnxTabPrintPreview.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTabPrintPreview.Name = "mnxTabPrintPreview"
        Me.mnxTabPrintPreview.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabPrintPreview.Text = "Prin&t preview..."
        '
        'mnxTabPrint
        '
        Me.mnxTabPrint.Image = CType(resources.GetObject("mnxTabPrint.Image"), System.Drawing.Image)
        Me.mnxTabPrint.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTabPrint.Name = "mnxTabPrint"
        Me.mnxTabPrint.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabPrint.Text = "&Print..."
        '
        'mnxConvertTo
        '
        Me.mnxConvertTo.Name = "mnxConvertTo"
        Me.mnxConvertTo.Size = New System.Drawing.Size(229, 6)
        '
        'mnxTabConvertToPlainText
        '
        Me.mnxTabConvertToPlainText.Name = "mnxTabConvertToPlainText"
        Me.mnxTabConvertToPlainText.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabConvertToPlainText.Text = "Convert to plain text"
        '
        'mnxTabConvertToRichText
        '
        Me.mnxTabConvertToRichText.Name = "mnxTabConvertToRichText"
        Me.mnxTabConvertToRichText.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabConvertToRichText.Text = "Convert to rich text"
        '
        'ToolStripMenuItem14
        '
        Me.ToolStripMenuItem14.Name = "ToolStripMenuItem14"
        Me.ToolStripMenuItem14.Size = New System.Drawing.Size(229, 6)
        '
        'mnxTabOpenContainingFolder
        '
        Me.mnxTabOpenContainingFolder.Name = "mnxTabOpenContainingFolder"
        Me.mnxTabOpenContainingFolder.Size = New System.Drawing.Size(232, 24)
        Me.mnxTabOpenContainingFolder.Text = "Open containing folder"
        '
        'tls
        '
        Me.tls.CanOverflow = False
        Me.tls.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tls.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tls_btnNew, Me.tls_btnSaveNow, Me.tls_btnRename, Me.tls_0, Me.tls_btnPrintPreview, Me.tls_btnPrint, Me.tls_1, Me.tls_btnCut, Me.tls_btnCopy, Me.tls_btnPaste, Me.ToolStripSeparator1, Me.tls_btnFont, Me.tls_btnBold, Me.tls_btnItalic, Me.tls_btnUnderline, Me.tls_btnStrikeout, Me.tls_RtfSeparator, Me.tls_btnUndo, Me.tls_btnRedo, Me.ToolStripSeparator3, Me.tls_btnFind, Me.tls_btnAlwaysOnTop, Me.tlbHelpAbout, Me.tlbHelpReportABug, Me.tls_btnOptions, Me.ToolStripSeparator2})
        Me.tls.Location = New System.Drawing.Point(0, 0)
        Me.tls.Name = "tls"
        Me.tls.Size = New System.Drawing.Size(542, 25)
        Me.tls.TabIndex = 2
        '
        'tls_btnNew
        '
        Me.tls_btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnNew.Image = CType(resources.GetObject("tls_btnNew.Image"), System.Drawing.Image)
        Me.tls_btnNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnNew.Name = "tls_btnNew"
        Me.tls_btnNew.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnNew.Text = "New..."
        Me.tls_btnNew.ToolTipText = "New tab (Ctrl+N)"
        '
        'tls_btnSaveNow
        '
        Me.tls_btnSaveNow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnSaveNow.Image = CType(resources.GetObject("tls_btnSaveNow.Image"), System.Drawing.Image)
        Me.tls_btnSaveNow.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnSaveNow.Name = "tls_btnSaveNow"
        Me.tls_btnSaveNow.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnSaveNow.Text = "Save now"
        Me.tls_btnSaveNow.ToolTipText = "Save now (Ctrl+S)"
        '
        'tls_btnRename
        '
        Me.tls_btnRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnRename.Image = CType(resources.GetObject("tls_btnRename.Image"), System.Drawing.Image)
        Me.tls_btnRename.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnRename.Name = "tls_btnRename"
        Me.tls_btnRename.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnRename.Text = "Rename"
        Me.tls_btnRename.ToolTipText = "Rename tab (F2)"
        '
        'tls_0
        '
        Me.tls_0.Name = "tls_0"
        Me.tls_0.Size = New System.Drawing.Size(6, 25)
        '
        'tls_btnPrintPreview
        '
        Me.tls_btnPrintPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnPrintPreview.Image = CType(resources.GetObject("tls_btnPrintPreview.Image"), System.Drawing.Image)
        Me.tls_btnPrintPreview.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnPrintPreview.Name = "tls_btnPrintPreview"
        Me.tls_btnPrintPreview.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnPrintPreview.Text = "Print preview"
        Me.tls_btnPrintPreview.ToolTipText = "Print preview"
        '
        'tls_btnPrint
        '
        Me.tls_btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnPrint.Image = CType(resources.GetObject("tls_btnPrint.Image"), System.Drawing.Image)
        Me.tls_btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnPrint.Name = "tls_btnPrint"
        Me.tls_btnPrint.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnPrint.Text = "Print"
        Me.tls_btnPrint.ToolTipText = "Print (Ctrl+P)"
        '
        'tls_1
        '
        Me.tls_1.Name = "tls_1"
        Me.tls_1.Size = New System.Drawing.Size(6, 25)
        '
        'tls_btnCut
        '
        Me.tls_btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnCut.Image = CType(resources.GetObject("tls_btnCut.Image"), System.Drawing.Image)
        Me.tls_btnCut.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnCut.Name = "tls_btnCut"
        Me.tls_btnCut.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnCut.Text = "Cut"
        Me.tls_btnCut.ToolTipText = "Cut (Ctrl+X)"
        '
        'tls_btnCopy
        '
        Me.tls_btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnCopy.Image = CType(resources.GetObject("tls_btnCopy.Image"), System.Drawing.Image)
        Me.tls_btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnCopy.Name = "tls_btnCopy"
        Me.tls_btnCopy.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnCopy.Text = "Copy"
        Me.tls_btnCopy.ToolTipText = "Copy (Ctrl+C)"
        '
        'tls_btnPaste
        '
        Me.tls_btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnPaste.Image = CType(resources.GetObject("tls_btnPaste.Image"), System.Drawing.Image)
        Me.tls_btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnPaste.Name = "tls_btnPaste"
        Me.tls_btnPaste.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnPaste.Text = "Paste"
        Me.tls_btnPaste.ToolTipText = "Paste (Ctrl+V)"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'tls_btnFont
        '
        Me.tls_btnFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnFont.Image = CType(resources.GetObject("tls_btnFont.Image"), System.Drawing.Image)
        Me.tls_btnFont.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnFont.Name = "tls_btnFont"
        Me.tls_btnFont.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnFont.Text = "Font"
        '
        'tls_btnBold
        '
        Me.tls_btnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnBold.Image = CType(resources.GetObject("tls_btnBold.Image"), System.Drawing.Image)
        Me.tls_btnBold.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnBold.Name = "tls_btnBold"
        Me.tls_btnBold.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnBold.Text = "Bold"
        Me.tls_btnBold.ToolTipText = "Bold (Ctrl+B)"
        '
        'tls_btnItalic
        '
        Me.tls_btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnItalic.Image = CType(resources.GetObject("tls_btnItalic.Image"), System.Drawing.Image)
        Me.tls_btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnItalic.Name = "tls_btnItalic"
        Me.tls_btnItalic.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnItalic.Text = "Italic"
        Me.tls_btnItalic.ToolTipText = "Italic (Ctrl+I)"
        '
        'tls_btnUnderline
        '
        Me.tls_btnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnUnderline.Image = CType(resources.GetObject("tls_btnUnderline.Image"), System.Drawing.Image)
        Me.tls_btnUnderline.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnUnderline.Name = "tls_btnUnderline"
        Me.tls_btnUnderline.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnUnderline.Text = "Underline"
        Me.tls_btnUnderline.ToolTipText = "Underline (Ctrl+U)"
        '
        'tls_btnStrikeout
        '
        Me.tls_btnStrikeout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnStrikeout.Image = CType(resources.GetObject("tls_btnStrikeout.Image"), System.Drawing.Image)
        Me.tls_btnStrikeout.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnStrikeout.Name = "tls_btnStrikeout"
        Me.tls_btnStrikeout.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnStrikeout.Text = "Strikeout"
        '
        'tls_RtfSeparator
        '
        Me.tls_RtfSeparator.Name = "tls_RtfSeparator"
        Me.tls_RtfSeparator.Size = New System.Drawing.Size(6, 25)
        '
        'tls_btnUndo
        '
        Me.tls_btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnUndo.Image = CType(resources.GetObject("tls_btnUndo.Image"), System.Drawing.Image)
        Me.tls_btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnUndo.Name = "tls_btnUndo"
        Me.tls_btnUndo.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnUndo.Text = "Undo"
        Me.tls_btnUndo.ToolTipText = "Undo (Ctrl+Z)"
        '
        'tls_btnRedo
        '
        Me.tls_btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnRedo.Image = CType(resources.GetObject("tls_btnRedo.Image"), System.Drawing.Image)
        Me.tls_btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnRedo.Name = "tls_btnRedo"
        Me.tls_btnRedo.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnRedo.Text = "Redo"
        Me.tls_btnRedo.ToolTipText = "Redo (Ctrl+Y)"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'tls_btnFind
        '
        Me.tls_btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnFind.Image = CType(resources.GetObject("tls_btnFind.Image"), System.Drawing.Image)
        Me.tls_btnFind.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnFind.Name = "tls_btnFind"
        Me.tls_btnFind.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnFind.Text = "Find"
        Me.tls_btnFind.ToolTipText = "Find (Ctrl+F)"
        '
        'tls_btnAlwaysOnTop
        '
        Me.tls_btnAlwaysOnTop.CheckOnClick = True
        Me.tls_btnAlwaysOnTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnAlwaysOnTop.Image = CType(resources.GetObject("tls_btnAlwaysOnTop.Image"), System.Drawing.Image)
        Me.tls_btnAlwaysOnTop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnAlwaysOnTop.Name = "tls_btnAlwaysOnTop"
        Me.tls_btnAlwaysOnTop.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnAlwaysOnTop.Text = "Always on top"
        Me.tls_btnAlwaysOnTop.ToolTipText = "Always on top (Ctrl+T)"
        '
        'tlbHelpAbout
        '
        Me.tlbHelpAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tlbHelpAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlbHelpAbout.Image = CType(resources.GetObject("tlbHelpAbout.Image"), System.Drawing.Image)
        Me.tlbHelpAbout.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tlbHelpAbout.Name = "tlbHelpAbout"
        Me.tlbHelpAbout.Size = New System.Drawing.Size(23, 22)
        Me.tlbHelpAbout.Text = "About"
        '
        'tlbHelpReportABug
        '
        Me.tlbHelpReportABug.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tlbHelpReportABug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlbHelpReportABug.Image = CType(resources.GetObject("tlbHelpReportABug.Image"), System.Drawing.Image)
        Me.tlbHelpReportABug.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tlbHelpReportABug.Name = "tlbHelpReportABug"
        Me.tlbHelpReportABug.Size = New System.Drawing.Size(23, 22)
        Me.tlbHelpReportABug.Text = "Report a bug"
        '
        'tls_btnOptions
        '
        Me.tls_btnOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tls_btnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tls_btnOptions.Image = CType(resources.GetObject("tls_btnOptions.Image"), System.Drawing.Image)
        Me.tls_btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tls_btnOptions.Name = "tls_btnOptions"
        Me.tls_btnOptions.Size = New System.Drawing.Size(23, 22)
        Me.tls_btnOptions.Text = "Options"
        Me.tls_btnOptions.ToolTipText = "Options"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'tmrUpdateToolbar
        '
        '
        'mnxTextBox
        '
        Me.mnxTextBox.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnxTextBoxUndo, Me.mnxTextBoxRedo, Me.mnxTextBox0, Me.mnxTextBoxCut, Me.mnxTextBoxCopy, Me.mnxTextBoxPaste, Me.mnxTextBoxCutCopyPasteAsTextSeparator, Me.mnxTextBoxCutAsText, Me.mnxTextBoxCopyAsText, Me.mnxTextBoxPasteAsText, Me.ToolStripMenuItem16, Me.mnxTextBoxSelectAll, Me.ToolStripMenuItem4, Me.mnxTextBoxFont, Me.mnxTextBoxBold, Me.mnxTextBoxItalic, Me.mnxTextBoxUnderline, Me.mnxTextBoxStrikeout, Me.mnxTextBoxRtfSeparator, Me.mnxTextBoxFormat})
        Me.mnxTextBox.Name = "mnxTextBox"
        Me.mnxTextBox.Size = New System.Drawing.Size(323, 394)
        '
        'mnxTextBoxUndo
        '
        Me.mnxTextBoxUndo.Image = CType(resources.GetObject("mnxTextBoxUndo.Image"), System.Drawing.Image)
        Me.mnxTextBoxUndo.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTextBoxUndo.Name = "mnxTextBoxUndo"
        Me.mnxTextBoxUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.mnxTextBoxUndo.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxUndo.Text = "&Undo"
        '
        'mnxTextBoxRedo
        '
        Me.mnxTextBoxRedo.Image = CType(resources.GetObject("mnxTextBoxRedo.Image"), System.Drawing.Image)
        Me.mnxTextBoxRedo.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTextBoxRedo.Name = "mnxTextBoxRedo"
        Me.mnxTextBoxRedo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y), System.Windows.Forms.Keys)
        Me.mnxTextBoxRedo.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxRedo.Text = "&Redo"
        '
        'mnxTextBox0
        '
        Me.mnxTextBox0.Name = "mnxTextBox0"
        Me.mnxTextBox0.Size = New System.Drawing.Size(319, 6)
        '
        'mnxTextBoxCut
        '
        Me.mnxTextBoxCut.Image = CType(resources.GetObject("mnxTextBoxCut.Image"), System.Drawing.Image)
        Me.mnxTextBoxCut.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTextBoxCut.Name = "mnxTextBoxCut"
        Me.mnxTextBoxCut.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
        Me.mnxTextBoxCut.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxCut.Text = "Cu&t"
        '
        'mnxTextBoxCopy
        '
        Me.mnxTextBoxCopy.Image = CType(resources.GetObject("mnxTextBoxCopy.Image"), System.Drawing.Image)
        Me.mnxTextBoxCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTextBoxCopy.Name = "mnxTextBoxCopy"
        Me.mnxTextBoxCopy.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.mnxTextBoxCopy.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxCopy.Text = "&Copy"
        '
        'mnxTextBoxPaste
        '
        Me.mnxTextBoxPaste.Image = CType(resources.GetObject("mnxTextBoxPaste.Image"), System.Drawing.Image)
        Me.mnxTextBoxPaste.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTextBoxPaste.Name = "mnxTextBoxPaste"
        Me.mnxTextBoxPaste.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
        Me.mnxTextBoxPaste.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxPaste.Text = "&Paste"
        '
        'mnxTextBoxCutCopyPasteAsTextSeparator
        '
        Me.mnxTextBoxCutCopyPasteAsTextSeparator.Name = "mnxTextBoxCutCopyPasteAsTextSeparator"
        Me.mnxTextBoxCutCopyPasteAsTextSeparator.Size = New System.Drawing.Size(319, 6)
        '
        'mnxTextBoxCutAsText
        '
        Me.mnxTextBoxCutAsText.Image = CType(resources.GetObject("mnxTextBoxCutAsText.Image"), System.Drawing.Image)
        Me.mnxTextBoxCutAsText.Name = "mnxTextBoxCutAsText"
        Me.mnxTextBoxCutAsText.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
                    Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
        Me.mnxTextBoxCutAsText.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxCutAsText.Text = "Cut without formatting"
        '
        'mnxTextBoxCopyAsText
        '
        Me.mnxTextBoxCopyAsText.Image = CType(resources.GetObject("mnxTextBoxCopyAsText.Image"), System.Drawing.Image)
        Me.mnxTextBoxCopyAsText.Name = "mnxTextBoxCopyAsText"
        Me.mnxTextBoxCopyAsText.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
                    Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.mnxTextBoxCopyAsText.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxCopyAsText.Text = "Copy without formatting"
        '
        'mnxTextBoxPasteAsText
        '
        Me.mnxTextBoxPasteAsText.Image = CType(resources.GetObject("mnxTextBoxPasteAsText.Image"), System.Drawing.Image)
        Me.mnxTextBoxPasteAsText.Name = "mnxTextBoxPasteAsText"
        Me.mnxTextBoxPasteAsText.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
                    Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
        Me.mnxTextBoxPasteAsText.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxPasteAsText.Text = "Paste without formatting"
        '
        'ToolStripMenuItem16
        '
        Me.ToolStripMenuItem16.Name = "ToolStripMenuItem16"
        Me.ToolStripMenuItem16.Size = New System.Drawing.Size(319, 6)
        '
        'mnxTextBoxSelectAll
        '
        Me.mnxTextBoxSelectAll.Name = "mnxTextBoxSelectAll"
        Me.mnxTextBoxSelectAll.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.mnxTextBoxSelectAll.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxSelectAll.Text = "Select &all"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(319, 6)
        '
        'mnxTextBoxFont
        '
        Me.mnxTextBoxFont.Image = CType(resources.GetObject("mnxTextBoxFont.Image"), System.Drawing.Image)
        Me.mnxTextBoxFont.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTextBoxFont.Name = "mnxTextBoxFont"
        Me.mnxTextBoxFont.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxFont.Text = "&Font..."
        '
        'mnxTextBoxBold
        '
        Me.mnxTextBoxBold.Image = CType(resources.GetObject("mnxTextBoxBold.Image"), System.Drawing.Image)
        Me.mnxTextBoxBold.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTextBoxBold.Name = "mnxTextBoxBold"
        Me.mnxTextBoxBold.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.B), System.Windows.Forms.Keys)
        Me.mnxTextBoxBold.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxBold.Text = "&Bold"
        '
        'mnxTextBoxItalic
        '
        Me.mnxTextBoxItalic.Image = CType(resources.GetObject("mnxTextBoxItalic.Image"), System.Drawing.Image)
        Me.mnxTextBoxItalic.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnxTextBoxItalic.Name = "mnxTextBoxItalic"
        Me.mnxTextBoxItalic.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys)
        Me.mnxTextBoxItalic.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxItalic.Text = "&Italic"
        '
        'mnxTextBoxUnderline
        '
        Me.mnxTextBoxUnderline.Image = CType(resources.GetObject("mnxTextBoxUnderline.Image"), System.Drawing.Image)
        Me.mnxTextBoxUnderline.Name = "mnxTextBoxUnderline"
        Me.mnxTextBoxUnderline.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.U), System.Windows.Forms.Keys)
        Me.mnxTextBoxUnderline.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxUnderline.Text = "&Underline"
        '
        'mnxTextBoxStrikeout
        '
        Me.mnxTextBoxStrikeout.Image = CType(resources.GetObject("mnxTextBoxStrikeout.Image"), System.Drawing.Image)
        Me.mnxTextBoxStrikeout.Name = "mnxTextBoxStrikeout"
        Me.mnxTextBoxStrikeout.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxStrikeout.Text = "&Strikeout"
        '
        'mnxTextBoxRtfSeparator
        '
        Me.mnxTextBoxRtfSeparator.Name = "mnxTextBoxRtfSeparator"
        Me.mnxTextBoxRtfSeparator.Size = New System.Drawing.Size(319, 6)
        '
        'mnxTextBoxFormat
        '
        Me.mnxTextBoxFormat.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnxTextBoxSortAZ, Me.mnxTextBoxSortZA, Me.ToolStripMenuItem9, Me.mnxTextBoxConvertCaseToUpper, Me.mnxTextBoxConvertCaseToLower, Me.mnxTextBoxConvertCaseToTitleCapitalizeAll, Me.mnxTextBoxConvertCaseToTitleDrGrammar})
        Me.mnxTextBoxFormat.Name = "mnxTextBoxFormat"
        Me.mnxTextBoxFormat.Size = New System.Drawing.Size(322, 24)
        Me.mnxTextBoxFormat.Text = "&Format"
        '
        'mnxTextBoxSortAZ
        '
        Me.mnxTextBoxSortAZ.Name = "mnxTextBoxSortAZ"
        Me.mnxTextBoxSortAZ.Size = New System.Drawing.Size(344, 24)
        Me.mnxTextBoxSortAZ.Text = "Sort &ascending"
        '
        'mnxTextBoxSortZA
        '
        Me.mnxTextBoxSortZA.Name = "mnxTextBoxSortZA"
        Me.mnxTextBoxSortZA.Size = New System.Drawing.Size(344, 24)
        Me.mnxTextBoxSortZA.Text = "Sort &descending"
        '
        'ToolStripMenuItem9
        '
        Me.ToolStripMenuItem9.Name = "ToolStripMenuItem9"
        Me.ToolStripMenuItem9.Size = New System.Drawing.Size(341, 6)
        '
        'mnxTextBoxConvertCaseToUpper
        '
        Me.mnxTextBoxConvertCaseToUpper.Name = "mnxTextBoxConvertCaseToUpper"
        Me.mnxTextBoxConvertCaseToUpper.Size = New System.Drawing.Size(344, 24)
        Me.mnxTextBoxConvertCaseToUpper.Text = "Convert to &upper case"
        '
        'mnxTextBoxConvertCaseToLower
        '
        Me.mnxTextBoxConvertCaseToLower.Name = "mnxTextBoxConvertCaseToLower"
        Me.mnxTextBoxConvertCaseToLower.Size = New System.Drawing.Size(344, 24)
        Me.mnxTextBoxConvertCaseToLower.Text = "Convert to &lower case"
        '
        'mnxTextBoxConvertCaseToTitleCapitalizeAll
        '
        Me.mnxTextBoxConvertCaseToTitleCapitalizeAll.Name = "mnxTextBoxConvertCaseToTitleCapitalizeAll"
        Me.mnxTextBoxConvertCaseToTitleCapitalizeAll.Size = New System.Drawing.Size(344, 24)
        Me.mnxTextBoxConvertCaseToTitleCapitalizeAll.Text = "Convert to &title case"
        '
        'mnxTextBoxConvertCaseToTitleDrGrammar
        '
        Me.mnxTextBoxConvertCaseToTitleDrGrammar.Name = "mnxTextBoxConvertCaseToTitleDrGrammar"
        Me.mnxTextBoxConvertCaseToTitleDrGrammar.Size = New System.Drawing.Size(344, 24)
        Me.mnxTextBoxConvertCaseToTitleDrGrammar.Text = "Convert to title case (Dr. &Grammar rules)"
        '
        'mnu
        '
        Me.mnu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFile, Me.mnuEdit, Me.mnuView, Me.mnuFormat, Me.mnuTools, Me.mnuHelp})
        Me.mnu.Location = New System.Drawing.Point(0, 0)
        Me.mnu.Name = "mnu"
        Me.mnu.Padding = New System.Windows.Forms.Padding(8, 2, 0, 2)
        Me.mnu.Size = New System.Drawing.Size(542, 28)
        Me.mnu.TabIndex = 3
        Me.mnu.Visible = False
        '
        'mnuFile
        '
        Me.mnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFileNew, Me.ToolStripMenuItem1, Me.mnuFileReopen, Me.ToolStripMenuItem2, Me.mnuFileConvertToPlainText, Me.mnuFileConvertToRichText, Me.ToolStripMenuItem15, Me.mnuFileSaveNow, Me.mnuFileSaveAll, Me.ToolStripMenuItem3, Me.mnuFileDelete, Me.mnuFileRename, Me.ToolStripMenuItem5, Me.mnuFilePrintPreview, Me.mnuFilePrint, Me.ToolStripMenuItem6, Me.mnuFileClose, Me.mnuFileExit})
        Me.mnuFile.Name = "mnuFile"
        Me.mnuFile.Size = New System.Drawing.Size(44, 24)
        Me.mnuFile.Text = "&File"
        '
        'mnuFileNew
        '
        Me.mnuFileNew.Image = CType(resources.GetObject("mnuFileNew.Image"), System.Drawing.Image)
        Me.mnuFileNew.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFileNew.Name = "mnuFileNew"
        Me.mnuFileNew.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.mnuFileNew.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileNew.Text = "&New"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(216, 6)
        '
        'mnuFileReopen
        '
        Me.mnuFileReopen.Name = "mnuFileReopen"
        Me.mnuFileReopen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.mnuFileReopen.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileReopen.Text = "Re&open"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(216, 6)
        '
        'mnuFileConvertToPlainText
        '
        Me.mnuFileConvertToPlainText.Name = "mnuFileConvertToPlainText"
        Me.mnuFileConvertToPlainText.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileConvertToPlainText.Text = "Convert to plain text"
        '
        'mnuFileConvertToRichText
        '
        Me.mnuFileConvertToRichText.Name = "mnuFileConvertToRichText"
        Me.mnuFileConvertToRichText.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileConvertToRichText.Text = "Convert to rich text"
        '
        'ToolStripMenuItem15
        '
        Me.ToolStripMenuItem15.Name = "ToolStripMenuItem15"
        Me.ToolStripMenuItem15.Size = New System.Drawing.Size(216, 6)
        '
        'mnuFileSaveNow
        '
        Me.mnuFileSaveNow.Image = CType(resources.GetObject("mnuFileSaveNow.Image"), System.Drawing.Image)
        Me.mnuFileSaveNow.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFileSaveNow.Name = "mnuFileSaveNow"
        Me.mnuFileSaveNow.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.mnuFileSaveNow.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileSaveNow.Text = "&Save"
        '
        'mnuFileSaveAll
        '
        Me.mnuFileSaveAll.Image = CType(resources.GetObject("mnuFileSaveAll.Image"), System.Drawing.Image)
        Me.mnuFileSaveAll.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFileSaveAll.Name = "mnuFileSaveAll"
        Me.mnuFileSaveAll.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
                    Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.mnuFileSaveAll.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileSaveAll.Text = "Save &all"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(216, 6)
        '
        'mnuFileDelete
        '
        Me.mnuFileDelete.Image = CType(resources.GetObject("mnuFileDelete.Image"), System.Drawing.Image)
        Me.mnuFileDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFileDelete.Name = "mnuFileDelete"
        Me.mnuFileDelete.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileDelete.Text = "&Delete"
        '
        'mnuFileRename
        '
        Me.mnuFileRename.Image = CType(resources.GetObject("mnuFileRename.Image"), System.Drawing.Image)
        Me.mnuFileRename.Name = "mnuFileRename"
        Me.mnuFileRename.ShortcutKeys = System.Windows.Forms.Keys.F2
        Me.mnuFileRename.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileRename.Text = "&Rename"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(216, 6)
        '
        'mnuFilePrintPreview
        '
        Me.mnuFilePrintPreview.Image = CType(resources.GetObject("mnuFilePrintPreview.Image"), System.Drawing.Image)
        Me.mnuFilePrintPreview.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFilePrintPreview.Name = "mnuFilePrintPreview"
        Me.mnuFilePrintPreview.Size = New System.Drawing.Size(219, 24)
        Me.mnuFilePrintPreview.Text = "Prin&t preview"
        '
        'mnuFilePrint
        '
        Me.mnuFilePrint.Image = CType(resources.GetObject("mnuFilePrint.Image"), System.Drawing.Image)
        Me.mnuFilePrint.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFilePrint.Name = "mnuFilePrint"
        Me.mnuFilePrint.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.P), System.Windows.Forms.Keys)
        Me.mnuFilePrint.Size = New System.Drawing.Size(219, 24)
        Me.mnuFilePrint.Text = "&Print"
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        Me.ToolStripMenuItem6.Size = New System.Drawing.Size(216, 6)
        '
        'mnuFileClose
        '
        Me.mnuFileClose.Name = "mnuFileClose"
        Me.mnuFileClose.ShortcutKeyDisplayString = "Escape"
        Me.mnuFileClose.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileClose.Text = "&Close"
        '
        'mnuFileExit
        '
        Me.mnuFileExit.Name = "mnuFileExit"
        Me.mnuFileExit.Size = New System.Drawing.Size(219, 24)
        Me.mnuFileExit.Text = "E&xit"
        '
        'mnuEdit
        '
        Me.mnuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuEditUndo, Me.mnuEditRedo, Me.ToolStripMenuItem7, Me.mnuEditCut, Me.mnuEditCopy, Me.mnuEditPaste, Me.mnuEditDelete, Me.ToolStripMenuItem8, Me.mnuEditSelectAll, Me.ToolStripMenuItem12, Me.mnuEditFind, Me.mnuEditFindNext})
        Me.mnuEdit.Name = "mnuEdit"
        Me.mnuEdit.Size = New System.Drawing.Size(47, 24)
        Me.mnuEdit.Text = "&Edit"
        '
        'mnuEditUndo
        '
        Me.mnuEditUndo.Image = CType(resources.GetObject("mnuEditUndo.Image"), System.Drawing.Image)
        Me.mnuEditUndo.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuEditUndo.Name = "mnuEditUndo"
        Me.mnuEditUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.mnuEditUndo.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditUndo.Text = "&Undo"
        '
        'mnuEditRedo
        '
        Me.mnuEditRedo.Image = CType(resources.GetObject("mnuEditRedo.Image"), System.Drawing.Image)
        Me.mnuEditRedo.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuEditRedo.Name = "mnuEditRedo"
        Me.mnuEditRedo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y), System.Windows.Forms.Keys)
        Me.mnuEditRedo.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditRedo.Text = "&Redo"
        '
        'ToolStripMenuItem7
        '
        Me.ToolStripMenuItem7.Name = "ToolStripMenuItem7"
        Me.ToolStripMenuItem7.Size = New System.Drawing.Size(187, 6)
        '
        'mnuEditCut
        '
        Me.mnuEditCut.Image = CType(resources.GetObject("mnuEditCut.Image"), System.Drawing.Image)
        Me.mnuEditCut.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuEditCut.Name = "mnuEditCut"
        Me.mnuEditCut.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditCut.Text = "Cu&t"
        '
        'mnuEditCopy
        '
        Me.mnuEditCopy.Image = CType(resources.GetObject("mnuEditCopy.Image"), System.Drawing.Image)
        Me.mnuEditCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuEditCopy.Name = "mnuEditCopy"
        Me.mnuEditCopy.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditCopy.Text = "&Copy"
        '
        'mnuEditPaste
        '
        Me.mnuEditPaste.Image = CType(resources.GetObject("mnuEditPaste.Image"), System.Drawing.Image)
        Me.mnuEditPaste.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuEditPaste.Name = "mnuEditPaste"
        Me.mnuEditPaste.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditPaste.Text = "&Paste"
        '
        'mnuEditDelete
        '
        Me.mnuEditDelete.Image = CType(resources.GetObject("mnuEditDelete.Image"), System.Drawing.Image)
        Me.mnuEditDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuEditDelete.Name = "mnuEditDelete"
        Me.mnuEditDelete.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditDelete.Text = "&Delete"
        '
        'ToolStripMenuItem8
        '
        Me.ToolStripMenuItem8.Name = "ToolStripMenuItem8"
        Me.ToolStripMenuItem8.Size = New System.Drawing.Size(187, 6)
        '
        'mnuEditSelectAll
        '
        Me.mnuEditSelectAll.Name = "mnuEditSelectAll"
        Me.mnuEditSelectAll.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.mnuEditSelectAll.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditSelectAll.Text = "Select &all"
        '
        'ToolStripMenuItem12
        '
        Me.ToolStripMenuItem12.Name = "ToolStripMenuItem12"
        Me.ToolStripMenuItem12.Size = New System.Drawing.Size(187, 6)
        '
        'mnuEditFind
        '
        Me.mnuEditFind.Image = CType(resources.GetObject("mnuEditFind.Image"), System.Drawing.Image)
        Me.mnuEditFind.Name = "mnuEditFind"
        Me.mnuEditFind.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F), System.Windows.Forms.Keys)
        Me.mnuEditFind.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditFind.Text = "&Find"
        '
        'mnuEditFindNext
        '
        Me.mnuEditFindNext.Image = CType(resources.GetObject("mnuEditFindNext.Image"), System.Drawing.Image)
        Me.mnuEditFindNext.Name = "mnuEditFindNext"
        Me.mnuEditFindNext.ShortcutKeys = System.Windows.Forms.Keys.F3
        Me.mnuEditFindNext.Size = New System.Drawing.Size(190, 24)
        Me.mnuEditFindNext.Text = "Find &next"
        '
        'mnuView
        '
        Me.mnuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuViewAlwaysOnTop, Me.ToolStripMenuItem10, Me.mnuViewMenu, Me.mnuViewToolbar, Me.ToolStripMenuItem17, Me.mnuViewZoomIn, Me.mnuViewZoomOut, Me.ToolStripMenuItem11, Me.mnuViewRefresh})
        Me.mnuView.Name = "mnuView"
        Me.mnuView.Size = New System.Drawing.Size(53, 24)
        Me.mnuView.Text = "&View"
        '
        'mnuViewAlwaysOnTop
        '
        Me.mnuViewAlwaysOnTop.CheckOnClick = True
        Me.mnuViewAlwaysOnTop.Image = CType(resources.GetObject("mnuViewAlwaysOnTop.Image"), System.Drawing.Image)
        Me.mnuViewAlwaysOnTop.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuViewAlwaysOnTop.Name = "mnuViewAlwaysOnTop"
        Me.mnuViewAlwaysOnTop.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.T), System.Windows.Forms.Keys)
        Me.mnuViewAlwaysOnTop.Size = New System.Drawing.Size(222, 24)
        Me.mnuViewAlwaysOnTop.Text = "Always on &top"
        '
        'ToolStripMenuItem10
        '
        Me.ToolStripMenuItem10.Name = "ToolStripMenuItem10"
        Me.ToolStripMenuItem10.Size = New System.Drawing.Size(219, 6)
        '
        'mnuViewMenu
        '
        Me.mnuViewMenu.CheckOnClick = True
        Me.mnuViewMenu.Name = "mnuViewMenu"
        Me.mnuViewMenu.Size = New System.Drawing.Size(222, 24)
        Me.mnuViewMenu.Text = "&Menu"
        '
        'mnuViewToolbar
        '
        Me.mnuViewToolbar.CheckOnClick = True
        Me.mnuViewToolbar.Name = "mnuViewToolbar"
        Me.mnuViewToolbar.Size = New System.Drawing.Size(222, 24)
        Me.mnuViewToolbar.Text = "&Toolbar"
        '
        'ToolStripMenuItem17
        '
        Me.ToolStripMenuItem17.Name = "ToolStripMenuItem17"
        Me.ToolStripMenuItem17.Size = New System.Drawing.Size(219, 6)
        '
        'mnuViewZoomIn
        '
        Me.mnuViewZoomIn.Name = "mnuViewZoomIn"
        Me.mnuViewZoomIn.Size = New System.Drawing.Size(222, 24)
        Me.mnuViewZoomIn.Text = "Zoom in"
        '
        'mnuViewZoomOut
        '
        Me.mnuViewZoomOut.Name = "mnuViewZoomOut"
        Me.mnuViewZoomOut.Size = New System.Drawing.Size(222, 24)
        Me.mnuViewZoomOut.Text = "Zoom out"
        '
        'ToolStripMenuItem11
        '
        Me.ToolStripMenuItem11.Name = "ToolStripMenuItem11"
        Me.ToolStripMenuItem11.Size = New System.Drawing.Size(219, 6)
        '
        'mnuViewRefresh
        '
        Me.mnuViewRefresh.Image = CType(resources.GetObject("mnuViewRefresh.Image"), System.Drawing.Image)
        Me.mnuViewRefresh.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuViewRefresh.Name = "mnuViewRefresh"
        Me.mnuViewRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.mnuViewRefresh.Size = New System.Drawing.Size(222, 24)
        Me.mnuViewRefresh.Text = "&Refresh"
        '
        'mnuFormat
        '
        Me.mnuFormat.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFormatFont, Me.mnuFormatBold, Me.mnuFormatItalic, Me.mnuFormatUnderline, Me.mnuFormatStrikeout, Me.mnuFormatRtfSeparator, Me.mnuFormatSortAscending, Me.mnuFormatSortDescending, Me.ToolStripMenuItem13, Me.mnuFormatConvertToLower, Me.mnuFormatConvertToUpper, Me.mnuFormatConvertToTitleCase, Me.mnuFormatConvertToTitleCaseDrGrammar})
        Me.mnuFormat.Name = "mnuFormat"
        Me.mnuFormat.Size = New System.Drawing.Size(68, 24)
        Me.mnuFormat.Text = "F&ormat"
        '
        'mnuFormatFont
        '
        Me.mnuFormatFont.Image = CType(resources.GetObject("mnuFormatFont.Image"), System.Drawing.Image)
        Me.mnuFormatFont.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFormatFont.Name = "mnuFormatFont"
        Me.mnuFormatFont.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatFont.Text = "&Font"
        '
        'mnuFormatBold
        '
        Me.mnuFormatBold.Image = CType(resources.GetObject("mnuFormatBold.Image"), System.Drawing.Image)
        Me.mnuFormatBold.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFormatBold.Name = "mnuFormatBold"
        Me.mnuFormatBold.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.B), System.Windows.Forms.Keys)
        Me.mnuFormatBold.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatBold.Text = "&Bold"
        '
        'mnuFormatItalic
        '
        Me.mnuFormatItalic.Image = CType(resources.GetObject("mnuFormatItalic.Image"), System.Drawing.Image)
        Me.mnuFormatItalic.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuFormatItalic.Name = "mnuFormatItalic"
        Me.mnuFormatItalic.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys)
        Me.mnuFormatItalic.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatItalic.Text = "&Italic"
        '
        'mnuFormatUnderline
        '
        Me.mnuFormatUnderline.Image = CType(resources.GetObject("mnuFormatUnderline.Image"), System.Drawing.Image)
        Me.mnuFormatUnderline.Name = "mnuFormatUnderline"
        Me.mnuFormatUnderline.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.U), System.Windows.Forms.Keys)
        Me.mnuFormatUnderline.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatUnderline.Text = "&Underline"
        '
        'mnuFormatStrikeout
        '
        Me.mnuFormatStrikeout.Image = CType(resources.GetObject("mnuFormatStrikeout.Image"), System.Drawing.Image)
        Me.mnuFormatStrikeout.Name = "mnuFormatStrikeout"
        Me.mnuFormatStrikeout.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatStrikeout.Text = "&Strikeout"
        '
        'mnuFormatRtfSeparator
        '
        Me.mnuFormatRtfSeparator.Name = "mnuFormatRtfSeparator"
        Me.mnuFormatRtfSeparator.Size = New System.Drawing.Size(341, 6)
        '
        'mnuFormatSortAscending
        '
        Me.mnuFormatSortAscending.Name = "mnuFormatSortAscending"
        Me.mnuFormatSortAscending.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatSortAscending.Text = "Sort &ascending"
        '
        'mnuFormatSortDescending
        '
        Me.mnuFormatSortDescending.Name = "mnuFormatSortDescending"
        Me.mnuFormatSortDescending.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatSortDescending.Text = "Sort &descending"
        '
        'ToolStripMenuItem13
        '
        Me.ToolStripMenuItem13.Name = "ToolStripMenuItem13"
        Me.ToolStripMenuItem13.Size = New System.Drawing.Size(341, 6)
        '
        'mnuFormatConvertToLower
        '
        Me.mnuFormatConvertToLower.Name = "mnuFormatConvertToLower"
        Me.mnuFormatConvertToLower.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatConvertToLower.Text = "Convert to &lower case"
        '
        'mnuFormatConvertToUpper
        '
        Me.mnuFormatConvertToUpper.Name = "mnuFormatConvertToUpper"
        Me.mnuFormatConvertToUpper.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatConvertToUpper.Text = "Convert to &upper case"
        '
        'mnuFormatConvertToTitleCase
        '
        Me.mnuFormatConvertToTitleCase.Name = "mnuFormatConvertToTitleCase"
        Me.mnuFormatConvertToTitleCase.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatConvertToTitleCase.Text = "Convert to &title case"
        '
        'mnuFormatConvertToTitleCaseDrGrammar
        '
        Me.mnuFormatConvertToTitleCaseDrGrammar.Name = "mnuFormatConvertToTitleCaseDrGrammar"
        Me.mnuFormatConvertToTitleCaseDrGrammar.Size = New System.Drawing.Size(344, 24)
        Me.mnuFormatConvertToTitleCaseDrGrammar.Text = "Convert to title case (Dr. &Grammar rules)"
        '
        'mnuTools
        '
        Me.mnuTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuToolsOptions})
        Me.mnuTools.Name = "mnuTools"
        Me.mnuTools.Size = New System.Drawing.Size(57, 24)
        Me.mnuTools.Text = "&Tools"
        '
        'mnuToolsOptions
        '
        Me.mnuToolsOptions.Image = CType(resources.GetObject("mnuToolsOptions.Image"), System.Drawing.Image)
        Me.mnuToolsOptions.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuToolsOptions.Name = "mnuToolsOptions"
        Me.mnuToolsOptions.Size = New System.Drawing.Size(130, 24)
        Me.mnuToolsOptions.Text = "&Options"
        '
        'mnuHelp
        '
        Me.mnuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuHelpReportABug, Me.mnuHelp0, Me.mnuHelpAbout})
        Me.mnuHelp.Name = "mnuHelp"
        Me.mnuHelp.Size = New System.Drawing.Size(53, 24)
        Me.mnuHelp.Text = "&Help"
        '
        'mnuHelpReportABug
        '
        Me.mnuHelpReportABug.Image = CType(resources.GetObject("mnuHelpReportABug.Image"), System.Drawing.Image)
        Me.mnuHelpReportABug.Name = "mnuHelpReportABug"
        Me.mnuHelpReportABug.Size = New System.Drawing.Size(165, 24)
        Me.mnuHelpReportABug.Text = "Report a bug"
        '
        'mnuHelp0
        '
        Me.mnuHelp0.Name = "mnuHelp0"
        Me.mnuHelp0.Size = New System.Drawing.Size(162, 6)
        '
        'mnuHelpAbout
        '
        Me.mnuHelpAbout.Image = CType(resources.GetObject("mnuHelpAbout.Image"), System.Drawing.Image)
        Me.mnuHelpAbout.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.mnuHelpAbout.Name = "mnuHelpAbout"
        Me.mnuHelpAbout.Size = New System.Drawing.Size(165, 24)
        Me.mnuHelpAbout.Text = "&About"
        '
        'tabFiles
        '
        Me.tabFiles.Cursor = System.Windows.Forms.Cursors.Default
        Me.tabFiles.Font = New System.Drawing.Font("Tahoma", 8.0!)
        Me.tabFiles.Location = New System.Drawing.Point(12, 31)
        Me.tabFiles.Margin = New System.Windows.Forms.Padding(0)
        Me.tabFiles.Name = "tabFiles"
        Me.tabFiles.SelectedIndex = 0
        Me.tabFiles.Size = New System.Drawing.Size(320, 197)
        Me.tabFiles.TabIndex = 0
        '
        'tmrQuickAutoSave
        '
        '
        'fswLocationTxt
        '
        Me.fswLocationTxt.EnableRaisingEvents = True
        Me.fswLocationTxt.NotifyFilter = System.IO.NotifyFilters.LastWrite
        Me.fswLocationTxt.SynchronizingObject = Me
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(542, 320)
        Me.Controls.Add(Me.tls)
        Me.Controls.Add(Me.tabFiles)
        Me.Controls.Add(Me.mnu)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.mnu
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MinimumSize = New System.Drawing.Size(320, 200)
        Me.Name = "MainForm"
        Me.Text = "QText"
        Me.mnxTab.ResumeLayout(False)
        Me.tls.ResumeLayout(False)
        Me.tls.PerformLayout()
        Me.mnxTextBox.ResumeLayout(False)
        Me.mnu.ResumeLayout(False)
        Me.mnu.PerformLayout()
        CType(Me.fswLocationTxt, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tabFiles As QTextAux.TabControlDnD
    Friend WithEvents tmrAutoSave As System.Windows.Forms.Timer
    Friend WithEvents mnxTab As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnxTabSaveNow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTabDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTabRename As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTabNew As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTab0 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTabReopen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTab1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTab2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tls As System.Windows.Forms.ToolStrip
    Friend WithEvents tls_btnNew As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnSaveNow As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnRename As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_0 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tls_btnOptions As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnCut As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnPaste As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnAlwaysOnTop As System.Windows.Forms.ToolStripButton

    Friend WithEvents tmrUpdateToolbar As System.Windows.Forms.Timer
    Friend WithEvents tls_btnUndo As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnxTabPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTabPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tls_btnPrintPreview As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnPrint As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTextBox As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnxTextBoxUndo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBox0 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTextBoxCut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxPaste As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tls_btnRedo As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnxTextBoxRedo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEdit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileNew As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuFileReopen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuFileSaveNow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSaveAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuFileDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileRename As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuFilePrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFilePrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuFileClose As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileExit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditUndo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditRedo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuEditCut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditPaste As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuEditSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewAlwaysOnTop As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem11 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuViewRefresh As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuTools As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuToolsOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelpAbout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tooltip As System.Windows.Forms.ToolTip
    Friend WithEvents mnuFormat As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFormatSortAscending As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFormatSortDescending As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem13 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuFormatConvertToLower As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFormatConvertToUpper As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFormatConvertToTitleCase As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFormatConvertToTitleCaseDrGrammar As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxFormat As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxSortAZ As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnu As QText.MenuStripExOnMainForm
    Friend WithEvents mnxTextBoxSortZA As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTextBoxConvertCaseToUpper As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxConvertCaseToLower As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxConvertCaseToTitleCapitalizeAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxConvertCaseToTitleDrGrammar As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tmrQuickAutoSave As System.Windows.Forms.Timer
    Friend WithEvents mnuFormatBold As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFormatItalic As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFormatUnderline As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFormatRtfSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuFormatStrikeout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxBold As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxItalic As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxUnderline As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxStrikeout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxRtfSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tls_btnBold As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnItalic As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnUnderline As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_btnStrikeout As System.Windows.Forms.ToolStripButton
    Friend WithEvents tls_RtfSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTextBoxFont As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tls_btnFont As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuFormatFont As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewToolbar As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents fswLocationTxt As System.IO.FileSystemWatcher
    Friend WithEvents mnxConvertTo As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTabConvertToPlainText As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTabConvertToRichText As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileConvertToPlainText As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileConvertToRichText As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem15 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuHelpReportABug As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelp0 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItem12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuEditFind As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditFindNext As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tlbHelpAbout As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlbHelpReportABug As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItem14 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTabOpenContainingFolder As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxCopyAsText As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnxTextBoxPasteAsText As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem16 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tls_btnFind As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnxTextBoxCutCopyPasteAsTextSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnxTextBoxCutAsText As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem17 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuViewZoomIn As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewZoomOut As System.Windows.Forms.ToolStripMenuItem
End Class
