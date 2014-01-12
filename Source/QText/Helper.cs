﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using System.Windows.Forms;

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

        public static bool DeleteTabFile(IWin32Window owner, TabFiles tabFiles, TabFile tabToDelete) {
            if (tabFiles == null) { throw new ArgumentNullException("tabFiles", "Tab parent cannot be null."); }
            if (tabToDelete == null) { throw new ArgumentNullException("tabToDelete", "Tab cannot be null."); }

            bool askForConfirmation = false;
            if (tabToDelete.CurrentFile.IsEncrypted) {
                askForConfirmation = true;
            } else {
                tabToDelete.Open();
                if (tabToDelete.TextBox.Text.Length > 0) { askForConfirmation |= true; }
            }
            if (askForConfirmation) {
                if (Medo.MessageBox.ShowQuestion(owner, string.Format(CultureInfo.CurrentUICulture, "Do you really want to delete \"{0}\"?", tabToDelete), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                    return false;
                }
            }
            try {
                tabFiles.Enabled = false;
                try {
                    tabFiles.RemoveTab(tabToDelete);
                    return true;
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(owner, string.Format(CultureInfo.CurrentUICulture, "Cannot delete file.\n\n{0}", ex.Message));
                    return false;
                }
            } finally {
                tabFiles.Enabled = true;
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


        public static string GetKeyString(Keys keyData) {
            if ((keyData & Keys.LWin) == Keys.LWin) { return string.Empty; }
            if ((keyData & Keys.RWin) == Keys.RWin) { return string.Empty; }

            var sb = new System.Text.StringBuilder();
            if ((keyData & Keys.Control) == Keys.Control) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Ctrl");
                keyData = keyData ^ Keys.Control;
            }

            if ((keyData & Keys.Alt) == Keys.Alt) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Alt");
                keyData = keyData ^ Keys.Alt;
            }

            if ((keyData & Keys.Shift) == Keys.Shift) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Shift");
                keyData = keyData ^ Keys.Shift;
            }

            switch (keyData) {
                case 0: return string.Empty;
                case Keys.ControlKey: return string.Empty;
                case Keys.Menu: return string.Empty;
                case Keys.ShiftKey: return string.Empty;
                default:
                    if (sb.Length > 0) { sb.Append("+"); }
                    sb.Append(keyData.ToString());
                    return sb.ToString();
            }
        }


        public static readonly ToolStripProfessionalRenderer ToolstripRenderer = new ToolStripBorderlessProfessionalRenderer();
        private class ToolStripBorderlessProfessionalRenderer : ToolStripProfessionalRenderer {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
                //base.OnRenderToolStripBorder(e);
            }
        }


        internal static class NativeMethods {

            public const uint MOVEFILE_COPY_ALLOWED = 0x02;
            public const uint MOVEFILE_WRITE_THROUGH = 0x08;


            [DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool MoveFileExW([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpExistingFileName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpNewFileName, uint dwFlags);

        }

    }
}
