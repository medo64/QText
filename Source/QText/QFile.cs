using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QText {
    internal class QFile {

        public QFile(string fullPath) {
            this.Path = fullPath;

            var fileName = System.IO.Path.GetFileName(this.Path);
            if (fileName.EndsWith(Extension.PlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Name = fileName.Substring(0, fileName.LastIndexOf(Extension.PlainText, StringComparison.OrdinalIgnoreCase));
                this.Type = QType.PlainText;
                this.IsEncrypted = false;
            } else if (fileName.EndsWith(Extension.RichText, StringComparison.OrdinalIgnoreCase)) {
                this.Name = fileName.Substring(0, fileName.LastIndexOf(Extension.RichText, StringComparison.OrdinalIgnoreCase));
                this.Type = QType.RichText;
                this.IsEncrypted = false;
            } else if (fileName.EndsWith(Extension.EncryptedPlainText, StringComparison.OrdinalIgnoreCase)) {
                this.Name = fileName.Substring(0, fileName.LastIndexOf(Extension.EncryptedPlainText, StringComparison.OrdinalIgnoreCase));
                this.Type = QType.PlainText;
                this.IsEncrypted = true;
            } else if (fileName.EndsWith(Extension.EncryptedRichText, StringComparison.OrdinalIgnoreCase)) {
                this.Name = fileName.Substring(0, fileName.LastIndexOf(Extension.EncryptedRichText, StringComparison.OrdinalIgnoreCase));
                this.Type = QType.RichText;
                this.IsEncrypted = true;
            } else {
                throw new FormatException("Cannot recognize file type.");
            }
        }

        public string Name { get; private set; }
        public string Title { get { return Helper.DecodeFileName(this.Name); } }
        public string Path { get; private set; }

        public QType Type { get; private set; }
        public bool IsPlainText { get { return this.Type == QType.PlainText; } }
        public bool IsRichText { get { return this.Type == QType.RichText; } }

        public bool IsEncrypted { get; private set; }

        public DateTime LastWriteTimeUtc { get { return File.GetLastWriteTimeUtc(this.Path); } }


        public void ChangeTitle(string newTitle) {
            var oldPath = this.Path;
            var newName = Helper.EncodeFileName(newTitle);
            var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Path), newName);
            switch (this.Type) {
                case QType.PlainText: newPath += this.IsEncrypted ? Extension.EncryptedPlainText : Extension.PlainText; break;
                case QType.RichText: newPath += this.IsEncrypted ? Extension.EncryptedRichText : Extension.RichText; break;
                default: throw new InvalidOperationException("Unknown type.");
            }
            Helper.MovePath(oldPath, newPath);
            this.Path = newPath;
            this.Name = newName;
        }


        public void ChangeType(QType newType) {
            if (newType == this.Type) { return; }
            var oldPath = this.Path;
            var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Path), this.Name);
            switch (newType) {
                case QType.PlainText: newPath += this.IsEncrypted ? Extension.EncryptedPlainText : Extension.PlainText; break;
                case QType.RichText: newPath += this.IsEncrypted ? Extension.EncryptedRichText : Extension.RichText; break;
                default: throw new InvalidOperationException("Unknown type.");
            }
            Helper.MovePath(oldPath, newPath);
            this.Path = newPath;
            this.Type = newType;
        }


        public void Encrypt() {
            if (this.IsEncrypted) { return; }
            var oldPath = this.Path;
            var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Path), this.Name);
            switch (this.Type) {
                case QType.PlainText: newPath += Extension.EncryptedPlainText; break;
                case QType.RichText: newPath += Extension.EncryptedRichText; break;
                default: throw new InvalidOperationException("Unknown type.");
            }
            Helper.MovePath(oldPath, newPath);
            this.Path = newPath;
            this.IsEncrypted = true;
        }

        public void Decrypt() {
            if (!this.IsEncrypted) { return; }
            var oldPath = this.Path;
            var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Path), this.Name);
            switch (this.Type) {
                case QType.PlainText: newPath += Extension.PlainText; break;
                case QType.RichText: newPath += Extension.RichText; break;
                default: throw new InvalidOperationException("Unknown type.");
            }
            Helper.MovePath(oldPath, newPath);
            this.Path = newPath;
            this.IsEncrypted = false;
        }


        private static class Extension {
            public static readonly string PlainText = ".txt";
            public static readonly string RichText = ".rtf";
            public static readonly string EncryptedPlainText = ".txt.aes256cbc";
            public static readonly string EncryptedRichText = ".rtf.aes256cbc";
        }

    }
}
