using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace QText {
    internal class RichTextBoxEx : RichTextBox {

        public RichTextBoxEx() {
        }


        protected override void OnLinkClicked(System.Windows.Forms.LinkClickedEventArgs e) {
            if (Settings.FollowURLs) {
                try {
                    System.Diagnostics.Process.Start(e.LinkText);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot execute link \"{0}\".\n\n{1}", e.LinkText, ex.Message));
                }
            }
        }


        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) {
            switch (keyData) {
                case Keys.Control | Keys.Back: { //deletes word before cursor
                        if (this.SelectionStart <= 0) { break; }
                        var endIndex = this.SelectionStart + this.SelectionLength - 1;
                        var startIndex = this.SelectionStart - 1;
                        while (startIndex >= 0) { //find start of word (backward)
                            if (char.IsWhiteSpace(this.Text[startIndex]) == false) { break; }
                            startIndex -= 1;
                        }
                        var category = char.GetUnicodeCategory(char.ToUpperInvariant(this.Text[startIndex]));
                        while (startIndex >= 0) { //find end of word (backward)
                            if (char.GetUnicodeCategory(char.ToUpperInvariant(this.Text[startIndex])) != category) { break; }
                            startIndex -= 1;
                        }
                        startIndex += 1;
                        this.SelectionStart = startIndex;
                        this.SelectionLength = endIndex - startIndex + 1;
                        this.SelectedText = "";
                    } return true;

                case Keys.Control | Keys.Delete: { //delete word
                        if (this.SelectionStart >= this.TextLength - 1) { break; }
                        var startIndex = this.SelectionStart;
                        var endIndex = this.SelectionStart;
                        var category = char.GetUnicodeCategory(char.ToUpperInvariant(this.Text[endIndex]));
                        while (endIndex < this.TextLength - 1) { //find end of word (forward)
                            if (char.GetUnicodeCategory(char.ToUpperInvariant(this.Text[endIndex])) != category) { break; }
                            endIndex += 1;
                        }
                        while (endIndex < this.TextLength - 1) { //include any trailing whitespace
                            if (char.IsWhiteSpace(this.Text[endIndex]) == false) { break; }
                            endIndex += 1;
                        }
                        endIndex -= 1;
                        this.SelectionStart = startIndex;
                        this.SelectionLength = endIndex - startIndex + 1;
                        this.SelectedText = "";
                    } return true;

                case Keys.Control | Keys.A: {
                        this.SelectAll();
                    } return true;

                case Keys.Tab: {
                        var startLine = this.GetLineFromCharIndex(this.SelectionStart);
                        var endLine = this.GetLineFromCharIndex(this.SelectionStart + this.SelectionLength - 1);
                        if (startLine >= endLine) { //single line tab
                            this.SelectedText = "\t";
                        } else {
                            var origStart = this.SelectionStart;
                            var origLen = this.SelectionLength;
                            var startIndex = this.GetFirstCharIndexFromLine(startLine);
                            var endIndex = this.GetFirstCharIndexFromLine(endLine + 1) - 1;
                            if (endIndex < 0) { endIndex = this.Text.Length - 1; }
                            if ((this.Text[endIndex] == '\r') || (this.Text[endIndex] == '\n')) { endIndex -= 1; } //if this is CRFL
                            var text = this.Text.Substring(startIndex, endIndex - startIndex);
                            var endOfLine = "\n"; if (text.Contains("\r\n")) { endOfLine = "\r\n"; } //to see whether LF or CRLF is used.
                            var lines = text.Split(new string[] { endOfLine }, StringSplitOptions.None);
                            for (int i = 0; i < lines.Length; i++) {
                                lines[i] = "\t" + lines[i];
                            }
                            this.BeginUpdate();
                            this.SelectionStart = startIndex;
                            this.SelectionLength = endIndex - startIndex;
                            this.SelectedText = string.Join(endOfLine, lines);
                            this.SelectionStart = (origStart == startIndex) ? origStart : origStart + 1;
                            this.SelectionLength = (origStart == startIndex) ? origLen + lines.Length * (endOfLine.Length) : origLen + lines.Length * (endOfLine.Length) - 1;
                            this.EndUpdate();
                        }
                    } return true;

                case Keys.Shift | Keys.Tab: {
                        var startLine = this.GetLineFromCharIndex(this.SelectionStart);
                        var endLine = this.GetLineFromCharIndex(this.SelectionStart + this.SelectionLength - 1);
                        var origStart = this.SelectionStart;
                        var origLen = this.SelectionLength;
                        if (startLine >= endLine) { //single line tab
                            if (this.SelectionStart > 0) {
                                if (this.Text[this.SelectionStart - 1] == '\t') {
                                    this.BeginUpdate();
                                    this.SelectionStart -= 1;
                                    this.SelectionLength = 1;
                                    this.SelectedText = "";
                                    this.SelectionStart = origStart - 1;
                                    this.SelectionLength = origLen;
                                    this.EndUpdate();
                                }
                            }
                        } else {
                            var startIndex = this.GetFirstCharIndexFromLine(startLine);
                            var endIndex = this.GetFirstCharIndexFromLine(endLine + 1) - 1;
                            if (endIndex < 0) { endIndex = this.Text.Length - 1; }
                            if ((this.Text[endIndex] == '\r') || (this.Text[endIndex] == '\n')) { endIndex -= 1; } //if this is CRFL
                            var text = this.Text.Substring(startIndex, endIndex - startIndex);
                            var endOfLine = "\n"; if (text.Contains("\r\n")) { endOfLine = "\r\n"; } //to see whether LF or CRLF is used.
                            var lines = text.Split(new string[] { endOfLine }, StringSplitOptions.None);
                            var countRemoved = 0;
                            for (int i = 0; i < lines.Length; i++) {
                                if (lines[i].StartsWith("\t", StringComparison.InvariantCultureIgnoreCase)) {
                                    lines[i] = lines[i].Substring(1);
                                    countRemoved += 1;
                                }
                            }
                            if (countRemoved > 0) {
                                this.BeginUpdate();
                                this.SelectionStart = startIndex;
                                this.SelectionLength = endIndex - startIndex;
                                this.SelectedText = string.Join(endOfLine, lines);
                                this.SelectionStart = (origStart == startIndex) ? origStart : origStart - 1;
                                this.SelectionLength = (origStart == startIndex) ? origLen - countRemoved : origLen - countRemoved + 1;
                                this.EndUpdate();
                            }
                        }
                    } return true;


            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_LBUTTONDBLCLK: {
                        if (this.TextLength == 0) { return; } //cannot find word in empty text
                        var startIndex = this.SelectionStart;
                        if (startIndex >= this.TextLength) { startIndex -= 1; }
                        if (this.Text[startIndex] == '\n') {
                            var line1 = this.GetLineFromCharIndex(startIndex);
                            var line2 = this.GetLineFromCharIndex(startIndex - 1);
                            if (line1 == line2) {
                                startIndex -= 1;
                            } else {
                                return; //cannot select word on empty row
                            }
                        }

                        var leftCount = 0;
                        while (startIndex - leftCount >= 0) { //find non whitespace
                            if (char.IsWhiteSpace(this.Text[startIndex - leftCount]) == false) { break; }
                            leftCount += 1;
                        }
                        if (startIndex - leftCount < 0) { leftCount = int.MinValue; }

                        var rightCount = 0;
                        while (startIndex + rightCount < this.TextLength - 1) { //find non whitespace
                            if (char.IsWhiteSpace(this.Text[startIndex + rightCount]) == false) { break; }
                            rightCount += 1;
                        }
                        if (startIndex + rightCount >= this.TextLength - 1) { rightCount = int.MinValue; }

                        if ((rightCount == int.MinValue) && (leftCount == int.MinValue)) {
                            return; //cannot select text if there is only whitespace
                        } else if (leftCount == int.MinValue) {
                            startIndex = startIndex + rightCount;
                        } else if (rightCount == int.MinValue) {
                            startIndex = startIndex - leftCount;
                        } else {
                            if (rightCount <= leftCount) {
                                startIndex = startIndex + rightCount;
                            } else {
                                startIndex = startIndex - leftCount;
                            }
                        }

                        var category = GetLikeUnicodeCategory(this.Text[startIndex]);
                        while (startIndex >= 0) { //find start of word
                            if (GetLikeUnicodeCategory(this.Text[startIndex]) != category) { break; }
                            startIndex -= 1;
                        }
                        startIndex += 1;
                        var endIndex = startIndex;
                        while (endIndex < this.TextLength - 1) { //find end of word
                            if (GetLikeUnicodeCategory(this.Text[endIndex]) != category) { break; }
                            endIndex += 1;
                        }
                        if (endIndex < this.TextLength - 1) { endIndex -= 1; }
                        this.SelectionStart = startIndex;
                        this.SelectionLength = endIndex - startIndex + 1;
                    } return;
            }
            base.WndProc(ref m);
        }

        private UnicodeCategory GetLikeUnicodeCategory(char ch) {
            if (char.IsWhiteSpace(ch)) {
                return UnicodeCategory.SpaceSeparator;
            } else if (char.IsPunctuation(ch)) {
                return UnicodeCategory.OtherSymbol;
            } else {
                return UnicodeCategory.LetterNumber;
            }
        }


        private int _beginUpdateCount;
        private IntPtr _originalEventMask;
        public void BeginUpdate() {
            this._beginUpdateCount += 1;
            if (this._beginUpdateCount > 1) {
                return;
            }

            _originalEventMask = NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.EM_SETEVENTMASK, 0, 0); // Prevent the control from raising any events.
            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.WM_SETREDRAW, 0, 0); //Prevent the control from redrawing itself.
        }

        public void EndUpdate() {
            this._beginUpdateCount -= 1;
            if (_beginUpdateCount > 0) { return; }

            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.WM_SETREDRAW, 1, 0); // Allow the control to redraw itself.
            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.EM_SETEVENTMASK, 0, _originalEventMask); // Allow the control to raise event messages.

            this.Refresh();
        }

        public void SelectLineBlock() {
            var startLine = this.GetLineFromCharIndex(this.SelectionStart);
            var endLine = this.GetLineFromCharIndex(this.SelectionStart + this.SelectionLength - 1);
            var startIndex = this.GetFirstCharIndexFromLine(startLine);
            var endIndex = this.GetFirstCharIndexFromLine(endLine + 1) - 1;
            if (endIndex < 0) { endIndex = this.Text.Length; }
            this.SelectionStart = startIndex;
            this.SelectionLength = endIndex - startIndex + 1;
        }


        private class NativeMethods {

            internal const int WM_SETREDRAW = 11;
            internal const int EM_SETEVENTMASK = 1073;
            internal const int WM_LBUTTONDBLCLK = 0x0203;


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

    }
}
