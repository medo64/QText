using System;
using System.Collections.Generic;
using System.IO;

namespace QText {

    internal class QFileInfo {

        public QFileInfo(string fileName) {
            FullName = Path.GetFullPath(fileName);
        }

        public string FullName { get; private set; }

        public string DirectoryName { get { return Path.GetDirectoryName(FullName); } }
        public string Name { get { return Path.GetFileName(FullName); } }
        public string Extension { get { return GetExtension(FullName); } }

        public string NameWithoutExtension { get { return GetFileNameWithoutExtension(FullName); } }

        public bool Exists { get { return File.Exists(FullName); } }

        public FileAttributes Attributes {
            get { return File.GetAttributes(FullName); }
            set { File.SetAttributes(FullName, value); }
        }
        public DateTime LastWriteTimeUtc { get { return File.GetLastWriteTimeUtc(FullName); } }


        public void Create() {
            File.Create(FullName);
        }

        public QFileInfo ChangeName(string newName) {
            return new QFileInfo(Path.Combine(DirectoryName, newName + Extension));
        }

        public bool IsEncrypted { get { return FullName.EndsWith(Extensions.PlainEncrypted, StringComparison.OrdinalIgnoreCase) || FullName.EndsWith(Extensions.RichEncrypted, StringComparison.OrdinalIgnoreCase); } }
        public bool IsRich { get { return IsFileRich(FullName); } }

        public QFileInfo ChangeExtension(string newExtension) {
            if (!newExtension.StartsWith(".", StringComparison.OrdinalIgnoreCase)) { newExtension = "." + newExtension; }
            return new QFileInfo(Path.Combine(DirectoryName, NameWithoutExtension + newExtension));
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
            if (obj is QFileInfo other) { return string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase); }
            if (obj is FileInfo otherFileInfo) { return string.Equals(FullName, otherFileInfo.FullName, StringComparison.OrdinalIgnoreCase); }
            if (obj is string otherString) { return string.Equals(FullName, otherString, StringComparison.OrdinalIgnoreCase); }
            return false;
        }

        public override int GetHashCode() {
            return FullName.GetHashCode();
        }

        public override string ToString() {
            return FullName;
        }



        internal static class Extensions {

            public static readonly string Plain = ".txt";
            public static readonly string Rich = ".rtf";
            public static readonly string PlainEncrypted = ".txt.aes256cbc";
            public static readonly string RichEncrypted = ".rtf.aes256cbc";

        }

    }

}
