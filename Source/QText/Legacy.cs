using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Win32;

namespace QText.Legacy {

    internal static class OrderedFiles {

        private static readonly Encoding Encoding = new UTF8Encoding(false);


        public static void Upgrade() {
            var orderedTitles = new List<string>();
            string selectedTitle = null;
            try {
                string fileName = Path.Combine(QText.Settings.FilesLocation, "QText.xml");
                if ((File.Exists(fileName))) {
                    using (var xr = new XmlTextReader(fileName)) {
                        var walk = new Stack<string>();

                        while (xr.Read()) {

                            if ((xr.NodeType == XmlNodeType.Element)) {
                                switch (xr.Name) {

                                    case "QText":
                                        if ((walk.Count > 0)) { throw new InvalidOperationException(); }
                                        if ((!xr.IsEmptyElement)) { walk.Push(xr.Name); }
                                        break;

                                    case "FileOrder":
                                        if ((walk.Peek() != "QText")) { throw new InvalidOperationException(); }
                                        if ((!xr.IsEmptyElement)) { walk.Push(xr.Name); }

                                        selectedTitle = xr.GetAttribute("selectedTitle");
                                        break;

                                    case "File":
                                        if ((walk.Peek() != "FileOrder")) { throw new InvalidOperationException(); }
                                        if ((!xr.IsEmptyElement)) { walk.Push(xr.Name); }

                                        string currTitle = xr.GetAttribute("title");
                                        if (string.IsNullOrEmpty(currTitle) == false) {
                                            orderedTitles.Add(currTitle);
                                        }
                                        break;
                                }

                            } else if ((xr.NodeType == System.Xml.XmlNodeType.EndElement)) {
                                switch (xr.Name) {

                                    case "QText":
                                    case "FileOrder":
                                    case "File":
                                        walk.Pop();
                                        break;
                                }
                            }

                        }
                    }

                }
                File.Delete(fileName);
            } catch (Exception) { }

            if (orderedTitles.Count > 0) {
                //write to new format
                try {
                    using (var fs = new FileStream(Path.Combine(QText.Settings.FilesLocation, ".qtext"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
                        try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                        fs.Position = fs.Length;
                        using (var sw = new StreamWriter(fs)) {
                            sw.WriteLine("/["); //always start block with /[
                            foreach (var title in orderedTitles) {
                                if (title == selectedTitle) { //selected file is written with //selected
                                    sw.WriteLine(title + "//selected");
                                } else {
                                    sw.WriteLine(title);
                                }
                            }
                            sw.WriteLine("]/"); //always end block with ]/
                        }
                    }
                } catch (IOException) { }
            }
        }

    }


    internal static class HiddenFiles {

        public static void Upgrade() {
            var filesAll = new List<string>();
            filesAll.AddRange(Directory.GetFiles(QText.Settings.FilesLocation, "*.txt"));
            filesAll.AddRange(Directory.GetFiles(QText.Settings.FilesLocation, "*.rtf"));

            var newPath = Path.Combine(QText.Settings.FilesLocation, "Hidden");
            Helper.CreatePath(newPath);

            var files = new List<string>();
            foreach (var fileNameA in filesAll) {
                var fi = new FileInfo(fileNameA);
                if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) {
                    try {
                        fi.Attributes = fi.Attributes ^ FileAttributes.Hidden;
                        fi.MoveTo(Path.Combine(newPath, fi.Name));
                    } catch (Exception ex) {
                        Medo.MessageBox.ShowWarning(null, string.Format("Hidden file {0} cannot be upgraded. It is still available in QText folder but it will not be available in tabs.\n\n{1}", fi.Name, ex.Message));
                    }
                }
            }
        }

    }


    internal static class Settings {
        public static void Upgrade() {
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
                            QText.Settings.FilesLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"jmedved\QText");
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
                    QText.Settings.LegacySettingsCopied = true;
                } catch (Exception) {
                }
            }
        }

    }

}
