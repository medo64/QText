using Medo.Security.Cryptography;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QText {
    public class DocumentFile {

        public DocumentFile(DocumentFolder folder, string fileName) {
            this.Folder = folder;

            if (fileName.EndsWith(Extensions.PlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Extension = Extensions.PlainText;
                this.Type = DocumentKind.PlainText;
                this.IsEncrypted = false;
            } else if (fileName.EndsWith(Extensions.RichText, StringComparison.OrdinalIgnoreCase)) {
                this.Extension = Extensions.RichText;
                this.Type = DocumentKind.RichText;
                this.IsEncrypted = false;
            } else if (fileName.EndsWith(Extensions.EncryptedPlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Extension = Extensions.EncryptedPlainText;
                this.Type = DocumentKind.PlainText;
                this.IsEncrypted = true;
            } else if (fileName.EndsWith(Extensions.EncryptedRichText, StringComparison.OrdinalIgnoreCase)) {
                this.Extension = Extensions.EncryptedRichText;
                this.Type = DocumentKind.RichText;
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
        /// Gets raw file information.
        /// </summary>
        public FileInfo Info {
            get { return new FileInfo(Path.Combine(this.Folder.Info.FullName, this.Name + this.Extension)); }
        }

        /// <summary>
        /// Gets name of file - for internal use.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets file extension.
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title {
            get { return Helper.DecodeTitle(this.Name); }
        }


        public DocumentKind Type { get; private set; }
        public bool IsPlainText { get { return this.Type == DocumentKind.PlainText; } }
        public bool IsRichText { get { return this.Type == DocumentKind.RichText; } }

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

        public DateTime LastWriteTimeUtc { get { return this.Info.LastWriteTimeUtc; } }


        #region Rename/Delete

        private void GatherRenameData(ref string newTitle, out string newName, out string newPath) {
            newTitle = newTitle.Trim();
            newName = Helper.EncodeTitle(newTitle);
            newPath = Path.Combine(this.Info.Directory.FullName, newName);
            switch (this.Type) {
                case DocumentKind.PlainText: newPath += this.IsEncrypted ? Extensions.EncryptedPlainText : Extensions.PlainText; break;
                case DocumentKind.RichText: newPath += this.IsEncrypted ? Extensions.EncryptedRichText : Extensions.RichText; break;
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
                Helper.MovePath(this.Info.FullName, newPath);
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }

            this.Name = newName;
        }

        public void Delete() {
            if (this.Folder.Document.DeleteToRecycleBin) {
                SHFile.Delete(this.Info.FullName);
            } else {
                this.Info.Delete();
            }
        }

        #endregion


        #region Kind

        public void ChangeKind(DocumentKind newType) {
            if (newType == this.Type) { return; }

            switch (newType) {
                case DocumentKind.PlainText:
                    this.Extension = this.IsEncrypted ? Extensions.EncryptedPlainText : Extensions.PlainText; break;
                case DocumentKind.RichText:
                    this.Extension = this.IsEncrypted ? Extensions.EncryptedRichText : Extensions.RichText; break;
                default: throw new ApplicationException("Unknown file kind.");
            }
            Helper.MovePath(this.Info.FullName, Path.Combine(this.Info.DirectoryName, this.Name + this.Extension));
            this.Type = newType;
        }


        public void Encrypt(string password) {
            if (this.IsEncrypted) { return; }

            var oldPath = this.Info.FullName;
            var newPath = Path.Combine(this.Info.Directory.FullName, this.Name);
            switch (this.Type) {
                case DocumentKind.PlainText: newPath += Extensions.EncryptedPlainText; break;
                case DocumentKind.RichText: newPath += Extensions.EncryptedRichText; break;
                default: throw new ApplicationException("Unknown file kind.");
            }

            using (var stream = new MemoryStream()) {
                this.Read(stream);
                Helper.MovePath(oldPath, newPath);
                this.Extension = (this.Type == DocumentKind.RichText) ? Extensions.EncryptedRichText : Extensions.EncryptedPlainText;
                this.IsEncrypted = true;
                this.Password = password;
                this.Write(stream);
            }
        }

        public void Decrypt() {
            if (!this.IsEncrypted) { return; }

            var oldPath = this.Info.FullName;
            var newPath = Path.Combine(this.Info.Directory.FullName, this.Name);
            switch (this.Type) {
                case DocumentKind.PlainText: newPath += Extensions.PlainText; break;
                case DocumentKind.RichText: newPath += Extensions.RichText; break;
                default: throw new ApplicationException("Unknown file kind.");
            }

            using (var stream = new MemoryStream()) {
                this.Read(stream);
                Helper.MovePath(oldPath, newPath);
                this.Extension = (this.Type == DocumentKind.RichText) ? Extensions.RichText : Extensions.PlainText;
                this.IsEncrypted = false;
                this.Password = null;
                this.Write(stream);
            }
        }

        #endregion


        #region Read/write


        public void Read(MemoryStream stream) {
            if (this.IsEncrypted && !this.HasPassword) { throw new ApplicationException("Missing password."); }

            using (var fileStream = new FileStream(this.Info.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
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
                using (var fileStream = new FileStream(this.Info.FullName, FileMode.Create, FileAccess.Write, FileShare.None)) {
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


        #region Extensions

        private static class Extensions {
            public static readonly string PlainText = ".txt";
            public static readonly string RichText = ".rtf";
            public static readonly string EncryptedPlainText = ".txt.aes256cbc";
            public static readonly string EncryptedRichText = ".rtf.aes256cbc";
        }

        #endregion


        #region Overrides

        public override bool Equals(object obj) {
            var other = obj as DocumentFile;
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
