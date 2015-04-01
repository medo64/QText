using System;
using System.IO;

namespace QText {
    internal class DocumentFolder {

        public DocumentFolder(DirectoryInfo directory, string name) {
            this.Directory = directory;
            this.Name = name;
            if (string.IsNullOrEmpty(name)) {
                this.Title = "(Default)";
            } else {
                this.Title = Helper.DecodeFileName(name);
            }
        }


        /// <summary>
        /// Gets directory.
        /// </summary>
        public DirectoryInfo Directory { get; private set; }

        /// <summary>
        /// Gets name of folder - for internal use.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets if given folder is root.
        /// </summary>
        public bool IsRoot { get { return string.IsNullOrEmpty(this.Name); } }


        #region Operations

        internal void Rename(string newTitle) {
            if (string.IsNullOrEmpty(this.Name)) { throw new IOException("Cannot rename root folder."); }
            if (string.IsNullOrEmpty(newTitle)) { throw new IOException("Folder name cannot be empty."); }
            newTitle = newTitle.Trim();

            try {
                var newName = Helper.EncodeFileName(newTitle);

                var oldPath = this.Directory.FullName;
                var newPath = Path.Combine(this.Directory.Parent.FullName, newName);

                Helper.MovePath(oldPath, newPath);

                this.Directory = new DirectoryInfo(newPath);
                this.Name = newName;
                this.Title = newTitle;
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        internal bool IsEmpty {
            get {
                return (this.Directory.GetFiles("*.txt").Length == 0) && (this.Directory.GetFiles("*.rtf").Length == 0);
            }
        }

        internal void Delete() {
            try {
                if (Settings.FilesDeleteToRecycleBin) {
                    SHFile.DeleteDirectory(this.Directory.FullName);
                } else {
                    this.Directory.Delete(true);
                }
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion


        #region Overrides

        public override bool Equals(object obj) {
            var other = obj as DocumentFolder;
            return (other != null) && (this.Directory.FullName.Equals(other.Directory.FullName, StringComparison.OrdinalIgnoreCase));
        }

        public override int GetHashCode() {
            return this.Directory.FullName.GetHashCode();
        }

        public override string ToString() {
            return this.Title;
        }

        #endregion

    }
}
