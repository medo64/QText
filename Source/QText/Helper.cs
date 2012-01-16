using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace QText {

    internal static class Helper {

        public static void CreatePath(string path) {
            if ((!Directory.Exists(path))) {
                string currPath = path;
                var allPaths = new List<string>();
                while (!(Directory.Exists(currPath))) {
                    allPaths.Add(currPath);
                    currPath = System.IO.Path.GetDirectoryName(currPath);
                    if (string.IsNullOrEmpty(currPath)) {
                        throw new IOException("Path \"" + path + "\" can not be created.");
                    }
                }

                try {
                    for (int i = allPaths.Count - 1; i >= 0; i += -1) {
                        System.IO.Directory.CreateDirectory(allPaths[i]);
                    }
                } catch (Exception) {
                    throw new System.IO.IOException("Path \"" + path + "\" can not be created.");
                }
            }
        }

        public static void MovePath(string currentPath, string newPath) {
            if (currentPath.StartsWith(@"\\?\", StringComparison.Ordinal) == false) { currentPath = @"\\?\" + currentPath; }
            if (newPath.StartsWith(@"\\?\", StringComparison.Ordinal) == false) { newPath = @"\\?\" + newPath; }
            if (NativeMethods.MoveFileExW(currentPath, newPath, NativeMethods.MOVEFILE_COPY_ALLOWED | NativeMethods.MOVEFILE_WRITE_THROUGH) == false) {
                var ex = new Win32Exception();
                throw new IOException(ex.Message, ex);
            }
        }


        #region Encode/decode file name

        public static string EncodeFileName(string fileName) {
            if (fileName == null) { throw new ArgumentNullException("fileName"); }
            var invalidChars = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder();
            foreach (var ch in fileName) {
                if (Array.IndexOf(invalidChars, ch) >= 0) {
                    var value = (int)ch;
                    if ((value < 0) || (value > 255)) { throw new InvalidOperationException("Expected single byte."); }
                    sb.Append("~");
                    sb.Append(value.ToString("x2"));
                    sb.Append("~");
                } else {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        public static string DecodeFileName(string fileName) {
            if (fileName == null) { throw new ArgumentNullException("fileName"); }
            var invalidChars = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder();
            StringBuilder sbDecode = null;
            var inEncoded = false;
            foreach (var ch in fileName) {
                if (inEncoded) {
                    if (ch == '~') { //end decode
                        if (sbDecode.Length == 2) { //could be
                            int value;
                            if (int.TryParse(sbDecode.ToString(), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value)) {
                                var charValue = System.Convert.ToChar(value);
                                if (Array.IndexOf(invalidChars, charValue) >= 0) {
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


        internal static class NativeMethods {

            public const uint MOVEFILE_COPY_ALLOWED = 0x02;
            public const uint MOVEFILE_WRITE_THROUGH = 0x08;


            [DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool MoveFileExW([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpExistingFileName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpNewFileName, uint dwFlags);

        }

    }
}
