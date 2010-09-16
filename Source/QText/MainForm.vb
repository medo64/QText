Imports System.Drawing
Imports QTextAux

Friend Class MainForm

    Friend _suppressMenuKey As Boolean = False
    Friend _dpiX As Single, _dpiY As Single
    Friend _dpiRatioX As Single, _dpiRatioY As Single
    Friend _findForm As FindForm

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.Font = System.Drawing.SystemFonts.MessageBoxFont

        Using g As System.Drawing.Graphics = Me.CreateGraphics()
            _dpiX = g.DpiX
            _dpiY = g.DpiY
            _dpiRatioX = CSng(Math.Round(_dpiX / 96, 2))
            _dpiRatioY = CSng(Math.Round(_dpiY / 96, 2))
            System.Diagnostics.Trace.TraceInformation("DPI: {0}x{1}; Ratio:{2}x{3}", Me._dpiX, Me._dpiY, Me._dpiRatioX, Me._dpiRatioY)
        End Using
        If (QTextAux.Settings.ZoomToolbarWithDpiChange) Then
            tls.ImageScalingSize = New Size(CInt(16 * _dpiRatioX), CInt(16 * _dpiRatioY))
            tls.Scale(New SizeF(_dpiRatioX, _dpiRatioY))
        End If

        ' Add any initialization after the InitializeComponent() call.
        tmrQuickAutoSave.Interval = Settings.QuickAutoSaveSeconds * 1000

        mnuFileClose.Visible = True
        tabFiles.Multiline = Settings.DisplayMultilineTabHeader
        If Settings.DisplayMinimizeMaximizeButtons Then
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
        Else
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.SizableToolWindow
        End If
        Me.ShowInTaskbar = Settings.DisplayShowInTaskbar

        Global.Medo.Windows.Forms.State.Load(Me)
    End Sub

    <System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags:=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)> _
     Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Select Case keyData

            Case Keys.Control Or Keys.Tab, Keys.Control Or Keys.Shift Or Keys.Tab
                If (tabFiles.TabPages.Count >= 1) AndAlso (tabFiles.SelectedTab Is Nothing) Then tabFiles.SelectedTab = tabFiles.TabPages(0)
                If (tabFiles.TabPages.Count >= 2) Then
                    Dim tp As TabPage = GetPreviousSelectedTab()
                    If (tp IsNot Nothing) Then
                        tabFiles.SelectedTab = tp
                    Else
                        Dim i As Integer = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab)
                        i = (i + 1) Mod tabFiles.TabPages.Count
                        tabFiles.SelectedTab = tabFiles.TabPages(i)
                    End If
                End If
                keyData = Keys.None
                Return True

            Case Else
                Return MyBase.ProcessCmdKey(msg, keyData)

        End Select
    End Function


    Private Sub frmMain_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        Me.tmrUpdateToolbar.Enabled = Settings.DisplayShowToolbar
        If (tabFiles.SelectedTab IsNot Nothing) Then
            DirectCast(tabFiles.SelectedTab, QTextAux.TabFile).TextBox.Focus()
        End If
    End Sub

    Private Sub frmMain_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
        mnu.Visible = Settings.ShowMenu 'TODO: Check this on XP when compiled
        Global.Medo.Windows.Forms.State.Save(Me)
        Me.tmrUpdateToolbar.Enabled = False
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        mnu.Visible = Settings.ShowMenu
        Call mnuViewRefresh_Click(Nothing, Nothing)
        Call frmMain_Resize(Nothing, Nothing)
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Global.Medo.Windows.Forms.State.Save(Me)

        Call mnu_Leave(Nothing, Nothing)
        Try
            If (Settings.SaveOnHide) Then SaveAllChanged()
            SaveFileOrder(e.CloseReason = CloseReason.WindowsShutDown)
        Catch ex As Exception
            Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
        End Try

        If (e.Cancel) Then Exit Sub

        If (e.CloseReason = CloseReason.UserClosing) Then
            e.Cancel = True
            Me.Hide()
        Else
            Try
                Application.Exit()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub frmMain_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Debug.WriteLine("MainForm.KeyDown:" + e.KeyData.ToString())
        tmrQuickAutoSave.Enabled = False

        Select Case e.KeyData

            Case Keys.Control Or Keys.D1
                If tabFiles.TabPages.Count >= 1 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(0)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D2
                If tabFiles.TabPages.Count >= 2 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(1)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D3
                If tabFiles.TabPages.Count >= 3 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(2)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D4
                If tabFiles.TabPages.Count >= 4 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(3)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D5
                If tabFiles.TabPages.Count >= 5 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(4)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D6
                If tabFiles.TabPages.Count >= 6 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(5)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D7
                If tabFiles.TabPages.Count >= 7 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(6)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D8
                If tabFiles.TabPages.Count >= 8 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(7)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D9
                If tabFiles.TabPages.Count >= 9 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(8)
                End If
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Control Or Keys.D0
                If tabFiles.TabPages.Count >= 10 Then
                    tabFiles.SelectedTab = tabFiles.TabPages(9)
                End If
                e.Handled = True : e.SuppressKeyPress = True

                'Case Keys.Control Or Keys.N
                '	'Call mnxFileNew_Click(Nothing, Nothing)
                '	e.Handled = True : e.SuppressKeyPress = True

                'Case Keys.Control Or Keys.O
                '	Call mnxFileImport_Click(Nothing, Nothing)
                '	e.Handled = True : e.SuppressKeyPress = True

                'Case Keys.Control Or Keys.R
                '	Call mnxFileReopen_Click(Nothing, Nothing)
                '	e.Handled = True : e.SuppressKeyPress = True

                'Case Keys.Control Or Keys.S
                '	Call mnxFileSaveNow_Click(Nothing, Nothing)
                '	e.Handled = True : e.SuppressKeyPress = True

                'Case Keys.Control Or Keys.Shift Or Keys.S
                '	Call mnxFileSaveAll_Click(Nothing, Nothing)
                '	e.Handled = True : e.SuppressKeyPress = True

                'Case Keys.F2
                '	Call mnxFileRename_Click(Nothing, Nothing)
                '	e.Handled = True : e.SuppressKeyPress = True

                'Case Keys.Control Or Keys.P
                '	Call mnxFilePrint_Click(Nothing, Nothing)
                '	e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Escape
                Call mnuFileClose_Click(Nothing, Nothing)
                e.Handled = True : e.SuppressKeyPress = True


                'Case Keys.Control Or Keys.T
                '	Options.DisplayAlwaysOnTop = Not Options.DisplayAlwaysOnTop
                '	Me.TopMost = Options.DisplayAlwaysOnTop
                '	e.Handled = True : e.SuppressKeyPress = True

                'Case Keys.F5
                '	Call mnxViewRefresh_Click(mnxViewRefresh, Nothing)
                '	e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.Back
                Call mnuEditUndo_Click(Nothing, Nothing)
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.Shift Or Keys.Back
                Call mnuEditRedo_Click(Nothing, Nothing)
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.Left, Keys.Control Or Keys.PageDown
                If (tabFiles.TabPages.Count >= 1) AndAlso (tabFiles.SelectedTab Is Nothing) Then tabFiles.SelectedTab = tabFiles.TabPages(0)
                If (tabFiles.TabPages.Count >= 2) Then
                    Dim i As Integer = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab)
                    i = (tabFiles.TabPages.Count + i - 1) Mod tabFiles.TabPages.Count
                    tabFiles.SelectedTab = tabFiles.TabPages(i)
                End If
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.Right, Keys.Control Or Keys.PageUp
                If (tabFiles.TabPages.Count >= 1) AndAlso (tabFiles.SelectedTab Is Nothing) Then tabFiles.SelectedTab = tabFiles.TabPages(0)
                If (tabFiles.TabPages.Count >= 2) Then
                    Dim i As Integer = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab)
                    i = (i + 1) Mod tabFiles.TabPages.Count
                    tabFiles.SelectedTab = tabFiles.TabPages(i)
                End If
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.F
                mnu.Visible = True
                mnuFile.ShowDropDown()
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.E
                mnu.Visible = True
                mnuEdit.ShowDropDown()
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.V
                mnu.Visible = True
                mnuView.ShowDropDown()
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.O
                mnu.Visible = True
                mnuFormat.ShowDropDown()
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.T
                mnu.Visible = True
                mnuTools.ShowDropDown()
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.H
                mnu.Visible = True
                mnuHelp.ShowDropDown()
                Me._suppressMenuKey = True
                e.Handled = True : e.SuppressKeyPress = True

            Case Keys.Alt Or Keys.Menu 'just to prevent suppressing menu key

            Case Else
                If e.Alt Then Me._suppressMenuKey = True

        End Select

        If (Settings.EnableQuickAutoSave) Then
            tmrQuickAutoSave.Enabled = True
        End If
    End Sub

    Private Sub MainForm_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        tmrQuickAutoSave.Enabled = False

        Debug.WriteLine("MainForm.KeyUp: " + e.KeyData.ToString())
        Select Case e.KeyData

            Case Keys.Menu
                If (Me._suppressMenuKey) Then
                    Debug.WriteLine("Suppress.")
                    Me._suppressMenuKey = False
                    Return
                End If
                If Not Settings.ShowMenu Then
                    If mnu.Visible = True Then Exit Sub
                    mnu.Visible = True
                    mnu.Select()
                    mnuFile.Select()
                    e.Handled = True : e.SuppressKeyPress = True
                End If

            Case Keys.Control Or Keys.B


        End Select

        If (Settings.EnableQuickAutoSave) Then
            tmrQuickAutoSave.Enabled = True
        End If
    End Sub


    Private frmMain_Resize_Reentry As Boolean = False

    Private Sub frmMain_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If frmMain_Resize_Reentry Then Exit Sub
        frmMain_Resize_Reentry = True

        If (Me.Visible) Then Global.Medo.Windows.Forms.State.Save(Me)
        If (Me.WindowState <> FormWindowState.Minimized) Then
            tls.Visible = Settings.DisplayShowToolbar

            Dim newTop As Integer = 0
            If (Settings.DisplayShowToolbar) Then
                newTop += tls.Height
            End If
            If (Settings.ShowMenu) OrElse (mnu.Visible) Then
                newTop += mnu.Height
            End If

            If (Settings.DisplayShowToolbar = True) Then
                If (mnu.Visible) Then
                    tabFiles.Left = Me.ClientRectangle.Left
                    tabFiles.Top = newTop
                    tabFiles.Width = Me.ClientRectangle.Width
                    tabFiles.Height = Me.ClientRectangle.Height - tabFiles.Top
                Else
                    tabFiles.Left = Me.ClientRectangle.Left
                    tabFiles.Top = newTop
                    tabFiles.Width = Me.ClientRectangle.Width
                    tabFiles.Height = Me.ClientRectangle.Height - tabFiles.Top
                End If
            Else
                tabFiles.Left = Me.ClientRectangle.Left
                tabFiles.Top = newTop
                tabFiles.Width = Me.ClientRectangle.Width
                tabFiles.Height = Me.ClientRectangle.Height - tabFiles.Top
            End If
        ElseIf (Settings.TrayOnMinimize) Then 'window has been minimized
            If (Settings.SaveOnHide) Then SaveAllChanged()
            Me.Hide()
            SaveFileOrder(True)
        End If

        frmMain_Resize_Reentry = False
    End Sub


    Private Sub mnxTab_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles mnxTab.Opening
        mnxTabConvertToPlainText.Visible = True
        mnxTabConvertToRichText.Visible = True
        mnxTabConvertToPlainText.Enabled = True
        mnxTabConvertToRichText.Enabled = True
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As QTextAux.TabFile = DirectCast(tabFiles.SelectedTab, QTextAux.TabFile)
            mnxTabConvertToPlainText.Visible = tf.IsRichTextFormat
            mnxTabConvertToRichText.Visible = Not tf.IsRichTextFormat
        Else
            mnxTabConvertToPlainText.Enabled = False
            mnxTabConvertToRichText.Enabled = False
        End If
    End Sub

    Private Sub mnxTabNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabNew.Click
        Call mnuFileNew_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTabReopen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabReopen.Click
        Call mnuFileReopen_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTabSaveNow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabSaveNow.Click
        Call mnuFileSaveNow_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTabDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabDelete.Click
        Call mnuFileDelete_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTabRename_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabRename.Click
        Call mnuFileRename_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTabPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabPrintPreview.Click
        Call mnuFilePrintPreview_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTabPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabPrint.Click
        Call mnuFilePrint_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTabConvertToPlainText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabConvertToPlainText.Click
        Call mnuFileConvertToPlainText_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTabConvertToRichText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabConvertToRichText.Click
        Call mnuFileConvertToRichText_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnNew.Click
        Call mnuFileNew_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnSaveNow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnSaveNow.Click
        Call mnuFileSaveNow_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnRename_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnRename.Click
        Call mnuFileRename_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnPrintPreview.Click
        Call mnuFilePrintPreview_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnPrint.Click
        Call mnuFilePrint_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnCut.Click
        Call mnuEditCut_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnCopy.Click
        Call mnuEditCopy_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnPaste.Click
        Call mnuEditPaste_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnFont.Click
        Call mnuFormatFont_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnBold_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnBold.Click
        Call mnuFormatBold_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnItalic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnItalic.Click
        Call mnuFormatItalic_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnUnderline_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnUnderline.Click
        Call mnuFormatUnderline_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnStrikeout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnStrikeout.Click
        Call mnuFormatStrikeout_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnUndo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnUndo.Click
        Call mnuEditUndo_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnRedo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnRedo.Click
        Call mnuEditRedo_Click(Nothing, Nothing)
    End Sub

    Private Sub tls_btnAlwaysOnTop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnAlwaysOnTop.Click
        Settings.DisplayAlwaysOnTop = tls_btnAlwaysOnTop.Checked
        Me.TopMost = Settings.DisplayAlwaysOnTop
    End Sub

    Private Sub tls_btnOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnOptions.Click
        Call mnuToolsOptions_Click(Nothing, Nothing)
    End Sub

    Private Sub tabFiles_ChangedOrder(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabFiles.ChangedOrder
        SaveFileOrder(True)
    End Sub

    Private Sub tabFiles_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tabFiles.MouseDown
        If (e.Button = Windows.Forms.MouseButtons.Right) Then
            For i As Integer = 0 To tabFiles.TabPages.Count - 1
                If (tabFiles.GetTabRect(i).Contains(e.X, e.Y)) Then
                    tabFiles.SelectedTab = tabFiles.TabPages(i)

                    Call mnuFile_DropDownOpening(Nothing, Nothing)
                    mnxTabNew.Enabled = mnuFileNew.Enabled
                    mnxTabReopen.Enabled = mnuFileReopen.Enabled
                    mnxTabSaveNow.Enabled = mnuFileSaveNow.Enabled
                    mnxTabDelete.Enabled = mnuFileDelete.Enabled
                    mnxTabRename.Enabled = mnuFileRename.Enabled
                    mnxTabPrintPreview.Enabled = mnuFilePrintPreview.Enabled
                    mnxTabPrint.Enabled = mnuFilePrint.Enabled
                    mnxTab.Show(tabFiles, e.X, e.Y)

                    Exit Sub
                End If
            Next

        End If
    End Sub

    Private Sub tabFiles_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tabFiles.SelectedIndexChanged
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, QTextAux.TabFile).TextBox
            txt.Refresh()
            txt.Select()
            txt.Focus()
        End If
        SetSelectedTab(tabFiles.SelectedTab)
    End Sub

    Private Sub tmrAutoSave_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrAutoSave.Tick
        fswLocationTxt.EnableRaisingEvents = False
        For i As Integer = 0 To tabFiles.TabPages.Count - 1
            Try
                Dim tf As QTextAux.TabFile = DirectCast(tabFiles.TabPages(i), QTextAux.TabFile)
                If tf.GetIsEligibleForSave(Settings.FilesAutoSaveInterval) Then
                    tf.Save()
                End If
            Catch ex As Exception
                Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
            End Try
        Next
        fswLocationTxt.EnableRaisingEvents = (tabFiles.TabPages.Count > 0)
    End Sub

    Private Sub frmMain_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        If (e.Button = Windows.Forms.MouseButtons.Right) Then
            If (tabFiles.TabPages.Count = 0) OrElse ((e.Y >= tabFiles.GetTabRect(0).Y) AndAlso (e.Y <= (tabFiles.GetTabRect(0).Y + tabFiles.GetTabRect(0).Height)) AndAlso (e.X >= tabFiles.GetTabRect(0).X)) Then
                mnxTabNew.Enabled = True
                mnxTabReopen.Enabled = False
                mnxTabSaveNow.Enabled = False
                mnxTabDelete.Enabled = False
                mnxTabRename.Enabled = False
                mnxTabPrintPreview.Enabled = False
                mnxTabPrint.Enabled = False
                mnxTab.Show(tabFiles, e.X, e.Y)
            End If
        End If
    End Sub



    Private _CurrSelectedTab As TabPage
    Private _PrevSelectedTab As TabPage

    Private Sub SetSelectedTab(ByVal tabPage As TabPage)
        Me._PrevSelectedTab = Me._CurrSelectedTab
        Me._CurrSelectedTab = tabPage
    End Sub

    Private Function GetPreviousSelectedTab() As TabPage
        If (Me._PrevSelectedTab IsNot Nothing) AndAlso (Not Me._PrevSelectedTab.Equals(Me._CurrSelectedTab)) Then
            If (tabFiles.TabPages.Contains(Me._PrevSelectedTab)) Then
                Return Me._PrevSelectedTab
            End If
        End If
        Return Nothing
    End Function


    Private Sub SaveFileOrder(ByVal dontThrowExceptions As Boolean)
        Try
            Using xw As New Global.Medo.Xml.XmlTagWriter(System.IO.Path.Combine(QTextAux.Settings.FilesLocation, "QText.xml"), New System.Text.UTF8Encoding(False))

                xw.XmlTextWriter.WriteStartDocument()

                xw.StartTag("QText") '<QText>

                If (Me.tabFiles.SelectedTab IsNot Nothing) Then
                    xw.StartTag("FileOrder", "selectedTitle", DirectCast(tabFiles.SelectedTab, QTextAux.TabFile).Title, "selectedFileName", DirectCast(tabFiles.SelectedTab, QTextAux.TabFile).FileName) '<FileOrder>
                Else
                    xw.StartTag("FileOrder") '<FileOrder>
                End If

                For i As Integer = 0 To tabFiles.TabPages.Count - 1
                    Dim tf As QTextAux.TabFile = DirectCast(tabFiles.TabPages(i), QTextAux.TabFile)
                    'tf.Save()
                    xw.WriteTag("File", "title", tf.Title, "fileName", tf.FileName)
                Next

                xw.EndTag() '</FileOrder>

                xw.EndTag()    '</QText>
            End Using
        Catch
            If (dontThrowExceptions = False) Then Throw
        End Try
    End Sub

    Private Sub tmrUpdateToolbar_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrUpdateToolbar.Tick
        If (Not Me.Created) Then Exit Sub

        If (tabFiles.SelectedTab IsNot Nothing) Then
            tls_btnNew.Enabled = True
            tls_btnSaveNow.Enabled = DirectCast(tabFiles.SelectedTab, QTextAux.TabFile).IsChanged
            tls_btnRename.Enabled = True
            tls_btnFind.Enabled = True
        Else
            tls_btnNew.Enabled = True
            tls_btnSaveNow.Enabled = False
            tls_btnRename.Enabled = False
            tls_btnFind.Enabled = False
        End If
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As QTextAux.TabFile = DirectCast(tabFiles.SelectedTab, QTextAux.TabFile)

            If (tf.IsRichTextFormat) Then
                tls_btnFont.Visible = True
                tls_btnBold.Visible = True
                tls_btnItalic.Visible = True
                tls_btnUnderline.Visible = True
                tls_btnStrikeout.Visible = True
                tls_RtfSeparator.Visible = True

                tls_btnBold.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Bold)
                tls_btnItalic.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Italic)
                tls_btnUnderline.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Underline)
                tls_btnStrikeout.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Strikeout)
            Else
                tls_btnFont.Visible = False
                tls_btnBold.Visible = False
                tls_btnItalic.Visible = False
                tls_btnUnderline.Visible = False
                tls_btnStrikeout.Visible = False
                tls_RtfSeparator.Visible = False
            End If

            'Dim txt As TextBoxBase = tf.TextBox

            tls_btnUndo.Enabled = tf.CanUndo
            tls_btnRedo.Enabled = tf.CanRedo
            tls_btnCut.Enabled = tf.CanCopy
            tls_btnCopy.Enabled = tf.CanCopy
            tls_btnPaste.Enabled = tf.CanPaste
        Else
            tls_btnFont.Visible = False
            tls_btnBold.Visible = False
            tls_btnItalic.Visible = False
            tls_btnUnderline.Visible = False
            tls_btnStrikeout.Visible = False
            tls_RtfSeparator.Visible = False

            tls_btnUndo.Enabled = False
            tls_btnRedo.Enabled = False
            tls_btnCut.Enabled = False
            tls_btnCopy.Enabled = False
            tls_btnPaste.Enabled = False
        End If
        tls_btnAlwaysOnTop.Checked = Settings.DisplayAlwaysOnTop
    End Sub


    Private Sub SaveAllChanged()
        fswLocationTxt.EnableRaisingEvents = False
        For i As Integer = 0 To tabFiles.TabPages.Count - 1
            Try
                Dim tf As QTextAux.TabFile = DirectCast(tabFiles.TabPages(i), QTextAux.TabFile)
                If (tf.IsChanged) Then
                    tf.Save()
                End If
            Catch ex As Exception
                Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
            End Try
        Next
        SaveFileOrder(True)
        fswLocationTxt.EnableRaisingEvents = True
    End Sub


