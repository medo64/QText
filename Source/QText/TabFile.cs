using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Medo.Security.Cryptography;

namespace QText {

    internal class TabFile : TabPage {

        public TabFile(string fullFileName)
            : base() {

            this.CurrentFile = new QFileInfo(fullFileName);
            this.LastSaveTime = System.DateTime.Now;
            this.Padding = new Padding(0, SystemInformation.Border3DSize.Height, 0, 0);

            base.Text = this.Title;

            this.GotFocus += txt_GotFocus;

            if (Settings.FilesPreload && (this.CurrentFile.IsEncrypted == false)) {
                this.Open();
            }
        }

        private void AddTextBox(RichTextBoxEx txt) {
            if (this.TextBox != null) { throw new InvalidOperationException("TextBox already exists"); }

            this.TextBox = txt;
            this.TextBox.ContextMenuStrip = this.ContextMenuStrip;

            this.TextBox.Enter += txt_GotFocus;
            this.TextBox.TextChanged += txt_TextChanged;
            this.TextBox.PreviewKeyDown += txt_PreviewKeyDown;

            base.Controls.Add(this.TextBox);
        }

        private static RichTextBoxEx GetEmptyTextBox() {
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
            return tb;
        }


        private static readonly UTF8Encoding Utf8EncodingWithoutBom = new UTF8Encoding(false);

        public QFileInfo CurrentFile { get; private set; }
        public string Title { get { return Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(this.CurrentFile.FullName)); } }
        public DateTime LastWriteTimeUtc { get { return this.CurrentFile.LastWriteTimeUtc; } }
        public DateTime LastSaveTime { get; private set; }

        public bool IsOpened { get; private set; }
        public string Password { get; set; }
        public bool NeedsPassword {
            get {
                return (this.CurrentFile.IsEncrypted && (this.Password == null));
            }
        }

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


        private ContextMenuStrip _contextMenuStrip;
        public override ContextMenuStrip ContextMenuStrip {
            get { return this._contextMenuStrip; }
            set {
                this._contextMenuStrip = value;
                if (this.TextBox != null) {
                    this.TextBox.ContextMenuStrip = value;
                }
            }
        }


        public static TabFile Create(string fullFileName) {
            foreach (var extension in QFileInfo.GetExtensions()) {
                var altFileName = QFileInfo.GetPathWithoutExtension(fullFileName) + extension;
                if (File.Exists(altFileName)) {
                    throw new IOException("File already exists.");
                }
            }

            if (QFileInfo.IsFileRich(fullFileName)) {
                using (RichTextBox dummy = new RichTextBox()) {
                    dummy.BackColor = Settings.DisplayBackgroundColor;
                    dummy.Font = Settings.DisplayFont;
                    dummy.ForeColor = Settings.DisplayForegroundColor;
                    dummy.SaveFile(fullFileName, RichTextBoxStreamType.RichText);
                }
            } else {
                File.WriteAllText(fullFileName, "");
            }
            return new TabFile(fullFileName);
        }



        public void Open() {
            if (this.IsOpened == false) {
                this.Reopen();
            }
        }

        public void Close() {
            if (this.IsOpened) {
                if (this.TextBox != null) {
                    this.Controls.Remove(this.TextBox);
                    this.TextBox = null;
                }
                this.Password = null;
                this.IsOpened = false;
            }
        }

