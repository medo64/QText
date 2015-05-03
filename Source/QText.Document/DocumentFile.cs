using Medo.Security.Cryptography;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QText {
    public class DocumentFile {

        public DocumentFile(DocumentFolder folder, string fileName) {
            this.Folder = folder;

            if (fileName.EndsWith(FileExtensions.PlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Kind = DocumentKind.PlainText;
                this.IsEncrypted = false;
            } else if (fileName.EndsWith(FileExtensions.RichText, StringComparison.OrdinalIgnoreCase)) {
                this.Kind = DocumentKind.RichText;
                this.IsEncrypted = false;
            } else if (fileName.EndsWith(FileExtensions.EncryptedPlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Kind = DocumentKind.PlainText;
                this.IsEncrypted = true;
            } else if (fileName.EndsWith(FileExtensions.EncryptedRichText, StringComparison.OrdinalIgnoreCase)) {
                this.Kind = DocumentKind.RichText;
                this.IsEncrypted = true;
            } else {
                throw new ApplicationException("Cannot recognize file type.");
            }
            this.Name = fileName.Substring(0, fileName.Length - this.Extension.Length);
        }


        /// <summary>
        /// Gets folder containing this file.
        /// </summary>
        public DocumentFolder Folder { get; private set; }

        /// <summary>
        /// Gets name of file - for internal use.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets kind of a document.
        /// </summary>
        public DocumentKind Kind { get; private set; }


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
                    case DocumentKind.PlainText: return this.IsEncrypted ? FileExtensions.EncryptedPlainText : FileExtensions.PlainText;
                    case DocumentKind.RichText: return this.IsEncrypted ? FileExtensions.EncryptedRichText : FileExtensions.RichText;
                    default: throw new InvalidOperationException("Cannot determine extension.");
                }
            }
        }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title {
            get { return Helper.DecodeTitle(this.Name); }
        }


        public bool IsPlainText { get { return this.Kind == DocumentKind.PlainText; } }
        public bool IsRichText { get { return this.Kind == DocumentKind.RichText; } }

        #region Encryption

        public bool IsEncrypted { get; private set; }

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
                case DocumentKind.PlainText: newPath += this.IsEncrypted ? FileExtensions.EncryptedPlainText : FileExtensions.PlainText; break;
                case DocumentKind.RichText: newPath += this.IsEncrypted ? FileExtensions.EncryptedRichText : FileExtensions.RichText; break;
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

            try {
                Helper.MovePath(this.FullPath, newPath);
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }

            this.Name = newName;
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
                Helper.MovePath(oldPath, newPath);
                this.Folder = newFolder;
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }


        public void Delete() {
            try {
                if (this.Folder.Document.DeleteToRecycleBin) {
                    SHFile.Delete(this.FullPath);
                } else {
                    File.Delete(this.FullPath);
                }
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion


        #region Kind

        public void ChangeKind(DocumentKind newKind) {
            if (newKind == this.Kind) { return; }

            string newExtension;
            switch (newKind) {
                case DocumentKind.PlainText: newExtension = this.IsEncrypted ? FileExtensions.EncryptedPlainText : FileExtensions.PlainText; break;
                case DocumentKind.RichText: newExtension = this.IsEncrypted ? FileExtensions.EncryptedRichText : FileExtensions.RichText; break;
                default: throw new ApplicationException("Unknown file kind.");
            }

            Helper.MovePath(this.FullPath, Path.Combine(this.Folder.FullPath, this.Name + newExtension));
            this.Kind = newKind;
        }


        public void Encrypt(string password) {
            if (this.IsEncrypted) { return; }

            var oldPath = this.FullPath;
            using (var stream = new MemoryStream()) {
                this.Read(stream);
                this.IsEncrypted = true;
                this.Password = password;
                this.Write(stream);
                File.Delete(oldPath);
            }
        }

        public void Decrypt() {
            if (!this.IsEncrypted) { return; }

            var oldPath = this.FullPath;
            using (var stream = new MemoryStream()) {
                this.Read(stream);
                this.IsEncrypted = false;
                this.Password = null;
                this.Write(stream);
                File.Delete(oldPath);
            }
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
                    foreach (var file in this.Folder.Document.GetAllFiles()) {
                        file.Selected = false;
                    }
                }
                this._selected = value;
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