#Region "Printing"

    Private _PageNumber As Integer

    Private Sub Document_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs)
        Me._PageNumber = 1
    End Sub

    Private Sub Document_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs)
        Using font As New System.Drawing.Font("Tahoma", 10)
            e.Graphics.DrawString(DirectCast(sender, Global.Medo.Drawing.Printing.FullText).Document.DocumentName, font, Drawing.Brushes.Black, 0, -10 / 25.4 * 100)

            'used now only for height measurements
            Dim textC As String = Global.Medo.Reflection.EntryAssembly.Title
            Dim sizeC As Drawing.SizeF = e.Graphics.MeasureString(textC, font)
            If (QTextAux.Settings.PrintApplicationName) Then
                e.Graphics.DrawString(textC, font, Drawing.Brushes.Black, (e.MarginBounds.Width - sizeC.Width) / 2, -10 / 25.4 * 100)
            End If

            Dim textR As String = Me._PageNumber.ToString
            Dim sizeR As Drawing.SizeF = e.Graphics.MeasureString(textR, font)
            e.Graphics.DrawString(textR, font, Drawing.Brushes.Black, e.MarginBounds.Right - sizeR.Width, -10 / 25.4 * 100)

            e.Graphics.DrawLine(Drawing.Pens.Black, 0.0, CSng(-10 / 25.4 * 100 + sizeC.Height), e.MarginBounds.Right, CSng(-10 / 25.4 * 100 + sizeC.Height))
        End Using

        Me._PageNumber += 1
    End Sub

