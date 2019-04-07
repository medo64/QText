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
            ShortcutsEnabled = false;
        }

        private void InitializeComponent() {
            PrintDocument = new System.Drawing.Printing.PrintDocument();
            SuspendLayout();
            // 
            // prdText
            // 
            PrintDocument.DocumentName = "";
            PrintDocument.BeginPrint += new System.Drawing.Printing.PrintEventHandler(prdText_BeginPrint);
            PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(prdText_PrintPage);
            ResumeLayout(false);
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
                var opts = (int)NativeMethods.SendMessage(Handle, NativeMethods.EM_GETLANGOPTIONS, IntPtr.Zero, IntPtr.Zero);
                opts |= NativeMethods.IMF_SPELLCHECKING;
                NativeMethods.SendMessage(Handle, NativeMethods.EM_SETLANGOPTIONS, IntPtr.Zero, new IntPtr(opts));
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
                        if (SelectionLength > 0) { //first delete selection
                            SelectedText = "";
                        } else {
                            var selection = TextSelection.FindWordStart(this, SelectionStart);
                            if (selection.IsNotEmpty) {
                                Select(selection.Start, SelectionStart - selection.Start);
                                SelectedText = "";
                            }
                        }
                    }
                    return true;

                case Keys.Control | Keys.Delete: { //delete word from cursor
                        if (SelectionLength > 0) { //first delete selection
                            SelectedText = "";
                        } else {
                            var selection = TextSelection.FindWordEnd(this, SelectionStart);
                            if (selection.IsNotEmpty) {
                                Select(SelectionStart, selection.Start - SelectionStart);
                                SelectedText = "";
                            }
                        }
                    }
                    return true;

                case Keys.Control | Keys.Left: {
                        var selection = TextSelection.FindWordStart(this, SelectionStart);
                        if (selection.IsNotEmpty) {
                            Select(selection.Start, 0);
                        }
                    }
                    return true;

                case Keys.Control | Keys.Shift | Keys.Left: {
                        TextSelection selection;
                        if ((SelectionStart + SelectionLength) <= CaretPosition) {
                            selection = TextSelection.FindWordStart(this, SelectionStart);
                        } else {
                            selection = TextSelection.FindWordStart(this, SelectionStart + SelectionLength);
                        }
                        if (selection.IsNotEmpty) {
                            NativeMethods.SendMessage(Handle, NativeMethods.EM_SETSEL, new IntPtr(CaretPosition), new IntPtr(selection.Start));
                        }
                    }
                    return true;

                case Keys.Control | Keys.Right: {
                        var selection = TextSelection.FindWordEnd(this, SelectionStart + SelectionLength);
                        if (selection.IsNotEmpty) {
                            Select(selection.Start, 0);
                        }
                    }
                    return true;

                case Keys.Control | Keys.Shift | Keys.Right: {
                        TextSelection selection;
                        if (SelectionStart >= CaretPosition) {
                            selection = TextSelection.FindWordEnd(this, SelectionStart + SelectionLength);
                        } else {
                            selection = TextSelection.FindWordEnd(this, SelectionStart);
                        }
                        if (selection.IsNotEmpty) {
                            NativeMethods.SendMessage(Handle, NativeMethods.EM_SETSEL, new IntPtr(CaretPosition), new IntPtr(selection.End));
                        }
                    }
                    return true;

                case Keys.Control | Keys.A: {
                        SelectAll();
                    }
                    return true;

                case Keys.Tab: {
                        var startLine = GetLineFromCharIndex(SelectionStart);
                        var endLine = GetLineFromCharIndex(SelectionStart + SelectionLength - 1);
                        if (startLine >= endLine) { //single line tab
                            SelectedText = "\t";
                        } else {
                            var origStart = SelectionStart;
                            var origLen = SelectionLength;
                            var startIndex = GetFirstCharIndexFromLine(startLine);
                            var endIndex = GetFirstCharIndexFromLine(endLine + 1) - 1;
                            if (endIndex < 0) { endIndex = Text.Length - 1; }
                            if (Text[endIndex] == '\n') { endIndex -= 1; } //if this is end of line
                            var text = Text.Substring(startIndex, endIndex - startIndex);
                            var lines = text.Split(new string[] { "\n" }, StringSplitOptions.None);
                            for (var i = 0; i < lines.Length; i++) {
                                lines[i] = "\t" + lines[i];
                            }
                            BeginUpdate();
                            SelectionStart = startIndex;
                            SelectionLength = endIndex - startIndex;
                            SelectedText = string.Join("\n", lines);
                            SelectionStart = (origStart == startIndex) ? origStart : origStart + 1;
                            SelectionLength = (origStart == startIndex) ? origLen + lines.Length : origLen + lines.Length - 1;
                            EndUpdate();
                        }
                    }
                    return true;

                case Keys.Shift | Keys.Tab: {
                        var startLine = GetLineFromCharIndex(SelectionStart);
                        var endLine = GetLineFromCharIndex(SelectionStart + SelectionLength - 1);
                        var origStart = SelectionStart;
                        var origLen = SelectionLength;
                        if (startLine >= endLine) { //single line tab
                            if (SelectionStart > 0) {
                                if (Text[SelectionStart - 1] == '\t') {
                                    BeginUpdate();
                                    SelectionStart -= 1;
                                    SelectionLength = 1;
                                    SelectedText = "";
                                    SelectionStart = origStart - 1;
                                    SelectionLength = origLen;
                                    EndUpdate();
                                }
                            }
                        } else {
                            var startIndex = GetFirstCharIndexFromLine(startLine);
                            var endIndex = GetFirstCharIndexFromLine(endLine + 1) - 1;
                            if (endIndex < 0) { endIndex = Text.Length - 1; }
                            if (Text[endIndex] == '\n') { endIndex -= 1; } //if this is end of line
                            var text = Text.Substring(startIndex, endIndex - startIndex);
                            var lines = text.Split(new string[] { "\n" }, StringSplitOptions.None);
                            var countRemoved = 0;
                            for (var i = 0; i < lines.Length; i++) {
                                if (lines[i].StartsWith("\t", StringComparison.InvariantCultureIgnoreCase)) {
                                    lines[i] = lines[i].Substring(1);
                                    countRemoved += 1;
                                }
                            }
                            if (countRemoved > 0) {
                                BeginUpdate();
                                SelectionStart = startIndex;
                                SelectionLength = endIndex - startIndex;
                                SelectedText = string.Join("\n", lines);
                                SelectionStart = (origStart == startIndex) ? origStart : origStart - 1;
                                SelectionLength = (origStart == startIndex) ? origLen - countRemoved : origLen - countRemoved + 1;
                                EndUpdate();
                            }
                        }
                    }
                    return true;


                case Keys.Control | Keys.Oemplus:
                case Keys.Control | Keys.Add:
                    ZoomIn();
                    return true;

                case Keys.Control | Keys.OemMinus:
                case Keys.Control | Keys.Subtract:
                    ZoomOut();
                    return true;

                case Keys.Control | Keys.D0:
                    ZoomReset();
                    return true;

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [SecurityPermissionAttribute(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_LBUTTONDBLCLK: {
                        var selection = TextSelection.FindWord(this, SelectionStart);
                        if (selection.IsEmpty == false) {
                            SelectionStart = selection.Start;
                            SelectionLength = selection.Length;
                        }
                    }
                    return;

                case NativeMethods.WM_SYSKEYDOWN:
                case NativeMethods.WM_SYSKEYUP:
                    switch (m.WParam.ToInt32()) { //keys to process
                        case NativeMethods.VK_MENU:
                            base.WndProc(ref m);
                            break;
                        case NativeMethods.VK_F4:
                            base.WndProc(ref m);
                            break;
                        case NativeMethods.VK_BACK:
                            base.WndProc(ref m);
                            break;
                    }
                    Debug.WriteLine("WndProc:WParam: " + m.WParam.ToInt64().ToString());
                    return;
            }
            base.WndProc(ref m);
        }


        private int CaretPosition; //used for selection
        public bool IsSelectionEmpty { get; private set; } = true;

        protected override void OnSelectionChanged(EventArgs e) {
            var range = new NativeMethods.CHARRANGE();
            NativeMethods.SendMessage(Handle, NativeMethods.EM_EXGETSEL, IntPtr.Zero, ref range); //check directly so SelectedText is not internally called
            IsSelectionEmpty = IsSelectionEmpty = (range.cpMin == range.cpMax);
            if (IsSelectionEmpty) { CaretPosition = range.cpMin; }
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
        private IntPtr _originalEventMask;
        public void BeginUpdate() {
            _beginUpdateCount += 1;
            if (_beginUpdateCount > 1) {
                return;
            }

            _originalEventMask = NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.EM_SETEVENTMASK, 0, 0); // Prevent the control from raising any events.
            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.WM_SETREDRAW, 0, 0); //Prevent the control from redrawing itself.
        }

        public void EndUpdate() {
            _beginUpdateCount -= 1;
            if (_beginUpdateCount > 0) { return; }

            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.WM_SETREDRAW, 1, 0); // Allow the control to redraw itself.
            NativeMethods.SendMessage(new HandleRef(this, Handle).Handle, NativeMethods.EM_SETEVENTMASK, 0, _originalEventMask); // Allow the control to raise event messages.

            Refresh();
        }

        public void SelectLineBlock() {
            var startLine = GetLineFromCharIndex(SelectionStart);
            var endLine = GetLineFromCharIndex(SelectionStart + SelectionLength - 1);
            var startIndex = GetFirstCharIndexFromLine(startLine);
            var endIndex = GetFirstCharIndexFromLine(endLine + 1) - 1;
            if (endIndex < 0) { endIndex = Text.Length; }
            SelectionStart = startIndex;
            SelectionLength = endIndex - startIndex + 1;
        }


        #region Decorations

        /// <summary>
        /// Resets font while keeping font attributes intact.
        /// </summary>
        public void ResetSelectionFont() {
            try {
                BeginUpdate();

                var selStart = SelectionStart;
                var selLength = SelectionLength;
                for (var i = selStart; i < selStart + selLength; i++) {
                    SelectionStart = i;
                    SelectionLength = 1;

                    var style = FontStyle.Regular;
                    if (SelectionFont.Bold) { style |= FontStyle.Bold; }
                    if (SelectionFont.Italic) { style |= FontStyle.Italic; }
                    if (SelectionFont.Underline) { style |= FontStyle.Underline; }
                    if (SelectionFont.Strikeout) { style |= FontStyle.Strikeout; }
                    SelectionFont = new Font(Settings.Current.DisplayFont, style); //to lazy to detect spans
                }

                SelectionStart = selStart;
                SelectionLength = selLength;
            } finally {
                EndUpdate();
            }
        }

        public void ResetSelectionParagraphSpacing() {
            try {
                BeginUpdate();

                var format = new NativeMethods.PARAFORMAT2() {
                    dwMask = NativeMethods.PFM_SPACEBEFORE | NativeMethods.PFM_SPACEAFTER,
                    dySpaceBefore = 0,    //no space before
                    dySpaceAfter = 0,     //no space after
                    bLineSpacingRule = 0, //single spacing
                };
                format.cbSize = Marshal.SizeOf(format);

                NativeMethods.SendMessage(Handle, NativeMethods.EM_SETPARAFORMAT, IntPtr.Zero, ref format);
            } finally {
                EndUpdate();
            }
        }

        public void ResetSelectionParagraphIndent() {
            try {
                BeginUpdate();

                var format = new NativeMethods.PARAFORMAT2() {
                    dwMask = NativeMethods.PFM_STARTINDENT,
                    dxStartIndent = 0,
                    dxOffset = 0,
                };
                format.cbSize = Marshal.SizeOf(format);

                NativeMethods.SendMessage(Handle, NativeMethods.EM_SETPARAFORMAT, IntPtr.Zero, ref format);
            } finally {
                EndUpdate();
            }
        }


        public bool SelectionNumbered {
            get {
                var format = new NativeMethods.PARAFORMAT2();
                format.cbSize = Marshal.SizeOf(format);
                NativeMethods.SendMessage(Handle, NativeMethods.EM_GETPARAFORMAT, IntPtr.Zero, ref format);
                return (format.wNumbering != 0) && (format.wNumbering != NativeMethods.PFN_BULLET);
            }
            set {
                if (value) {
                    var formatFirst = new NativeMethods.PARAFORMAT2() {
                        dwMask = NativeMethods.PFM_NUMBERINGSTART,
                        wNumberingStart = 1,
                    };
                    formatFirst.cbSize = Marshal.SizeOf(formatFirst);
                    NativeMethods.SendMessage(Handle, NativeMethods.EM_SETPARAFORMAT, IntPtr.Zero, ref formatFirst); //has to be a separate message otherwise multi-line selection would number all paragraphs as 1)

                    var formatAll = new NativeMethods.PARAFORMAT2() {
                        dwMask = NativeMethods.PFM_NUMBERING | NativeMethods.PFM_NUMBERINGSTYLE,
                        wNumbering = NativeMethods.PFN_ARABIC,
                    };
                    formatAll.cbSize = Marshal.SizeOf(formatAll);
                    NativeMethods.SendMessage(Handle, NativeMethods.EM_SETPARAFORMAT, IntPtr.Zero, ref formatAll);
                } else {
                    var format = new NativeMethods.PARAFORMAT2() {
                        dwMask = NativeMethods.PFM_NUMBERING,
                        wNumbering = NativeMethods.PFN_NONE,
                    };
                    format.cbSize = Marshal.SizeOf(format);
                    NativeMethods.SendMessage(Handle, NativeMethods.EM_SETPARAFORMAT, IntPtr.Zero, ref format);
                }
            }
        }

        #endregion Decorations


        #region Zooming

        public bool HasZoom {
            get { return (ZoomFactor != 0); }
        }

        public void ZoomIn() {
            ZoomFactor = (float)Math.Round(Math.Min(5.0f, ZoomFactor + 0.1f), 1);
        }

        public void ZoomOut() {
            ZoomFactor = (float)Math.Round(Math.Max(0.1f, ZoomFactor - 0.1f), 1);
        }

        public void ZoomReset() {
            ZoomFactor = 2.1f; //workaround for bug
            ZoomFactor = 1.0f;
        }

        #endregion


        #region Printing

        internal PrintDocument PrintDocument { get; private set; }

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
                e.Graphics.DrawString(PrintDocument.DocumentName, font, brush, e.MarginBounds.Left, e.MarginBounds.Top - topPull, sfTopLeft);
            }

            currentPrintLocation = PrintRtf(currentPrintLocation, TextLength, e);
            e.HasMorePages = (currentPrintLocation < TextLength);
        }


        private int PrintRtf(int charFrom, int charTo, PrintPageEventArgs e) {
            var hdc = IntPtr.Zero;
            var lParam = IntPtr.Zero;
            try {
                hdc = e.Graphics.GetHdc();
                var fmtRange = new NativeMethods.FORMATRANGE(hdc, e.PageBounds, e.MarginBounds, charFrom, charTo);

                lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
                Marshal.StructureToPtr(fmtRange, lParam, false);

                var res = NativeMethods.SendMessage(Handle, NativeMethods.EM_FORMATRANGE, new IntPtr(1), lParam);
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
            internal const int EM_SETLANGOPTIONS = WM_USER + 120;
            internal const int EM_GETLANGOPTIONS = WM_USER + 121;

            internal const int WM_LBUTTONDBLCLK = 0x0203;
            internal const int WM_SETREDRAW = 11;
            internal const int WM_SYSKEYDOWN = 0x0104;
            internal const int WM_SYSKEYUP = 0x0105;

            internal const int WM_USER = 0x0400;
            internal const int EM_EXGETSEL = WM_USER + 52;
            internal const int EM_FORMATRANGE = WM_USER + 57;

            internal const int EM_GETPARAFORMAT = 1085;
            internal const int EM_SETPARAFORMAT = 1095;
            internal const int PFM_STARTINDENT = 0x0001;
            internal const int PFM_NUMBERING = 0x0020;
            internal const int PFM_SPACEBEFORE = 0x0040;
            internal const int PFM_SPACEAFTER = 0x0080;
            internal const int PFM_NUMBERINGSTYLE = 0x2000;
            internal const int PFM_NUMBERINGSTART = 0x8000;
            internal const short PFN_NONE = 0;
            internal const short PFN_BULLET = 1;
            internal const short PFN_ARABIC = 2;
            internal const ushort PFNS_NEWNUMBER = 0x8000;


            [StructLayout(LayoutKind.Sequential)]
            internal struct RECT {
                public RECT(Rectangle rectangleInHundredsOfAnInch) {
                    Left = (int)(rectangleInHundredsOfAnInch.Left * 14.4);
                    Top = (int)(rectangleInHundredsOfAnInch.Top * 14.4);
                    Right = (int)(rectangleInHundredsOfAnInch.Right * 14.4);
                    Bottom = (int)(rectangleInHundredsOfAnInch.Bottom * 14.4);
                }
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct CHARRANGE {
                internal CHARRANGE(int charFrom, int CharTo) {
                    cpMin = charFrom;
                    cpMax = CharTo;
                }
                public int cpMin;
                public int cpMax;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct FORMATRANGE {
                public FORMATRANGE(IntPtr hdc, Rectangle pageRectangle, Rectangle drawingRectangle, int charFrom, int charTo) {
                    this.hdc = hdc;
                    hdcTarget = hdc;
                    rc = new RECT(drawingRectangle);
                    rcPage = new RECT(pageRectangle);
                    chrg = new CHARRANGE(charFrom, charTo);
                }
                public IntPtr hdc;             //Actual DC to draw on
                public IntPtr hdcTarget;       //Target DC for determining text formatting
                public RECT rc;                //Region of the DC to draw to (in twips)
                public RECT rcPage;            //Region of the whole DC (page size) (in twips)
                public CHARRANGE chrg;         //Range of text to draw (see earlier declaration)
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct PARAFORMAT2 {
                public int cbSize;
                public int dwMask;
                public short wNumbering;
                public short wReserved;
                public int dxStartIndent;
                public int dxRightIndent;
                public int dxOffset;
                public short wAlignment;
                public short cTabCount;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                public int[] rgxTabs;
                public int dySpaceBefore;
                public int dySpaceAfter;
                public int dyLineSpacing;
                public short sStyle;
                public byte bLineSpacingRule;
                public byte bOutlineLevel;
                public short wShadingWeight;
                public short wShadingStyle;
                public short wNumberingStart;
                public ushort wNumberingStyle;
                public short wNumberingTab;
                public short wBorderSpace;
                public short wBorderWidth;
                public short wBorders;
            }


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string path);


            internal static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam) {
                return SendMessage(hWnd, Msg, new IntPtr(wParam), ref lParam);
            }

            internal static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam) {
                var x = new IntPtr(lParam);
                return SendMessage(hWnd, Msg, new IntPtr(wParam), ref x);
            }

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SendMessage([InAttribute()] IntPtr hWnd, uint Msg, IntPtr wParam, ref IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SendMessage([InAttribute()] IntPtr hWnd, uint Msg, IntPtr wParam, ref PARAFORMAT2 lParam);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SendMessage([InAttribute()] IntPtr hWnd, int Msg, IntPtr wParam, ref CHARRANGE lParam);

        }

    }
}
