﻿using System;
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

            this.GotFocus += txt_GotFocus;
            this.TextBox.Enter += txt_GotFocus;
            this.TextBox.TextChanged += txt_TextChanged;
            this.TextBox.PreviewKeyDown += txt_PreviewKeyDown;

            base.Controls.Add(this.TextBox);
        }


        private static readonly UTF8Encoding Utf8EncodingWithoutBom = new UTF8Encoding(false);

        public QFileInfo CurrentFile { get; private set; }
        public string Title { get { return Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(this.CurrentFile.FullName)); } }
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

        public void ConvertToPlainText() {
            if (this.IsRichTextFormat == false) { return; }

            this.Open();
            var newFile = this.CurrentFile.ChangeExtension(this.CurrentFile.IsEncrypted ? QFileInfo.Extensions.PlainEncrypted : QFileInfo.Extensions.Plain);
            Helper.MovePath(this.CurrentFile.FullName, newFile.FullName);
            this.CurrentFile = newFile;
            this.Save();
            this.Reopen();
        }

        public void ConvertToRichText() {
            if (this.IsRichTextFormat) { return; }

            this.Open();
            string text = this.TextBox.Text;

            var newFile = this.CurrentFile.ChangeExtension(this.CurrentFile.IsEncrypted ? QFileInfo.Extensions.RichEncrypted : QFileInfo.Extensions.Rich);
            Helper.MovePath(this.CurrentFile.FullName, newFile.FullName);
            this.CurrentFile = newFile;
            this.Save();
            this.Reopen();
        }

        public bool IsRichTextFormat {
            get {
                return this.CurrentFile.IsRich;
            }
        }

        public void Reopen() {
            this.IsOpened = false;
            if (this.IsRichTextFormat) {
                try {
                    OpenAsRich();
                } catch (ArgumentException) {
                    OpenAsPlain();
                }
            } else {
                OpenAsPlain();
            }
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

            if (this.IsRichTextFormat) {
                SaveAsRich();
            } else {
                SaveAsPlain();
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

                var fiBase = new QFileInfo(this.CurrentFile.FullName);
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
            get { return this.TextBox.SelectionLength > 0; }
        }

        public void Cut(bool forceText) {
            try {
                if (this.CanCopy) {
                    if ((this.IsRichTextFormat == false) || forceText) {
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
            get { return this.TextBox.SelectionLength > 0; }
        }

        public void Copy(bool forceText) {
            try {
                if (this.CanCopy) {
                    if ((this.IsRichTextFormat == false) || forceText) {
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
                        this.TextBox.SelectionFont = Settings.DisplayFont;
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

        private void OpenAsPlain() {
            using (var stream = new FileStream(this.CurrentFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                if (this.CurrentFile.IsEncrypted) {
                    OpenAsPlainEncrypted(stream);
                } else {
                    OpenAsPlain(stream);
                }
            }
        }

        private void OpenAsPlain(Stream stream) {
            string text;
            using (var sr = new StreamReader(stream, Utf8EncodingWithoutBom)) {
                text = sr.ReadToEnd();
                var lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                text = string.Join("\n", lines);
            }
            this.TextBox.ResetText();
            this.TextBox.Text = text;
        }

        private void OpenAsPlainEncrypted(Stream stream) {
            using (var aesStream = new OpenSslAesStream(stream, "", CryptoStreamMode.Read, 256, CipherMode.CBC)) {
                OpenAsPlain(aesStream);
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
            using (var aesStream = new OpenSslAesStream(stream, "", CryptoStreamMode.Write, 256, CipherMode.CBC)) {
                SaveAsPlain(aesStream);
            }
        }


        private void OpenAsRich() {
            using (var stream = new FileStream(this.CurrentFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                if (this.CurrentFile.IsEncrypted) {
                    OpenAsRichEncrypted(stream);
                } else {
                    OpenAsRich(stream);
                }
            }
        }

        private void OpenAsRich(Stream stream) {
            this.TextBox.LoadFile(stream, RichTextBoxStreamType.RichText);
        }

        private void OpenAsRichEncrypted(Stream stream) {
            using (var aesStream = new OpenSslAesStream(stream, "", CryptoStreamMode.Read, 256, CipherMode.CBC)) {
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
                    OpenAsRich(memStream);
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
            using (var aesStream = new OpenSslAesStream(stream, "", CryptoStreamMode.Write, 256, CipherMode.CBC)) {
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
