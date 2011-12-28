using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace QText {

    internal class TabFile : TabPage {

        public TabFile(string fullFileName)
            : base() {

            this.CurrentFile = new FileInfo(fullFileName);
            this.LastSaveTime = System.DateTime.Now;

            base.Text = this.Title;

            var tb = new RichTextBoxEx();
            tb.AcceptsTab = true;
            tb.BackColor = Settings.DisplayBackgroundColor;
            tb.Dock = DockStyle.Fill;
            tb.Font = Settings.DisplayFont;
            tb.ForeColor = Settings.DisplayForegroundColor;
            tb.HideSelection = false;
            tb.MaxLength = 0;
            tb.Multiline = true;
            tb.ShortcutsEnabled = false;
            tb.DetectUrls = Settings.DetectUrls;
            switch (Settings.ScrollBars) {
                case ScrollBars.None:
                    tb.ScrollBars = RichTextBoxScrollBars.None;
                    break;
                case ScrollBars.Horizontal:
                    tb.ScrollBars = RichTextBoxScrollBars.ForcedHorizontal;
                    break;
                case ScrollBars.Vertical:
                    tb.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
                    break;
                case ScrollBars.Both:
                    tb.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
                    break;
            }
            tb.WordWrap = Settings.DisplayWordWrap;

            this.TextBox = tb;
            if (Settings.FilesPreload) { this.Open(); }

            UpdateTabWidth();

            this.GotFocus += txt_GotFocus;
            this.TextBox.Enter += txt_GotFocus;
            this.TextBox.TextChanged += txt_TextChanged;
            this.TextBox.PreviewKeyDown += txt_PreviewKeyDown;

            base.Controls.Add(this.TextBox);
        }


        private static readonly UTF8Encoding Utf8EncodingWithoutBom = new UTF8Encoding(false);

        public FileInfo CurrentFile { get; private set; }
        public string Title { get { return Path.GetFileNameWithoutExtension(this.CurrentFile.FullName); } }
        public DateTime LastWriteTimeUtc { get { return this.CurrentFile.LastWriteTimeUtc; } }
        public DateTime LastSaveTime { get; private set; }

        public bool IsOpened { get; private set; }

        private bool _isChanged;
        public bool IsChanged {
            get {
                return this._isChanged && this.IsOpened; //only open file can be changed
            }
            private set {
                this._isChanged = value;
                UpdateText();
            }
        }

        public RichTextBoxEx TextBox { get; private set; }


        public override ContextMenuStrip ContextMenuStrip {
            get { return this.TextBox.ContextMenuStrip; }
            set { this.TextBox.ContextMenuStrip = value; }
        }


        public static TabFile Create(string fullFileName) {
            if (File.Exists(fullFileName)) {
                throw new IOException("File already exists.");
            } else if (File.Exists(Path.ChangeExtension(fullFileName, ".txt"))) {
                throw new IOException("File already exists.");
            } else if (File.Exists(Path.ChangeExtension(fullFileName, ".rtf"))) {
                throw new IOException("File already exists.");
            } else {
                if (Path.GetExtension(fullFileName).Equals(".rtf", StringComparison.OrdinalIgnoreCase)) {
                    using (RichTextBox dummy = new RichTextBox()) {
                        dummy.BackColor = Settings.DisplayBackgroundColor;
                        dummy.Font = Settings.DisplayFont;
                        dummy.ForeColor = Settings.DisplayForegroundColor;
                        dummy.SaveFile(fullFileName, RichTextBoxStreamType.RichText);
                    }
                } else {
                    File.WriteAllText(fullFileName, "");
                }
            }
            return new TabFile(fullFileName);
        }



        public void Open() {
            if (this.IsOpened == false) {
                this.Reopen();
            }
        }

        public void ConvertToPlainText() {
            if (this.IsRichTextFormat == false) { return; }

            this.Open();
            string oldFileName = this.CurrentFile.FullName;
            string newFileName = Path.ChangeExtension(oldFileName, ".txt");
            File.Move(oldFileName, newFileName);
            this.CurrentFile = new FileInfo(newFileName);
            this.Save();
            this.Reopen();
        }

        public void ConvertToRichText() {
            if (this.IsRichTextFormat) { return; }

            this.Open();
            string text = this.TextBox.Text;

            string oldFileName = this.CurrentFile.FullName;
            string newFileName = Path.ChangeExtension(oldFileName, ".rtf");
            File.Move(oldFileName, newFileName);
            this.CurrentFile = new FileInfo(newFileName);
            this.Save();
            this.Reopen();
        }

        public bool IsRichTextFormat {
            get {
                return (string.Compare(this.CurrentFile.Extension, ".rtf", StringComparison.OrdinalIgnoreCase) == 0);
            }
        }

        public void Reopen() {
            this.IsOpened = false;
            if (this.IsRichTextFormat) {
                try {
                    OpenAsRtf();
                } catch (System.ArgumentException) {
                    OpenAsTxt();
                }
            } else {
                OpenAsTxt();
            }
            this.TextBox.SelectionStart = 0;
            this.TextBox.SelectionLength = 0;
            this.TextBox.ClearUndo();
            this.LastSaveTime = DateTime.Now;
            this.CurrentFile.Refresh();
            this.IsChanged = false;
            this.IsOpened = true;
        }

        public void QuickSaveWithoutException() {
            try {
                this.Save();
            } catch (Exception) { }
        }

        private int QuickSaveFailedCounter;

        public void QuickSave() { //allow for three failed  attempts
            try {
                this.Save();
                this.QuickSaveFailedCounter = 0;
            } catch (Exception) {
                this.QuickSaveFailedCounter += 1;
                if (this.QuickSaveFailedCounter == 4) {
                    throw;
                }
            }
        }

        public void Save() {
            if (this.IsOpened == false) { return; }

            if (this.IsRichTextFormat) {
                SaveAsRtf();
            } else {
                SaveAsTxt();
            }

            this.LastSaveTime = DateTime.Now;
            this.CurrentFile.Refresh();
            this.IsChanged = false;
            base.Text = this.Title;

            SaveCarbonCopy();
        }

        public void SaveCarbonCopy() {
            this.Open();
            if ((!Settings.CarbonCopyUse) || (string.IsNullOrEmpty(Settings.CarbonCopyFolder))) { return; }

            string destFile = null;
            try {
                if ((!Directory.Exists(Settings.CarbonCopyFolder)) && (Settings.CarbonCopyCreateFolder)) {
                    Helper.CreatePath(Settings.CarbonCopyFolder);
                }

                var fiBase = new FileInfo(this.CurrentFile.FullName);
                destFile = Path.Combine(Settings.CarbonCopyFolder, fiBase.Name);
                if (this.IsRichTextFormat) {
                    this.TextBox.SaveFile(destFile, RichTextBoxStreamType.RichText);
                } else {
                    File.WriteAllText(destFile, this.TextBox.Text);
                }
            } catch (Exception) {
                if (Settings.CarbonCopyIgnoreErrors == false) {
                    if (!string.IsNullOrEmpty(destFile)) {
                        Medo.MessageBox.ShowWarning(this, "Error making carbon copy to \"" + destFile + "\".", MessageBoxButtons.OK);
                    } else {
                        Medo.MessageBox.ShowWarning(this, "Error making carbon copy from \"" + this.CurrentFile.FullName + "\".", MessageBoxButtons.OK);
                    }
                }
            }
        }

        public void Rename(string newTitle) {
            var oldInfo = new FileInfo(this.CurrentFile.FullName);
            var newInfo = new FileInfo(Path.Combine(oldInfo.DirectoryName, newTitle) + oldInfo.Extension);

            if (File.Exists(newInfo.FullName)) {
                throw new IOException("File already exists.");
            } else if (File.Exists(Path.ChangeExtension(newInfo.FullName, ".txt"))) {
                throw new IOException("File already exists.");
            } else if (File.Exists(Path.ChangeExtension(newInfo.FullName, ".rtf"))) {
                throw new IOException("File already exists.");
            } else {
                File.Move(oldInfo.FullName, newInfo.FullName);
                this.CurrentFile = newInfo;
                UpdateText();
            }
        }

        public bool GetIsEligibleForSave(int timeout) {
            return (this.IsChanged) && (this.LastSaveTime.AddSeconds(timeout) <= DateTime.Now);
        }


        #region txt

        private void txt_GotFocus(System.Object sender, System.EventArgs e) {
            this.Open();
            this.TextBox.Select();
        }

        private void txt_TextChanged(System.Object sender, System.EventArgs e) {
            if (this.IsOpened && (this.IsChanged == false)) {
                this.IsChanged = true;
            }
        }

        private void txt_PreviewKeyDown(System.Object sender, System.Windows.Forms.PreviewKeyDownEventArgs e) {
            Debug.WriteLine("TabFile.PreviewKeyDown: " + e.KeyData.ToString());
            switch (e.KeyData) {

                case Keys.Control | Keys.X:
                    this.Cut(QText.Settings.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.C:
                    this.Copy(QText.Settings.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.V:
                    this.Paste(QText.Settings.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;


                case Keys.Shift | Keys.Delete:
                    this.Cut(true);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.Insert:
                    this.Copy(true);
                    e.IsInputKey = false;
                    break;

                case Keys.Shift | Keys.Insert:
                    this.Paste(true);
                    e.IsInputKey = false;
                    break;


                case Keys.Control | Keys.Z:
                case Keys.Alt | Keys.Back:
                    this.Undo();
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.Y:
                case Keys.Alt | Keys.Shift | Keys.Back:
                    this.Redo();
                    e.IsInputKey = false;
                    break;

                case Keys.X:
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.A:
                    this.TextBox.SelectAll();
                    e.IsInputKey = false;
                    break;
            }
        }

        #endregion

        #region Cut/Copy/Paste/Undo/Redo

        public bool CanCut {
            get { return this.TextBox.SelectionLength > 0; }
        }

        public void Cut(bool forceText) {
            try {
                if (this.CanCopy) {
                    if ((this.IsRichTextFormat == false) || forceText) {
                        Clipboard.Clear();
                        Clipboard.SetText(this.TextBox.SelectedText, TextDataFormat.UnicodeText);
                        this.TextBox.SelectedText = "";
                    } else {
                        this.TextBox.Cut();
                    }
                }
            } catch (ExternalException) {
            }
        }

        public bool CanCopy {
            get { return this.TextBox.SelectionLength > 0; }
        }

        public void Copy(bool forceText) {
            try {
                if (this.CanCopy) {
                    if ((this.IsRichTextFormat == false) || forceText) {
                        Clipboard.Clear();
                        Clipboard.SetText(this.TextBox.SelectedText, TextDataFormat.UnicodeText);
                    } else {
                        this.TextBox.Copy();
                    }
                }
            } catch (ExternalException) { }
        }

        public bool CanPaste {
            get {
                try {
                    return Clipboard.ContainsText();
                } catch (ExternalException) {
                    return false;
                }
            }
        }

        public void Paste(bool forceText) {
            try {
                if (CanPaste) {
                    if ((this.IsRichTextFormat == false) || forceText) {
                        var text = Clipboard.GetText(TextDataFormat.UnicodeText);
                        this.TextBox.SelectedText = text;
                    } else {
                        this.TextBox.Paste();
                    }
                }
            } catch (ExternalException) { }
        }


        public bool CanUndo {
            get { return this.TextBox.CanUndo; }
        }

        public void Undo() {
            if (this.CanUndo) {
                this.TextBox.Undo();
            }
        }

        public bool CanRedo {
            get { return this.TextBox.CanRedo; }
        }

        public void Redo() {
            if (this.CanRedo) {
                this.TextBox.Redo();
            }
        }

        #endregion

        #region Search

        public bool Find(string text, bool caseSensitive) {
            StringComparison comparisionType = default(StringComparison);
            if (caseSensitive) {
                comparisionType = StringComparison.CurrentCulture;
            } else {
                comparisionType = StringComparison.CurrentCultureIgnoreCase;
            }

            int index = this.TextBox.Text.IndexOf(text, this.TextBox.SelectionStart + this.TextBox.SelectionLength, comparisionType);
            if ((index < 0) && (this.TextBox.SelectionStart + this.TextBox.SelectionLength > 0)) {
                index = this.TextBox.Text.IndexOf(text, 0, comparisionType);
            }

            if (index >= 0) {
                this.TextBox.SelectionStart = index;
                this.TextBox.SelectionLength = text.Length;
                return true;
            } else {
                return false;
            }
        }

        public Rectangle GetSelectedRectangle() {
            Point pt1 = this.TextBox.PointToScreen(this.TextBox.GetPositionFromCharIndex(this.TextBox.SelectionStart));

            Point ptEnd = this.TextBox.PointToScreen(this.TextBox.GetPositionFromCharIndex(this.TextBox.SelectionStart + this.TextBox.SelectionLength));
            Point pt2 = default(Point);
            using (Graphics g = this.TextBox.CreateGraphics()) {
                if ((this.TextBox != null) && (this.TextBox.SelectionFont != null)) {
                    pt2 = new Point(ptEnd.X, ptEnd.Y + g.MeasureString("XXX", this.TextBox.SelectionFont).ToSize().Height);
                } else {
                    pt2 = new Point(ptEnd.X, ptEnd.Y + g.MeasureString("XXX", this.TextBox.Font).ToSize().Height);
                }
            }

            var thisRectangle = this.RectangleToScreen(this.Bounds);

            int left = Math.Max(Math.Min(pt1.X, pt2.X), thisRectangle.Left) - 32;
            int top = Math.Max(Math.Min(pt1.Y, pt2.Y), thisRectangle.Top) - 32;
            int right = Math.Min(Math.Max(pt1.X, pt2.X), thisRectangle.Right) + 32;
            int bottom = Math.Min(Math.Max(pt1.Y, pt2.Y), thisRectangle.Bottom) + 32;

            return new Rectangle(left, top, right - left, bottom - top);
        }

        #endregion

        public void UpdateTabWidth() {
            List<int> lot = new List<int>();
            for (int i = 1; i <= 32; i++) {
                lot.Add(4 * i * Settings.DisplayTabWidth);
            }
            int[] array = lot.ToArray();
            NativeMethods.SendMessage(this.TextBox.Handle, NativeMethods.EM_SETTABSTOPS, lot.Count, array[0]);
        }

        private void UpdateText() {
            base.Text = this.IsChanged ? this.Title + "*" : this.Title;
        }


        #region Zoom

        public void ZoomIn() {
            this.TextBox.ZoomFactor = (float)Math.Round(Math.Min(5.0f, this.TextBox.ZoomFactor + 0.1f), 1);
            this.TextBox.Refresh();
        }

        public void ZoomOut() {
            this.TextBox.ZoomFactor = (float)Math.Round(Math.Max(0.1f, this.TextBox.ZoomFactor - 0.1f), 1);
            this.TextBox.Refresh();
        }

        public void ZoomReset() {
            this.TextBox.ZoomFactor = 2.1f;
            this.TextBox.Refresh();
            this.TextBox.ZoomFactor = 1.0f;
            this.TextBox.Refresh();
        }

        #endregion


        public override string ToString() {
            return this.Title;
        }


        #region File loading

        private void OpenAsTxt() {
            this.TextBox.ResetText();
            var text = File.ReadAllText(this.CurrentFile.FullName, Utf8EncodingWithoutBom);
            var lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            text = string.Join("\n", lines);
            this.TextBox.Text = text;
        }

        private void SaveAsTxt() {
            using (var fileStream = new FileStream(this.CurrentFile.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) {
                var text = string.Join(Environment.NewLine, this.TextBox.Lines);
                byte[] bytes = Utf8EncodingWithoutBom.GetBytes(text);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.SetLength(bytes.Length);
            }
        }

        private void OpenAsRtf() {
            this.TextBox.LoadFile(this.CurrentFile.FullName, RichTextBoxStreamType.RichText);
        }

        private void SaveAsRtf() {
            this.TextBox.SaveFile(this.CurrentFile.FullName, RichTextBoxStreamType.RichText);
        }

        #endregion


        private class NativeMethods {

            private NativeMethods() {
            }


            internal const int EM_SETTABSTOPS = 0xcb;
            public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam) {
                var x = new IntPtr(lParam);
                return SendMessageW(hWnd, Msg, new IntPtr(wParam), ref x);
            }

            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SendMessageW")]
            public static extern IntPtr SendMessageW([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, uint Msg, IntPtr wParam, ref IntPtr lParam);

        }

    }
}