        public void ConvertToPlainText() {
            if (this.IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (this.CurrentFile.IsRich == false) { throw new InvalidOperationException("File is already in plain text format."); }

            var newFile = this.CurrentFile.ChangeExtension(this.CurrentFile.IsEncrypted ? QFileInfo.Extensions.PlainEncrypted : QFileInfo.Extensions.Plain);
            Helper.MovePath(this.CurrentFile.FullName, newFile.FullName);
            this.CurrentFile = newFile;
            this.Save();
            this.Reopen();
        }

        public void ConvertToRichText() {
            if (this.IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (this.CurrentFile.IsRich) { throw new InvalidOperationException("File is already in rich text format."); }

            string text = this.TextBox.Text;

            var newFile = this.CurrentFile.ChangeExtension(this.CurrentFile.IsEncrypted ? QFileInfo.Extensions.RichEncrypted : QFileInfo.Extensions.Rich);
            Helper.MovePath(this.CurrentFile.FullName, newFile.FullName);
            this.CurrentFile = newFile;
            this.Save();
            this.Reopen();
        }


        public void Encrypt(string password) {
            if (this.IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (this.CurrentFile.IsEncrypted) { throw new InvalidOperationException("File is already encrypted."); }

            string text = this.TextBox.Text;

            var newFile = this.CurrentFile.ChangeExtension(this.CurrentFile.IsRich ? QFileInfo.Extensions.RichEncrypted : QFileInfo.Extensions.PlainEncrypted);
            Helper.MovePath(this.CurrentFile.FullName, newFile.FullName);
            this.CurrentFile = newFile;
            this.Password = password;
            this.Save();
            this.Reopen();
        }

        public void Decrypt() {
            if (this.IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (this.CurrentFile.IsEncrypted == false) { throw new InvalidOperationException("File is already decrypted."); }
            if (this.Password == null) { throw new InvalidOperationException("No decryption password found."); }

            string text = this.TextBox.Text;

            var newFile = this.CurrentFile.ChangeExtension(this.CurrentFile.IsRich ? QFileInfo.Extensions.Rich : QFileInfo.Extensions.Plain);
            Helper.MovePath(this.CurrentFile.FullName, newFile.FullName);
            this.CurrentFile = newFile;
            this.Password = null;
            this.Save();
            this.Reopen();
        }


        public void Reopen() {
            if ((this.CurrentFile.IsEncrypted) && (this.Password == null)) { throw new CryptographicException("No password provided."); }

            var txt = (this.TextBox != null) ? this.TextBox : GetEmptyTextBox();

            this.IsOpened = false;
            if (this.CurrentFile.IsRich) {
                try {
                    OpenAsRich(txt);
                } catch (ArgumentException) {
                    OpenAsPlain(txt);
                }
            } else {
                OpenAsPlain(txt);
            }

            if (this.TextBox == null) { AddTextBox(txt); }

            UpdateTabWidth();
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

            if (this.CurrentFile.IsRich) {
                SaveAsRich();
            } else {
                SaveAsPlain();
            }

            this.LastSaveTime = DateTime.Now;
            this.CurrentFile.Refresh();
            this.IsChanged = false;
            base.Text = this.Title;

            SaveCarbonCopy(null);
        }

        public void SaveCarbonCopy(IWin32Window owner) {
            SaveCarbonCopy(owner, this.CurrentFile.FullName);
        }

        public static void SaveCarbonCopy(IWin32Window owner, string fullFileName) {
            if ((Settings.CarbonCopyUse == false) || string.IsNullOrEmpty(Settings.CarbonCopyFolder)) { return; }

            try {
                var baseDirectory = new DirectoryInfo(Settings.FilesLocation).FullName;
                if (fullFileName.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase)) {
                    var sufix = fullFileName.Remove(0, baseDirectory.Length);
                    if (sufix.StartsWith("\\", StringComparison.Ordinal)) { sufix = sufix.Remove(0, 1); }
                    var newFile = new FileInfo(Path.Combine(Settings.CarbonCopyFolder, sufix));
                    if ((Directory.Exists(newFile.DirectoryName) == false) && Settings.CarbonCopyCreateFolder) {
                        Helper.CreatePath(newFile.DirectoryName);
                    }
                    File.Copy(fullFileName, newFile.FullName, true);
                } else {
                    throw new InvalidOperationException("Cannot determine base path for carbon copy.");
                }
            } catch (Exception ex) {
                if (Settings.CarbonCopyIgnoreErrors == false) {
                    var title = QFileInfo.GetFileNameWithoutExtension(fullFileName);
                    Medo.MessageBox.ShowWarning(owner, string.Format("Error making carbon copy of \"{0}\".\n\n{1}", title, ex.Message), MessageBoxButtons.OK);
                }
            }
        }

        public void Rename(string newTitle) {
            if (newTitle == null) { throw new ArgumentNullException("newTitle", "Title cannot be null."); }
            newTitle = newTitle.Trim();
            if (newTitle == null) { throw new ArgumentException("Title cannot be empty.", "newTitle"); }

            var oldInfo = new QFileInfo(this.CurrentFile.FullName);
            var newInfo = oldInfo.ChangeName(Helper.EncodeFileName(newTitle));

            if (string.Equals(this.Title, newTitle, StringComparison.OrdinalIgnoreCase) == false) {
                if (QFileInfo.IsNameAlreadyTaken(newInfo.DirectoryName, newInfo.Name)) {
                    throw new IOException("File already exists.");
                }
            }

            Helper.MovePath(oldInfo.FullName, newInfo.FullName);
            this.CurrentFile = newInfo;
            UpdateText();
        }

        public bool GetIsEligibleForSave(int timeout) {
            return (this.IsChanged) && (this.LastSaveTime.AddSeconds(timeout) <= DateTime.Now);
        }


        #region txt

        private void txt_GotFocus(System.Object sender, System.EventArgs e) {
            if (this.TextBox != null) {
                this.TextBox.Select();
            }
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
                    this.Undo();
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.Y:
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
            get { return (this.TextBox != null) && (this.TextBox.SelectionLength > 0); }
        }

        public void Cut(bool forceText) {
            try {
                if (this.CanCopy) {
                    if ((this.CurrentFile.IsRich == false) || forceText) {
                        Clipboard.Clear();
                        Clipboard.SetText(this.TextBox.SelectedText.Replace("\n", "\r\n"), TextDataFormat.UnicodeText);
                        this.TextBox.SelectedText = "";
                    } else {
                        this.TextBox.Cut();
                    }
                }
            } catch (ExternalException) {
            }
        }

        public bool CanCopy {
            get { return (this.TextBox != null) && (this.TextBox.SelectionLength > 0); }
        }

        public void Copy(bool forceText) {
            try {
                if (this.CanCopy) {
                    if ((this.CurrentFile.IsRich == false) || forceText) {
                        Clipboard.Clear();
                        Clipboard.SetText(this.TextBox.SelectedText.Replace("\n", "\r\n"), TextDataFormat.UnicodeText);
                    } else {
                        this.TextBox.Copy();
                    }
                }
            } catch (ExternalException) { }
        }

        public bool CanPaste {
            get {
                try {
                    return (this.TextBox != null) && Clipboard.ContainsText();
                } catch (ExternalException) {
                    return false;
                }
            }
        }

        public void Paste(bool forceText) {
            try {
                if (CanPaste) {
                    if ((this.CurrentFile.IsRich == false) || forceText) {
                        var text = Clipboard.GetText(TextDataFormat.UnicodeText);
                        this.TextBox.SelectionFont = Settings.DisplayFont;
                        this.TextBox.SelectedText = text;
                    } else {
                        this.TextBox.Paste();
                    }
                }
            } catch (ExternalException) { }
        }


        public bool CanUndo {
            get { return (this.TextBox != null) && this.TextBox.CanUndo; }
        }

        public void Undo() {
            if (this.CanUndo) {
                this.TextBox.Undo();
            }
        }

        public bool CanRedo {
            get { return (this.TextBox != null) && this.TextBox.CanRedo; }
        }

        public void Redo() {
            if (this.CanRedo) {
                this.TextBox.Redo();
            }
        }

        #endregion

        #region Font

        public bool IsTextBold {
            get { return (this.TextBox != null) && (this.TextBox.SelectionFont != null) && (this.TextBox.SelectionFont.Bold); }
        }

        public bool IsTextItalic {
            get { return (this.TextBox != null) && (this.TextBox.SelectionFont != null) && (this.TextBox.SelectionFont.Italic); }
        }

        public bool IsTextUnderline {
            get { return (this.TextBox != null) && (this.TextBox.SelectionFont != null) && (this.TextBox.SelectionFont.Underline); }
        }

        public bool IsTextStrikeout {
            get { return (this.TextBox != null) && (this.TextBox.SelectionFont != null) && (this.TextBox.SelectionFont.Strikeout); }
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
            if (this.TextBox == null) { return; }

            int dotWidth = TextRenderer.MeasureText(".", Settings.DisplayFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            int dotXWidth = TextRenderer.MeasureText("X.", Settings.DisplayFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            int charWidth = dotXWidth - dotWidth;

            var tabs2 = new List<int>();
            for (int i = 1; i <= 32; i++) { tabs2.Add((i * charWidth) * Settings.DisplayTabWidth); }

            var ss = this.TextBox.SelectionStart;
            var sl = this.TextBox.SelectionLength;
            this.TextBox.SelectAll();
            this.TextBox.SelectionTabs = tabs2.ToArray();
            this.TextBox.SelectionStart = this.TextBox.TextLength;
            this.TextBox.SelectionLength = 0;
            this.TextBox.SelectionTabs = tabs2.ToArray();
            this.TextBox.SelectionStart = ss;
            this.TextBox.SelectionLength = sl;

            this.TextBox.Refresh();
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


        #region Open/Save

        private void OpenAsPlain(RichTextBoxEx txt) {
            using (var stream = new FileStream(this.CurrentFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                if (this.CurrentFile.IsEncrypted) {
                    this.OpenAsPlainEncrypted(txt, stream);
                } else {
                    this.OpenAsPlain(txt, stream);
                }
            }
        }

        private void OpenAsPlain(RichTextBoxEx txt, Stream stream) {
            string text;
            using (var sr = new StreamReader(stream, Utf8EncodingWithoutBom)) {
                text = sr.ReadToEnd();
                var lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                text = string.Join("\n", lines);
            }
            txt.ResetText();
            txt.Text = text;
        }

        private void OpenAsPlainEncrypted(RichTextBoxEx txt, Stream stream) {
            using (var aesStream = new OpenSslAesStream(stream, this.Password, CryptoStreamMode.Read, 256, CipherMode.CBC)) {
                this.OpenAsPlain(txt, aesStream);
            }
        }


        private void SaveAsPlain() {
            using (var stream = new FileStream(this.CurrentFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                if (this.CurrentFile.IsEncrypted) {
                    SaveAsPlainEncrypted(stream);
                } else {
                    SaveAsPlain(stream);
                }
            }
        }

        private void SaveAsPlain(Stream stream) {
            var text = string.Join(Environment.NewLine, this.TextBox.Lines);
            var bytes = Utf8EncodingWithoutBom.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void SaveAsPlainEncrypted(Stream stream) {
            using (var aesStream = new OpenSslAesStream(stream, this.Password, CryptoStreamMode.Write, 256, CipherMode.CBC)) {
                SaveAsPlain(aesStream);
            }
        }


        private void OpenAsRich(RichTextBoxEx txt) {
            using (var stream = new FileStream(this.CurrentFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                if (this.CurrentFile.IsEncrypted) {
                    this.OpenAsRichEncrypted(txt, stream);
                } else {
                    this.OpenAsRich(txt, stream);
                }
            }
        }

        private void OpenAsRich(RichTextBoxEx txt, Stream stream) {
            txt.LoadFile(stream, RichTextBoxStreamType.RichText);
        }

        private void OpenAsRichEncrypted(RichTextBoxEx txt, Stream stream) {
            using (var aesStream = new OpenSslAesStream(stream, this.Password, CryptoStreamMode.Read, 256, CipherMode.CBC)) {
                var buffer = new byte[65536];
                using (var memStream = new MemoryStream()) { //because RichTextBox seeks through stream
                    while (true) {
                        var len = aesStream.Read(buffer, 0, buffer.Length);
                        if (len == 0) {
                            memStream.Position = 0;
                            break;
                        }
                        memStream.Write(buffer, 0, len);
                    }
                    this.OpenAsRich(txt, memStream);
                }
            }
        }


        private void SaveAsRich() {
            using (var stream = new FileStream(this.CurrentFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                if (this.CurrentFile.IsEncrypted) {
                    SaveAsRichEncrypted(stream);
                } else {
                    SaveAsRich(stream);
                }
            }
        }

        private void SaveAsRich(Stream stream) {
            this.TextBox.SaveFile(stream, RichTextBoxStreamType.RichText);
        }

        private void SaveAsRichEncrypted(Stream stream) {
            using (var aesStream = new OpenSslAesStream(stream, this.Password, CryptoStreamMode.Write, 256, CipherMode.CBC)) {
                SaveAsRich(aesStream);
            }
        }

        #endregion


        private static class NativeMethods {

            internal const int EM_SETTABSTOPS = 0xcb;
            public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam) {
                var x = new IntPtr(lParam);
                return SendMessageW(hWnd, Msg, new IntPtr(wParam), ref x);
            }

            [DllImportAttribute("user32.dll", EntryPoint = "SendMessageW")]
            public static extern IntPtr SendMessageW([InAttribute()] IntPtr hWnd, UInt32 Msg, IntPtr wParam, ref IntPtr lParam);

        }

    }
}
