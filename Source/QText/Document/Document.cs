using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal static class Document {

        #region Enumerate

        public static DocumentFolder GetRootFolder() {
            var path = new DirectoryInfo(Settings.FilesLocation);
            return new DocumentFolder(path, string.Empty);
        }

        public static IEnumerable<DocumentFolder> GetFolders() {
            yield return Document.GetRootFolder();
            foreach (var folder in Document.GetSubFolders()) {
                yield return folder;
            }
        }

        public static IEnumerable<DocumentFolder> GetSubFolders() {
            var root = Document.GetRootFolder().Directory;

            var directories = new List<DirectoryInfo>();
            foreach (var directory in root.GetDirectories()) {
                directories.Add(directory);
            }
            directories.Sort(delegate(DirectoryInfo item1, DirectoryInfo item2) {
                return string.Compare(item1.Name, item2.Name);
            });
            foreach (var directory in directories) {
                yield return new DocumentFolder(directory, directory.Name);
            }
        }


        public static DocumentFolder GetFolder(string name) {
            foreach (var folder in Document.GetFolders()) {
                if (folder.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    return folder;
                }
            }
            return null;
        }

        #endregion


        #region New folder

        public static DocumentFolder NewFolder() {
            try {
                var newTitle = "New folder";
                var newPath = Path.Combine(Settings.FilesLocation, Helper.EncodeFileName(newTitle));
                if (Directory.Exists(newPath)) {
                    int n = 1;
                    while (true) {
                        newTitle = string.Format(CultureInfo.CurrentUICulture, "New folder ({0})", n);
                        newPath = Path.Combine(Settings.FilesLocation, Helper.EncodeFileName(newTitle));
                        if (!Directory.Exists(newPath)) { break; } //this folder is available
                        n += 1;
                    }
                }

                Directory.CreateDirectory(newPath);

                return new DocumentFolder(new DirectoryInfo(newPath), newTitle);
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion


        public static IEnumerable<String> GetTitles(DocumentFolder folder) {
            var files = new List<string>();
            foreach (var extension in QFileInfo.GetExtensions()) {
                files.AddRange(Directory.GetFiles(folder.Directory.FullName, "*" + extension));
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


        public static IEnumerable<String> GetFilePaths(DocumentFolder folder) {
            var files = new List<string>();
            foreach (var extension in QFileInfo.GetExtensions()) {
                files.AddRange(Directory.GetFiles(folder.Directory.FullName, "*" + extension));
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
                var tab = new TabFile(new QFile(file));
                tab.ContextMenuStrip = contextMenuStrip;
                yield return tab;
            }
        }


        #region Ordering

        public static IList<String> ReadOrderedTitles(DocumentFolder folder, out String selectedTitle) {
            selectedTitle = null;
            string currentSelectedTitle = null;
            IList<string> orderedTitles = null;
            IList<string> currentOrderedTitles = null;
            try {
                var orderFile = Path.Combine(folder.Directory.FullName, ".qtext");

                FileStream fs = null;
                try {
                    fs = new FileStream(orderFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using (var sr = new StreamReader(fs)) {
                        fs = null;
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
                } finally {
                    if (fs != null) { fs.Dispose(); }
                }
            } catch (IOException) { }
            return orderedTitles;
        }

        public static void WriteOrderedTitles(TabFiles tabFiles) {
            try {
                var orderFile = Path.Combine(tabFiles.CurrentFolder.Directory.FullName, ".qtext");
                var fi = new QFileInfo(orderFile);
                if (fi.Exists == false) {
                    fi.Create();
                }
                fi.Attributes = FileAttributes.Hidden;

                FileStream fs = null;
                try {
                    fs = new FileStream(orderFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                    fs.Position = fs.Length;
                    using (var sw = new StreamWriter(fs)) {
                        fs = null;
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
                } finally {
                    if (fs != null) { fs.Dispose(); }
                }
            } catch (IOException) { }
        }

        public static void CleanOrderedTitles(DocumentFolder folder) {
            try {
                var orderFile = Path.Combine(folder.Directory.FullName, ".qtext");
                var fi = new QFileInfo(orderFile);
                if (fi.Exists == false) {
                    fi.Create();
                }
                fi.Attributes = FileAttributes.Hidden;

                FileStream fs = null;
                try {
                    fs = new FileStream(orderFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                    fs.Position = fs.Length;
                    using (var sw = new StreamWriter(fs)) {
                        fs = null;
                        sw.WriteLine("/["); //always start block with /[
                        sw.WriteLine("]/"); //always end block with ]/
                    }
                } finally {
                    if (fs != null) { fs.Dispose(); }
                }
            } catch (IOException) { }
        }

        #endregion

    }
}
