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

        public TabFile(DocumentFile file)
            : base() {

            this.BaseFile = file;
            this.Padding = new Padding(0, SystemInformation.Border3DSize.Height, 0, 0);

            base.Text = this.Title;

            this.GotFocus += txt_GotFocus;

            if (Settings.Current.FilesPreload && (this.BaseFile.IsEncrypted == false)) {
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
            tb.BackColor = Settings.Current.DisplayBackgroundColor;
            tb.Dock = DockStyle.Fill;
            tb.Font = Settings.Current.DisplayFont;
            tb.ForeColor = Settings.Current.DisplayForegroundColor;
            tb.HideSelection = false;
            tb.MaxLength = 0;
            tb.Multiline = true;
            tb.ShortcutsEnabled = false;
            tb.DetectUrls = Settings.Current.DetectUrls;
            switch (Settings.Current.ScrollBars) {
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
            tb.WordWrap = Settings.Current.DisplayWordWrap;
            return tb;
        }


        private static readonly UTF8Encoding Utf8EncodingWithoutBom = new UTF8Encoding(false);

        public DocumentFile BaseFile { get; private set; }
        public string Title { get { return this.BaseFile.Title; } }

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


        public static TabFile Create(DocumentFolder folder, string title, DocumentKind kind) {
            var newFile = folder.NewFile(title, kind);

            if (kind == DocumentKind.RichText) {
                using (RichTextBox dummy = new RichTextBox()) {
                    dummy.BackColor = Settings.Current.DisplayBackgroundColor;
                    dummy.Font = Settings.Current.DisplayFont;
                    dummy.ForeColor = Settings.Current.DisplayForegroundColor;

                    using (var stream = new MemoryStream()) {
                        dummy.SaveFile(stream, RichTextBoxStreamType.RichText);
                        newFile.Write(stream);
                    }
                }
            }

            return new TabFile(newFile);
        }



        public void Open() {
            this.Reopen();
        }

        public void Close() {
            if (this.IsOpened) {
                if (this.TextBox != null) {
                    this.Controls.Remove(this.TextBox);
                    this.TextBox = null;
                }
                this.BaseFile.Password = null;
                this.IsOpened = false;
            }
        }

        public void ConvertToPlainText() {
            if (this.IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (this.BaseFile.IsPlainText) { throw new InvalidOperationException("File is already in plain text format."); }

            SaveAsPlain();
            this.BaseFile.ChangeStyle(DocumentStyle.PlainText);
            this.Reopen();
        }

        public void ConvertToRichText() {
            if (this.IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (this.BaseFile.IsRichText) { throw new InvalidOperationException("File is already in rich text format."); }

            string text = this.TextBox.Text;

            this.BaseFile.ChangeStyle(DocumentStyle.RichText);
            this.SaveAsRich();
            this.Reopen();
        }


        public void Encrypt(string password) {
            if (this.IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (this.BaseFile.IsEncrypted) { throw new InvalidOperationException("File is already encrypted."); }

            string text = this.TextBox.Text;

            this.BaseFile.Encrypt(password);
            this.Reopen();
        }

        public void Decrypt() {
            if (this.IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (this.BaseFile.IsEncrypted == false) { throw new InvalidOperationException("File is already decrypted."); }
            if (!this.BaseFile.HasPassword) { throw new InvalidOperationException("No decryption password found."); }

            string text = this.TextBox.Text;

            this.BaseFile.Decrypt();
            this.Save();
            this.Reopen();
        }


        public void Reopen() {
            if (this.BaseFile.IsEncrypted && !this.BaseFile.HasPassword) { throw new InvalidOperationException("No password provided."); }

            try {
                var txt = (this.TextBox != null) ? this.TextBox : GetEmptyTextBox();
                var oldSelStart = txt.SelectionStart;
                var oldSelLength = txt.SelectionLength;

                this.IsOpened = false;
                if (this.BaseFile.IsRichText) {
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
                this.TextBox.SelectionStart = oldSelStart;
                this.TextBox.SelectionLength = oldSelLength;
                this.TextBox.ClearUndo();
                this.IsChanged = false;
                this.IsOpened = true;
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
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

            if (this.BaseFile.IsRichText) {
                SaveAsRich();
            } else {
                SaveAsPlain();
            }

            this.IsChanged = false;
            base.Text = this.Title;
        }


        public void Rename(string newTitle) {
            if (newTitle == null) { throw new ArgumentNullException("newTitle", "Title cannot be null."); }
            this.BaseFile.Rename(newTitle);
            UpdateText();
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
                    this.Cut(QText.Settings.Current.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.C:
                    this.Copy(QText.Settings.Current.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.V:
                    this.Paste(QText.Settings.Current.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;


                case Keys.Control | Keys.Shift | Keys.X:
                case Keys.Shift | Keys.Delete:
                    this.Cut(true);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.Shift | Keys.C:
                case Keys.Control | Keys.Insert:
                    this.Copy(true);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.Shift | Keys.V:
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
                    if (this.BaseFile.IsPlainText || forceText) {
                        CopyTextToClipboard(this.TextBox);
                        this.TextBox.SelectedText = "";
                    } else {
                        this.TextBox.Cut();
                    }
                }
            } catch (ExternalException) {
            }
        }

        public bool CanCopy {
            get { return (this.TextBox != null) && !this.TextBox.IsSelectionEmpty; }
        }

        public void Copy(bool forceText) {
            try {
                if (this.CanCopy) {
                    if (this.BaseFile.IsPlainText || forceText) {
                        CopyTextToClipboard(this.TextBox);
                    } else {
                        this.TextBox.Copy();
                    }
                }
            } catch (ExternalException) { }
        }

        public bool CanPaste {
            get {
                try {
                    return (this.TextBox != null)
                        && (Clipboard.ContainsText(TextDataFormat.UnicodeText) || (this.BaseFile.IsRichText && Settings.Current.FullRichTextClipboard));
                } catch (ExternalException) {
                    return false;
                }
            }
        }

        public void Paste(bool forceText) {
            try {
                if (this.CanPaste) {
                    if (this.BaseFile.IsPlainText || forceText) {
                        var text = GetTextFromClipboard();
                        if (text != null) {
                            this.TextBox.SelectionFont = Settings.Current.DisplayFont;
                            this.TextBox.SelectedText = text;
                        }
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

            if (!(this.IsOpened)) {
                if (this.BaseFile.NeedsPassword && !this.BaseFile.HasPassword) { return false; }
                this.Open();
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

        public bool FindForward(string text, bool caseSensitive, int startingIndex) {
            StringComparison comparisionType = default(StringComparison);
            if (caseSensitive) {
                comparisionType = StringComparison.CurrentCulture;
            } else {
                comparisionType = StringComparison.CurrentCultureIgnoreCase;
            }

            if (!(this.IsOpened)) {
                if (this.BaseFile.NeedsPassword && !this.BaseFile.HasPassword) { return false; }
                this.Open();
            }
            int index = this.TextBox.Text.IndexOf(text, startingIndex, comparisionType);

            if ((index >= 0) && (index < int.MaxValue)) {
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

            int dotWidth = TextRenderer.MeasureText(".", Settings.Current.DisplayFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            int dotXWidth = TextRenderer.MeasureText("X.", Settings.Current.DisplayFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            int charWidth = dotXWidth - dotWidth;

            var tabs2 = new List<int>();
            for (int i = 1; i <= 32; i++) { tabs2.Add((i * charWidth) * Settings.Current.DisplayTabWidth); }

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


        public override string ToString() {
            return this.Title;
        }


        #region Open/Save

        private void OpenAsPlain(RichTextBoxEx txt) {
            using (var stream = new MemoryStream()) {
                this.BaseFile.Read(stream);
                using (var sr = new StreamReader(stream, Utf8EncodingWithoutBom)) {
                    var text = sr.ReadToEnd();
                    var lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                    txt.ResetText();
                    txt.Text = string.Join("\n", lines);
                }
            }
        }

        private void SaveAsPlain() {
            using (var stream = new MemoryStream()) {
                var text = string.Join(Settings.Current.PlainLineEndsWithLf ? "\n" : Environment.NewLine, this.TextBox.Lines);
                var bytes = Utf8EncodingWithoutBom.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
                this.BaseFile.Write(stream);
            }
        }


        private void OpenAsRich(RichTextBoxEx txt) {
            using (var stream = new MemoryStream()) {
                this.BaseFile.Read(stream);
                txt.LoadFile(stream, RichTextBoxStreamType.RichText);
            }
        }


        private void SaveAsRich() {
            using (var stream = new MemoryStream()) {
                this.TextBox.SaveFile(stream, RichTextBoxStreamType.RichText);
                this.BaseFile.Write(stream);
            }
        }


        private static void CopyTextToClipboard(RichTextBoxEx textBox) {
            var text = textBox.SelectedText.Replace("\n", Environment.NewLine);
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }

        private static string GetTextFromClipboard() {
            var text = Clipboard.GetText(TextDataFormat.UnicodeText);
            return (text != string.Empty) ? text : null;
        }

        #endregion

    }
}