#End Region

    Private Sub tls_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tls.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Dim tsi As ToolStripItem = tls.Items(tls.Items.Count - 1)
            If (e.X > tsi.Bounds.Right) Then
                'mnu.Show(tls, e.Location)
            End If
        End If
    End Sub

#Region "TextBox ContextMenu"

    Private Sub mnxTextBox_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles mnxTextBox.Opening
        mnxTextBoxCutAsText.Visible = (Not QTextAux.Settings.ForceTextCopyPaste)
        mnxTextBoxCopyAsText.Visible = (Not QTextAux.Settings.ForceTextCopyPaste)
        mnxTextBoxPasteAsText.Visible = (Not QTextAux.Settings.ForceTextCopyPaste)
        mnxTextBoxCutCopyPasteAsTextSeparator.Visible = (Not QTextAux.Settings.ForceTextCopyPaste)

        mnxTextBoxFont.Visible = False
        mnxTextBoxBold.Visible = False
        mnxTextBoxItalic.Visible = False
        mnxTextBoxUnderline.Visible = False
        mnxTextBoxStrikeout.Visible = False
        mnxTextBoxRtfSeparator.Visible = False

        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As QTextAux.TabFile = DirectCast(tabFiles.SelectedTab, TabFile)

            If tf.IsRichTextFormat Then
                mnxTextBoxFont.Visible = True
                mnxTextBoxBold.Visible = True
                mnxTextBoxItalic.Visible = True
                mnxTextBoxUnderline.Visible = True
                mnxTextBoxStrikeout.Visible = True
                mnxTextBoxRtfSeparator.Visible = False

                mnxTextBoxBold.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Bold)
                mnxTextBoxItalic.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Italic)
                mnxTextBoxUnderline.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Underline)
                mnxTextBoxStrikeout.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Strikeout)
            End If

            Dim txt As TextBoxBase = tf.TextBox

            mnxTextBoxUndo.Enabled = tf.CanUndo
            mnxTextBoxRedo.Enabled = tf.CanRedo
            mnxTextBoxCut.Enabled = tf.CanCopy
            mnxTextBoxCopy.Enabled = tf.CanCopy
            mnxTextBoxPaste.Enabled = tf.CanPaste
            mnxTextBoxCutAsText.Enabled = tf.CanCopy
            mnxTextBoxCopyAsText.Enabled = tf.CanCopy
            mnxTextBoxPasteAsText.Enabled = tf.CanPaste
            mnxTextBoxSelectAll.Enabled = (txt.Text.Length > 0)
            mnxTextBoxFormat.Enabled = (txt.SelectedText.Length > 0)
        Else
            mnxTextBoxUndo.Enabled = False
            mnxTextBoxRedo.Enabled = False
            mnxTextBoxCut.Enabled = False
            mnxTextBoxCopy.Enabled = False
            mnxTextBoxPaste.Enabled = False
            mnxTextBoxSelectAll.Enabled = False
            mnxTextBoxFormat.Enabled = False
        End If
    End Sub

    Private Sub mnxTextBoxUndo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxUndo.Click
        Call mnuEditUndo_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxRedo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxRedo.Click
        Call mnuEditRedo_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxCut.Click
        Call mnuEditCut_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxCopy.Click
        Call mnuEditCopy_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxPaste.Click
        Call mnuEditPaste_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxSelectAll.Click
        Call mnuEditSelectAll_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxFont.Click
        Call mnuFormatFont_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxBold_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxBold.Click
        Call mnuFormatBold_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxItalic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxItalic.Click
        Call mnuFormatItalic_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxUnderline_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxUnderline.Click
        Call mnuFormatUnderline_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxStrikeout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxStrikeout.Click
        Call mnuFormatStrikeout_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxSortAZ_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxSortAZ.Click
        Call mnuFormatSortAscending_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxSortZA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxSortZA.Click
        Call mnuFormatSortDescending_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxConvertCaseToUpper_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxConvertCaseToUpper.Click
        Call mnuFormatConvertToUpper_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxConvertCaseToLower_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxConvertCaseToLower.Click
        Call mnuFormatConvertToLower_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxConvertCaseToTitleCapitalizeAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxConvertCaseToTitleCapitalizeAll.Click
        Call mnuFormatConvertToTitleCase_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxConvertCaseToTitleDrGrammar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxConvertCaseToTitleDrGrammar.Click
        Call mnuFormatConvertToTitleCaseDrGrammar_Click(Nothing, Nothing)
    End Sub

