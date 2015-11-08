using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace QText {
    internal class RichTextBoxEx : RichTextBox {

        public RichTextBoxEx() {
            InitializeComponent();
            this.ShortcutsEnabled = false;
        }

        private void InitializeComponent() {
            this.prdText = new System.Drawing.Printing.PrintDocument();
            this.SuspendLayout();
            // 
            // prdText
            // 
            this.prdText.DocumentName = "";
            this.prdText.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.prdText_BeginPrint);
            this.prdText.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.prdText_PrintPage);
            this.ResumeLayout(false);
        }


        #region CreateParams

        private static IntPtr RichTextLibraryHandle;

        protected override CreateParams CreateParams {
            get {
                var createParams = base.CreateParams;

                if (Settings.Current.UseRichText50) {
                    if (RichTextLibraryHandle == IntPtr.Zero) { RichTextLibraryHandle = NativeMethods.LoadLibrary("msftedit.dll"); }

                    if (RichTextLibraryHandle != IntPtr.Zero) { //if library has been loaded, change create params
                        createParams.ClassName = "RichEdit50W";
                    }
                }

                return createParams;
            }
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            if (Settings.Current.UseSpellCheck) {
                int opts = (int)NativeMethods.SendMessage(this.Handle, NativeMethods.EM_GETLANGOPTIONS, IntPtr.Zero, IntPtr.Zero);
                opts |= NativeMethods.IMF_SPELLCHECKING;
                NativeMethods.SendMessage(this.Handle, NativeMethods.EM_SETLANGOPTIONS, IntPtr.Zero, new IntPtr(opts));
            }
        }

        #endregion


        [UIPermissionAttribute(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool IsInputKey(Keys keyData) {
            Debug.WriteLine("RichTextBoxEx_IsInputKey: " + keyData.ToString());
            switch (keyData) {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.F13:
                case Keys.F14:
                case Keys.F15:
                case Keys.F16:
                case Keys.F17:
                case Keys.F18:
                case Keys.F19:
                case Keys.F20:
                case Keys.Control | Keys.B:
                case Keys.Control | Keys.C:
                case Keys.Control | Keys.D:
                case Keys.Control | Keys.E:
                case Keys.Control | Keys.F:
                case Keys.Control | Keys.G:
                case Keys.Control | Keys.H:
                case Keys.Control | Keys.I:
                case Keys.Control | Keys.J:
                case Keys.Control | Keys.K:
                case Keys.Control | Keys.L:
                case Keys.Control | Keys.M:
                case Keys.Control | Keys.N:
                case Keys.Control | Keys.O:
                case Keys.Control | Keys.P:
                case Keys.Control | Keys.Q:
                case Keys.Control | Keys.R:
                case Keys.Control | Keys.S:
                case Keys.Control | Keys.T:
                case Keys.Control | Keys.U:
                case Keys.Control | Keys.V:
                case Keys.Control | Keys.W:
                case Keys.Control | Keys.X:
                case Keys.Control | Keys.Y:
                case Keys.Control | Keys.Z:
                case Keys.Alt | Keys.D0:
                case Keys.Alt | Keys.D1:
                case Keys.Alt | Keys.D2:
                case Keys.Alt | Keys.D3:
                case Keys.Alt | Keys.D4:
                case Keys.Alt | Keys.D5:
                case Keys.Alt | Keys.D6:
                case Keys.Alt | Keys.D7:
                case Keys.Alt | Keys.D8:
                case Keys.Alt | Keys.D9:
                    return false;
            }
            return base.IsInputKey(keyData);
        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [SecurityPermissionAttribute(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) {
            Debug.WriteLine("RichTextBoxEx_ProcessCmdKey: " + keyData.ToString());
            switch (keyData) {
                case Keys.Control | Keys.R:
                    return false;

                case Keys.Control | Keys.Back: { //deletes word before cursor
                        if (this.SelectionLength > 0) { //first delete selection
                            this.SelectedText = "";
                        } else {
                            var selection = TextSelection.FindWordStart(this, this.SelectionStart);
                            if (selection.IsNotEmpty) {
                                this.Select(selection.Start, this.SelectionStart - selection.Start);
                                this.SelectedText = "";
                            }
                        }
                    } return true;

                case Keys.Control | Keys.Delete: { //delete word from cursor
                        if (this.SelectionLength > 0) { //first delete selection
                            this.SelectedText = "";
                        } else {
                            var selection = TextSelection.FindWordEnd(this, this.SelectionStart);
                            if (selection.IsNotEmpty) {
                                this.Select(this.SelectionStart, selection.Start - this.SelectionStart);
                                this.SelectedText = "";
                            }
                        }
                    } return true;

                case Keys.Control | Keys.Left: {
                        var selection = TextSelection.FindWordStart(this, this.SelectionStart);
                        if (selection.IsNotEmpty) {
                            this.Select(selection.Start, 0);
                        }
                    } return true;

                case Keys.Control | Keys.Shift | Keys.Left: {
                        TextSelection selection;
                        if ((this.SelectionStart + this.SelectionLength) <= this.CaretPosition) {
                            selection = TextSelection.FindWordStart(this, this.SelectionStart);
                        } else {
                            selection = TextSelection.FindWordStart(this, this.SelectionStart + this.SelectionLength);
                        }
                        if (selection.IsNotEmpty) {
                            NativeMethods.SendMessage(this.Handle, NativeMethods.EM_SETSEL, new IntPtr(this.CaretPosition), new IntPtr(selection.Start));
                        }
                    } return true;

                case Keys.Control | Keys.Right: {
                        var selection = TextSelection.FindWordEnd(this, this.SelectionStart + this.SelectionLength);
                        if (selection.IsNotEmpty) {
                            this.Select(selection.Start, 0);
                        }
                    } return true;

                case Keys.Control | Keys.Shift | Keys.Right: {
                        TextSelection selection;
                        if (this.SelectionStart >= this.CaretPosition) {
                            selection = TextSelection.FindWordEnd(this, this.SelectionStart + this.SelectionLength);
                        } else {
                            selection = TextSelection.FindWordEnd(this, this.SelectionStart);
                        }
                        if (selection.IsNotEmpty) {
                            NativeMethods.SendMessage(this.Handle, NativeMethods.EM_SETSEL, new IntPtr(this.CaretPosition), new IntPtr(selection.End));
                        }
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
                            if (this.Text[endIndex] == '\n') { endIndex -= 1; } //if this is end of line
                            var text = this.Text.Substring(startIndex, endIndex - startIndex);
                            var lines = text.Split(new string[] { "\n" }, StringSplitOptions.None);
                            for (int i = 0; i < lines.Length; i++) {
                                lines[i] = "\t" + lines[i];
                            }
                            this.BeginUpdate();
                            this.SelectionStart = startIndex;
                            this.SelectionLength = endIndex - startIndex;
                            this.SelectedText = string.Join("\n", lines);
                            this.SelectionStart = (origStart == startIndex) ? origStart : origStart + 1;
                            this.SelectionLength = (origStart == startIndex) ? origLen + lines.Length : origLen + lines.Length - 1;
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
                            if (this.Text[endIndex] == '\n') { endIndex -= 1; } //if this is end of line
                            var text = this.Text.Substring(startIndex, endIndex - startIndex);
                            var lines = text.Split(new string[] { "\n" }, StringSplitOptions.None);
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
                                this.SelectedText = string.Join("\n", lines);
                                this.SelectionStart = (origStart == startIndex) ? origStart : origStart - 1;
                                this.SelectionLength = (origStart == startIndex) ? origLen - countRemoved : origLen - countRemoved + 1;
                                this.EndUpdate();
                            }
                        }
                    } return true;


                case Keys.Control | Keys.Oemplus:
                case Keys.Control | Keys.Add:
                    this.ZoomIn();
                    return true;

                case Keys.Control | Keys.OemMinus:
                case Keys.Control | Keys.Subtract:
                    this.ZoomOut();
                    return true;

                case Keys.Control | Keys.D0:
                    this.ZoomReset();
                    return true;

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [SecurityPermissionAttribute(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_LBUTTONDBLCLK: {
                        var selection = TextSelection.FindWord(this, this.SelectionStart);
                        if (selection.IsEmpty == false) {
                            this.SelectionStart = selection.Start;
                            this.SelectionLength = selection.Length;
                        }
                    } return;

                case NativeMethods.WM_SYSKEYDOWN:
                case NativeMethods.WM_SYSKEYUP:
                    switch (m.WParam.ToInt32()) { //keys to process
                        case NativeMethods.VK_MENU: base.WndProc(ref m); break;
                        case NativeMethods.VK_F4: base.WndProc(ref m); break;
                        case NativeMethods.VK_BACK: base.WndProc(ref m); break;
                    }
                    Debug.WriteLine("WndProc:WParam: " + m.WParam.ToInt64().ToString());
                    return;
            }
            base.WndProc(ref m);
        }


        private Int32 CaretPosition; //used for selection

        protected override void OnSelectionChanged(EventArgs e) {
            if (this.SelectionLength == 0) { this.CaretPosition = this.SelectionStart; }
            base.OnSelectionChanged(e);
        }

        protected override void OnLinkClicked(System.Windows.Forms.LinkClickedEventArgs e) {
            if (Settings.Current.FollowURLs) {
                try {
                    if ((e != null) && (string.IsNullOrEmpty(e.LinkText) == false)) {
                        System.Diagnostics.Process.Start(e.LinkText);
                    }
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot execute link \"{0}\".\n\n{1}", e.LinkText, ex.Message));
                }
            }
        }


        private int _beginUpdateCount;
        private System.Drawing.Printing.PrintDocument prdText;
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


        #region Zooming

        public Boolean HasZoom {
            get { return (this.ZoomFactor != 0); }
        }

        public void ZoomIn() {
            this.ZoomFactor = (float)Math.Round(Math.Min(5.0f, this.ZoomFactor + 0.1f), 1);
        }

        public void ZoomOut() {
            this.ZoomFactor = (float)Math.Round(Math.Max(0.1f, this.ZoomFactor - 0.1f), 1);
        }

        public void ZoomReset() {
            this.ZoomFactor = 2.1f; //workaround for bug
            this.ZoomFactor = 1.0f;
        }

        #endregion


        #region Printing

        internal PrintDocument PrintDocument { get { return prdText; } }

        private int currentPrintLocation;
        private int currentPrintPage;

        private void prdText_BeginPrint(object sender, PrintEventArgs e) {
            currentPrintPage = 0;
            currentPrintLocation = 0;
        }

        private readonly StringFormat sfTopLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far };
        private readonly StringFormat sfTopRight = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far };
        private readonly int topPull = 5;

        private void prdText_PrintPage(object sender, PrintPageEventArgs e) {
            currentPrintPage += 1;
            using (var font = new Font("Arial", 8))
            using (var brush = new SolidBrush(Color.FromArgb(63, 63, 63)))
            using (var pen = new Pen(brush, 1)) {
                e.Graphics.DrawLine(pen, e.MarginBounds.Left, e.MarginBounds.Top - topPull, e.MarginBounds.Right, e.MarginBounds.Top - topPull);
                e.Graphics.DrawString(currentPrintPage.ToString(), font, brush, e.MarginBounds.Right, e.MarginBounds.Top - topPull, sfTopRight);
                e.Graphics.DrawString(this.PrintDocument.DocumentName, font, brush, e.MarginBounds.Left, e.MarginBounds.Top - topPull, sfTopLeft);
            }

            currentPrintLocation = PrintRtf(currentPrintLocation, this.TextLength, e);
            e.HasMorePages = (currentPrintLocation < this.TextLength);
        }


        private int PrintRtf(int charFrom, int charTo, PrintPageEventArgs e) {
            IntPtr hdc = IntPtr.Zero;
            IntPtr lParam = IntPtr.Zero;
            try {
                hdc = e.Graphics.GetHdc();
                var fmtRange = new NativeMethods.FORMATRANGE(hdc, e.PageBounds, e.MarginBounds, charFrom, charTo);

                lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
                Marshal.StructureToPtr(fmtRange, lParam, false);

                var res = NativeMethods.SendMessage(this.Handle, NativeMethods.EM_FORMATRANGE, new IntPtr(1), lParam);
                return res.ToInt32();
            } finally {
                if (lParam != IntPtr.Zero) { Marshal.FreeCoTaskMem(lParam); }
                if (hdc != IntPtr.Zero) { e.Graphics.ReleaseHdc(hdc); }
            }
        }

        #endregion


        private class NativeMethods {

            internal const int VK_BACK = 0x08;
            internal const int VK_F4 = 0x73;
            internal const int VK_MENU = 0x12;

            internal const int EM_SETEVENTMASK = 1073;
            internal const int EM_SETSEL = 0x00B1;

            internal const int IMF_SPELLCHECKING = 0x0800;
            internal const int EM_SETLANGOPTIONS = 0x0400 + 120;
            internal const int EM_GETLANGOPTIONS = 0x0400 + 121;

            internal const int WM_LBUTTONDBLCLK = 0x0203;
            internal const int WM_SETREDRAW = 11;
            internal const int WM_SYSKEYDOWN = 0x0104;
            internal const int WM_SYSKEYUP = 0x0105;

            internal const int WM_USER = 0x0400;
            internal const int EM_FORMATRANGE = WM_USER + 57;


            [StructLayout(LayoutKind.Sequential)]
            internal struct RECT {
                public RECT(Rectangle rectangleInHundredsOfAnInch) {
                    this.Left = (int)(rectangleInHundredsOfAnInch.Left * 14.4);
                    this.Top = (int)(rectangleInHundredsOfAnInch.Top * 14.4);
                    this.Right = (int)(rectangleInHundredsOfAnInch.Right * 14.4);
                    this.Bottom = (int)(rectangleInHundredsOfAnInch.Bottom * 14.4);
                }
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct CHARRANGE {
                internal CHARRANGE(int charFrom, int CharTo) {
                    this.cpMin = charFrom;
                    this.cpMax = CharTo;
                }
                public int cpMin;
                public int cpMax;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct FORMATRANGE {
                public FORMATRANGE(IntPtr hdc, Rectangle pageRectangle, Rectangle drawingRectangle, int charFrom, int charTo) {
                    this.hdc = hdc;
                    this.hdcTarget = hdc;
                    this.rc = new RECT(drawingRectangle);
                    this.rcPage = new RECT(pageRectangle);
                    this.chrg = new CHARRANGE(charFrom, charTo);
                }
                public IntPtr hdc;             //Actual DC to draw on
                public IntPtr hdcTarget;       //Target DC for determining text formatting
                public RECT rc;                //Region of the DC to draw to (in twips)
                public RECT rcPage;            //Region of the whole DC (page size) (in twips)
                public CHARRANGE chrg;         //Range of text to draw (see earlier declaration)
            }


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr LoadLibrary(String path);


            public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam) {
                return SendMessage(hWnd, Msg, new IntPtr(wParam), ref lParam);
            }

            public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam) {
                var x = new IntPtr(lParam);
                return SendMessage(hWnd, Msg, new IntPtr(wParam), ref x);
            }

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr SendMessage([InAttribute()] IntPtr hWnd, uint Msg, IntPtr wParam, ref IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        }

    }
}
