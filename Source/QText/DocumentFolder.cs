using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace QText {
    internal static class DocumentFolder {

        public static IEnumerable<String> GetFolders() {
            yield return "";
            foreach (var folder in GetSubFolders()) {
                yield return folder;
            }
        }

        public static IEnumerable<String> GetTitles(String folder) {
            var files = new List<string>();
            foreach (var extension in QFileInfo.GetExtensions()) {
                files.AddRange(Directory.GetFiles(GetDirectory(folder), "*" + extension));
            }

            string selectedTitle = null;
            var orderedTitles = ReadOrderedTitles(folder, out selectedTitle);
            files.Sort(delegate(string file1, string file2) {
                var title1 = Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file1));
                var title2 = Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file2));
                if (orderedTitles != null) {
                    var titleIndex1 = orderedTitles.IndexOf(title1);
                    var titleIndex2 = orderedTitles.IndexOf(title2);
                    if ((titleIndex1 != -1) && (titleIndex2 != -1)) { //both are ordered
                        return (titleIndex1 < titleIndex2) ? -1 : 1;
                    } else if (titleIndex1 != -1) { //first one is ordered
                        return -1;
                    } else if (titleIndex2 != -1) { //second one is ordered 
                        return 1;
                    }
                }
                return string.Compare(title1, title2); //just sort alphabetically
            });

            foreach (var file in files) {
                yield return Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file));
            }
        }


        public static IEnumerable<String> GetFilePaths(String folder) {
            var files = new List<string>();
            foreach (var extension in QFileInfo.GetExtensions()) {
                files.AddRange(Directory.GetFiles(GetDirectory(folder), "*" + extension));
            }

            string selectedTitle = null;
            var orderedTitles = ReadOrderedTitles(folder, out selectedTitle);
            files.Sort(delegate(string file1, string file2) {
                var title1 = Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file1));
                var title2 = Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file2));
                if (orderedTitles != null) {
                    var titleIndex1 = orderedTitles.IndexOf(title1);
                    var titleIndex2 = orderedTitles.IndexOf(title2);
                    if ((titleIndex1 != -1) && (titleIndex2 != -1)) { //both are ordered
                        return (titleIndex1 < titleIndex2) ? -1 : 1;
                    } else if (titleIndex1 != -1) { //first one is ordered
                        return -1;
                    } else if (titleIndex2 != -1) { //second one is ordered 
                        return 1;
                    }
                }
                return string.Compare(title1, title2); //just sort alphabetically
            });

            foreach (var file in files) {
                yield return file;
            }
        }

        public static IEnumerable<TabFile> GetTabs(IEnumerable<String> filePaths, ContextMenuStrip contextMenuStrip) {
            foreach (var file in filePaths) {
                var tab = new TabFile(file);
                tab.ContextMenuStrip = contextMenuStrip;
                yield return tab;
            }
        }

        public static IEnumerable<String> GetSubFolders() {
            var folders = new List<string>();
            foreach (var directory in Directory.GetDirectories(Settings.FilesLocation)) {
                folders.Add(Helper.DecodeFileName(new DirectoryInfo(directory).Name));
            }
            folders.Sort();
            foreach (var folder in folders) {
                yield return folder;
            }
        }

        public static String GetDirectory(String folder) {
            if (string.IsNullOrEmpty(folder)) { return Settings.FilesLocation; }
            return Path.Combine(Settings.FilesLocation, Helper.EncodeFileName(folder));
        }


        #region Ordering

        public static IList<String> ReadOrderedTitles(String folder, out String selectedTitle) {
            selectedTitle = null;
            string currentSelectedTitle = null;
            IList<string> orderedTitles = null;
            IList<string> currentOrderedTitles = null;
            try {
                var orderFile = Path.Combine(GetDirectory(folder), ".qtext");

                using (var fs = new FileStream(orderFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    using (var sr = new StreamReader(fs)) {
                        while (sr.EndOfStream == false) {
                            var line = sr.ReadLine();
                            if (line.Equals("/[")) { //start of block
                                currentOrderedTitles = new List<string>();
                                currentSelectedTitle = null;
                            } else if (line.Equals("]/")) { //end of block
                                orderedTitles = currentOrderedTitles;
                                selectedTitle = currentSelectedTitle;
                            } else {
                                if (currentOrderedTitles != null) {
                                    var parts = line.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                                    var title = Helper.DecodeFileName(parts[0]);
                                    var attrs = parts.Length > 1 ? parts[1] : null;
                                    if ("selected".Equals(attrs)) { currentSelectedTitle = title; }
                                    currentOrderedTitles.Add(title);
                                }
                            }
                        }
                    }
                }
            } catch (IOException) { }
            return orderedTitles;
        }

        public static void WriteOrderedTitles(TabFiles tabFiles) {
            try {
                var orderFile = Path.Combine(GetDirectory(tabFiles.CurrentFolder), ".qtext");
                var fi = new QFileInfo(orderFile);
                if (fi.Exists == false) {
                    fi.Create();
                }
                fi.Attributes = FileAttributes.Hidden;
                using (var fs = new FileStream(orderFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
                    try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                    fs.Position = fs.Length;
                    using (var sw = new StreamWriter(fs)) {
                        sw.WriteLine("/["); //always start block with /[
                        foreach (TabFile tab in tabFiles.TabPages) {
                            if (tab.Equals(tabFiles.SelectedTab)) { //selected file is written with //selected
                                sw.WriteLine(tab.Title + "//selected");
                            } else {
                                sw.WriteLine(tab.Title);
                            }
                        }
                        sw.WriteLine("]/"); //always end block with ]/
                    }
                }
            } catch (IOException) { }
        }

        public static void CleanOrderedTitles(String folder) {
            try {
                var orderFile = Path.Combine(GetDirectory(folder), ".qtext");
                var fi = new QFileInfo(orderFile);
                if (fi.Exists == false) {
                    fi.Create();
                }
                fi.Attributes = FileAttributes.Hidden;
                using (var fs = new FileStream(orderFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
                    try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                    fs.Position = fs.Length;
                    using (var sw = new StreamWriter(fs)) {
                        sw.WriteLine("/["); //always start block with /[
                        sw.WriteLine("]/"); //always end block with ]/
                    }
                }
            } catch (IOException) { }
        }

        #endregion

    }
}
