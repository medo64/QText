using System;
using System.Collections.Generic;

namespace QText {
    internal class FileExtensions {

        public static IEnumerable<string> All {
            get {
                yield return FileExtensions.PlainText;
                yield return FileExtensions.RichText;
                yield return FileExtensions.EncryptedPlainText;
                yield return FileExtensions.EncryptedRichText;
            }
        }

        public static readonly string PlainText = ".txt";
        public static readonly string RichText = ".rtf";
        public static readonly string EncryptedPlainText = ".txt.aes256cbc";
        public static readonly string EncryptedRichText = ".rtf.aes256cbc";

    }
}
