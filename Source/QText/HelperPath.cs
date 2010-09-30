using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace QText {
    namespace Helper {

        public static class Path {

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

        }


        public static class LegacySettings {
            public static void CopyLegacySettingsIfNeeded() {
                if (QText.Settings.LegacySettingsCopied == false) {
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
                                rkSource.DeleteSubKeyTree("QText");
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
