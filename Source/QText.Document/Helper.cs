using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace QText {

    internal static class Helper {

        #region Encode/decode file name

        private static readonly char[] InvalidTitleChars = new char[] { '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007', 
                                                                        '\u0008', '\u0009', '\u000A', '\u000B', '\u000C', '\u000D', '\u000E', '\u000F',
                                                                        '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017',
                                                                        '\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', '\u001E', '\u001F',
                                                                        '\u0022', '\u002a', '\u002f', '\u003a', '\u003c', '\u003e', '\u003f', '\u005c', '\u007c' }; // " * / : < > ? \ |

        public static string EncodeTitle(string title) {
            var sb = new StringBuilder();
            foreach (var ch in title) {
                if (Array.IndexOf(InvalidTitleChars, ch) >= 0) {
                    sb.Append("~");
                    sb.Append(((byte)ch).ToString("x2"));
                    sb.Append("~");
                } else {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        public static string DecodeTitle(string name) {
            var sb = new StringBuilder();
            StringBuilder sbDecode = null;
            var inEncoded = false;
            foreach (var ch in name) {
                if (inEncoded) {
                    if (ch == '~') { //end decode
                        if (sbDecode.Length == 2) { //could be
                            int value;
                            if (int.TryParse(sbDecode.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value)) {
                                var charValue = Convert.ToChar(value);
                                if (Array.IndexOf(InvalidTitleChars, charValue) >= 0) {
                                    sb.Append(charValue);
                                    inEncoded = false;
                                } else { //not a char to be decoded
                                    sb.Append("~");
                                    sb.Append(sbDecode);
                                    sbDecode.Length = 0;
                                }
                            } else { //cannot decode
                                sb.Append("~");
                                sb.Append(sbDecode);
                                sbDecode.Length = 0;
                            }
                        } else {
                            sb.Append("~");
                            sb.Append(sbDecode);
                            sbDecode.Length = 0;
                        }
                    } else {
                        sbDecode.Append(ch);
                        if (sbDecode.Length > 2) {
                            sb.Append("~");
                            sb.Append(sbDecode);
                            inEncoded = false;
                        }
                    }
                } else {
                    if (ch == '~') { //start decode
                        if (sbDecode == null) { sbDecode = new StringBuilder(); } else { sbDecode.Length = 0; }
                        inEncoded = true;
                    } else {
                        sb.Append(ch);
                    }
                }
            }
            if (inEncoded) {
                sb.Append("~");
                sb.Append(sbDecode);
            }
            return sb.ToString();
        }

        #endregion

        public static void MovePath(string currentPath, string newPath) {
            if (currentPath.StartsWith(@"\\?\", StringComparison.Ordinal) == false) { currentPath = @"\\?\" + currentPath; }
            if (newPath.StartsWith(@"\\?\", StringComparison.Ordinal) == false) { newPath = @"\\?\" + newPath; }
            if (NativeMethods.MoveFileExW(currentPath, newPath, NativeMethods.MOVEFILE_COPY_ALLOWED | NativeMethods.MOVEFILE_WRITE_THROUGH) == false) {
                var ex = new Win32Exception();
                throw new IOException(ex.Message, ex);
            }
        }

        internal static class NativeMethods {

            public const uint MOVEFILE_COPY_ALLOWED = 0x02;
            public const uint MOVEFILE_WRITE_THROUGH = 0x08;


            [DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool MoveFileExW([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpExistingFileName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpNewFileName, uint dwFlags);

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

        #region Extensions

        internal static IEnumerable<string> GetExtensions() {
            yield return Extensions.Plain;
            yield return Extensions.Rich;
            yield return Extensions.PlainEncrypted;
            yield return Extensions.RichEncrypted;
        }

        internal static class Extensions {

            public static readonly string Plain = ".txt";
            public static readonly string Rich = ".rtf";
            public static readonly string PlainEncrypted = ".txt.aes256cbc";
            public static readonly string RichEncrypted = ".rtf.aes256cbc";

        }

        #endregion


        internal class FileSystemToggler : IDisposable {

            public FileSystemToggler(FileSystemWatcher watcher) {
                this.Watcher = watcher;
                this.WasEnabled = watcher.EnableRaisingEvents;
                this.Watcher.EnableRaisingEvents = false;
            }

            private readonly FileSystemWatcher Watcher;
            private readonly bool WasEnabled;


            void IDisposable.Dispose() {
                this.Watcher.EnableRaisingEvents = this.WasEnabled;
            }
        }

    }
}
