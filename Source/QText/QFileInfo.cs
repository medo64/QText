using System;
using System.Collections.Generic;
using System.IO;

namespace QText {

    internal class QFileInfo {

        public QFileInfo(string fileName) {
            this.FullName = Path.GetFullPath(fileName);
        }

        public string FullName { get; private set; }

        public string DirectoryName { get { return Path.GetDirectoryName(this.FullName); } }
        public string Name { get { return Path.GetFileName(this.FullName); } }
        public string Extension { get { return GetExtension(this.FullName); } }

        public string NameWithoutExtension { get { return GetFileNameWithoutExtension(this.FullName); } }

        public bool Exists { get { return File.Exists(this.FullName); } }

        public FileAttributes Attributes {
            get { return File.GetAttributes(this.FullName); }
            set { File.SetAttributes(this.FullName, value); }
        }
        public DateTime LastWriteTimeUtc { get { return File.GetLastWriteTimeUtc(this.FullName); } }


        public void Create() {
            File.Create(this.FullName);
        }

        public QFileInfo ChangeName(string newName) {
            return new QFileInfo(Path.Combine(this.DirectoryName, newName + this.Extension));
        }

        public bool IsEncrypted { get { return this.FullName.EndsWith(Extensions.PlainEncrypted, StringComparison.OrdinalIgnoreCase) || this.FullName.EndsWith(Extensions.RichEncrypted, StringComparison.OrdinalIgnoreCase); } }
        public bool IsRich { get { return IsFileRich(this.FullName); } }

        public QFileInfo ChangeExtension(string newExtension) {
            if (!newExtension.StartsWith(".", StringComparison.OrdinalIgnoreCase)) { newExtension = "." + newExtension; }
            return new QFileInfo(Path.Combine(this.DirectoryName, this.NameWithoutExtension + newExtension));
        }

        public void Refresh() { }

        public static IEnumerable<string> GetExtensions() {
            yield return Extensions.Plain;
            yield return Extensions.Rich;
            yield return Extensions.PlainEncrypted;
            yield return Extensions.RichEncrypted;
        }

        private static string GetExtension(string path) {
            foreach (var extension in GetExtensions()) {
                if (path.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) {
                    return extension;
                }
            }
            throw new InvalidOperationException("Unexpected data type for file \"" + path + "\"");
        }

        public static bool IsNameAlreadyTaken(string basePath, string fileNameWithoutExtension) {
            bool alreadyTaken = false;
            foreach (var extension in GetExtensions()) {
                alreadyTaken |= File.Exists(Path.Combine(basePath, fileNameWithoutExtension + extension));
                alreadyTaken |= Directory.Exists(Path.Combine(basePath, fileNameWithoutExtension + extension));
            }
            return alreadyTaken;
        }

        public static bool IsFileRich(string path) {
            return path.EndsWith(Extensions.Rich, StringComparison.OrdinalIgnoreCase) || path.EndsWith(Extensions.RichEncrypted, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetFileNameWithoutExtension(string path) {
            var name = Path.GetFileName(path);
            foreach (var extension in GetExtensions()) {
                if (name.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) {
                    return name.Substring(0, name.Length - extension.Length);
                }
            }
            throw new InvalidOperationException("Unexpected data type for file \"" + path + "\"");
        }

        public static string GetPathWithoutExtension(string path) {
            foreach (var extension in GetExtensions()) {
                if (path.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) {
                    return path.Substring(0, path.Length - extension.Length);
                }
            }
            throw new InvalidOperationException("Unexpected data type for file \"" + path + "\"");
        }


        public override bool Equals(object obj) {
            var other = obj as QFileInfo;
            if (other != null) { return string.Equals(this.FullName, other.FullName, StringComparison.OrdinalIgnoreCase); }
            var otherFileInfo = obj as FileInfo;
            if (otherFileInfo != null) { return string.Equals(this.FullName, otherFileInfo.FullName, StringComparison.OrdinalIgnoreCase); }
            var otherString = obj as string;
            if (otherString != null) { return string.Equals(this.FullName, otherString, StringComparison.OrdinalIgnoreCase); }
            return false;
        }

        public override int GetHashCode() {
            return this.FullName.GetHashCode();
        }

        public override string ToString() {
            return this.FullName;
        }



        internal static class Extensions {

            public static readonly string Plain = ".txt";
            public static readonly string Rich = ".rtf";
            public static readonly string PlainEncrypted = ".txt.aes256cbc";
            public static readonly string RichEncrypted = ".rtf.aes256cbc";

        }

    }

}
