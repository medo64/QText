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
        /// Gets name of folder - for internal use.
        /// </summary>
        public string Name { get; internal set; }


        /// <summary>
        /// Gets full directory path.
        /// </summary>
        public string FullPath {
            get { return string.IsNullOrEmpty(this.Name) ? this.Document.RootDirectory.FullName : Path.Combine(this.Document.RootDirectory.FullName, this.Name); }
        }

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

                    var oldPath = this.FullPath;
                    var newPath = Path.Combine(Directory.GetParent(this.FullPath).FullName, newName);

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
                return (Directory.GetFiles(this.FullPath, "*.txt").Length == 0) && (Directory.GetFiles(this.FullPath, "*.rtf").Length == 0);
            }
        }

        public void Delete() {
            Debug.WriteLine("Delete: " + this.Name);
            try {
                using (var watcher = new Helper.FileSystemToggler(this.Document.Watcher)) {
                    if (this.Document.DeleteToRecycleBin) {
                        SHFile.DeleteDirectory(this.FullPath);
                    } else {
                        Directory.Delete(this.FullPath, true);
                    }
                    this.Document.ProcessFolderDelete(this.FullPath);
                }
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion


        #region Enumerate

        public IEnumerable<DocumentFile> GetFiles() {
            return this.Document.GetFiles(this);
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

        #endregion


        #region New

        /// <summary>
        /// Returns if new file can be created.
        /// Only check for matching name is done.
        /// </summary>
        /// <param name="title">File title.</param>
        public bool CanNewFile(string title) {
            return this.Document.CanNewFile(this, title);
        }

        /// <summary>
        /// Returns newly created file.
        /// </summary>
        /// <param name="title">File title.</param>
        /// <param name="kind">File kind.</param>
        /// <returns></returns>
        public DocumentFile NewFile(string title, DocumentKind kind) {
            return this.Document.NewFile(this, title, kind);
        }

        #endregion


        #region Overrides

        public override bool Equals(object obj) {
            var other = obj as DocumentFolder;
            return (other != null) && (this.FullPath.Equals(other.FullPath, StringComparison.OrdinalIgnoreCase));
        }

        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        public override string ToString() {
            return this.Title;
        }

        #endregion

    }
}
