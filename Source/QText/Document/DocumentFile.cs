using Medo.Security.Cryptography;
using System;
using System.IO;
using System.Security.Cryptography;

namespace QText {
    internal class DocumentFile {

        public DocumentFile(DocumentFolder folder, FileInfo file) {
            this.Folder = folder;
            this.Info = file;

            if (file.Name.EndsWith(Extension.PlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Name = file.Name.Substring(0, file.Name.LastIndexOf(Extension.PlainText, StringComparison.OrdinalIgnoreCase));
                this.Type = DocumentKind.PlainText;
                this.IsEncrypted = false;
            } else if (file.Name.EndsWith(Extension.RichText, StringComparison.OrdinalIgnoreCase)) {
                this.Name = file.Name.Substring(0, file.Name.LastIndexOf(Extension.RichText, StringComparison.OrdinalIgnoreCase));
                this.Type = DocumentKind.RichText;
                this.IsEncrypted = false;
            } else if (file.Name.EndsWith(Extension.EncryptedPlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Name = file.Name.Substring(0, file.Name.LastIndexOf(Extension.EncryptedPlainText, StringComparison.OrdinalIgnoreCase));
                this.Type = DocumentKind.PlainText;
                this.IsEncrypted = true;
            } else if (file.Name.EndsWith(Extension.EncryptedRichText, StringComparison.OrdinalIgnoreCase)) {
                this.Name = file.Name.Substring(0, file.Name.LastIndexOf(Extension.EncryptedRichText, StringComparison.OrdinalIgnoreCase));
                this.Type = DocumentKind.RichText;
                this.IsEncrypted = true;
            } else {
                throw new FormatException("Cannot recognize file type.");
            }

            this.Title = Helper.DecodeFileName(this.Name);
        }


        /// <summary>
        /// Gets folder containing this file.
        /// </summary>
        public DocumentFolder Folder { get; private set; }

        /// <summary>
        /// Gets raw file information.
        /// </summary>
        public FileInfo Info { get; private set; }

        /// <summary>
        /// Gets name of folder - for internal use.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title { get; private set; }


        public DocumentKind Type { get; private set; }
        public bool IsPlainText { get { return this.Type == DocumentKind.PlainText; } }
        public bool IsRichText { get { return this.Type == DocumentKind.RichText; } }

        public bool IsEncrypted { get; private set; }

        public DateTime LastWriteTimeUtc { get { return this.Info.LastWriteTimeUtc; } }


        #region Rename/Delete

        private void GatherRenameData(ref string newTitle, out string newName, out string newPath) {
            newTitle = newTitle.Trim();
            newName = Helper.EncodeFileName(newTitle);
            newPath = Path.Combine(this.Info.Directory.FullName, newName);
            switch (this.Type) {
                case DocumentKind.PlainText: newPath += this.IsEncrypted ? Extension.EncryptedPlainText : Extension.PlainText; break;
                case DocumentKind.RichText: newPath += this.IsEncrypted ? Extension.EncryptedRichText : Extension.RichText; break;
                default: throw new InvalidOperationException("Unknown type.");
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

            this.Info = new FileInfo(newPath);
            this.Name = newName;
            this.Title = newTitle;
        }

        internal void Delete() {
            if (Settings.FilesDeleteToRecycleBin) {
                SHFile.Delete(this.Info.FullName);
            } else {
                this.Info.Delete();
            }
        }

        #endregion


        #region Kind

        public void ChangeKind(DocumentKind newType) {
            if (newType == this.Type) { return; }
            var oldPath = this.Info.FullName;
            var newPath = Path.Combine(this.Info.Directory.FullName, this.Name);
            switch (newType) {
                case DocumentKind.PlainText: newPath += this.IsEncrypted ? Extension.EncryptedPlainText : Extension.PlainText; break;
                case DocumentKind.RichText: newPath += this.IsEncrypted ? Extension.EncryptedRichText : Extension.RichText; break;
                default: throw new InvalidOperationException("Unknown type.");
            }
            Helper.MovePath(oldPath, newPath);

            this.Info = new FileInfo(newPath);
            this.Type = newType;
        }


        public void Encrypt() {
            if (this.IsEncrypted) { return; }
            var oldPath = this.Info.FullName;
            var newPath = Path.Combine(this.Info.Directory.FullName, this.Name);
            switch (this.Type) {
                case DocumentKind.PlainText: newPath += Extension.EncryptedPlainText; break;
                case DocumentKind.RichText: newPath += Extension.EncryptedRichText; break;
                default: throw new InvalidOperationException("Unknown type.");
            }
            Helper.MovePath(oldPath, newPath);

            this.Info = new FileInfo(newPath);
            this.IsEncrypted = true;
        }

        public void Decrypt() {
            if (!this.IsEncrypted) { return; }
            var oldPath = this.Info.FullName;
            var newPath = Path.Combine(this.Info.Directory.FullName, this.Name);
            switch (this.Type) {
                case DocumentKind.PlainText: newPath += Extension.PlainText; break;
                case DocumentKind.RichText: newPath += Extension.RichText; break;
                default: throw new InvalidOperationException("Unknown type.");
            }
            Helper.MovePath(oldPath, newPath);

            this.Info = new FileInfo(newPath);
            this.IsEncrypted = false;
        }

        #endregion


        #region Read/write


        public void Read(MemoryStream stream, string password = null) {
            if (this.IsEncrypted && (password == null)) { throw new ApplicationException("Missing password."); }

            using (var fileStream = new FileStream(this.Info.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var buffer = new byte[65536];
                int len;

                if (this.IsEncrypted) {
                    using (var aesStream = new OpenSslAesStream(fileStream, password, CryptoStreamMode.Read, 256, CipherMode.CBC)) {
                        while ((len = aesStream.Read(buffer, 0, buffer.Length)) > 0) {
                            stream.Write(buffer, 0, len);
                        }
                        stream.Position = 0;
                    }
                } else {
                    while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0) {
                        stream.Write(buffer, 0, len);
                    }
                    stream.Position = 0;
                }
            }
        }

        public void Write(MemoryStream stream, string password = null) {
            stream.Position = 0;
            if (this.IsEncrypted && (password == null)) { throw new ApplicationException("Missing password."); }

            using (var fileStream = new FileStream(this.Info.FullName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                var buffer = new byte[65536];
                int len;

                if (this.IsEncrypted) {
                    using (var aesStream = new OpenSslAesStream(fileStream, password, CryptoStreamMode.Write, 256, CipherMode.CBC)) {
                        while ((len = stream.Read(buffer, 0, buffer.Length)) > 0) {
                            aesStream.Write(buffer, 0, len);
                        }
                    }
                } else {
                    while ((len = stream.Read(buffer, 0, buffer.Length)) > 0) {
                        fileStream.Write(buffer, 0, len);
                    }
                }
            }
        }

        #endregion


        #region Extensions

        private static class Extension {
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
