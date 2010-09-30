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

        private string _fileName;
        private DateTime _lastSaveTime;
        private static readonly UTF8Encoding Utf8EncodingWithoutBom = new UTF8Encoding(false);

        public TabFile(string fileName, ContextMenuStrip contextMenu)
            : this(fileName, contextMenu, false) {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This program is not intended to be localized.")]
        public TabFile(string fileName, ContextMenuStrip contextMenu, bool createFile)
            : base() {

            var fileInfo = new FileInfo(fileName);
            bool isRichTextFormat = string.Compare(fileInfo.Extension, ".rtf", StringComparison.OrdinalIgnoreCase) == 0;
            string fileTitle = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);

            if ((createFile)) {
                if ((File.Exists(fileName))) {
                    throw new IOException("File already exists.");
                } else if ((File.Exists(Path.Combine(Settings.FilesLocation, fileTitle + ".txt")))) {
                    throw new IOException("Title already exists.");
                } else if ((File.Exists(Path.Combine(Settings.FilesLocation, fileTitle + ".rtf")))) {
                    throw new IOException("Title already exists.");
                } else {
                    //rich text box
                    if ((isRichTextFormat)) {
                        using (RichTextBox r = new RichTextBox()) {
                            r.BackColor = Settings.DisplayBackgroundColor;
                            r.Font = Settings.DisplayFont;
                            r.ForeColor = Settings.DisplayForegroundColor;
                            r.SaveFile(fileName, RichTextBoxStreamType.RichText);
                        }
                    } else {
                        System.IO.File.WriteAllText(fileName, "");
                    }
                }
            }

            this._fileName = fileName;
            this.Title = fileTitle;
            this._lastSaveTime = System.DateTime.Now;

            base.Text = this.Title;

            var tb = new QText.RichTextBoxEx();
            tb.AcceptsTab = true;
            tb.BackColor = Settings.DisplayBackgroundColor;
            tb.ContextMenuStrip = contextMenu;
            tb.Dock = DockStyle.Fill;
            tb.Font = Settings.DisplayFont;
            tb.ForeColor = Settings.DisplayForegroundColor;
            tb.HideSelection = false;
            tb.MaxLength = 0;
            tb.Multiline = true;
            tb.ShortcutsEnabled = false;
            tb.DetectUrls = Settings.DisplayUnderlineURLs;
            switch (Settings.DisplayScrollbars) {
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


            this.TextBox.Tag = null;
            if ((Settings.FilesPreload)) {
                this.Open();
            }

            UpdateTabWidth();

            this.TextBox.ClearUndo();

            this.GotFocus += txt_GotFocus;
            this.TextBox.Enter += txt_GotFocus;
            this.TextBox.TextChanged += txt_TextChanged;
            this.TextBox.PreviewKeyDown += txt_PreviewKeyDown;

            base.Controls.Add(this.TextBox);
        }

        public RichTextBoxEx TextBox { get; private set; }

        public void Open() {
            if (this.TextBox.Tag == null) {
                this.Reopen();
            }
        }

        public void ConvertToPlainText(FileOrder fileOrder) {
            if (!this.IsRichTextFormat) { return; }

            Open();
            string oldFileName = this._fileName;
            string newFileName = Path.ChangeExtension(oldFileName, ".txt");
            File.Move(oldFileName, newFileName);
            this._fileName = newFileName;
            Save();
            Reopen();
            //fileOrder.Rename(oldFileName, newFileName); //TODO
        }

        public void ConvertToRichText(FileOrder fileOrder) {
            if (this.IsRichTextFormat) { return; }

            Open();
            string text = this.TextBox.Text;

            string oldFileName = this._fileName;
            string newFileName = Path.ChangeExtension(oldFileName, ".rtf");
            File.Move(oldFileName, newFileName);
            this._fileName = newFileName;
            Save();
            Reopen();
            //fileOrder.Rename(oldFileName, newFileName); //TODO
        }

        public bool IsRichTextFormat {
            get {
                var fileInfo = new FileInfo(this._fileName);
                return (string.Compare(fileInfo.Extension, ".rtf", StringComparison.OrdinalIgnoreCase) == 0);
            }
        }

        public void Reopen() {
            if (this.IsRichTextFormat) {
                try {
                    this.TextBox.LoadFile(this._fileName, RichTextBoxStreamType.RichText);
                } catch (System.ArgumentException) {
                    this.TextBox.Text = File.ReadAllText(this._fileName);
                }
            } else {
                this.TextBox.ResetText();
                this.TextBox.Text = File.ReadAllText(this._fileName);
            }

            this.TextBox.Tag = "";
            base.Text = this.Title;
            this.TextBox.SelectionStart = 0;
            this.TextBox.SelectionLength = 0;
            this._lastSaveTime = DateTime.Now;
        }

        public void Save() {
            if (this.TextBox.Tag == null) { return; }

            if (this.IsRichTextFormat) {
                this.TextBox.SaveFile(this._fileName, RichTextBoxStreamType.RichText);
            } else {
                using (var fileStream = new FileStream(this._fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) {
                    byte[] bytes = Utf8EncodingWithoutBom.GetBytes(this.TextBox.Text);
                    fileStream.SetLength(0);
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }

            this.TextBox.Tag = "";
            this._lastSaveTime = DateTime.Now;
            base.Text = this.Title;

            SaveCarbonCopy();
        }

        public void SaveCarbonCopy() {
            Open();
            if ((!Settings.CarbonCopyUse) || (string.IsNullOrEmpty(Settings.CarbonCopyFolder))) { return; }

            string destFile = null;
            try {
                if ((!Directory.Exists(Settings.CarbonCopyFolder)) && (Settings.CarbonCopyCreateFolder)) {
                    Helper.Path.CreatePath(Settings.CarbonCopyFolder);
                }

                var fiBase = new FileInfo(this._fileName);
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
                        Medo.MessageBox.ShowWarning(this, "Error making carbon copy from \"" + this._fileName + "\".", MessageBoxButtons.OK);
                    }
                }
            }
        }

        public void Delete() {
            if ((Settings.FilesDeleteToRecycleBin)) {
                SHFile.Delete(this._fileName);
            } else {
                File.Delete(this._fileName);
            }
            this.TextBox.Clear();
            this.TextBox.Tag = null;
            base.Controls.Clear();
        }

        public void Rename(string newTitle) {
            var oldInfo = new FileInfo(this._fileName);
            var newInfo = new FileInfo(Path.Combine(Settings.FilesLocation, newTitle) + oldInfo.Extension);

            if (File.Exists(newInfo.FullName)) {
                throw new IOException("File already exists.");
            } else if (File.Exists(System.IO.Path.Combine(Settings.FilesLocation, newTitle + ".txt"))) {
                throw new IOException("Title already exists.");
            } else if (File.Exists(System.IO.Path.Combine(Settings.FilesLocation, newTitle + ".rtf"))) {
                throw new IOException("Title already exists.");
            } else {
                File.Move(oldInfo.FullName, newInfo.FullName);
                this._fileName = newInfo.FullName;

                this.Title = newInfo.Name.Remove(newInfo.Name.Length - newInfo.Extension.Length, newInfo.Extension.Length);
                if ((this.IsChanged == true)) {
                    base.Text = this.Title + "*";
                } else {
                    base.Text = this.Title;
                }
            }
        }

        public bool GetIsEligibleForSave(int timeout) {
            return (this.IsChanged) && (this._lastSaveTime.AddSeconds(timeout) <= DateTime.Now);
        }

        public bool IsChanged {
            get {
                if (string.IsNullOrEmpty(this.TextBox.Tag as string)) {
                    return false;
                } else {
                    return true;
                }
            }
        }

        public string Title { get; private set; }

        public string FileName {
            get {
                var fileInfo = new FileInfo(this._fileName);
                return fileInfo.Name;
            }
        }

        public string FullFileName {
            get { return this._fileName; }
        }


        public void Me_GotFocus(System.Object sender, System.EventArgs e) {
            Debug.WriteLine("tab");
            this.TextBox.Focus();
        }

        private void txt_GotFocus(System.Object sender, System.EventArgs e) {
            this.Open();
            this.TextBox.Select();
        }

        private void txt_TextChanged(System.Object sender, System.EventArgs e) {
            if ((this.TextBox.Tag == null) || (string.IsNullOrEmpty(this.TextBox.Tag.ToString()))) {
                this.TextBox.Tag = "*";
                base.Text = this.Title + "*";
            }
        }

        private void txt_PreviewKeyDown(System.Object sender, System.Windows.Forms.PreviewKeyDownEventArgs e) {
            Debug.WriteLine("TabFile.PreviewKeyDown: " + e.KeyData.ToString());
            switch (e.KeyData) {

                case Keys.Control | Keys.X:
                case Keys.Shift | Keys.Delete:
                    this.Cut(QText.Settings.ForceTextCopyPaste);
                    e.IsInputKey = false;

                    break;
                case Keys.Control | Keys.Alt | Keys.X:
                    this.Cut(true);
                    e.IsInputKey = false;

                    break;
                case Keys.Control | Keys.C:
                case Keys.Control | Keys.Insert:
                    this.Copy(QText.Settings.ForceTextCopyPaste);
                    e.IsInputKey = false;

                    break;
                case Keys.Control | Keys.Alt | Keys.C:
                    this.Copy(true);
                    e.IsInputKey = false;

                    break;
                case Keys.Control | Keys.V:
                case Keys.Shift | Keys.Insert:
                    this.Paste(QText.Settings.ForceTextCopyPaste);
                    e.IsInputKey = false;

                    break;
                case Keys.Control | Keys.Alt | Keys.V:
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


        public void Cut(bool forceText) {
            try {
                if (this.CanCopy) {
                    if ((!this.IsRichTextFormat) || forceText) {
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
                    if (!this.IsRichTextFormat || forceText) {
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
                    if (!this.IsRichTextFormat || forceText) {
                        this.TextBox.Paste(DataFormats.GetFormat(DataFormats.UnicodeText));
                    } else {
                        this.TextBox.Paste();
                    }
                }
            } catch (ExternalException) { }
        }

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

        public void Undo() {
            if (this.CanUndo) {
                this.TextBox.Undo();
            }
        }

        public void Redo() {
            if (this.CanRedo) {
                this.TextBox.Redo();
            }
        }

        public bool CanUndo {
            get { return this.TextBox.CanUndo; }
        }

        public bool CanRedo {
            get { return this.TextBox.CanRedo; }
        }

        public void UpdateTabWidth() {
            List<int> lot = new List<int>();
            for (int i = 1; i <= 32; i++) {
                lot.Add(4 * i * Settings.DisplayTabWidth);
            }
            int[] array = lot.ToArray();
            NativeMethods.SendMessage(this.TextBox.Handle, NativeMethods.EM_SETTABSTOPS, lot.Count, array[0]);
        }

        public void ZoomIn() {
            this.TextBox.ZoomFactor = (float)Math.Round(Math.Min(5.0f, this.TextBox.ZoomFactor + 0.1f), 1);
            this.TextBox.Refresh();
        }

        public void ZoomOut() {
            this.TextBox.ZoomFactor = (float)Math.Round(Math.Max(0.1f, this.TextBox.ZoomFactor - 0.1f), 1);
            this.TextBox.Refresh();
        }

        public void ZoomReset() {
            this.TextBox.ZoomFactor = 2.1f; //TODO: Blog about this
            this.TextBox.Refresh();
            this.TextBox.ZoomFactor = 1.0f;
            this.TextBox.Refresh();
        }


        public override string ToString() {
            return this.Title;
        }


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