#End Region


#Region "Comparers"

    Private Class SortAZ : Implements IComparer
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Return String.Compare(x.ToString, y.ToString)
        End Function
    End Class

    Private Class SortZA : Implements IComparer
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Return -String.Compare(x.ToString, y.ToString)
        End Function
    End Class

#End Region

    Private Sub mnu_MenuDeactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu.MenuDeactivate
        'mnu.Visible = False
    End Sub

    Private Sub mnu_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu.Leave
        If Not Settings.ShowMenu Then
            If (mnu.Visible <> False) Then
                mnu.Visible = False
                Call frmMain_Resize(Nothing, Nothing)
            End If
        End If
    End Sub

    Private Sub mnu_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu.VisibleChanged
        frmMain_Resize(Nothing, Nothing)
    End Sub


    Private Sub mnuFile_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFile.DropDownOpening
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            mnuFileNew.Enabled = True
            mnuFileReopen.Enabled = True
            mnuFileConvertToPlainText.Enabled = tf.IsRichTextFormat
            mnuFileConvertToRichText.Enabled = Not tf.IsRichTextFormat
            mnuFileSaveNow.Enabled = True
            mnuFileSaveAll.Enabled = True
            mnuFileDelete.Enabled = True
            mnuFileRename.Enabled = True
            mnuFilePrintPreview.Enabled = True
            mnuFilePrint.Enabled = True
            mnuFileClose.Enabled = True
            mnuFileExit.Enabled = True
        Else
            mnuFileNew.Enabled = True
            mnuFileReopen.Enabled = False
            mnuFileConvertToPlainText.Enabled = False
            mnuFileConvertToRichText.Enabled = False
            mnuFileSaveNow.Enabled = False
            mnuFileSaveAll.Enabled = False
            mnuFileDelete.Enabled = False
            mnuFileRename.Enabled = False
            mnuFilePrintPreview.Enabled = False
            mnuFilePrint.Enabled = False
            mnuFileClose.Enabled = True
            mnuFileExit.Enabled = True
        End If
    End Sub

    Private Sub mnuFileNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileNew.Click
        fswLocationTxt.EnableRaisingEvents = False
        SaveFileOrder(True)

        Using frm As New QTextAux.NewFileForm("")
            If (frm.ShowDialog(Me) = Windows.Forms.DialogResult.OK) Then
                Try
                    Dim t As TabFile
                    If (frm.IsRichText) Then
                        t = New TabFile(System.IO.Path.Combine(QTextAux.Settings.FilesLocation, frm.FileName) + ".rtf", App.Form.mnxTextBox, True)
                    Else
                        t = New TabFile(System.IO.Path.Combine(QTextAux.Settings.FilesLocation, frm.FileName) + ".txt", App.Form.mnxTextBox, True)
                    End If
                    tabFiles.TabPages.Add(t)
                    tabFiles.SelectedTab = t
                Catch ex As Exception
                    Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
                End Try
            End If
        End Using

        SaveFileOrder(True)
        fswLocationTxt.EnableRaisingEvents = True
    End Sub

    Private Sub mnuFileReopen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileReopen.Click
        fswLocationTxt.EnableRaisingEvents = False
        SaveFileOrder(True)

        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            If tf.IsChanged Then
                Select Case Global.Medo.MessageBox.ShowQuestion(Me, "File is not saved. Are you sure?", Global.Medo.Reflection.EntryAssembly.Title + ": Reload", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2)
                    Case Windows.Forms.DialogResult.Yes
                        Try
                            tf.Reopen()
                        Catch ex As Exception
                            Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
                        End Try
                    Case Windows.Forms.DialogResult.No
                End Select
            End If
        End If

        fswLocationTxt.EnableRaisingEvents = True
    End Sub

    Private Sub mnuFileConvertToPlainText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileConvertToPlainText.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            If Global.Medo.MessageBox.ShowQuestion(Me, "Conversion will remove all formating (font, style, etc.). Do you want to continue?", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
                tf.ConvertToPlainText(New FileOrder())
                SaveFileOrder(True)
            End If
        End If
    End Sub

    Private Sub mnuFileConvertToRichText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileConvertToRichText.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            tf.ConvertToRichText(New FileOrder())
            SaveFileOrder(True)
        End If
    End Sub

    Private Sub mnuFileSaveNow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileSaveNow.Click
        fswLocationTxt.EnableRaisingEvents = False
        SaveFileOrder(True)

        If (tabFiles.SelectedTab IsNot Nothing) Then
            Try
                Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
                tf.Save()
                SaveFileOrder(True)
            Catch ex As Exception
                Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
            End Try
        End If

        fswLocationTxt.EnableRaisingEvents = True
    End Sub

    Private Sub mnuFileSaveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileSaveAll.Click
        fswLocationTxt.EnableRaisingEvents = False
        SaveFileOrder(True)

        For i As Integer = 0 To tabFiles.TabPages.Count - 1
            Try
                Dim tf As TabFile = DirectCast(tabFiles.TabPages(i), TabFile)
                tf.Save()
            Catch ex As Exception
                Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message)
            End Try
        Next

        fswLocationTxt.EnableRaisingEvents = True
    End Sub

    Private Sub mnuFileDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileDelete.Click
        fswLocationTxt.EnableRaisingEvents = False
        SaveFileOrder(True)

        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            If (tf.TextBox.Text.Length > 0) Then
                Select Case Global.Medo.MessageBox.ShowQuestion(Me, "Are you sure?", Global.Medo.Reflection.EntryAssembly.Title + " : Delete", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2)
                    Case Windows.Forms.DialogResult.Yes
                    Case Windows.Forms.DialogResult.No
                        Exit Sub
                End Select
            End If
            Try
                Dim tindex As Integer = tabFiles.TabPages.IndexOf(tf) + 1 'select next tab
                If (tindex >= tabFiles.TabPages.Count) Then
                    tindex -= 2 'go to one in front of it
                End If
                If (tindex > 0) AndAlso (tindex < tabFiles.TabPages.Count) Then
                    tabFiles.SelectedTab = tabFiles.TabPages(tindex)
                End If

                tf.Delete()
                tabFiles.TabPages.Remove(tf)
            Catch ex As Exception
                Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
            End Try
        End If

        fswLocationTxt.EnableRaisingEvents = True
    End Sub

    Private Sub mnuFileRename_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileRename.Click
        fswLocationTxt.EnableRaisingEvents = False
        SaveFileOrder(True)

        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            Try
                Using frm As New QTextAux.RenameFileForm(tf.Title)
                    If (frm.ShowDialog(Me) = Windows.Forms.DialogResult.OK) Then
                        tf.Rename(frm.Title)
                    End If
                End Using
            Catch ex As Exception
                Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
            End Try
        End If

        fswLocationTxt.EnableRaisingEvents = True
    End Sub

    Private Sub mnuFilePrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFilePrintPreview.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            Try
                Dim ol As New Global.Medo.Drawing.Printing.FullText(tf.Title, 10, 10, 20, 10)
                AddHandler ol.BeginPrint, AddressOf Document_BeginPrint
                AddHandler ol.StartPrintPage, AddressOf Document_PrintPage
                ol.Font = QTextAux.Settings.DisplayFont
                ol.Text = tf.TextBox.Text
                Dim x As New Global.Medo.Windows.Forms.PrintPreviewDialog(ol.Document)
                x.ShowDialog(Me)
            Catch ex As Exception
                Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
            End Try
        End If
    End Sub

    Private Sub mnuFilePrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFilePrint.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            Try
                Dim ol As New Medo.Drawing.Printing.FullText(tf.Title, 10, 10, 20, 10)
                AddHandler ol.BeginPrint, AddressOf Document_BeginPrint
                AddHandler ol.StartPrintPage, AddressOf Document_PrintPage
                ol.Font = QTextAux.Settings.DisplayFont
                ol.Text = tf.TextBox.Text
                ol.Print()
            Catch ex As Exception
                Global.Medo.MessageBox.ShowWarning(Me, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
            End Try
        End If
    End Sub

    Private Sub mnuFileClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileClose.Click
        If (Settings.SaveOnHide) Then SaveAllChanged()
        Me.Hide()
        SaveFileOrder(True)
    End Sub

    Private Sub mnuFileExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileExit.Click
        Application.Exit()
    End Sub


    Private Sub mnuEdit_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEdit.DropDownOpening
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            Dim txt As TextBoxBase = tf.TextBox
            mnuEditUndo.Enabled = tf.CanUndo
            mnuEditRedo.Enabled = tf.CanRedo
            mnuEditCut.Enabled = tf.CanCopy
            mnuEditCopy.Enabled = tf.CanCopy
            mnuEditPaste.Enabled = tf.CanPaste
            mnuEditDelete.Enabled = (txt.SelectedText.Length > 0) OrElse (txt.SelectionStart < txt.Text.Length)
            mnuEditSelectAll.Enabled = (txt.Text.Length > 0)
            mnuEditFind.Enabled = True
            mnuEditFindNext.Enabled = Not String.IsNullOrEmpty(QTextAux.SearchStatus.Text)
        Else
            mnuEditUndo.Enabled = False
            mnuEditRedo.Enabled = False
            mnuEditCut.Enabled = False
            mnuEditCopy.Enabled = False
            mnuEditPaste.Enabled = False
            mnuEditDelete.Enabled = False
            mnuEditSelectAll.Enabled = False
            mnuEditFind.Enabled = False
            mnuEditFindNext.Enabled = False
        End If
    End Sub

    Friend Sub mnuEditUndo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditUndo.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            DirectCast(tabFiles.SelectedTab, TabFile).Undo()
        End If
    End Sub

    Friend Sub mnuEditRedo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditRedo.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            DirectCast(tabFiles.SelectedTab, TabFile).Redo()
        End If
    End Sub

    Private Sub mnuEditCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditCut.Click
        Try
            If (tabFiles.SelectedTab IsNot Nothing) Then
                DirectCast(tabFiles.SelectedTab, TabFile).Cut(QTextAux.Settings.ForceTextCopyPaste)
            End If
        Catch ex As Exception
            Global.Medo.MessageBox.ShowWarning(Me, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message)
        End Try
    End Sub

    Private Sub mnuEditCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditCopy.Click
        Try
            If (tabFiles.SelectedTab IsNot Nothing) Then
                DirectCast(tabFiles.SelectedTab, TabFile).Copy(QTextAux.Settings.ForceTextCopyPaste)
            End If
        Catch ex As Exception
            Global.Medo.MessageBox.ShowWarning(Me, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message)
        End Try
    End Sub

    Private Sub mnuEditPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditPaste.Click
        Try
            If (tabFiles.SelectedTab IsNot Nothing) Then
                DirectCast(tabFiles.SelectedTab, TabFile).Paste(QTextAux.Settings.ForceTextCopyPaste)
            End If
        Catch ex As Exception
            Global.Medo.MessageBox.ShowWarning(Me, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message)
        End Try
    End Sub

    Private Sub mnuEditDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditDelete.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            If (txt.SelectedText.Length > 0) Then
                txt.SelectedText = ""
            ElseIf (txt.SelectionStart < txt.Text.Length) Then
                txt.SelectionLength = 1
                txt.SelectedText = ""
            End If
        End If
    End Sub

    Private Sub mnuEditSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditSelectAll.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            txt.SelectAll()
        End If
    End Sub


    Private Sub mnuView_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuView.DropDownOpening
        mnuViewAlwaysOnTop.Checked = Settings.DisplayAlwaysOnTop
        mnuViewMenu.Checked = Settings.ShowMenu
        mnuViewToolbar.Checked = Settings.DisplayShowToolbar
    End Sub

    Private Sub mnuViewAlwaysOnTop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewAlwaysOnTop.Click
        Settings.DisplayAlwaysOnTop = mnuViewAlwaysOnTop.Checked
        Me.TopMost = mnuViewAlwaysOnTop.Checked
    End Sub

    Private Sub mnuViewMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewMenu.Click
        Settings.ShowMenu = mnuViewMenu.Checked
        mnu.Visible = Settings.ShowMenu
        Call frmMain_Resize(Nothing, Nothing)
    End Sub

    Private Sub mnuViewToolbar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewToolbar.Click
        Settings.DisplayShowToolbar = mnuViewToolbar.Checked
        tls.Visible = Settings.DisplayShowToolbar
        Call frmMain_Resize(Nothing, Nothing)
    End Sub

    Private Sub mnuViewRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewRefresh.Click
        Dim unsaved As Boolean = False
        For i As Integer = 0 To tabFiles.TabPages.Count - 1
            Dim tf As TabFile = DirectCast(tabFiles.TabPages(i), TabFile)
            If tf.IsChanged Then
                unsaved = True
                Exit For
            End If
        Next

        If unsaved Then
            fswLocationTxt.EnableRaisingEvents = False
            Select Case Global.Medo.MessageBox.ShowQuestion(Me, "Content of unsaved files will be lost if not saved. Do you with to save it now?", MessageBoxButtons.YesNo)
                Case Windows.Forms.DialogResult.Yes
                    For i As Integer = 0 To tabFiles.TabPages.Count - 1
                        Dim tf As TabFile = DirectCast(tabFiles.TabPages(i), TabFile)
                        If tf.IsChanged Then
                            tf.Save()
                        End If
                    Next
            End Select
            fswLocationTxt.EnableRaisingEvents = True
        End If

        If (sender IsNot Nothing) Then SaveFileOrder(True)

        Dim selectedTitle As String = "*"
        If (tabFiles.SelectedTab IsNot Nothing) Then selectedTitle = DirectCast(tabFiles.SelectedTab, TabFile).Title

        tabFiles.Visible = False
        tabFiles.TabPages.Clear()
        Try 'if files cannot be found
            Dim fo As New FileOrder()
            If (Settings.StartupRememberSelectedFile) AndAlso ((String.IsNullOrEmpty(selectedTitle)) OrElse (selectedTitle = "*")) Then selectedTitle = fo.SelectedTitle
            Dim fs As String() = fo.GetFiles
            For i As Integer = 0 To fs.Length - 1
                Dim t As New TabFile(fs(i), App.Form.mnxTextBox)
                tabFiles.TabPages.Add(t)
            Next

            tabFiles.Visible = True
        Catch ex As Exception
            Global.Medo.MessageBox.ShowWarning(Me, "Files could not be loaded." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK)
        End Try

        If (tabFiles.TabPages.Count > 0) Then
            For i As Integer = 0 To tabFiles.TabPages.Count - 1
                If (String.Compare(tabFiles.TabPages(i).Text, selectedTitle) = 0) Then
                    selectedTitle = ""
                    tabFiles.SelectedTab = tabFiles.TabPages(i)

                End If
            Next
            If (Not String.IsNullOrEmpty(selectedTitle)) Then tabFiles.SelectedTab = tabFiles.TabPages(0)
            Call tabFiles_SelectedIndexChanged(Nothing, Nothing)
        End If

        Me.TopMost = Settings.DisplayAlwaysOnTop
    End Sub


    Private Sub mnuFormat_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormat.DropDownOpening
        mnuFormatFont.Visible = False
        mnuFormatBold.Visible = False
        mnuFormatItalic.Visible = False
        mnuFormatUnderline.Visible = False
        mnuFormatStrikeout.Visible = False
        mnuFormatRtfSeparator.Visible = False

        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)

            If tf.IsRichTextFormat Then
                mnuFormatFont.Visible = True
                mnuFormatBold.Visible = True
                mnuFormatItalic.Visible = True
                mnuFormatUnderline.Visible = True
                mnuFormatStrikeout.Visible = True
                mnuFormatRtfSeparator.Visible = True

                mnuFormatBold.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Bold)
                mnuFormatItalic.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Italic)
                mnuFormatUnderline.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Underline)
                mnuFormatStrikeout.Checked = (tf.TextBox.SelectionFont IsNot Nothing) AndAlso (tf.TextBox.SelectionFont.Strikeout)
            End If

            Dim txt As TextBoxBase = tf.TextBox

            mnuFormatSortAscending.Enabled = (txt.SelectedText.Length > 0)
            mnuFormatSortDescending.Enabled = (txt.SelectedText.Length > 0)
            mnuFormatConvertToLower.Enabled = (txt.SelectedText.Length > 0)
            mnuFormatConvertToUpper.Enabled = (txt.SelectedText.Length > 0)
            mnuFormatConvertToTitleCase.Enabled = (txt.SelectedText.Length > 0)
            mnuFormatConvertToTitleCaseDrGrammar.Enabled = (txt.SelectedText.Length > 0)
        Else
            mnuFormatSortAscending.Enabled = False
            mnuFormatSortDescending.Enabled = False
            mnuFormatConvertToLower.Enabled = False
            mnuFormatConvertToUpper.Enabled = False
            mnuFormatConvertToTitleCase.Enabled = False
            mnuFormatConvertToTitleCaseDrGrammar.Enabled = False
        End If
    End Sub

    Private Sub mnuFormatFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatFont.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            If (tf.IsRichTextFormat) Then
                Using f As New System.Windows.Forms.FontDialog
                    f.AllowScriptChange = True
                    f.AllowSimulations = True
                    f.AllowVectorFonts = True
                    If (tf.TextBox.SelectionFont IsNot Nothing) Then
                        f.Color = tf.TextBox.SelectionColor
                        f.Font = tf.TextBox.SelectionFont
                    Else
                        Dim selLength = tf.TextBox.SelectionLength
                        tf.TextBox.SelectionLength = 1
                        f.Color = tf.TextBox.SelectionColor
                        f.Font = tf.TextBox.SelectionFont
                        tf.TextBox.SelectionLength = selLength
                    End If
                    f.ShowColor = True
                    f.ShowEffects = True
                    If (f.ShowDialog(Me) = Windows.Forms.DialogResult.OK) Then
                        tf.TextBox.SelectionColor = f.Color
                        tf.TextBox.SelectionFont = f.Font
                    End If
                End Using
            End If
        End If
    End Sub

    Private Sub mnuFormatBold_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatBold.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            If (tf.IsRichTextFormat) Then
                Helper.ToogleStyle(tf.TextBox, FontStyle.Bold)
            End If
        End If
    End Sub

    Private Sub mnuFormatItalic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatItalic.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            If (tf.IsRichTextFormat) Then
                Helper.ToogleStyle(tf.TextBox, FontStyle.Italic)
            End If
        End If
    End Sub

    Private Sub mnuFormatUnderline_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatUnderline.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            If (tf.IsRichTextFormat) Then
                If (tf.TextBox.SelectionFont IsNot Nothing) Then
                    Helper.ToogleStyle(tf.TextBox, FontStyle.Underline)
                End If
            End If
        End If
    End Sub

    Private Sub mnuFormatStrikeout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatStrikeout.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As TabFile = DirectCast(tabFiles.SelectedTab, TabFile)
            If (tf.IsRichTextFormat) Then
                If (tf.TextBox.SelectionFont IsNot Nothing) Then
                    Helper.ToogleStyle(tf.TextBox, FontStyle.Strikeout)
                End If
            End If
        End If
    End Sub

    Private Sub mnuFormatSortAscending_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatSortAscending.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            Dim ss As Integer = txt.SelectionStart
            Dim sl As Integer = txt.SelectionLength

            Dim selSplit As String() = txt.SelectedText.Split(New String() {New String(New Char() {System.Convert.ToChar(13), System.Convert.ToChar(10)}), System.Convert.ToChar(13), System.Convert.ToChar(10)}, StringSplitOptions.None)
            Array.Sort(selSplit, New SortAZ)
            txt.SelectedText = String.Join(Environment.NewLine, selSplit)

            txt.SelectionStart = ss
            txt.SelectionLength = sl
        End If
    End Sub

    Private Sub mnuFormatSortDescending_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatSortDescending.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            Dim ss As Integer = txt.SelectionStart
            Dim sl As Integer = txt.SelectionLength

            Dim selSplit As String() = txt.SelectedText.Split(New String() {New String(New Char() {System.Convert.ToChar(13), System.Convert.ToChar(10)}), System.Convert.ToChar(13), System.Convert.ToChar(10)}, StringSplitOptions.None)
            Array.Sort(selSplit, New SortZA)
            txt.SelectedText = String.Join(Environment.NewLine, selSplit)

            txt.SelectionStart = ss
            txt.SelectionLength = sl
        End If
    End Sub

    Private Sub mnuFormatConvertToLower_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatConvertToLower.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            Dim ss As Integer = txt.SelectionStart
            Dim sl As Integer = txt.SelectionLength

            txt.SelectedText = txt.SelectedText.ToLower

            txt.SelectionStart = ss
            txt.SelectionLength = sl
        End If
    End Sub

    Private Sub mnuFormatConvertToUpper_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatConvertToUpper.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            Dim ss As Integer = txt.SelectionStart
            Dim sl As Integer = txt.SelectionLength

            txt.SelectedText = txt.SelectedText.ToUpper

            txt.SelectionStart = ss
            txt.SelectionLength = sl
        End If
    End Sub

    Private Sub mnuFormatConvertToTitleCase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatConvertToTitleCase.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            Dim ss As Integer = txt.SelectionStart
            Dim sl As Integer = txt.SelectionLength

            txt.SelectedText = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToLower(txt.SelectedText))

            txt.SelectionStart = ss
            txt.SelectionLength = sl
        End If
    End Sub

    Private Sub mnuFormatConvertToTitleCaseDrGrammar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFormatConvertToTitleCaseDrGrammar.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            Dim ss As Integer = txt.SelectionStart
            Dim sl As Integer = txt.SelectionLength

            Dim sb As New System.Text.StringBuilder(txt.SelectedText.ToLower())
            Dim startOfWord As Integer = -1
            For i As Integer = 0 To sb.Length
                If (i < sb.Length) AndAlso (Char.IsLetter(sb(i)) OrElse (sb(i) = "'"c)) Then
                    If (startOfWord = -1) Then startOfWord = i
                Else
                    If (startOfWord > -1) Then
                        Dim word As String = txt.SelectedText.Substring(startOfWord, i - startOfWord).ToLower
                        Select Case word
                            Case "a", "an", "the", "in", "of", "to", "and", "but"
                            Case Else
                                sb(startOfWord) = Char.ToUpper(sb(startOfWord))
                        End Select
                        startOfWord = -1
                    End If
                End If
            Next

            txt.SelectedText = sb.ToString

            txt.SelectionStart = ss
            txt.SelectionLength = sl
        End If
    End Sub


    Private Sub mnuToolsOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsOptions.Click
        Using frm As New OptionsForm
            fswLocationTxt.EnableRaisingEvents = False
            tmrQuickAutoSave.Enabled = False
            SaveAllChanged()
            SaveFileOrder(True)
            Me.tmrUpdateToolbar.Enabled = False
            Call mnuViewRefresh_Click(Nothing, Nothing)
            If (frm.ShowDialog(Me) = Windows.Forms.DialogResult.OK) Then
                If (Settings.StartupShow = False) Then
                    App.Tray.Show()
                Else
                    App.Tray.Hide()
                End If
                mnu.Visible = Settings.ShowMenu
                tabFiles.Multiline = Settings.DisplayMultilineTabHeader
                If Settings.DisplayMinimizeMaximizeButtons Then
                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
                Else
                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.SizableToolWindow
                End If
                If (QTextAux.Settings.ZoomToolbarWithDpiChange) Then
                    tls.ImageScalingSize = New Size(CInt(16 * _dpiRatioX), CInt(16 * _dpiRatioY))
                    tls.Scale(New SizeF(_dpiRatioX, _dpiRatioY))
                Else
                    Dim rx As Single = CSng(16 / tls.ImageScalingSize.Width)
                    Dim ry As Single = CSng(16 / tls.ImageScalingSize.Height)
                    tls.ImageScalingSize = New Size(16, 16)
                    tls.Scale(New SizeF(rx, ry))
                End If
                Me.ShowInTaskbar = Settings.DisplayShowInTaskbar
                Me.TopMost = Settings.DisplayAlwaysOnTop
                Call mnuViewRefresh_Click(Nothing, Nothing)
                Call frmMain_Resize(Nothing, Nothing)
                Me.tmrUpdateToolbar.Enabled = Settings.DisplayShowToolbar
                tmrQuickAutoSave.Interval = Settings.QuickAutoSaveSeconds * 1000
                fswLocationTxt.Path = QTextAux.Settings.FilesLocation
                fswLocationTxt.EnableRaisingEvents = True

                If (QTextAux.Settings.CarbonCopyUse) Then
                    For i As Integer = 0 To tabFiles.TabPages.Count - 1
                        Dim currTab As TabFile = DirectCast(tabFiles.TabPages(i), TabFile)
                        currTab.SaveCarbonCopy()
                    Next
                End If

                'TODO: Apply backup
            End If
        End Using
    End Sub


    Private Sub mnuHelpAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuHelpAbout.Click
        Medo.Windows.Forms.AboutBox.ShowDialog(Me, New Uri("http://www.jmedved.com/?page=qtext"), Global.Medo.Reflection.EntryAssembly.Product + " (beta 3)")

        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, TabFile).TextBox
            txt.Focus()
        End If
    End Sub

    Private Sub tmrQuickAutoSave_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrQuickAutoSave.Tick
        tmrQuickAutoSave.Enabled = False
        Debug.WriteLine("QText: QuickAutoSave")
        fswLocationTxt.EnableRaisingEvents = False
        For i As Integer = 0 To tabFiles.TabPages.Count - 1
            Try
                Dim tf As TabFile = DirectCast(tabFiles.TabPages(i), TabFile)
                If tf.IsChanged Then
                    tf.Save()
                    Trace.WriteLine("QText: QuickAutoSave: Saved " + tf.Title + ".")
                    tmrQuickAutoSave.Enabled = True
                    Exit For
                End If
            Catch ex As Exception
            End Try
        Next
        fswLocationTxt.EnableRaisingEvents = True
    End Sub


    Private Class Helper

        Friend Shared Sub ToogleStyle(ByVal richTextBox As QTextAux.RichTextBoxEx, ByVal fontStyle As FontStyle)
            If (richTextBox.SelectionFont IsNot Nothing) Then
                richTextBox.SelectionFont = New Font(richTextBox.SelectionFont, richTextBox.SelectionFont.Style Xor fontStyle)
            Else
                richTextBox.BeginUpdate()
                Dim selStart As Integer = richTextBox.SelectionStart
                Dim selLength As Integer = richTextBox.SelectionLength
                Dim toRegular As Boolean
                For i As Integer = selStart To selStart + selLength - 1
                    richTextBox.SelectionStart = i
                    richTextBox.SelectionLength = 1
                    Dim isStyleOn As Boolean = (richTextBox.SelectionFont.Style And fontStyle) = fontStyle
                    If (i = selStart) Then
                        toRegular = isStyleOn
                    End If
                    If ((isStyleOn = True) AndAlso (toRegular = True)) OrElse ((isStyleOn = False) AndAlso (toRegular = False)) Then
                        richTextBox.SelectionFont = New Font(richTextBox.SelectionFont, richTextBox.SelectionFont.Style Xor fontStyle)
                    End If
                Next
                richTextBox.SelectionStart = selStart
                richTextBox.SelectionLength = selLength
                richTextBox.EndUpdate()
            End If
        End Sub

    End Class

    Private Sub fswLocation_Changed(ByVal sender As System.Object, ByVal e As System.IO.FileSystemEventArgs) Handles fswLocationTxt.Renamed, fswLocationTxt.Deleted, fswLocationTxt.Created, fswLocationTxt.Changed
        'mnuViewRefresh_Click(Nothing, Nothing)
    End Sub

    Private Sub MainForm_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        'frmMain_Resize(Nothing, Nothing)
        Try
            fswLocationTxt.Path = QTextAux.Settings.FilesLocation
            fswLocationTxt.NotifyFilter = System.IO.NotifyFilters.FileName Or System.IO.NotifyFilters.LastWrite
            fswLocationTxt.EnableRaisingEvents = True
        Catch
        End Try
    End Sub

    Private Sub mnuHelpReportABug_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuHelpReportABug.Click
        Medo.Diagnostics.ErrorReport.TopMost = Me.TopMost
        Global.Medo.Diagnostics.ErrorReport.ShowDialog(Me, Nothing, New Uri("http://jmedved.com/ErrorReport/"))
    End Sub

    Private Sub mnuEditFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditFind.Click
        If (_findForm Is Nothing) OrElse (_findForm.IsDisposed) Then
            _findForm = New FindForm(Me.tabFiles)
            _findForm.Left = Me.Left + (Me.Width - _findForm.Width) \ 2
            _findForm.Top = Me.Top + (Me.Height - _findForm.Height) \ 2
        End If
        If (Not _findForm.Visible) Then
            _findForm.Show(Me)
        End If
        _findForm.Activate()
    End Sub

    Private Sub mnuEditFindNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditFindNext.Click
        If (_findForm Is Nothing) OrElse (_findForm.IsDisposed) OrElse (Not _findForm.Visible) Then
            If (_tabFiles.SelectedTab IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(QTextAux.SearchStatus.Text)) Then
                Dim tf As TabFile = DirectCast(_tabFiles.SelectedTab, TabFile)
                If (tf.Find(QTextAux.SearchStatus.Text, QTextAux.SearchStatus.CaseSensitive) = False) Then
                    Global.Medo.MessageBox.ShowInformation(Me, "Text """ + QTextAux.SearchStatus.Text + """ cannot be found.")
                End If
            End If
        Else
            _findForm.FindNext()
        End If
    End Sub

    Private Sub tlbHelpAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tlbHelpAbout.Click
        Call mnuHelpAbout_Click(Nothing, Nothing)
    End Sub

    Private Sub tlbHelpReportABug_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tlbHelpReportABug.Click
        mnuHelpReportABug_Click(Nothing, Nothing)
    End Sub

    Private Sub mnxTextBoxCopyAsText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxCopyAsText.Click
        Try
            If (tabFiles.SelectedTab IsNot Nothing) Then
                DirectCast(tabFiles.SelectedTab, TabFile).Copy(True)
            End If
        Catch ex As Exception
            Global.Medo.MessageBox.ShowWarning(Me, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message)
        End Try
    End Sub

    Private Sub mnxTextBoxPasteAsText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxPasteAsText.Click
        Try
            If (tabFiles.SelectedTab IsNot Nothing) Then
                DirectCast(tabFiles.SelectedTab, TabFile).Paste(True)
            End If
        Catch ex As Exception
            Global.Medo.MessageBox.ShowWarning(Me, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message)
        End Try
    End Sub

    Private Sub mnxTabOpenContainingFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTabOpenContainingFolder.Click
        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim file As String = DirectCast(tabFiles.SelectedTab, TabFile).FullFileName
            Dim exe As New System.Diagnostics.ProcessStartInfo("explorer.exe", "/select,""" + file + """")
            System.Diagnostics.Process.Start(exe)
        End If
    End Sub

    Private Sub tls_btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tls_btnFind.Click
        mnuEditFind_Click(Nothing, Nothing)
    End Sub

    Private Sub MainForm_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
        If (_findForm Is Nothing) OrElse (_findForm.IsDisposed) OrElse (_findForm.Visible = False) Then
        Else
            _findForm.Close()
        End If
    End Sub

    Private Sub mnxTextBoxCutAsText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnxTextBoxCutAsText.Click
        Try
            If (tabFiles.SelectedTab IsNot Nothing) Then
                DirectCast(tabFiles.SelectedTab, TabFile).Cut(True)
            End If
        Catch ex As Exception
            Global.Medo.MessageBox.ShowWarning(Me, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message)
        End Try
    End Sub

    Private Sub mnuViewZoomIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewZoomIn.Click

    End Sub

    Private Sub mnuViewZoomOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewZoomOut.Click

    End Sub
End Class