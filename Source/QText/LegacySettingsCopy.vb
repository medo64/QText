Friend Class LegacySettingsCopy

    Public Shared Sub CopyIfNeeded()
        If (Not QTextAux.Settings.LegacySettingsCopied) Then
            Try
                Dim wasOldVersionInstalled As Boolean = False
                Dim hasOldVersionDataPath As Boolean = False
                Using rkSource As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\jmedved\QText", False), rkDestination As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Josip Medved\QText", True)
                    If (rkSource IsNot Nothing) And (rkDestination IsNot Nothing) Then
                        wasOldVersionInstalled = True
                        For Each iName As String In rkSource.GetValueNames()
                            Dim iValue As Object = rkSource.GetValue(iName)
                            If (iName = "DataPath") Then
                                hasOldVersionDataPath = True
                            End If
                            rkDestination.SetValue(iName, iValue)
                        Next
                    End If
                End Using
                Using rkSource As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\jmedved", True)
                    If (rkSource IsNot Nothing) Then
                        rkSource.DeleteSubKeyTree("QText")
                    End If
                End Using
                Using rkSource As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", True)
                    If (rkSource IsNot Nothing) Then
                        Try
                            rkSource.DeleteSubKey("jmedved")
                        Catch ex As InvalidOperationException
                        Catch ex As Exception
                        End Try
                    End If
                End Using
                If (wasOldVersionInstalled) And (Not hasOldVersionDataPath) Then 'should copy all files to new location
                    Dim oldDataPath As String = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "jmedved\QText")
                    Dim sourceFileNames As New List(Of String)
                    Try
                        sourceFileNames.AddRange(System.IO.Directory.GetFiles(oldDataPath, "*.txt"))
                        sourceFileNames.AddRange(System.IO.Directory.GetFiles(oldDataPath, "*.rtf"))
                        If (System.IO.File.Exists(System.IO.Path.Combine(oldDataPath, "QText.xml"))) Then
                            sourceFileNames.Add(System.IO.Path.Combine(oldDataPath, "QText.xml"))
                        End If
                        For Each iSourceFileName As String In sourceFileNames
                            Dim iSource As System.IO.FileInfo = New System.IO.FileInfo(iSourceFileName)
                            Dim iDestinationFileName As String = System.IO.Path.Combine(QTextAux.Settings.FilesLocation, iSource.Name)
                            System.IO.File.Copy(iSource.FullName, iDestinationFileName)
                        Next
                    Catch ex As Exception
                        QTextAux.Settings.FilesLocation = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "jmedved\QText")
                    End Try
                    Try
                        For Each iSourceFileName As String In sourceFileNames
                            System.IO.File.Delete(iSourceFileName)
                        Next
                        Try
                            System.IO.Directory.Delete(oldDataPath, False)
                        Catch ex As Exception 'if directory cannot be accessed or is not empty
                        End Try
                    Catch ex As Exception
                    End Try
                End If
                QTextAux.Settings.LegacySettingsCopied = True
            Catch ex As Exception
            End Try
        End If
    End Sub

End Class
