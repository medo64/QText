Imports System.Drawing


Friend Class FindForm

    Private _tabFiles As QTextAux.TabControlDnD

    Public Sub New(ByVal tabFiles As QTextAux.TabControlDnD)
        InitializeComponent()
        Me.Font = System.Drawing.SystemFonts.MessageBoxFont

        Me._tabFiles = tabFiles
    End Sub


    <System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags:=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)> _
    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Select Case keyData
            Case Keys.Control Or Keys.F
                txtText.SelectAll()
                txtText.Focus()
                Return True

            Case Keys.F3
                Me.FindNext()
                Return True

            Case Else
                Return MyBase.ProcessCmdKey(msg, keyData)
        End Select
    End Function

    Private Sub FindForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If (_tabFiles.SelectedTab IsNot Nothing) Then
            Dim tf As QTextAux.TabFile = DirectCast(_tabFiles.SelectedTab, QTextAux.TabFile)
            If (tf.TextBox.SelectedText.Length > 0) Then
                QTextAux.SearchStatus.Text = tf.TextBox.SelectedText
            End If
        End If
        txtText.Text = QTextAux.SearchStatus.Text
        chbCaseSensitive.Checked = QTextAux.SearchStatus.CaseSensitive
    End Sub


    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click
        If (txtText.Text.Length > 0) Then
            QTextAux.SearchStatus.Text = txtText.Text
            QTextAux.SearchStatus.CaseSensitive = chbCaseSensitive.Checked
            If (_tabFiles.SelectedTab IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(QTextAux.SearchStatus.Text)) Then
                Dim tf As QTextAux.TabFile = DirectCast(_tabFiles.SelectedTab, QTextAux.TabFile)
                If (tf.Find(QTextAux.SearchStatus.Text, QTextAux.SearchStatus.CaseSensitive)) Then 'found characters, move window if needed
                    Dim selRect As Rectangle = tf.GetSelectedRectangle()
                    Dim thisRect = Me.Bounds
                    If (thisRect.IntersectsWith(selRect)) Then
                        Dim screenRect As Rectangle = Screen.GetWorkingArea(selRect.Location)
                        Dim rightSpace As Integer = screenRect.Right - selRect.Right
                        Dim leftSpace As Integer = selRect.Left - screenRect.Left
                        Dim topSpace As Integer = selRect.Top - screenRect.Top
                        Dim bottomSpace As Integer = screenRect.Bottom - selRect.Bottom

                        If (bottomSpace >= thisRect.Height) Then
                            Me.Location = New Point(thisRect.Left, selRect.Bottom)
                        ElseIf (topSpace >= thisRect.Height) Then
                            Me.Location = New Point(thisRect.Left, selRect.Top - thisRect.Height)
                        ElseIf (rightSpace >= thisRect.Width) Then
                            Me.Location = New Point(selRect.Right, thisRect.Top)
                        ElseIf (leftSpace >= thisRect.Width) Then
                            Me.Location = New Point(selRect.Left - thisRect.Width, thisRect.Top)
                        End If
                    End If
                Else
                    Medo.MessageBox.ShowInformation(Me, "Text """ + QTextAux.SearchStatus.Text + """ cannot be found.")
                End If
            End If
        End If
    End Sub

    Private Sub txtText_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtText.TextChanged
        btnFind.Enabled = (txtText.Text.Length > 0)
    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Public Sub FindNext()
        Me.btnFind_Click(Nothing, Nothing)
    End Sub

End Class