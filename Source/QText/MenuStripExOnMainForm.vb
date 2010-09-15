Friend Class MenuStripExOnMainForm : Inherits System.Windows.Forms.MenuStrip

    Private Sub MenuStripEx_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        Debug.WriteLine("MenuStripEx.KeyUp: " + e.KeyData.ToString())
        Dim mainForm As MainForm = TryCast(Me.Parent, MainForm)
        If (mainForm Is Nothing) Then Return
        Dim tabFiles As QTextAux.TabControlDnD = mainForm.tabFiles

        Select Case e.KeyData
            Case Keys.Menu
                If (Me.Visible) Then
                    If (Not Settings.ShowMenu) Then
                        Me.Visible = False
                    End If
                    If (tabFiles.SelectedTab IsNot Nothing) Then
                        Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, QTextAux.TabFile).TextBox
                        txt.Focus()
                    End If
                End If

        End Select
    End Sub

    Private Sub MenuStripEx_MenuDeactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MenuDeactivate
        Debug.WriteLine("MenuStripEx.Deactivate")
        Dim mainForm As MainForm = TryCast(Me.Parent, MainForm)
        If (mainForm Is Nothing) Then Return
        Dim tabFiles As QTextAux.TabControlDnD = mainForm.tabFiles

        Me.Visible = Settings.ShowMenu

        If (tabFiles.SelectedTab IsNot Nothing) Then
            Dim txt As TextBoxBase = DirectCast(tabFiles.SelectedTab, QTextAux.TabFile).TextBox
            txt.Focus()
        End If
    End Sub

End Class
