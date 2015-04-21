using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace QText {
    public class DocumentFolder {

        public DocumentFolder(Document document, string name) {
            this.Document = document;

            this.Name = name;
        }


        internal readonly Document Document;

        /// <summary>
        /// Gets directory.
        /// </summary>
        public DirectoryInfo Info {
            get { return new DirectoryInfo(string.IsNullOrEmpty(this.Name) ? this.Document.RootDirectory.FullName : Path.Combine(this.Document.RootDirectory.FullName, this.Name)); }
        }

        /// <summary>
        /// Gets name of folder - for internal use.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title {
            get { return string.IsNullOrEmpty(this.Name) ? "(Default)" : Helper.DecodeTitle(this.Name); }
        }

        /// <summary>
        /// Gets if given folder is root.
        /// </summary>
        public bool IsRoot { get { return string.IsNullOrEmpty(this.Name); } }


        #region Operations

        public void Rename(string newTitle) {
            if (this.IsRoot) { throw new IOException("Cannot rename root folder."); }
            if (string.IsNullOrEmpty(newTitle)) { throw new IOException("Folder name cannot be empty."); }
            newTitle = newTitle.Trim();

            Debug.WriteLine("Rename: " + this.Name + " -> " + newTitle);

            try {
                using (var watcher = new Helper.FileSystemToggler(this.Document.Watcher)) {
                    var newName = Helper.EncodeTitle(newTitle);

                    var oldPath = this.Info.FullName;
                    var newPath = Path.Combine(this.Info.Parent.FullName, newName);

                    Helper.MovePath(oldPath, newPath);

                    this.Name = newName;
                    this.Document.SortFolders();
                }
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public bool IsEmpty { //TODO
            get {
                return (this.Info.GetFiles("*.txt").Length == 0) && (this.Info.GetFiles("*.rtf").Length == 0);
            }
        }

        public void Delete() {
            Debug.WriteLine("Delete: " + this.Name);
            try {
                using (var watcher = new Helper.FileSystemToggler(this.Document.Watcher)) {
                    if (this.Document.DeleteToRecycleBin) {
                        SHFile.DeleteDirectory(this.Info.FullName);
                    } else {
                        this.Info.Delete(true);
                    }
                    this.Document.ProcessFolderDelete(this.Info.FullName);
                }
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion


        #region Enumerate

        public IEnumerable<DocumentFile> GetFiles() {
            var files = new List<FileInfo>();
            foreach (var extension in Helper.GetExtensions()) {
                files.AddRange(this.Info.GetFiles("*" + extension));
            }

            string selectedTitle = null;
            var orderedTitles = ReadOrderedTitles(out selectedTitle);
            files.Sort(delegate(FileInfo file1, FileInfo file2) {
                var title1 = Helper.DecodeTitle(Helper.GetFileNameWithoutExtension(file1.Name));
                var title2 = Helper.DecodeTitle(Helper.GetFileNameWithoutExtension(file2.Name));
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
                yield return new DocumentFile(this, file.Name);
            }
        }

        public DocumentFile GetFileByName(string name) {
            foreach (var file in this.GetFiles()) {
                if (file.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    return file;
                }
            }
            return null;
        }

        public DocumentFile GetFileByTitle(string title) {
            foreach (var file in this.GetFiles()) {
                if (file.Title.Equals(title, StringComparison.OrdinalIgnoreCase)) {
                    return file;
                }
            }
            return null;
        }


        private IList<String> ReadOrderedTitles(out String selectedTitle) {
            selectedTitle = null;
            string currentSelectedTitle = null;
            IList<string> orderedTitles = null;
            IList<string> currentOrderedTitles = null;
            try {
                var orderFile = Path.Combine(this.Info.FullName, ".qtext");

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
                                    var title = Helper.DecodeTitle(parts[0]);
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

        #endregion


        #region Overrides

        public override bool Equals(object obj) {
            var other = obj as DocumentFolder;
            return (other != null) && (this.Info.FullName.Equals(other.Info.FullName, StringComparison.OrdinalIgnoreCase));
        }

        public override int GetHashCode() {
            return this.Info.FullName.GetHashCode();
        }

        public override string ToString() {
            return this.Title;
        }

        #endregion

    }
}
