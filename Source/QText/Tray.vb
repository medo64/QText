Imports System.Drawing

Friend Class Tray

    Private Shared mnxNotifyShow As New MenuItem("&Show")
    Private Shared mnxNotifyShowOnPrimary As New MenuItem("&Show on primary screen")
    Private Shared mnxNotifyExit As New MenuItem("E&xit")
    Private Shared notMain As New System.Windows.Forms.NotifyIcon
    Private Shared ReadOnly SyncRoot As New Object

	<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification:="This program is not intended to be localized.")> _
	Shared Sub New()
        App._form.CreateControl()
        App._form.Handle.GetType()

        mnxNotifyShow.DefaultItem = True
        AddHandler mnxNotifyShow.Click, AddressOf mnxNotifyShow_Click
        AddHandler mnxNotifyShowOnPrimary.Click, AddressOf mnxNotifyShowOnPrimary_Click
        AddHandler mnxNotifyExit.Click, AddressOf mnxNotifyExit_Click

        notMain.ContextMenu = New ContextMenu(New MenuItem() {mnxNotifyShow, mnxNotifyShowOnPrimary, New MenuItem("-"), mnxNotifyExit})
        notMain.Icon = Global.Medo.Resources.ManifestResources.GetIcon("QText.App.ico", 16, 16)
        notMain.Text = Global.Medo.Reflection.EntryAssembly.Title
        AddHandler notMain.MouseClick, AddressOf notMain_MouseClick
        AddHandler notMain.MouseDoubleClick, AddressOf notMain_MouseDoubleClick
    End Sub


    Public Shared Sub Init()
    End Sub

    Public Shared Sub Show()
        notMain.Visible = True
    End Sub

    Public Shared Sub Hide()
        notMain.Visible = False
    End Sub

    Private Shared Sub mnxNotifyShow_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        SyncLock Tray.SyncRoot
            App._form.Show()
            App._form.Activate()
        End SyncLock
    End Sub

    Private Shared Sub mnxNotifyShowOnPrimary_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        SyncLock Tray.SyncRoot
            App._form.Show()

            Dim priBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
            Dim currBounds As Rectangle = App._form.Bounds
            Dim normalBounds As Rectangle
            If (App._form.WindowState = FormWindowState.Normal) Then
                normalBounds = App._form.Bounds
            Else
                normalBounds = App._form.RestoreBounds
            End If

            If (currBounds.Left >= priBounds.Left) AndAlso (currBounds.Right <= priBounds.Right) AndAlso (currBounds.Top >= priBounds.Top) AndAlso (currBounds.Bottom <= priBounds.Bottom) Then
            Else
                Dim oldState As FormWindowState = App._form.WindowState

                If oldState <> FormWindowState.Normal Then
                    App._form.WindowState = FormWindowState.Normal
                End If

                If (normalBounds.Width > priBounds.Width) Then
                    App._form.Width = priBounds.Width
                End If
                If (normalBounds.Left < priBounds.Left) Then
                    App._form.Left = priBounds.Left
                End If
                If (normalBounds.Right > priBounds.Right) Then
                    App._form.Left = priBounds.Right - normalBounds.Width
                End If

                If (normalBounds.Height > priBounds.Height) Then
                    App._form.Height = priBounds.Height
                End If
                If (normalBounds.Top < priBounds.Top) Then
                    App._form.Top = priBounds.Top
                End If
                If (normalBounds.Bottom > priBounds.Bottom) Then
                    App._form.Top = priBounds.Bottom - normalBounds.Height
                End If

                App._form.WindowState = oldState
            End If

            App._form.Activate()
        End SyncLock
    End Sub

	Private Shared Sub mnxNotifyExit_Click(ByVal sender As Object, ByVal e As System.EventArgs)
		Tray.Hide()
		Application.Exit()
	End Sub

    Private Shared Sub notMain_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If (e.Button = MouseButtons.Left) AndAlso (Settings.TrayOneClickActivation) Then
            Call mnxNotifyShow_Click(sender, New System.EventArgs)
        End If
    End Sub

	Private Shared Sub notMain_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If (e.Button = MouseButtons.Left) AndAlso (Not Settings.TrayOneClickActivation) Then
            Call mnxNotifyShow_Click(sender, New System.EventArgs)
        End If
	End Sub

End Class
