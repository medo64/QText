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

            BaseFile = file;
            Padding = new Padding(0, SystemInformation.Border3DSize.Height, 0, 0);

            base.Text = Title;

            GotFocus += txt_GotFocus;

            if (Settings.Current.FilesPreload && (BaseFile.IsEncrypted == false)) {
                Open();
            }
        }

        private void AddTextBox(RichTextBoxEx txt) {
            if (TextBox != null) { throw new InvalidOperationException("TextBox already exists"); }

            TextBox = txt;
            TextBox.ContextMenuStrip = ContextMenuStrip;

            TextBox.Enter += txt_GotFocus;
            TextBox.TextChanged += txt_TextChanged;
            TextBox.PreviewKeyDown += txt_PreviewKeyDown;

            base.Controls.Add(TextBox);
        }

        private static RichTextBoxEx GetEmptyTextBox() {
            var tb = new RichTextBoxEx {
                AcceptsTab = true,
                BackColor = Settings.Current.DisplayBackgroundColor,
                Dock = DockStyle.Fill,
                Font = Settings.Current.DisplayFont,
                ForeColor = Settings.Current.DisplayForegroundColor,
                HideSelection = false,
                MaxLength = 0,
                Multiline = true,
                ShortcutsEnabled = false,
                DetectUrls = Settings.Current.DetectUrls
            };
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
        public string Title { get { return BaseFile.Title; } }

        public bool IsOpened { get; private set; }

        private bool _isChanged;
        public bool IsChanged {
            get {
                return _isChanged && IsOpened; //only open file can be changed
            }
            private set {
                _isChanged = value;
                UpdateText();
            }
        }

        public RichTextBoxEx TextBox { get; private set; }


        private ContextMenuStrip _contextMenuStrip;
        public override ContextMenuStrip ContextMenuStrip {
            get { return _contextMenuStrip; }
            set {
                _contextMenuStrip = value;
                if (TextBox != null) {
                    TextBox.ContextMenuStrip = value;
                }
            }
        }


        public static TabFile Create(DocumentFolder folder, string title, DocumentKind kind) {
            var newFile = folder.NewFile(title, kind);

            if (kind == DocumentKind.RichText) {
                using (var dummy = new RichTextBox()) {
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
            Reopen();
        }

        public void Close() {
            if (IsOpened) {
                if (TextBox != null) {
                    Controls.Remove(TextBox);
                    TextBox = null;
                }
                BaseFile.Password = null;
                IsOpened = false;
            }
        }

        public void ConvertToPlainText() {
            if (IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (BaseFile.IsPlainText) { throw new InvalidOperationException("File is already in plain text format."); }

            SaveAsPlain();
            BaseFile.ChangeStyle(DocumentStyle.PlainText);
            Reopen();
        }

        public void ConvertToRichText() {
            if (IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (BaseFile.IsRichText) { throw new InvalidOperationException("File is already in rich text format."); }

            var text = TextBox.Text;

            BaseFile.ChangeStyle(DocumentStyle.RichText);
            SaveAsRich();
            Reopen();
        }


        public void Encrypt(string password) {
            if (IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (BaseFile.IsEncrypted) { throw new InvalidOperationException("File is already encrypted."); }

            var text = TextBox.Text;

            BaseFile.Encrypt(password);
            Reopen();
        }

        public void Decrypt() {
            if (IsOpened == false) { throw new InvalidOperationException("File is not loaded."); }
            if (BaseFile.IsEncrypted == false) { throw new InvalidOperationException("File is already decrypted."); }
            if (!BaseFile.HasPassword) { throw new InvalidOperationException("No decryption password found."); }

            var text = TextBox.Text;

            BaseFile.Decrypt();
            Save();
            Reopen();
        }


        public void Reopen() {
            if (BaseFile.IsEncrypted && !BaseFile.HasPassword) { throw new InvalidOperationException("No password provided."); }

            try {
                var txt = TextBox ?? GetEmptyTextBox();
                var oldSelStart = txt.SelectionStart;
                var oldSelLength = txt.SelectionLength;

                IsOpened = false;
                if (BaseFile.IsRichText) {
                    try {
                        OpenAsRich(txt);
                    } catch (ArgumentException) {
                        OpenAsPlain(txt);
                    }
                } else {
                    OpenAsPlain(txt);
                }

                if (TextBox == null) { AddTextBox(txt); }

                UpdateTabWidth();
                TextBox.SelectionStart = oldSelStart;
                TextBox.SelectionLength = oldSelLength;
                TextBox.ClearUndo();
                IsChanged = false;
                IsOpened = true;
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public void QuickSaveWithoutException() {
            try {
                Save();
            } catch (Exception) { }
        }

        private int QuickSaveFailedCounter;

        public void QuickSave() { //allow for three failed  attempts
            try {
                Save();
                QuickSaveFailedCounter = 0;
            } catch (Exception) {
                QuickSaveFailedCounter += 1;
                if (QuickSaveFailedCounter == 4) {
                    throw;
                }
            }
        }

        public void Save() {
            if (IsOpened == false) { return; }

            if (BaseFile.IsRichText) {
                SaveAsRich();
            } else {
                SaveAsPlain();
            }

            IsChanged = false;
            base.Text = Title;
        }


        public void Rename(string newTitle) {
            if (newTitle == null) { throw new ArgumentNullException("newTitle", "Title cannot be null."); }
            BaseFile.Rename(newTitle);
            UpdateText();
        }


        #region txt

        private void txt_GotFocus(object sender, EventArgs e) {
            if (TextBox != null) {
                TextBox.Select();
            }
        }

        private void txt_TextChanged(object sender, EventArgs e) {
            if (IsOpened && (IsChanged == false)) {
                IsChanged = true;
            }
        }

        private void txt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            Debug.WriteLine("TabFile.PreviewKeyDown: " + e.KeyData.ToString());
            switch (e.KeyData) {

                case Keys.Control | Keys.X:
                    Cut(QText.Settings.Current.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.C:
                    Copy(QText.Settings.Current.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.V:
                    Paste(QText.Settings.Current.ForceTextCopyPaste);
                    e.IsInputKey = false;
                    break;


                case Keys.Control | Keys.Shift | Keys.X:
                case Keys.Shift | Keys.Delete:
                    Cut(true);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.Shift | Keys.C:
                case Keys.Control | Keys.Insert:
                    Copy(true);
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.Shift | Keys.V:
                case Keys.Shift | Keys.Insert:
                    Paste(true);
                    e.IsInputKey = false;
                    break;


                case Keys.Control | Keys.Z:
                    Undo();
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.Y:
                    Redo();
                    e.IsInputKey = false;
                    break;

                case Keys.X:
                    e.IsInputKey = false;
                    break;

                case Keys.Control | Keys.A:
                    TextBox.SelectAll();
                    e.IsInputKey = false;
                    break;
            }
        }

        #endregion

        #region Cut/Copy/Paste/Undo/Redo

        public bool CanCut {
            get { return (TextBox != null) && (TextBox.SelectionLength > 0); }
        }

        public void Cut(bool forceText) {
            try {
                if (CanCopy) {
                    if (BaseFile.IsPlainText || forceText) {
                        CopyTextToClipboard(TextBox);
                        TextBox.SelectedText = "";
                    } else {
                        TextBox.Cut();
                    }
                }
            } catch (ExternalException) {
            }
        }

        public bool CanCopy {
            get { return (TextBox != null) && !TextBox.IsSelectionEmpty; }
        }

        public void Copy(bool forceText) {
            try {
                if (CanCopy) {
                    if (BaseFile.IsPlainText || forceText) {
                        CopyTextToClipboard(TextBox);
                    } else {
                        TextBox.Copy();
                    }
                }
            } catch (ExternalException) { }
        }

        public bool CanPaste(bool forceText = false) {
            try {
                if (TextBox != null) {
                    if (BaseFile.IsRichText) {
                        if (Settings.Current.UnrestrictedRichTextClipboard) {
                            return TextBox.CanPaste(DataFormats.GetFormat(DataFormats.UnicodeText))
                                || TextBox.CanPaste(DataFormats.GetFormat(DataFormats.Text))
                                || (TextBox.CanPaste(DataFormats.GetFormat(DataFormats.Rtf)) && !forceText)
                                || (TextBox.CanPaste(DataFormats.GetFormat(DataFormats.Bitmap)) && !forceText)
                                || (TextBox.CanPaste(DataFormats.GetFormat(DataFormats.EnhancedMetafile)) && !forceText);
                        } else {
                            return Clipboard.ContainsText(TextDataFormat.UnicodeText);
                        }
                    } else {
                        return Clipboard.ContainsText(TextDataFormat.UnicodeText);
                    }
                }
            } catch (ExternalException) { }
            return false;
        }

        public void Paste(bool forceText) {
            try {
                if (CanPaste(forceText)) {
                    if (BaseFile.IsPlainText || forceText) {
                        var text = GetTextFromClipboard();
                        if (text != null) {
                            TextBox.SelectionFont = Settings.Current.DisplayFont;
                            TextBox.SelectedText = text;
                        }
                    } else {
                        var text = GetRichTextFromClipboard(out var isRich);
                        if (text != null) {
                            if (isRich) {
                                TextBox.SelectedRtf = text;
                            } else {
                                TextBox.SelectedText = text;
                            }
                        } else if (Settings.Current.UnrestrictedRichTextClipboard) { //maybe an image
                            TextBox.Paste(); //just do raw paste
                        }
                    }
                }
            } catch (ExternalException) { }
        }


        public bool CanUndo {
            get { return (TextBox != null) && TextBox.CanUndo; }
        }

        public void Undo() {
            if (CanUndo) {
                TextBox.Undo();
            }
        }

        public bool CanRedo {
            get { return (TextBox != null) && TextBox.CanRedo; }
        }

        public void Redo() {
            if (CanRedo) {
                TextBox.Redo();
            }
        }

        #endregion

        #region Font

        public bool IsTextBold {
            get { return (TextBox != null) && (TextBox.SelectionFont != null) && (TextBox.SelectionFont.Bold); }
        }

        public bool IsTextItalic {
            get { return (TextBox != null) && (TextBox.SelectionFont != null) && (TextBox.SelectionFont.Italic); }
        }

        public bool IsTextUnderline {
            get { return (TextBox != null) && (TextBox.SelectionFont != null) && (TextBox.SelectionFont.Underline); }
        }

        public bool IsTextStrikeout {
            get { return (TextBox != null) && (TextBox.SelectionFont != null) && (TextBox.SelectionFont.Strikeout); }
        }


        public bool IsTextBulleted {
            get { return (TextBox != null) && TextBox.SelectionBullet; }
        }

        public bool IsTextNumbered {
            get { return (TextBox != null) && TextBox.SelectionNumbered; }
        }

        #endregion

        #region Search

        public bool Find(string text, bool caseSensitive) {
            var comparisionType = default(StringComparison);
            if (caseSensitive) {
                comparisionType = StringComparison.CurrentCulture;
            } else {
                comparisionType = StringComparison.CurrentCultureIgnoreCase;
            }

            if (!(IsOpened)) {
                if (BaseFile.NeedsPassword && !BaseFile.HasPassword) { return false; }
                Open();
            }
            var index = TextBox.Text.IndexOf(text, TextBox.SelectionStart + TextBox.SelectionLength, comparisionType);
            if ((index < 0) && (TextBox.SelectionStart + TextBox.SelectionLength > 0)) {
                index = TextBox.Text.IndexOf(text, 0, comparisionType);
            }

            if (index >= 0) {
                TextBox.SelectionStart = index;
                TextBox.SelectionLength = text.Length;
                return true;
            } else {
                return false;
            }
        }

        public bool FindForward(string text, bool caseSensitive, int startingIndex) {
            var comparisionType = default(StringComparison);
            if (caseSensitive) {
                comparisionType = StringComparison.CurrentCulture;
            } else {
                comparisionType = StringComparison.CurrentCultureIgnoreCase;
            }

            if (!(IsOpened)) {
                if (BaseFile.NeedsPassword && !BaseFile.HasPassword) { return false; }
                Open();
            }
            var index = TextBox.Text.IndexOf(text, startingIndex, comparisionType);

            if ((index >= 0) && (index < int.MaxValue)) {
                TextBox.SelectionStart = index;
                TextBox.SelectionLength = text.Length;
                return true;
            } else {
                return false;
            }
        }

        public Rectangle GetSelectedRectangle() {
            var pt1 = TextBox.PointToScreen(TextBox.GetPositionFromCharIndex(TextBox.SelectionStart));

            var ptEnd = TextBox.PointToScreen(TextBox.GetPositionFromCharIndex(TextBox.SelectionStart + TextBox.SelectionLength));
            var pt2 = default(Point);
            using (var g = TextBox.CreateGraphics()) {
                if ((TextBox != null) && (TextBox.SelectionFont != null)) {
                    pt2 = new Point(ptEnd.X, ptEnd.Y + g.MeasureString("XXX", TextBox.SelectionFont).ToSize().Height);
                } else {
                    pt2 = new Point(ptEnd.X, ptEnd.Y + g.MeasureString("XXX", TextBox.Font).ToSize().Height);
                }
            }

            var thisRectangle = RectangleToScreen(Bounds);

            var left = Math.Max(Math.Min(pt1.X, pt2.X), thisRectangle.Left) - 32;
            var top = Math.Max(Math.Min(pt1.Y, pt2.Y), thisRectangle.Top) - 32;
            var right = Math.Min(Math.Max(pt1.X, pt2.X), thisRectangle.Right) + 32;
            var bottom = Math.Min(Math.Max(pt1.Y, pt2.Y), thisRectangle.Bottom) + 32;

            return new Rectangle(left, top, right - left, bottom - top);
        }

        #endregion

        public void UpdateTabWidth() {
            if (TextBox == null) { return; }

            var dotWidth = TextRenderer.MeasureText(".", Settings.Current.DisplayFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            var dotXWidth = TextRenderer.MeasureText("X.", Settings.Current.DisplayFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            var charWidth = dotXWidth - dotWidth;

            var tabs2 = new List<int>();
            for (var i = 1; i <= 32; i++) { tabs2.Add((i * charWidth) * Settings.Current.DisplayTabWidth); }

            var ss = TextBox.SelectionStart;
            var sl = TextBox.SelectionLength;
            TextBox.SelectAll();
            TextBox.SelectionTabs = tabs2.ToArray();
            TextBox.SelectionStart = TextBox.TextLength;
            TextBox.SelectionLength = 0;
            TextBox.SelectionTabs = tabs2.ToArray();
            TextBox.SelectionStart = ss;
            TextBox.SelectionLength = sl;

            TextBox.Refresh();
        }

        private void UpdateText() {
            base.Text = IsChanged ? Title + "*" : Title;
        }


        public override string ToString() {
            return Title;
        }


        #region Open/Save

        private void OpenAsPlain(RichTextBoxEx txt) {
            using (var stream = new MemoryStream()) {
                BaseFile.Read(stream);
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
                var text = string.Join(Settings.Current.PlainLineEndsWithLf ? "\n" : Environment.NewLine, TextBox.Lines);
                var bytes = Utf8EncodingWithoutBom.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
                BaseFile.Write(stream);
            }
        }


        private void OpenAsRich(RichTextBoxEx txt) {
            using (var stream = new MemoryStream()) {
                BaseFile.Read(stream);
                txt.LoadFile(stream, RichTextBoxStreamType.RichText);
            }
        }


        private void SaveAsRich() {
            using (var stream = new MemoryStream()) {
                TextBox.SaveFile(stream, RichTextBoxStreamType.RichText);
                BaseFile.Write(stream);
            }
        }


        private static void CopyTextToClipboard(RichTextBoxEx textBox) {
            var text = textBox.SelectedText.Replace("\n", Environment.NewLine);
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }

        private static string GetTextFromClipboard() {
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText)) {
                return Clipboard.GetText(TextDataFormat.UnicodeText);
            } else if (Clipboard.ContainsText(TextDataFormat.Text)) {
                return Clipboard.GetText(TextDataFormat.Text);
            }

            return null;
        }

        private static string GetRichTextFromClipboard(out bool isRich) {
            if (Clipboard.ContainsText(TextDataFormat.Rtf)) {
                isRich = true;
                var richText = Clipboard.GetText(TextDataFormat.Rtf);
                return Helper.FilterRichText(richText) ?? GetTextFromClipboard();
            } else if (Clipboard.ContainsText(TextDataFormat.UnicodeText)) {
                isRich = false;
                return Clipboard.GetText(TextDataFormat.UnicodeText);
            } else if (Clipboard.ContainsText(TextDataFormat.Text)) {
                isRich = false;
                return Clipboard.GetText(TextDataFormat.Text);
            }

            isRich = false;
            return null;
        }

        #endregion

    }
}
