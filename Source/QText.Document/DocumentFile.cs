using Medo.Security.Cryptography;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QText {
    public class DocumentFile {

        public DocumentFile(DocumentFolder folder, string fileName) {
            this.Folder = folder;

            if (fileName.EndsWith(FileExtensions.PlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Kind = DocumentKind.PlainText;
            } else if (fileName.EndsWith(FileExtensions.RichText, StringComparison.OrdinalIgnoreCase)) {
                this.Kind = DocumentKind.RichText;
            } else if (fileName.EndsWith(FileExtensions.EncryptedPlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Kind = DocumentKind.EncryptedPlainText;
            } else if (fileName.EndsWith(FileExtensions.EncryptedRichText, StringComparison.OrdinalIgnoreCase)) {
                this.Kind = DocumentKind.EncryptedRichText;
            } else {
                throw new ApplicationException("Cannot recognize file extension.");
            }
            this.Name = fileName.Substring(0, fileName.Length - this.Extension.Length);
        }


        /// <summary>
        /// Gets folder containing this file.
        /// </summary>
        public DocumentFolder Folder { get; internal set; }

        /// <summary>
        /// Gets name of file - for internal use.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets file's kind.
        /// </summary>
        public DocumentKind Kind { get; internal set; }


        /// <summary>
        /// Gets kind of a document without special handling for encryption.
        /// </summary>
        public DocumentStyle Style {
            get {
                switch (this.Kind) {
                    case DocumentKind.RichText:
                    case DocumentKind.EncryptedRichText: return DocumentStyle.RichText;
                    default: return DocumentStyle.PlainText;
                }
            }
        }

        /// <summary>
        /// Gets if document is encrypted
        /// </summary>
        public bool IsEncrypted { get { return (this.Kind == DocumentKind.EncryptedPlainText) || (this.Kind == DocumentKind.EncryptedRichText); } }


        /// <summary>
        /// Gets full file path.
        /// </summary>
        public string FullPath {
            get { return Path.Combine(this.Folder.FullPath, this.Name + this.Extension); }
        }

        /// <summary>
        /// Gets file extension.
        /// </summary>
        public string Extension {
            get {
                switch (this.Kind) {
                    case DocumentKind.PlainText: return FileExtensions.PlainText;
                    case DocumentKind.RichText: return FileExtensions.RichText;
                    case DocumentKind.EncryptedPlainText: return FileExtensions.EncryptedPlainText;
                    case DocumentKind.EncryptedRichText: return FileExtensions.EncryptedRichText;
                    default: throw new InvalidOperationException("Cannot determine kind.");
                }
            }
        }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title {
            get { return Helper.DecodeTitle(this.Name); }
        }


        public bool IsPlainText { get { return this.Style == DocumentStyle.PlainText; } }
        public bool IsRichText { get { return this.Style == DocumentStyle.RichText; } }


        #region Encryption

        private byte[] PasswordBytes;

        public string Password {
            set {
                if (value != null) {
                    this.PasswordBytes = UTF8Encoding.UTF8.GetBytes(value);
                    this.ProtectPassword();
                } else {
                    this.PasswordBytes = null;
                }
            }
        }

        public bool NeedsPassword { get { return (this.IsEncrypted && (this.PasswordBytes == null)); } }
        public bool HasPassword { get { return (this.IsEncrypted && (this.PasswordBytes != null)); } }

        public void ProtectPassword() {
            if (this.PasswordBytes == null) { return; }

            var bytes = this.PasswordBytes;
            this.PasswordBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            Array.Clear(bytes, 0, bytes.Length);
        }

        public void UnprotectPassword() {
            if (this.PasswordBytes == null) { return; }

            this.PasswordBytes = ProtectedData.Unprotect(this.PasswordBytes, null, DataProtectionScope.CurrentUser);
        }

        #endregion

        public DateTime LastWriteTimeUtc { get { return File.GetLastAccessTimeUtc(this.FullPath); } }


        #region Rename/Delete

        private void GatherRenameData(ref string newTitle, out string newName, out string newPath) {
            newTitle = newTitle.Trim();
            newName = Helper.EncodeTitle(newTitle);
            newPath = Path.Combine(this.Folder.FullPath, newName);
            switch (this.Kind) {
                case DocumentKind.PlainText: newPath += FileExtensions.PlainText; break;
                case DocumentKind.RichText: newPath += FileExtensions.RichText; break;
                case DocumentKind.EncryptedPlainText: newPath += FileExtensions.EncryptedPlainText; break;
                case DocumentKind.EncryptedRichText: newPath += FileExtensions.EncryptedRichText; break;
                default: throw new ApplicationException("Unknown file kind.");
            }
        }

        public bool CanRename(string newTitle) {
            string newName, newPath;
            GatherRenameData(ref newTitle, out newName, out newPath);
            return !File.Exists(newPath);
        }

        public void Rename(string newTitle) {
            string newName, newPath;
            GatherRenameData(ref newTitle, out newName, out newPath);

            Debug.WriteLine("File.Rename: " + this.Name + " -> " + newName);
            try {
                using (var watcher = new Helper.FileSystemToggler(this.Folder.Document.Watcher)) {
                    Helper.MovePath(this.FullPath, newPath);
                }
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }

            this.Name = newName;
            this.Folder.Document.WriteOrder();
        }

        public bool CanMove(DocumentFolder newFolder) {
            foreach (var file in newFolder.GetFiles()) {
                if (file.Title.Equals(this.Title, StringComparison.OrdinalIgnoreCase)) { return false; }
            }
            return true;
        }

        public void Move(DocumentFolder newFolder) {
            try {
                var oldPath = this.FullPath;
                var newPath = Path.Combine(newFolder.FullPath, this.Name + this.Extension);

                Debug.WriteLine("File.Move: " + this.Folder.Name + " -> " + newFolder.Name);
                using (var watcher = new Helper.FileSystemToggler(this.Folder.Document.Watcher)) {
                    Helper.MovePath(oldPath, newPath);
                    this.Folder = newFolder;
                }
                this.OrderBefore(null);
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }


        public void Delete() {
            Debug.WriteLine("File.Delete: " + this.Name);
            try {
                using (var watcher = new Helper.FileSystemToggler(this.Folder.Document.Watcher)) {
                    if (this.Folder.Document.DeleteToRecycleBin) {
                        SHFile.Delete(this.FullPath);
                    } else {
                        File.Delete(this.FullPath);
                    }
                }
                this.Folder.Document.ProcessDelete(this.FullPath);
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion


        #region Kind

        public void ChangeStyle(DocumentStyle newStyle) {
            if (newStyle == this.Style) { return; }

            DocumentKind newKind;
            switch (newStyle) {
                case DocumentStyle.PlainText: newKind = this.IsEncrypted ? DocumentKind.EncryptedPlainText : DocumentKind.PlainText; break;
                case DocumentStyle.RichText: newKind = this.IsEncrypted ? DocumentKind.EncryptedRichText : DocumentKind.RichText; break;
                default: throw new ApplicationException("Unknown file style.");
            }

            Debug.WriteLine("File.ChangeStyle: " + this.Name + " (" + this.Style.ToString() + " -> " + newStyle.ToString() + ")");
            using (var watcher = new Helper.FileSystemToggler(this.Folder.Document.Watcher)) {
                Helper.MovePath(this.FullPath, Path.Combine(this.Folder.FullPath, this.Name + FileExtensions.GetFromKind(newKind)));
            }
            this.Kind = newKind;
        }


        public void Encrypt(string password) {
            if (this.IsEncrypted) { return; }

            Debug.WriteLine("File.Encrypt: " + this.Name);
            var oldPath = this.FullPath;
            using (var stream = new MemoryStream()) {
                this.Read(stream);
                this.Kind = (this.Style == DocumentStyle.RichText) ? DocumentKind.EncryptedRichText : DocumentKind.EncryptedPlainText;
                this.Password = password;
                using (var watcher = new Helper.FileSystemToggler(this.Folder.Document.Watcher)) {
                    this.Write(stream);
                    File.Delete(oldPath);
                }
            }
        }

        public void Decrypt() {
            if (!this.IsEncrypted) { return; }

            Debug.WriteLine("File.Decrypt: " + this.Name);
            var oldPath = this.FullPath;
            using (var stream = new MemoryStream()) {
                this.Read(stream);
                this.Kind = (this.Style == DocumentStyle.RichText) ? DocumentKind.RichText : DocumentKind.PlainText;
                this.Password = null;
                using (var watcher = new Helper.FileSystemToggler(this.Folder.Document.Watcher)) {
                    this.Write(stream);
                    File.Delete(oldPath);
                }
            }
        }

        #endregion


        #region Move

        public void OrderBefore(DocumentFile pivotFile) {
            this.Folder.Document.OrderBefore(this, pivotFile);
        }

        public void OrderAfter(DocumentFile pivotFile) {
            this.Folder.Document.OrderAfter(this, pivotFile);
        }

        #endregion


        #region Read/write

        public void Read(MemoryStream stream) {
            if (this.IsEncrypted && !this.HasPassword) { throw new ApplicationException("Missing password."); }

            using (var fileStream = new FileStream(this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var buffer = new byte[65536];
                int len;

                if (this.IsEncrypted) {
                    this.UnprotectPassword();
                    try {
                        using (var aesStream = new OpenSslAesStream(fileStream, this.PasswordBytes, CryptoStreamMode.Read, 256, CipherMode.CBC)) {
                            while ((len = aesStream.Read(buffer, 0, buffer.Length)) > 0) {
                                stream.Write(buffer, 0, len);
                            }
                            stream.Position = 0;
                        }
                    } finally {
                        this.ProtectPassword();
                    }
                } else {
                    while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0) {
                        stream.Write(buffer, 0, len);
                    }
                    stream.Position = 0;
                }
            }
        }

        public void Write(MemoryStream stream) {
            if (this.IsEncrypted && !this.HasPassword) { throw new ApplicationException("Missing password."); }


            using (var watcher = new Helper.FileSystemToggler(this.Folder.Document.Watcher)) {
                stream.Position = 0;
                using (var fileStream = new FileStream(this.FullPath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    var buffer = new byte[65536];
                    int len;

                    if (this.IsEncrypted) {
                        this.UnprotectPassword();
                        try {
                            using (var aesStream = new OpenSslAesStream(fileStream, this.PasswordBytes, CryptoStreamMode.Write, 256, CipherMode.CBC)) {
                                while ((len = stream.Read(buffer, 0, buffer.Length)) > 0) {
                                    aesStream.Write(buffer, 0, len);
                                }
                            }
                        } finally {
                            this.ProtectPassword();
                        }
                    } else {
                        while ((len = stream.Read(buffer, 0, buffer.Length)) > 0) {
                            fileStream.Write(buffer, 0, len);
                        }
                    }
                }
            }
        }

        #endregion


        #region Selection

        private bool _selected;
        /// <summary>
        /// Gets/sets if file is currently selected.
        /// </summary>
        public bool Selected {
            get { return this._selected; }
            set {
                if (value) {
                    foreach (var file in this.Folder.GetFiles()) {
                        file.Selected = false;
                    }
                }
                this._selected = value;
                this.Folder.Document.WriteOrder();
            }
        }

        #endregion


        #region Overrides

        public override bool Equals(object obj) {
            var other = obj as DocumentFile;
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
