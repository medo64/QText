using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Medo.Security.Cryptography;

namespace QText {
    public class DocumentFile {

        public DocumentFile(DocumentFolder folder, string fileName) {
            Folder = folder;

            if (fileName.EndsWith(FileExtensions.PlainText, StringComparison.OrdinalIgnoreCase)) {
                Kind = DocumentKind.PlainText;
            } else if (fileName.EndsWith(FileExtensions.RichText, StringComparison.OrdinalIgnoreCase)) {
                Kind = DocumentKind.RichText;
            } else if (fileName.EndsWith(FileExtensions.EncryptedPlainText, StringComparison.OrdinalIgnoreCase)) {
                Kind = DocumentKind.EncryptedPlainText;
            } else if (fileName.EndsWith(FileExtensions.EncryptedRichText, StringComparison.OrdinalIgnoreCase)) {
                Kind = DocumentKind.EncryptedRichText;
            } else {
                throw new NotSupportedException("Cannot recognize file extension.");
            }
            Name = fileName.Substring(0, fileName.Length - Extension.Length);
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
        /// Gets a full name of a file - for internal use
        /// </summary>
        public string FullName {
            get { return Path.Combine(Folder.FullPath, Name); }
        }


        /// <summary>
        /// Returns true if file exists
        /// </summary>
        public bool Exists {
            get { return File.Exists(FullName); }
        }


        /// <summary>
        /// Gets file's kind.
        /// </summary>
        public DocumentKind Kind { get; internal set; }


        /// <summary>
        /// Gets kind of a document without special handling for encryption.
        /// </summary>
        public DocumentStyle Style {
            get {
                switch (Kind) {
                    case DocumentKind.RichText:
                    case DocumentKind.EncryptedRichText:
                        return DocumentStyle.RichText;
                    default:
                        return DocumentStyle.PlainText;
                }
            }
        }

        /// <summary>
        /// Gets if document is encrypted
        /// </summary>
        public bool IsEncrypted { get { return (Kind == DocumentKind.EncryptedPlainText) || (Kind == DocumentKind.EncryptedRichText); } }


        /// <summary>
        /// Gets full file path.
        /// </summary>
        internal string FullPath {
            get { return Path.Combine(Folder.FullPath, Name + Extension); }
        }

        /// <summary>
        /// Gets file extension.
        /// </summary>
        public string Extension {
            get {
                switch (Kind) {
                    case DocumentKind.PlainText:
                        return FileExtensions.PlainText;
                    case DocumentKind.RichText:
                        return FileExtensions.RichText;
                    case DocumentKind.EncryptedPlainText:
                        return FileExtensions.EncryptedPlainText;
                    case DocumentKind.EncryptedRichText:
                        return FileExtensions.EncryptedRichText;
                    default:
                        throw new InvalidOperationException("Cannot determine kind.");
                }
            }
        }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title {
            get { return Helper.DecodeTitle(Name); }
        }


        public bool IsPlainText { get { return Style == DocumentStyle.PlainText; } }
        public bool IsRichText { get { return Style == DocumentStyle.RichText; } }


        #region Encryption

        private byte[] PasswordBytes;

        public string Password {
            set {
                if (value != null) {
                    PasswordBytes = UTF8Encoding.UTF8.GetBytes(value);
                    ProtectPassword();
                } else {
                    PasswordBytes = null;
                }
            }
        }

        public bool NeedsPassword { get { return (IsEncrypted && (PasswordBytes == null)); } }
        public bool HasPassword { get { return (IsEncrypted && (PasswordBytes != null)); } }

        public void ProtectPassword() {
            if (PasswordBytes == null) { return; }

            var bytes = PasswordBytes;
            PasswordBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            Array.Clear(bytes, 0, bytes.Length);
        }

        public void UnprotectPassword() {
            if (PasswordBytes == null) { return; }

            PasswordBytes = ProtectedData.Unprotect(PasswordBytes, null, DataProtectionScope.CurrentUser);
        }

        #endregion

        public DateTime LastWriteTimeUtc { get { return File.GetLastAccessTimeUtc(FullPath); } }


        #region Rename/Delete

        private void GatherRenameData(ref string newTitle, out string newName, out string newPath) {
            newTitle = newTitle.Trim();
            newName = Helper.EncodeTitle(newTitle);
            newPath = Path.Combine(Folder.FullPath, newName);
            switch (Kind) {
                case DocumentKind.PlainText:
                    newPath += FileExtensions.PlainText;
                    break;
                case DocumentKind.RichText:
                    newPath += FileExtensions.RichText;
                    break;
                case DocumentKind.EncryptedPlainText:
                    newPath += FileExtensions.EncryptedPlainText;
                    break;
                case DocumentKind.EncryptedRichText:
                    newPath += FileExtensions.EncryptedRichText;
                    break;
                default:
                    throw new NotSupportedException("Unknown file kind.");
            }
        }

        public bool CanRename(string newTitle) {
            GatherRenameData(ref newTitle, out var newName, out var newPath);
            return !File.Exists(newPath);
        }

        public void Rename(string newTitle) {
            GatherRenameData(ref newTitle, out var newName, out var newPath);

            Debug.WriteLine("File.Rename: " + Name + " -> " + newName);
            try {
                using (var watcher = new Helper.FileSystemToggler(Folder.Document.Watcher)) {
                    Helper.MovePath(FullPath, newPath);
                }
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }

            Name = newName;
            Folder.Document.WriteOrder();

            Folder.Document.OnFileChanged(new DocumentFileEventArgs(this));
        }

        public bool CanMove(DocumentFolder newFolder) {
            foreach (var file in newFolder.GetFiles()) {
                if (file.Title.Equals(Title, StringComparison.OrdinalIgnoreCase)) { return false; }
            }
            return true;
        }

        public void Move(DocumentFolder newFolder) {
            try {
                var oldFolder = Folder;
                var oldPath = FullPath;
                var newPath = Path.Combine(newFolder.FullPath, Name + Extension);

                Debug.WriteLine("File.Move: " + Folder.Name + " -> " + newFolder.Name);
                using (var watcher = new Helper.FileSystemToggler(Folder.Document.Watcher)) {
                    Helper.MovePath(oldPath, newPath);
                    Folder = newFolder;
                }
                OrderBefore(null);

                Folder.Document.OnFolderChanged(new DocumentFolderEventArgs(oldFolder));
                Folder.Document.OnFolderChanged(new DocumentFolderEventArgs(Folder));
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }


        public void Delete() {
            Debug.WriteLine("File.Delete: " + Name);
            try {
                using (var watcher = new Helper.FileSystemToggler(Folder.Document.Watcher)) {
                    if (Folder.Document.DeleteToRecycleBin) {
                        SHFile.Delete(FullPath);
                    } else {
                        File.Delete(FullPath);
                    }
                }
                Folder.Document.ProcessDelete(FullPath);
                Folder.Document.OnFolderChanged(new DocumentFolderEventArgs(Folder));
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }


        public void OpenInExplorer() {
            var exe = new ProcessStartInfo("explorer.exe", "/select,\"" + FullPath + "\"");
            Process.Start(exe);
        }

        #endregion


        #region Kind

        public void ChangeStyle(DocumentStyle newStyle) {
            if (newStyle == Style) { return; }

            DocumentKind newKind;
            switch (newStyle) {
                case DocumentStyle.PlainText:
                    newKind = IsEncrypted ? DocumentKind.EncryptedPlainText : DocumentKind.PlainText;
                    break;
                case DocumentStyle.RichText:
                    newKind = IsEncrypted ? DocumentKind.EncryptedRichText : DocumentKind.RichText;
                    break;
                default:
                    throw new NotSupportedException("Unknown file style.");
            }

            Debug.WriteLine("File.ChangeStyle: " + Name + " (" + Style.ToString() + " -> " + newStyle.ToString() + ")");
            using (var watcher = new Helper.FileSystemToggler(Folder.Document.Watcher)) {
                Helper.MovePath(FullPath, Path.Combine(Folder.FullPath, Name + FileExtensions.GetFromKind(newKind)));
            }
            Kind = newKind;

            Folder.Document.OnFileChanged(new DocumentFileEventArgs(this));
        }


        public void Encrypt(string password) {
            if (IsEncrypted) { return; }

            Debug.WriteLine("File.Encrypt: " + Name);
            var oldPath = FullPath;
            using (var stream = new MemoryStream()) {
                Read(stream);
                Kind = (Style == DocumentStyle.RichText) ? DocumentKind.EncryptedRichText : DocumentKind.EncryptedPlainText;
                Password = password;
                using (var watcher = new Helper.FileSystemToggler(Folder.Document.Watcher)) {
                    Write(stream);
                    File.Delete(oldPath);
                }
            }

            Folder.Document.OnFileChanged(new DocumentFileEventArgs(this));
        }

        public void Decrypt() {
            if (!IsEncrypted) { return; }

            Debug.WriteLine("File.Decrypt: " + Name);
            var oldPath = FullPath;
            using (var stream = new MemoryStream()) {
                Read(stream);
                Kind = (Style == DocumentStyle.RichText) ? DocumentKind.RichText : DocumentKind.PlainText;
                Password = null;
                using (var watcher = new Helper.FileSystemToggler(Folder.Document.Watcher)) {
                    Write(stream);
                    File.Delete(oldPath);
                }
            }

            Folder.Document.OnFileChanged(new DocumentFileEventArgs(this));
        }

        #endregion


        #region Order

        public void OrderBefore(DocumentFile pivotFile) {
            Folder.Document.OrderBefore(this, pivotFile);
        }

        public void OrderAfter(DocumentFile pivotFile) {
            Folder.Document.OrderAfter(this, pivotFile);
        }

        #endregion


        #region Read/write

        public void Read(MemoryStream stream) {
            if (IsEncrypted && !HasPassword) { throw new InvalidOperationException("Missing password."); }

            using (var fileStream = new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var buffer = new byte[65536];
                int len;

                if (IsEncrypted) {
                    UnprotectPassword();
                    try {
                        using (var aesStream = new OpenSslAesStream(fileStream, PasswordBytes, CryptoStreamMode.Read, 256, CipherMode.CBC)) {
                            while ((len = aesStream.Read(buffer, 0, buffer.Length)) > 0) {
                                stream.Write(buffer, 0, len);
                            }
                            stream.Position = 0;
                        }
                    } catch (CryptographicException) {
                        PasswordBytes = null; //clear password
                        throw new InvalidOperationException("Decryption failed.");
                    } finally {
                        ProtectPassword();
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
            if (IsEncrypted && !HasPassword) { throw new InvalidOperationException("Missing password."); }

            using (var watcher = new Helper.FileSystemToggler(Folder.Document.Watcher)) {
                stream.Position = 0;
                using (var fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    var buffer = new byte[65536];
                    int len;

                    if (IsEncrypted) {
                        UnprotectPassword();
                        try {
                            using (var aesStream = new OpenSslAesStream(fileStream, PasswordBytes, CryptoStreamMode.Write, 256, CipherMode.CBC)) {
                                while ((len = stream.Read(buffer, 0, buffer.Length)) > 0) {
                                    aesStream.Write(buffer, 0, len);
                                }
                            }
                        } finally {
                            ProtectPassword();
                        }
                    } else {
                        while ((len = stream.Read(buffer, 0, buffer.Length)) > 0) {
                            fileStream.Write(buffer, 0, len);
                        }
                    }
                }
            }

            WriteCarbonCopy();
        }

        internal void WriteCarbonCopy() {
            if (Folder.Document.CarbonCopyRootPath != null) {
                try {
                    var ccPath = Folder.IsRoot ? Folder.Document.CarbonCopyRootPath : Path.Combine(Folder.Document.CarbonCopyRootPath, Folder.Name);
                    Helper.CreatePath(ccPath);

                    var ccFullPath = Path.Combine(ccPath, Name + Extension);
                    File.Copy(FullPath, ccFullPath, true);
                } catch (Exception ex) {
                    if (!Folder.Document.CarbonCopyIgnoreErrors) {
                        throw new InvalidOperationException("Error writing carbon copy for " + Title + ".\n\n" + ex.Message, ex);
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
            get { return _selected; }
            set {
                if (value) {
                    foreach (var file in Folder.GetFiles()) {
                        file._selected = false;
                    }
                }
                _selected = value;
                Folder.Document.WriteOrder();
            }
        }

        #endregion


        #region Overrides

        public override bool Equals(object obj) {
            return (obj is DocumentFile other) && (FullPath.Equals(other.FullPath, StringComparison.OrdinalIgnoreCase));
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
