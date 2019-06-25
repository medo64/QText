using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace QText {
    public class DocumentFolder {

        public DocumentFolder(Document document, string name) {
            Document = document;
            Name = name;
        }


        internal readonly Document Document;


        /// <summary>
        /// Gets name of folder - for internal use.
        /// </summary>
        public string Name { get; internal set; }


        /// <summary>
        /// Gets full directory path.
        /// </summary>
        internal string FullPath {
            get { return string.IsNullOrEmpty(Name) ? Document.RootPath : Path.Combine(Document.RootPath, Name); }
        }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title {
            get { return string.IsNullOrEmpty(Name) ? "(Default)" : Helper.DecodeFolderTitle(Name); }
        }

        /// <summary>
        /// Gets if given folder is root.
        /// </summary>
        public bool IsRoot { get { return string.IsNullOrEmpty(Name); } }


        #region Operations

        public void Rename(string newTitle) {
            if (IsRoot) { throw new IOException("Cannot rename root folder."); }
            newTitle = newTitle.Trim();
            if (string.IsNullOrEmpty(newTitle)) { throw new IOException("Folder name cannot be empty."); }
            var newName = Helper.EncodeFolderTitle(newTitle);

            Debug.WriteLine("Folder.Rename: " + Name + " -> " + newName);
            try {
                using (var watcher = new Helper.FileSystemToggler(Document.Watcher)) {
                    var oldPath = FullPath;
                    var newPath = Path.Combine(Directory.GetParent(FullPath).FullName, newName);

                    Helper.MovePath(oldPath, newPath);

                    Name = newName;
                    Document.SortFolders();
                    Document.WriteOrder();
                }
                Document.OnFolderChanged(new DocumentFolderEventArgs(this));
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public bool IsEmpty {
            get {
                foreach (var file in GetFiles()) {
                    return false;
                }
                return true;
            }
        }

        public void Delete() {
            Debug.WriteLine("Folder.Delete: " + Name);
            try {
                using (var watcher = new Helper.FileSystemToggler(Document.Watcher)) {
                    if (Document.DeleteToRecycleBin) {
                        SHFile.DeleteDirectory(FullPath);
                    } else {
                        Directory.Delete(FullPath, true);
                    }
                    Document.ProcessDelete(FullPath);
                }
                Document.OnChanged(new EventArgs());
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }


        public void OpenInExplorer() {
            var exe = new ProcessStartInfo("explorer.exe", "\"" + FullPath + "\"");
            Process.Start(exe);
        }


        public void Sort() {
            Document.SortFiles(this);
            Document.WriteOrder();
        }

        #endregion


        #region Enumerate

        public IEnumerable<DocumentFile> GetFiles() {
            return Document.GetFiles(this);
        }

        public DocumentFile GetFileByName(string name) {
            foreach (var file in GetFiles()) {
                if (file.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    return file;
                }
            }
            return null;
        }

        public DocumentFile GetFileByTitle(string title) {
            foreach (var file in GetFiles()) {
                if (file.Title.Equals(title, StringComparison.OrdinalIgnoreCase)) {
                    return file;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets last selected file.
        /// </summary>
        public DocumentFile SelectedFile {
            get {
                DocumentFile firstFile = null;
                foreach (var file in GetFiles()) {
                    if (file.Selected) { return file; }
                    if (firstFile == null) { firstFile = file; }
                }
                return firstFile;
            }
        }

        #endregion


        #region New

        /// <summary>
        /// Returns if new file can be created.
        /// Only check for matching name is done.
        /// </summary>
        /// <param name="title">File title.</param>
        public bool CanNewFile(string title) {
            return Document.CanNewFile(this, title);
        }

        /// <summary>
        /// Returns newly created file.
        /// </summary>
        /// <param name="title">File title.</param>
        /// <param name="kind">File kind.</param>
        /// <returns></returns>
        public DocumentFile NewFile(string title, DocumentKind kind) {
            return Document.NewFile(this, title, kind);
        }

        #endregion


        #region Overrides

        public override bool Equals(object obj) {
            return (obj is DocumentFolder other) && (FullPath.Equals(other.FullPath, StringComparison.OrdinalIgnoreCase));
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public override string ToString() {
            return Title;
        }

        #endregion

    }
}
