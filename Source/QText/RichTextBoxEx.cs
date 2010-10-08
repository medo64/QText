using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;

namespace QText {
    public class RichTextBoxEx : RichTextBox {

        private bool _useMultilineTab = true;
        /// <summary> 
        /// Gets or sets whether multiline tabs will be allowed. If Multiline is not true than this property is ignored. 
        /// </summary> 
        [System.ComponentModel.Category("Behaviour")]
        [System.ComponentModel.Description("Gets or sets whether multiline tabs will be allowed.")]
        [System.ComponentModel.MergableProperty(true)]
        [System.ComponentModel.DefaultValue(true)]
        public bool UseMultilineTab {
            get { return this._useMultilineTab; }
            set { this._useMultilineTab = value; }
        }

        protected override void OnLinkClicked(System.Windows.Forms.LinkClickedEventArgs e) {
            if (!Settings.FollowURLs) { return; }
            try {
                System.Diagnostics.Process.Start(e.LinkText);
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.InvariantCulture, "Cannot execute link \"{0}\".", e.LinkText) + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }


        private static int _beginUpdateCount;

        private static IntPtr _originalEventMask;
        public void BeginUpdate() {
            _beginUpdateCount += 1;
            if (_beginUpdateCount > 1) {
                return;
            }

            _originalEventMask = NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.EM_SETEVENTMASK, 0, 0);
            // Prevent the control from raising any events.
            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.WM_SETREDRAW, 0, 0);
            //Prevent the control from redrawing itself.
        }

        public void EndUpdate() {
            _beginUpdateCount -= 1;
            if ((_beginUpdateCount > 0))
                return;

            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.WM_SETREDRAW, 1, 0); // Allow the control to redraw itself.
            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.EM_SETEVENTMASK, 0, _originalEventMask); // Allow the control to raise event messages.

            this.Refresh();
        }


        private class NativeMethods {

            internal const int WM_SETREDRAW = 11;
            internal const int EM_SETEVENTMASK = 1073;

            public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam) {
                return SendMessageW(hWnd, Msg, new IntPtr(wParam), ref lParam);
            }

            public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam) {
                var x = new IntPtr(lParam);
                return SendMessageW(hWnd, Msg, new IntPtr(wParam), ref x);
            }

            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SendMessageW")]
            public static extern IntPtr SendMessageW([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, uint Msg, IntPtr wParam, ref IntPtr lParam);

        }


        /// <summary> 
        /// Processes a command key. 
        /// </summary> 
        /// <param name="msg">A System.Windows.Forms.Message, passed by reference, that represents the window message to process.</param> 
        /// <param name="keyData">One of the System.Windows.Forms.Keys values that represents the key to process.</param> 
        /// <returns>True if the character was processed by the control; otherwise, false.</returns> 
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) {
            switch (keyData) {
                case System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Back:

                    if (this.ShortcutsEnabled) {
                        int iEnd = this.SelectionStart + this.SelectionLength;
                        int i = iEnd - 1;
                        while (i > 0) {
                            if (!char.IsWhiteSpace(this.Text[i])) {
                                break; // TODO: might not be correct. Was : Exit While
                            }
                            i -= 1;
                        }
                        int iStart = 0;
                        while (i > 0) {
                            if (char.IsWhiteSpace(this.Text[i])) {
                                iStart = i + 1;
                                break; // TODO: might not be correct. Was : Exit While
                            }
                            i -= 1;
                        }
                        if (iStart < iEnd) {
                            this.SelectionStart = iStart;
                            this.SelectionLength = iEnd - iStart;
                            this.SelectedText = "";
                        }
                        return true;
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete:

                    if (this.ShortcutsEnabled) {
                        int iStart = this.SelectionStart;
                        int i = iStart;
                        while (i < this.Text.Length) {
                            if (!char.IsWhiteSpace(this.Text[i])) {
                                break; // TODO: might not be correct. Was : Exit While
                            }
                            i += 1;
                        }
                        int iEnd = 0;
                        while (i < this.Text.Length) {
                            if (char.IsWhiteSpace(this.Text[i])) {
                                if (this.Text[i] == System.Convert.ToChar(13)) {
                                    iEnd = i;
                                } else {
                                    iEnd = i + 1;
                                }
                                break; // TODO: might not be correct. Was : Exit While
                            }
                            i += 1;
                        }
                        if (iStart < iEnd) {
                            this.SelectionStart = iStart;
                            this.SelectionLength = iEnd - iStart;
                            this.SelectedText = "";
                        }
                        return true;
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A:

                    if (this.ShortcutsEnabled) {
                        this.SelectAll();
                        return true;
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case System.Windows.Forms.Keys.Tab:

                    if ((this.Multiline) && (this.UseMultilineTab)) {
                        int selStart = base.SelectionStart;
                        int selLength = base.SelectionLength;
                        int lineFrom = base.GetLineFromCharIndex(selStart);
                        int lineTo = base.GetLineFromCharIndex(selStart + selLength);
                        if (lineFrom == lineTo) {
                            //tab within single line 
                            base.SelectedText = System.Convert.ToChar(9).ToString();
                        } else {
                            //multiline tab 
                            string[] lines = base.Lines;
                            int lineToM = System.Math.Min(base.GetLineFromCharIndex(selStart + selLength - 1), lines.Length - 1);
                            for (int i = lineFrom; i <= lineToM; i++) {
                                lines[i] = System.Convert.ToChar(9) + lines[i];
                            }
                            base.Lines = lines;
                            base.SelectionStart = selStart + 1;
                            if (lineTo == lineToM) {
                                base.SelectionLength = selLength + (lineTo - lineFrom);
                            } else {
                                base.SelectionLength = selLength + (lineToM - lineFrom);
                            }
                        }
                        return true;
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Tab:

                    if ((this.Multiline) && (this.UseMultilineTab)) {
                        int selStart = base.SelectionStart;
                        int selLength = base.SelectionLength;
                        int lineFrom = base.GetLineFromCharIndex(selStart);
                        int lineTo = base.GetLineFromCharIndex(selStart + selLength);
                        if (lineFrom == lineTo) {
                            //single line tab back 
                            int selFirst = base.GetFirstCharIndexFromLine(lineTo);
                            if (selStart > selFirst) {
                                string[] lines = base.Lines;
                                if (lines[lineFrom][selStart - selFirst - 1] == System.Convert.ToChar(9)) {
                                    lines[lineFrom] = lines[lineFrom].Remove(selStart - selFirst - 1, 1);
                                    base.Lines = lines;
                                    base.SelectionStart = selStart - 1;
                                }
                            }
                        } else {
                            //multiline tab back 
                            string[] lines = base.Lines;
                            int lineToM = System.Math.Min(base.GetLineFromCharIndex(selStart + selLength - 1), lines.Length - 1);
                            int tabCount = 0;
                            bool tabInFirstLine = false;
                            for (int i = lineFrom; i <= lineToM; i++) {
                                if (lines[i].StartsWith(System.Convert.ToChar(9).ToString())) {
                                    if (i == lineFrom) {
                                        tabInFirstLine = true;
                                    }
                                    lines[i] = lines[i].Remove(0, 1);
                                    tabCount += 1;
                                }
                            }
                            if (tabCount > 0) {
                                base.Lines = lines;
                                if (selStart == 0) {
                                    base.SelectionStart = 0;
                                } else {
                                    if (tabInFirstLine == true) {
                                        base.SelectionStart = selStart - 1;
                                    } else {
                                        base.SelectionStart = selStart;
                                    }
                                }
                                if (tabCount <= (lineToM - lineFrom)) {
                                    base.SelectionLength = selLength - tabCount;
                                } else {
                                    base.SelectionLength = selLength - (lineToM - lineFrom);
                                }
                            }
                        }
                        return true;
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                default:

                    break; // TODO: might not be correct. Was : Exit Select

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
