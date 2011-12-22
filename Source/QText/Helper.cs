﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.ComponentModel;

namespace QText {
    namespace Helper {

        internal static class Path {

            public static void CreatePath(string path) {
                if ((!Directory.Exists(path))) {
                    string currPath = path;
                    var allPaths = new List<string>();
                    while (!(Directory.Exists(currPath))) {
                        allPaths.Add(currPath);
                        currPath = System.IO.Path.GetDirectoryName(currPath);
                        if (string.IsNullOrEmpty(currPath)) {
                            throw new IOException("Path \"" + path + "\" can not be created.");
                        }
                    }

                    try {
                        for (int i = allPaths.Count - 1; i >= 0; i += -1) {
                            System.IO.Directory.CreateDirectory(allPaths[i]);
                        }
                    } catch (Exception) {
                        throw new System.IO.IOException("Path \"" + path + "\" can not be created.");
                    }
                }
            }

            public static string GetTitle(string fileName) {
                return GetTitle(new FileInfo(fileName));
            }

            public static string GetTitle(FileInfo fileInfo) {
                return fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);
            }

        }


        internal static class LegacySettings {
            public static void CopyLegacySettingsIfNeeded() {
                if (QText.Settings.LegacySettingsCopied == false) {

                    //kill old process
                    var currProcess = Process.GetCurrentProcess();
                    var currProcessId = currProcess.Id;
                    var currProcessName = currProcess.ProcessName;
                    var currProcessFileName = currProcess.MainModule.FileName;
                    foreach (var iProcess in Process.GetProcesses()) {
                        Debug.WriteLine(iProcess.ProcessName);
                        try {
                            if (string.CompareOrdinal(iProcess.ProcessName, "QText") == 0) {
                                if (iProcess.Id != currProcessId) {
                                    iProcess.Kill();
                                }
                            }
                        } catch (Win32Exception) { }
                    }

                    //copy registry
                    try {
                        var wasOldVersionInstalled = false;
                        var hasOldVersionDataPath = false;
                        using (var rkSource = Registry.CurrentUser.OpenSubKey(@"Software\jmedved\QText", false))
                        using (var rkDestination = Registry.CurrentUser.OpenSubKey(@"Software\Josip Medved\QText", true)) {
                            if ((rkSource != null) && (rkDestination != null)) {
                                wasOldVersionInstalled = true;
                                foreach (var iName in rkSource.GetValueNames()) {
                                    var iValue = rkSource.GetValue(iName);
                                    if (iName == "DataPath") {
                                        hasOldVersionDataPath = true;
                                    }
                                    rkDestination.SetValue(iName, iValue);
                                }
                            }
                        }
                        using (var rkSource = Registry.CurrentUser.OpenSubKey(@"Software\jmedved", true)) {
                            if (rkSource != null) {
                                try {
                                    rkSource.DeleteSubKeyTree("QText");
                                } catch (ArgumentException) { }
                            }
                        }
                        using (var rkSource = Registry.CurrentUser.OpenSubKey("Software", true)) {
                            if (rkSource != null) {
                                try {
                                    rkSource.DeleteSubKey("jmedved");
                                } catch (InvalidOperationException) {
                                } catch (Exception) {
                                }
                            }
                        }
                        if ((wasOldVersionInstalled) && (!hasOldVersionDataPath)) { //should copy all files to new location
                            var oldDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"jmedved\QText");
                            var sourceFileNames = new List<string>();
                            try {
                                sourceFileNames.AddRange(System.IO.Directory.GetFiles(oldDataPath, "*.txt"));
                                sourceFileNames.AddRange(System.IO.Directory.GetFiles(oldDataPath, "*.rtf"));
                                if (System.IO.File.Exists(System.IO.Path.Combine(oldDataPath, "QText.xml"))) {
                                    sourceFileNames.Add(System.IO.Path.Combine(oldDataPath, "QText.xml"));
                                }
                                foreach (var iSourceFileName in sourceFileNames) {
                                    var iSource = new System.IO.FileInfo(iSourceFileName);
                                    var iDestinationFileName = System.IO.Path.Combine(QText.Settings.FilesLocation, iSource.Name);
                                    System.IO.File.Copy(iSource.FullName, iDestinationFileName);
                                }
                            } catch (Exception) {
                                Settings.FilesLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"jmedved\QText");
                            }
                            try {
                                foreach (var iSourceFileName in sourceFileNames) {
                                    System.IO.File.Delete(iSourceFileName);
                                }
                                try {
                                    System.IO.Directory.Delete(oldDataPath, false);
                                } catch (Exception) { //if directory cannot be accessed or is not empty
                                }
                            } catch (Exception) {
                            }
                        }
                        Settings.LegacySettingsCopied = true;
                    } catch (Exception) {
                    }
                }
            }

        }

    }
}
