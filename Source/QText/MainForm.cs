using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QText {

    internal partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            tmrQuickSave.Interval = Settings.QuickSaveInterval;

            if (Settings.DisplayMinimizeMaximizeButtons) {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            } else {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            }
            this.ShowInTaskbar = Settings.DisplayShowInTaskbar;
            mnu.Renderer = Helper.ToolstripRenderer;
            Helper.ScaleToolstrip(mnu, mnxTab, mnxText);

            tabFiles.Multiline = Settings.MultilineTabs;

            Medo.Windows.Forms.State.Load(this);
        }


        internal bool SuppressMenuKey = false;

        protected override bool ProcessDialogKey(Keys keyData) {
            Debug.WriteLine("MainForm_ProcessDialogKey: " + keyData.ToString());
            if (((keyData & Keys.Alt) == Keys.Alt) && (keyData != (Keys.Alt | Keys.Menu))) { this.SuppressMenuKey = true; }

            switch (keyData) {

                case Keys.F10:
                    ToggleMenu();
                    return true;

                case Keys.Control | Keys.N:
                    mnuNew.PerformClick();
                    return true;

                case Keys.Control | Keys.R:
                    mnxTabReopen.PerformClick();
                    return true;

                case Keys.Control | Keys.S:
                    mnuSaveNow.PerformClick();
                    return true;

                case Keys.F2:
                    mnuRename.PerformClick();
                    return true;

                case Keys.Control | Keys.P:
                    mnuPrint.PerformClick();
                    return true;


                case Keys.Control | Keys.B:
                    mnuBold.PerformClick();
                    return true;

                case Keys.Control | Keys.I:
                    mnuItalic.PerformClick();
                    return true;

                case Keys.Control | Keys.U:
                    mnuUnderline.PerformClick();
                    return true;


                case Keys.Control | Keys.F:
                    mnuFindFind_Click(null, null);
                    return true;

                case Keys.F3:
                    mnuFindFindNext_Click(null, null);
                    return true;

                case Keys.Control | Keys.G:
                    mnuFindGoto_Click(null, null);
                    return true;

                case Keys.Control | Keys.T:
                    mnuAlwaysOnTop_Click(null, null);
                    return true;


                case Keys.F1:
                    mnuApp.ShowDropDown();
                    mnuAppAbout.Select();
                    return true;

                case Keys.Escape:
                    this.Close();
                    return true;


                case Keys.Alt | Keys.Apps:
                    tabFiles.ContextMenuStrip.Show(tabFiles, tabFiles.Width / 2, tabFiles.Height / 2);
                    break;


                case Keys.Alt | Keys.D1:
                    if (tabFiles.TabPages.Count >= 1) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                    }
                    return true;

                case Keys.Alt | Keys.D2:
                    if (tabFiles.TabPages.Count >= 2) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[1];
                    }
                    return true;

                case Keys.Alt | Keys.D3:
                    if (tabFiles.TabPages.Count >= 3) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[2];
                    }
                    return true;

                case Keys.Alt | Keys.D4:
                    if (tabFiles.TabPages.Count >= 4) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[3];
                    }
                    return true;

                case Keys.Alt | Keys.D5:
                    if (tabFiles.TabPages.Count >= 5) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[4];
                    }
                    return true;

                case Keys.Alt | Keys.D6:
                    if (tabFiles.TabPages.Count >= 6) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[5];
                    }
                    return true;

                case Keys.Alt | Keys.D7:
                    if (tabFiles.TabPages.Count >= 7) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[6];
                    }
                    return true;

                case Keys.Alt | Keys.D8:
                    if (tabFiles.TabPages.Count >= 8) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[7];
                    }
                    return true;

                case Keys.Alt | Keys.D9:
                    if (tabFiles.TabPages.Count >= 9) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[8];
                    }
                    return true;

                case Keys.Alt | Keys.D0:
                    if (tabFiles.TabPages.Count >= 10) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[9];
                    }
                    return true;


                case Keys.Alt | Keys.Left: {
                        if (tabFiles.SelectedTab == null) {
                            if (tabFiles.TabPages.Count > 0) {
                                tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                            }
                        } else {
                            var newIndex = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab) - 1;
                            if (newIndex >= 0) {
                                tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[newIndex];
                            }
                        }
                    } return true;


                case Keys.Alt | Keys.Right: {
                        if (tabFiles.SelectedTab == null) {
                            if (tabFiles.TabPages.Count > 0) {
                                tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[tabFiles.TabPages.Count - 1];
                            }
                        } else {
                            var newIndex = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab) + 1;
                            if (newIndex < tabFiles.TabPages.Count) {
                                tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[newIndex];
                            }
                        }
                    } return true;

                case Keys.Alt | Keys.PageUp:
                case Keys.Alt | Keys.Up: {
                        var currFolder = tabFiles.CurrentFolder;
                        var list = new List<DocumentFolder>(Document.GetFolders());
                        var index = list.FindIndex(delegate(DocumentFolder folder) { return folder.Equals(currFolder); });
                        if (index > 0) {
                            tabFiles.FolderOpen(list[index - 1]);
                            mnuFolder.Text = tabFiles.CurrentFolder.Title;
                            Settings.LastFolder = tabFiles.CurrentFolder;
                        }
                    } return true;


                case Keys.Alt | Keys.PageDown:
                case Keys.Alt | Keys.Down: {
                        var currFolder = tabFiles.CurrentFolder;
                        var list = new List<DocumentFolder>(Document.GetFolders());
                        var index = list.FindIndex(delegate(DocumentFolder folder) { return folder.Equals(currFolder); });
                        if (index < list.Count - 1) {
                            tabFiles.FolderOpen(list[index + 1]);
                            mnuFolder.Text = tabFiles.CurrentFolder.Title;
                            Settings.LastFolder = tabFiles.CurrentFolder;
                        }
                    } return true;

                case Keys.Alt | Keys.Shift | Keys.D: {
                        if ((tabFiles.SelectedTab != null) && (tabFiles.SelectedTab.IsOpened)) {
                            tabFiles.SelectedTab.TextBox.SelectedText = DateTime.Now.ToShortDateString();
                        }
                    } return true;

                case Keys.Alt | Keys.Shift | Keys.T: {
                        if ((tabFiles.SelectedTab != null) && (tabFiles.SelectedTab.IsOpened)) {
                            tabFiles.SelectedTab.TextBox.SelectedText = DateTime.Now.ToShortTimeString();
                        }
                    } return true;

            }

            return base.ProcessDialogKey(keyData);
        }


        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) {
            Debug.WriteLine("MainForm_ProcessCmdKey: " + keyData.ToString());
            switch (keyData) {

                case Keys.Shift | Keys.F1: {
                        mnxTextSelectionSpelling_Click(null, null);
                    } return true;


                case Keys.Control | Keys.Tab:
                case Keys.Control | Keys.Shift | Keys.Tab:
                    if ((tabFiles.TabPages.Count >= 1) && (tabFiles.SelectedTab == null))
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                    if ((tabFiles.TabPages.Count >= 2)) {
                        var tp = GetPreviousSelectedTab();
                        if ((tp != null)) {
                            tabFiles.SelectedTab = tp;
                        } else {
                            int i = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab);
                            i = (i + 1) % tabFiles.TabPages.Count;
                            tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[i];
                        }
                    }
                    keyData = Keys.None;
                    return true;

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            Debug.WriteLine("MainForm_OnKeyUp: " + e.KeyData.ToString());
            tmrQuickSave.Enabled = false;

            if ((e != null) && (e.KeyData == Keys.Menu)) {
                if (this.SuppressMenuKey) { this.SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyUp(e);
            }

            tmrQuickSave.Enabled = true;
        }

        private void Form_Load(object sender, EventArgs e) {
            RefreshAll(null, null);
            Form_Resize(null, null);
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
#if !DEBUG
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                this.Hide();
                App.Tray.ShowBalloonOnMinimize();
            }
#endif

            Document.WriteOrderedTitles(tabFiles);

            var failedTitles = new List<string>();
            var failedExceptions = new List<Exception>();
            foreach (TabFile file in tabFiles.TabPages) {
                try {
                    if (file.IsChanged) { file.Save(); }
                } catch (Exception ex) {
                    failedTitles.Add(file.Title);
                    failedExceptions.Add(ex);
                }
                if (file.BaseFile.IsEncrypted) {
                    file.Close(); //forget passwords
                }
            }
            tabFiles.SelectNextTab(tabFiles.SelectedTab);
            if (failedTitles.Count > 0) {
                var sb = new StringBuilder("Cannot save ");
                sb.Append((failedTitles.Count == 1) ? "file" : "files");
                sb.Append(" ");
                for (int i = 0; i < failedTitles.Count; i++) {
                    if (i != 0) {
                        sb.Append((i < failedTitles.Count - 1) ? ", " : " and");
                    }
                    sb.AppendFormat("\"{0}\"", failedTitles[i]);
                }
                sb.Append(".");
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(failedExceptions[0].Message);
                if (failedExceptions.Count > 1) {
                    sb.AppendLine();
                    sb.Append("...");
                }
                Medo.MessageBox.ShowWarning(null, sb.ToString());
            }
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            Application.Exit();
        }


        private void Form_Activated(object sender, EventArgs e) {
            this.tmrUpdateToolbar.Enabled = Settings.ShowToolbar;
            if (tabFiles == null) { return; }

            if ((tabFiles.SelectedTab != null) && (tabFiles.SelectedTab.IsOpened)) {
                tabFiles.SelectedTab.Select();
            }
        }

        private void Form_Deactivate(object sender, EventArgs e) {
            this.tmrUpdateToolbar.Enabled = false;
            tmrQuickSave_Tick(null, null);
        }


        private void Form_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                if (tabFiles.TabPages.Count > 0) {
                    var rect = tabFiles.GetTabRect(tabFiles.TabCount - 1);
                    rect.Offset(tabFiles.Left, tabFiles.Top);
                    if ((e.Y >= rect.Top) && (e.Y <= rect.Bottom) && (e.X >= rect.Right)) {
                        tabFiles.SelectedTab = null;
                        tabFiles.ContextMenuStrip.Show(tabFiles, e.X - tabFiles.Left, e.Y - tabFiles.Top);
                    }
                } else {
                    Rectangle rect = tabFiles.ClientRectangle;
                    if ((e.Y >= rect.Top) && (e.Y <= rect.Bottom)) {
                        tabFiles.ContextMenuStrip.Show(tabFiles, e.X - tabFiles.Left, e.Y - tabFiles.Top);
                    }
                }
            }
        }

        private void Form_Move(object sender, EventArgs e) {
            if (this.Visible && (this.WindowState == FormWindowState.Normal)) { Medo.Windows.Forms.State.Save(this); }
        }

        private bool _form_ResizeReentry = false;
        private void Form_Resize(object sender, EventArgs e) {
            if (_form_ResizeReentry) { return; }
            _form_ResizeReentry = true;

            if (this.WindowState != FormWindowState.Minimized) {
                if (this.Visible) { Medo.Windows.Forms.State.Save(this); }
                mnu.Visible = Settings.ShowToolbar;
            } else if (Settings.TrayOnMinimize) {
                this.Visible = false;
                this.Close();
            }

            _form_ResizeReentry = false;
        }

        private void Form_VisibleChanged(object sender, EventArgs e) {
            tmrCheckFileUpdate.Enabled = this.Visible;
            tmrQuickSave.Enabled = this.Visible;
            tmrUpdateToolbar.Enabled = this.Visible;
        }


        private void tabFiles_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                    if (tabFiles.GetTabRect(i).Contains(e.X, e.Y)) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[i];
                        break;
                    }
                }
            }
        }

        private void tabFiles_SelectedIndexChanged(object sender, EventArgs e) {
            if (tabFiles.Enabled == false) { return; }
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                foreach (TabFile file in tabFiles.TabPages) {
                    if (file.IsChanged) { file.QuickSaveWithoutException(); }
                }

                try {
                    if (tabFiles.SelectedTab.BaseFile.NeedsPassword) {
                        this.Focus();
                        if (this.Visible == false) { return; }
                        using (var frm = new PasswordForm(tabFiles.SelectedTab.Title)) {
                            frm.TopMost = this.TopMost;
                            if (frm.ShowDialog(this) == DialogResult.OK) {
                                tabFiles.SelectedTab.BaseFile.Password = frm.Password;
                                tabFiles.SelectedTab.Open();
                                SetSelectedTab(tabFiles.SelectedTab);
                            } else {
                                tabFiles.SelectedTab.BaseFile.Password = null;
                                tabFiles.SelectedTab = null;
                            }
                        }
                    } else {
                        tabFiles.SelectedTab.Open();
                        SetSelectedTab(tabFiles.SelectedTab);
                    }
                } catch (CryptographicException) {
                    Medo.MessageBox.ShowWarning(this, "File cannot be decrypted. Possible password mismatch.");
                    tabFiles.SelectedTab.BaseFile.Password = null;
                    tabFiles.SelectedTab = null;
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot open file.\n\n{0}", ex.Message));
                    tabFiles.SelectedTab = this._CurrSelectedTab;
                }
                if ((tabFiles.SelectedTab != null) && (tabFiles.SelectedTab.TextBox != null)) {
                    tabFiles.SelectedTab.TextBox.Select();
                }
                tmrQuickSave.Enabled = true;
            }
        }


        #region Menu

        private void mnuNew_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            using (FileNewForm frm = new FileNewForm(tabFiles.CurrentFolder)) {
                frm.TopMost = this.TopMost;
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    try {
                        tabFiles.AddTab(frm.Title, frm.IsRichText);
                        SetSelectedTab(tabFiles.SelectedTab);
                        tabFiles.SelectedTab.Open();
                        tabFiles.SelectedTab.TextBox.Select();
                    } catch (Exception ex) {
                        Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot create file.\n\n{0}", ex.Message));
                    }
                }
            }
            tmrQuickSave.Enabled = true;
        }

        private void mnuSaveNow_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                try {
                    tabFiles.SelectedTab.Save();
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot save file.\n\n{0}", ex.Message));
                }
            }
        }

        private void mnuRename_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                using (var frm = new FileRenameForm(tabFiles.SelectedTab.BaseFile)) {
                    frm.TopMost = this.TopMost;
                    try {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            tabFiles.SelectedTab.Rename(frm.NewTitle);
                            Document.WriteOrderedTitles(tabFiles);
                        }
                    } catch (Exception ex) {
                        Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot rename file.\n\n{0}", ex.Message));
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }


        private void mnuPrint_Click(object sender, EventArgs e) {
            SaveAllChanged();
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                Print(tabFiles.SelectedTab);
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuPrintPreview_Click(object sender, EventArgs e) {
            SaveAllChanged();
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                Print(tabFiles.SelectedTab, true);
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuPrintSetup_Click(object sender, EventArgs e) {
            var pageSettings = new PageSettings();
            try { pageSettings.PaperSize.PaperName = Settings.PrintPaperName; } catch (ArgumentException) { }
            try { pageSettings.PaperSource.SourceName = Settings.PrintPaperSource; } catch (ArgumentException) { }
            pageSettings.Landscape = Settings.PrintIsPaperLandscape;
            pageSettings.Margins = Settings.PrintMargins;

            using (var frm = new PageSetupDialog() { PageSettings = pageSettings }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    Settings.PrintPaperName = pageSettings.PaperSize.PaperName;
                    Settings.PrintPaperSource = pageSettings.PaperSource.SourceName;
                    Settings.PrintIsPaperLandscape = pageSettings.Landscape;
                    Settings.PrintMargins = pageSettings.Margins;
                }
            }
        }


        private void mnuCut_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Cut(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation could not be completed.\n\n{0}", ex.Message));
            }
            tmrQuickSave.Enabled = true;
        }

        private void mnuCopy_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Copy(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation could not be completed.\n\n{0}", ex.Message));
            }
            tmrQuickSave.Enabled = true;
        }

        private void mnuPaste_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Paste(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation could not be completed.\n\n{0}", ex.Message));
            }
            tmrQuickSave.Enabled = true;
        }


        private void mnuFont_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                TabFile tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    using (var f = new System.Windows.Forms.FontDialog()) {
                        f.AllowScriptChange = true;
                        f.AllowSimulations = true;
                        f.AllowVectorFonts = true;
                        if ((tf.TextBox.SelectionFont != null)) {
                            f.Color = tf.TextBox.SelectionColor;
                            f.Font = tf.TextBox.SelectionFont;
                        } else {
                            var selLength = tf.TextBox.SelectionLength;
                            tf.TextBox.SelectionLength = 1;
                            f.Color = tf.TextBox.SelectionColor;
                            f.Font = tf.TextBox.SelectionFont;
                            tf.TextBox.SelectionLength = selLength;
                        }
                        f.ShowColor = true;
                        f.ShowEffects = true;
                        if (f.ShowDialog(this) == DialogResult.OK) {
                            tf.TextBox.SelectionColor = f.Color;
                            tf.TextBox.SelectionFont = f.Font;
                        }
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuBold_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                TabFile tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    ToogleStyle(tf.TextBox, FontStyle.Bold);
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuItalic_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                TabFile tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    ToogleStyle(tf.TextBox, FontStyle.Italic);
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuUnderline_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                TabFile tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    if (tf.TextBox.SelectionFont != null) {
                        ToogleStyle(tf.TextBox, FontStyle.Underline);
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuStrikeout_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                TabFile tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    if (tf.TextBox.SelectionFont != null) {
                        ToogleStyle(tf.TextBox, FontStyle.Strikeout);
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }


        private void mnuUndo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                tabFiles.SelectedTab.Undo();
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuRedo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                tabFiles.SelectedTab.Redo();
                tmrQuickSave.Enabled = true;
            }
        }


        private void mnuFind_ButtonClick(object sender, EventArgs e) {
            mnuFindFind_Click(null, null);
        }

        private void mnuFind_DropDownOpening(object sender, EventArgs e) {
            mnuFindFindNext.Enabled = !string.IsNullOrEmpty(SearchStatus.Text);
        }

        private void mnuFindFind_Click(object sender, EventArgs e) {
            FindFirst();
        }

        private void mnuFindFindNext_Click(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(SearchStatus.Text)) {
                Search.FindNext(this, this.tabFiles, this.tabFiles.SelectedTab);
            }
        }

        private void mnuFindGoto_Click(object sender, EventArgs e) {
            bool hasText = false;
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                hasText = tf.IsOpened;
            }

            using (var frm = new GotoForm(hasText)) {
                frm.TopMost = this.TopMost;
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    var destination = frm.SelectedItem;
                    if (destination.IsLineNumber) {
                        if (tabFiles.SelectedTab != null) {
                            TabFile tf = tabFiles.SelectedTab;
                            var index = tf.TextBox.GetFirstCharIndexFromLine(destination.LineNumber.Value - 1);
                            if (index == -1) { //go to last line
                                var lastLine = tf.TextBox.GetLineFromCharIndex(tf.TextBox.TextLength);
                                index = tf.TextBox.GetFirstCharIndexFromLine(lastLine);
                            }
                            tf.TextBox.SelectionStart = index;
                            tf.TextBox.SelectionLength = 0;
                        }
                    } else if (destination.IsFile) {
                        if (TryFolderSave()) {
                            var oldFolder = tabFiles.CurrentFolder;
                            var newFolder = destination.Folder;
                            FolderChange(oldFolder, newFolder);
                            foreach (TabFile tab in this.tabFiles.TabPages) {
                                if (string.Equals(tab.Title, destination.File.Title)) {
                                    tabFiles.SelectedTab = tab;
                                    break;
                                }
                            }
                        }
                    } else if (frm.SelectedItem.IsFolder) {
                        if (TryFolderSave()) {
                            var oldFolder = tabFiles.CurrentFolder;
                            var newFolder = destination.Folder;
                            FolderChange(oldFolder, newFolder);
                        }
                    }
                }
            }
        }



        private void mnuAlwaysOnTop_Click(object sender, EventArgs e) {
            mnuAlwaysOnTop.Checked = !mnuAlwaysOnTop.Checked;
            Settings.DisplayAlwaysOnTop = mnuAlwaysOnTop.Checked;
            this.TopMost = Settings.DisplayAlwaysOnTop;
        }


        private void mnuFolder_DropDownOpening(object sender, EventArgs e) {
            mnuFolder.DropDownItems.Clear();

            foreach (var folder in Document.GetFolders()) {
                var item = new ToolStripMenuItem(folder.Title, null, mnuFolder_Click) { Tag = folder };
                item.Enabled = !folder.Equals(tabFiles.CurrentFolder);
                mnuFolder.DropDownItems.Add(item);
            }

            mnuFolder.DropDownItems.Add(new ToolStripSeparator());

            mnuFolder.DropDownItems.Add(new ToolStripMenuItem("Edit folders", null, mnuFolderEdit_Click));
            mnuFolder.DropDownItems.Add(new ToolStripMenuItem("Edit files", null, mnuFilesEdit_Click));
        }

        private void mnuFolder_Click(object sender, EventArgs e) {
            if (TryFolderSave()) {
                var oldFolder = tabFiles.CurrentFolder;
                var newFolder = (DocumentFolder)(((ToolStripMenuItem)sender).Tag);
                FolderChange(oldFolder, newFolder);
            }
        }

        private void mnuFolderEdit_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            tabFiles.Enabled = false;
            tabFiles.FolderSave();
            using (var frm = new FolderEditForm(tabFiles.CurrentFolder)) {
                frm.TopMost = this.TopMost;
                frm.ShowDialog(this);
                if (!tabFiles.CurrentFolder.Equals(frm.CurrentFolder)) {
                    tabFiles.Enabled = true;
                    tabFiles.FolderOpen(frm.CurrentFolder, false);
                    mnuFolder.Text = tabFiles.CurrentFolder.Title;
                    Settings.LastFolder = tabFiles.CurrentFolder;
                }
            }
            tabFiles.Enabled = true;
            if (tabFiles.SelectedTab != null) { tabFiles.SelectedTab.Select(); }
            tmrQuickSave.Enabled = true;
        }

        private void mnuFilesEdit_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            tabFiles.Enabled = false;
            tabFiles.FolderSave();
            using (var frm = new FilesEditForm(tabFiles)) {
                frm.TopMost = this.TopMost;
                frm.ShowDialog(this);
            }
            tabFiles.FolderSave();
            tabFiles.Enabled = true;
            if (tabFiles.SelectedTab != null) { tabFiles_SelectedIndexChanged(null, null); }
            tmrQuickSave.Enabled = true;
        }


        private void mnuAppOptions_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            using (var frm = new OptionsForm()) {
                frm.TopMost = this.TopMost;
                SaveAllChanged();
                this.tmrUpdateToolbar.Enabled = false;
                RefreshAll(null, null);
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    if (Settings.DisplayMinimizeMaximizeButtons) {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    } else {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                    }
                    this.ShowInTaskbar = Settings.DisplayShowInTaskbar;
                    tabFiles.Multiline = Settings.MultilineTabs;
                    this.TopMost = Settings.DisplayAlwaysOnTop;
                    RefreshAll(null, null);
                    Form_Resize(null, null);
                    this.tmrUpdateToolbar.Enabled = Settings.ShowToolbar;
                    tmrQuickSave.Interval = Settings.QuickSaveInterval;
                    tabFiles.SaveCarbonCopies(this);
                }
            }
            tmrQuickSave.Enabled = true;
        }


        private void mnuAppFeedback_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            Medo.Diagnostics.ErrorReport.TopMost = this.TopMost;
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("http://jmedved.com/feedback/"));
            tmrQuickSave.Enabled = true;
        }

        private void mnuAppUpgrade_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            Medo.Services.Upgrade.ShowDialog(this, new Uri("http://jmedved.com/upgrade/"));
            tmrQuickSave.Enabled = true;
        }

        private void mnuAppDonate_Click(object sender, EventArgs e) {
            Process.Start("http://www.jmedved.com/donate/");
        }

        private void mnuAppAbout_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("http://www.jmedved.com/qtext/"));

            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.TextBox.Select();
            }
            tmrQuickSave.Enabled = true;
        }

        #endregion


        #region Tabs menu

        private void mnxTab_Opening(object sender, CancelEventArgs e) {
            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRich = isTabSelected && tabFiles.SelectedTab.BaseFile.IsRichText;
            bool isTabPlain = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsRichText == false);
            bool isTabEncryptable = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsEncrypted == false);
            bool isTabDecryptable = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsEncrypted);
            bool isZoomResetable = isTabSelected && tabFiles.SelectedTab.TextBox.HasZoom;

            mnxTabReopen.Enabled = isTabSelected;
            mnxTabSaveNow.Enabled = isTabSelected;
            mnxTabDelete.Enabled = isTabSelected;
            mnxTabRename.Enabled = isTabSelected;
            mnxTabConvert.Visible = isTabRich || isTabPlain || isTabEncryptable || isTabDecryptable;
            mnxTabZoomReset.Visible = isZoomResetable;
            mnxTabConvertPlain.Visible = isTabRich;
            mnxTabConvertRich.Visible = isTabPlain;
            mnxTabEncrypt.Visible = isTabEncryptable;
            mnxTabChangePassword.Visible = isTabDecryptable;
            mnxTabDecrypt.Visible = isTabDecryptable;
            mnxTabMoveTo.Enabled = isTabSelected;
            mnxTabPrintPreview.Enabled = isTabSelected;
            mnxTabPrint.Enabled = isTabSelected;
        }

        private void mnxTab_Closed(object sender, ToolStripDropDownClosedEventArgs e) {
            mnxTab.Tag = null;
        }


        private void mnxTabReopen_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsChanged) {
                    switch (Medo.MessageBox.ShowQuestion(this, "File is not saved. Are you sure?", global::Medo.Reflection.EntryAssembly.Title + ": Reload", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2)) {
                        case System.Windows.Forms.DialogResult.Yes:
                            try {
                                tf.Reopen();
                            } catch (Exception ex) {
                                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot open file.\n\n{0}", ex.Message));
                            }
                            break;

                        case System.Windows.Forms.DialogResult.No:
                            break;
                    }
                } else {
                    tabFiles.SelectedTab.Reopen();
                }
            }
        }

        private void mnxTabDelete_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                Helper.DeleteTabFile(this, tabFiles, tabFiles.SelectedTab);
            }
        }

        private void mnxTabMoveTo_DropDownOpening(object sender, EventArgs e) {
            mnxTabMoveTo.DropDownItems.Clear();

            foreach (var folder in Document.GetFolders()) {
                var item = new ToolStripMenuItem(folder.Title, null, mnxTabMoveTo_Click) { Tag = folder };
                item.Enabled = !folder.Equals(tabFiles.CurrentFolder);
                mnxTabMoveTo.DropDownItems.Add(item);
            }
        }

        private void mnxTabMoveTo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var folder = (DocumentFolder)((ToolStripMenuItem)sender).Tag;
                string oldPath, newPath;
                tabFiles.MoveTabPreview(tabFiles.SelectedTab, folder.Name, out oldPath, out newPath);
                if (File.Exists(newPath)) {
                    Medo.MessageBox.ShowError(this, "File already exists at destination.");
                } else {
                    tabFiles.MoveTab(tabFiles.SelectedTab, folder.Name);
                }
            }
        }


        private void mnxTabZoomReset_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.TextBox.ZoomReset();
            }
        }

        private void mnxTabConvertPlain_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                if (Medo.MessageBox.ShowQuestion(this, "Conversion will remove all formating (font, style, etc.). Do you want to continue?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    tabFiles.SelectedTab.ConvertToPlainText();
                }
            }
        }

        private void mnxTabConvertRich_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.ConvertToRichText();
            }
        }

        private void mnxTabEncrypt_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                using (var frm = new ChangePasswordForm(this.Text)) {
                    frm.TopMost = this.TopMost;
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        tabFiles.SelectedTab.Encrypt(frm.Password);
                    }
                }
            }
        }

        private void mnxTabChangePassword_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                using (var frm = new ChangePasswordForm(this.Text)) {
                    frm.TopMost = this.TopMost;
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        tabFiles.SelectedTab.BaseFile.Password = frm.Password;
                        tabFiles.SelectedTab.Save(); ;
                    }
                }
            }
        }

        private void mnxTabDecrypt_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.Decrypt();
            }
        }

        private void mnxTabOpenContainingFolder_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var exe = new ProcessStartInfo("explorer.exe", "/select,\"" + tabFiles.SelectedTab.BaseFile.Info.FullName + "\"");
                Process.Start(exe);
            } else {
                var exe = new ProcessStartInfo("explorer.exe", "\"" + tabFiles.CurrentFolder.Info.FullName + "\"");
                Process.Start(exe);
            }
        }

        #endregion


        #region Text menu

        private void mnxText_Opening(object sender, CancelEventArgs e) {
            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.BaseFile.IsRichText;
            bool isTabPlainText = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsRichText == false);
            bool isTextSelected = isTabSelected && (tabFiles.SelectedTab.TextBox.SelectedText.Length > 0);
            bool hasText = isTabSelected && (tabFiles.SelectedTab.TextBox.Text.Length > 0);
            bool isZoomResetable = isTabSelected && (tabFiles.SelectedTab.TextBox.ZoomFactor != 1);

            mnxTextUndo.Enabled = isTabSelected && tabFiles.SelectedTab.CanUndo;
            mnxTextRedo.Enabled = isTabSelected && tabFiles.SelectedTab.CanRedo;

            mnxTextCut.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnxTextCopy.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnxTextPaste.Enabled = isTabSelected && tabFiles.SelectedTab.CanPaste;

            mnxTextCutPlain.Visible = isTabRichText && (Settings.ForceTextCopyPaste == false);
            mnxTextCopyPlain.Visible = isTabRichText && (Settings.ForceTextCopyPaste == false);
            mnxTextPastePlain.Visible = isTabRichText && (Settings.ForceTextCopyPaste == false);
            mnxTextBoxCutCopyPasteAsTextSeparator.Visible = isTabRichText && (Settings.ForceTextCopyPaste == false);
            mnxTextCutPlain.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnxTextCopyPlain.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnxTextPastePlain.Enabled = isTabSelected && tabFiles.SelectedTab.CanPaste;

            mnxTextSelectAll.Enabled = isTabSelected && (tabFiles.SelectedTab.Text.Length > 0);

            mnxTextFont.Visible = isTabRichText;
            mnxTextBold.Visible = isTabRichText;
            mnxTextItalic.Visible = isTabRichText;
            mnxTextUnderline.Visible = isTabRichText;
            mnxTextStrikeout.Visible = isTabRichText;
            mnxTextRtfSeparator.Visible = isTabRichText;

            mnxTextSelectionLower.Enabled = isTextSelected;
            mnxTextSelectionUpper.Enabled = isTextSelected;
            mnxTextSelectionTitle.Enabled = isTextSelected;
            mnxTextSelectionDrGrammar.Enabled = isTextSelected;

            mnxTextLinesSortAsc.Enabled = hasText;
            mnxTextLinesSortDesc.Enabled = hasText;
            mnxTextLines.Enabled = mnxTextLinesSortAsc.Enabled || mnxTextLinesSortDesc.Enabled;
        }

        private void mnxTextCutPlain_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Cut(true);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation could not be completed.\n\n{0}", ex.Message));
            }
        }

        private void mnxTextCopyPlain_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Copy(true);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation could not be completed.\n\n{0}", ex.Message));
            }
        }

        private void mnxTextPastePlain_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Paste(true);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation could not be completed.\n\n{0}", ex.Message));
            }
        }

        private void mnxTextSelectAll_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var txt = tabFiles.SelectedTab.TextBox;
                txt.SelectAll();
            }
        }


        private void mnxTextSelectionLower_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = txt.SelectedText.ToLower();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextSelectionUpper_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = txt.SelectedText.ToUpper();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextSelectionTitle_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToLower(txt.SelectedText));

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextSelectionDrGrammar_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                System.Text.StringBuilder sb = new System.Text.StringBuilder(txt.SelectedText.ToLower());
                int startOfWord = -1;
                for (int i = 0; i <= sb.Length; i++) {
                    if ((i < sb.Length) && (char.IsLetter(sb[i]) || (sb[i] == '\''))) {
                        if (startOfWord == -1) { startOfWord = i; }
                    } else {
                        if (startOfWord > -1) {
                            string word = txt.SelectedText.Substring(startOfWord, i - startOfWord).ToLower();
                            switch (word) {
                                case "a":
                                case "an":
                                case "the":
                                case "in":
                                case "of":
                                case "to":
                                case "and":
                                case "but":
                                    break;
                                default:
                                    sb[startOfWord] = char.ToUpper(sb[startOfWord]);
                                    break;
                            }
                            startOfWord = -1;
                        }
                    }
                }

                txt.SelectedText = sb.ToString();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextSelectionSpelling_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                using (var frm = new SpellingForm(tabFiles.SelectedTab.TextBox.SelectedText.Replace('\n', ' '))) {
                    frm.TopMost = this.TopMost;
                    frm.ShowDialog(this);
                }
            } else {
                using (var frm = new SpellingForm()) {
                    frm.TopMost = this.TopMost;
                    frm.ShowDialog(this);
                }
            }
        }


        private void mnxTextLinesSortAsc_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var txt = tabFiles.SelectedTab.TextBox;
                txt.SelectLineBlock();

                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                var text = txt.SelectedText;
                bool addLf = false;
                if (text.EndsWith("\n")) {
                    addLf = true;
                    text = text.Remove(text.Length - 1);
                }
                string[] selSplit = text.Split(new string[] { "\n" }, StringSplitOptions.None);
                Array.Sort(selSplit, new SortAZ());
                text = string.Join("\n", selSplit);
                if (addLf) { text += "\n"; }
                txt.SelectedText = text;

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextLinesSortDesc_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var txt = tabFiles.SelectedTab.TextBox;
                txt.SelectLineBlock();

                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                var text = txt.SelectedText;
                bool addLf = false;
                if (text.EndsWith("\n")) {
                    addLf = true;
                    text = text.Remove(text.Length - 1);
                }
                string[] selSplit = text.Split(new string[] { "\n" }, StringSplitOptions.None);
                Array.Sort(selSplit, new SortZA());
                text = string.Join("\n", selSplit);
                if (addLf) { text += "\n"; }
                txt.SelectedText = text;

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }


        private void mnxTextLinesTrim_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var txt = tabFiles.SelectedTab.TextBox;
                txt.SelectLineBlock();

                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                var text = txt.SelectedText;
                bool addLf = false;
                if (text.EndsWith("\n")) {
                    addLf = true;
                    text = text.Remove(text.Length - 1);
                }
                string[] selSplit = text.Split(new string[] { "\n" }, StringSplitOptions.None);
                var lenDiff = 0;
                for (int i = 0; i < selSplit.Length; i++) {
                    var beforeLen = selSplit[i].Length;
                    selSplit[i] = selSplit[i].Trim();
                    var afterLen = selSplit[i].Length;
                    lenDiff += afterLen - beforeLen;
                }
                text = string.Join("\n", selSplit);
                if (addLf) { text += "\n"; }
                txt.SelectedText = text;

                txt.SelectionStart = ss;
                txt.SelectionLength = sl + lenDiff;
            }
        }

        #endregion


        #region Helpers

        private TabFile _CurrSelectedTab;
        private TabFile _PrevSelectedTab;

        private void SetSelectedTab(TabFile tabPage) {
            this._PrevSelectedTab = this._CurrSelectedTab;
            this._CurrSelectedTab = tabPage;
        }

        private TabFile GetPreviousSelectedTab() {
            if ((this._PrevSelectedTab != null) && (!this._PrevSelectedTab.Equals(this._CurrSelectedTab))) {
                if (tabFiles.TabPages.Contains(this._PrevSelectedTab)) {
                    return this._PrevSelectedTab;
                }
            }
            return null;
        }

        private void SaveAllChanged() {
            try {
                tabFiles.FolderSave();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot save files.\n\n{0}", ex.Message));
            }
        }

        private void RefreshAll(object sender, EventArgs e) {
            bool unsaved = false;
            for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                TabFile tf = (TabFile)tabFiles.TabPages[i];
                if (tf.IsChanged) {
                    unsaved = true;
                    break;
                }
            }

            if (unsaved) {
                switch (Medo.MessageBox.ShowQuestion(this, "Content of unsaved files will be lost if not saved. Do you with to save it now?", MessageBoxButtons.YesNo)) {
                    case DialogResult.Yes:
                        for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                            TabFile tf = (TabFile)tabFiles.TabPages[i];
                            if (tf.IsChanged) {
                                tf.Save();
                            }
                        }

                        break;
                }
            }

            var currentFolder = tabFiles.CurrentFolder ?? Document.GetRootFolder();

            tabFiles.FolderOpen(Settings.LastFolder);
            try {
                SetSelectedTab(tabFiles.SelectedTab);
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot load folder.\n\n{0}", ex.Message));
                tabFiles.FolderOpen(currentFolder);
            }

            mnuFolder.Text = tabFiles.CurrentFolder.Title;
        }

        private static void ToogleStyle(RichTextBoxEx richTextBox, FontStyle fontStyle) {
            if (richTextBox.SelectionFont != null) {
                richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, richTextBox.SelectionFont.Style ^ fontStyle);
            } else {
                richTextBox.BeginUpdate();
                int selStart = richTextBox.SelectionStart;
                int selLength = richTextBox.SelectionLength;
                bool toRegular = false;
                for (int i = selStart; i <= selStart + selLength - 1; i++) {
                    richTextBox.SelectionStart = i;
                    richTextBox.SelectionLength = 1;
                    bool isStyleOn = (richTextBox.SelectionFont.Style & fontStyle) == fontStyle;
                    if (i == selStart) {
                        toRegular = isStyleOn;
                    }
                    if (((isStyleOn == true) && (toRegular == true)) || ((isStyleOn == false) && (toRegular == false))) {
                        richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, richTextBox.SelectionFont.Style ^ fontStyle);
                    }
                }
                richTextBox.SelectionStart = selStart;
                richTextBox.SelectionLength = selLength;
                richTextBox.EndUpdate();
            }
        }

        private class SortAZ : IComparer {
            public int Compare(object x, object y) {
                return string.Compare(x.ToString(), y.ToString());
            }
        }

        private class SortZA : IComparer {
            public int Compare(object x, object y) {
                return -string.Compare(x.ToString(), y.ToString());
            }
        }

        #endregion


        #region Printing

        private void Print(TabFile document, bool preview = false) {
            try {
                var printDocument = document.TextBox.PrintDocument;
                printDocument.DocumentName = document.Title;

                try { printDocument.DefaultPageSettings.PaperSize.PaperName = Settings.PrintPaperName; } catch (ArgumentException) { }
                try { printDocument.DefaultPageSettings.PaperSource.SourceName = Settings.PrintPaperSource; } catch (ArgumentException) { }
                printDocument.DefaultPageSettings.Landscape = Settings.PrintIsPaperLandscape;
                printDocument.DefaultPageSettings.Margins = Settings.PrintMargins;

                if (preview) {
                    using (var frm = new Medo.Windows.Forms.PrintPreviewDialog(printDocument)) {
                        frm.TopMost = this.TopMost;
                        frm.ShowDialog(this);
                    }
                } else {
                    using (var frm = new PrintDialog() { Document = printDocument, UseEXDialog = true }) {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            printDocument.Print();
                        }
                    }
                }
            } catch (InvalidPrinterException ex) {
                Medo.MessageBox.ShowError(this, string.Format("{0}\n\n{1}", (preview ? "Cannot show print preview." : "Cannot print."), ex.Message));
            }
        }

        #endregion


        #region Timers

        private void tmrQuickSave_Tick(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            foreach (TabFile file in tabFiles.TabPages) {
                try {
                    if (file.IsChanged) { file.QuickSave(); }
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format("Cannot save \"{0}\".\n\n{1}", file.Title, ex.Message));
                }
            }
            tmrQuickSave.Enabled = true;
        }

        private void tmrUpdateToolbar_Tick(object sender, EventArgs e) {
            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.BaseFile.IsRichText;
            bool isTabPlainText = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsRichText == false);

            mnuSaveNow.Enabled = isTabSelected;
            mnuRename.Enabled = isTabSelected;
            mnuPrintPreview.Enabled = isTabSelected;
            mnuPrint.Enabled = isTabSelected;

            mnuCut.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnuCopy.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnuPaste.Enabled = isTabSelected && tabFiles.SelectedTab.CanPaste;

            mnuFont.Visible = isTabRichText;
            mnuBold.Visible = isTabRichText;
            mnuItalic.Visible = isTabRichText;
            mnuUnderline.Visible = isTabRichText;
            mnuStrikeout.Visible = isTabRichText;
            mnuRtfSeparator.Visible = isTabRichText;

            mnuUndo.Enabled = isTabSelected && tabFiles.SelectedTab.CanUndo;
            mnuRedo.Enabled = isTabSelected && tabFiles.SelectedTab.CanRedo;

            mnuFind.Enabled = isTabSelected;


            if (isTabRichText) {
                mnuBold.Checked = tabFiles.SelectedTab.IsTextBold;
                mnuItalic.Checked = tabFiles.SelectedTab.IsTextItalic;
                mnuUnderline.Checked = tabFiles.SelectedTab.IsTextUnderline;
                mnuStrikeout.Checked = tabFiles.SelectedTab.IsTextStrikeout;
            }
        }


        int nextIndexToCheck = 0;

        private void tmrCheckFileUpdate_Tick(object sender, EventArgs e) {
            if (tabFiles.Enabled == false) { return; }
            tmrCheckFileUpdate.Enabled = false;
            if (tabFiles.TabCount > 0) {
                nextIndexToCheck = nextIndexToCheck % tabFiles.TabPages.Count;
                var currTab = (TabFile)tabFiles.TabPages[nextIndexToCheck];
                var fi = new QFileInfo(currTab.BaseFile.Info.FullName);
                if (fi.LastWriteTimeUtc != currTab.LastWriteTimeUtc) {
                    if (currTab.IsChanged) {
                        currTab.Save();
                    } else {
                        try {
                            currTab.Reopen();
                        } catch (Exception) { }
                    }
                }
                nextIndexToCheck += 1;
            } else {
                nextIndexToCheck = 0;
            }
            tmrCheckFileUpdate.Enabled = true;
        }

        #endregion


        private void FindFirst() {
            using (var frm = new FindForm(this.tabFiles)) {
                frm.TopMost = this.TopMost;
                frm.ShowDialog(this);
            }
        }

        private bool TryFolderSave() {
            try {
                tabFiles.FolderSave();
                return true;
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format("Cannot save folder.\n\n{0}", ex.Message));
                return false;
            }
        }

        private void FolderChange(DocumentFolder oldFolder, DocumentFolder newFolder) {
            if (!newFolder.Equals(oldFolder)) {
                tabFiles.FolderOpen(newFolder);
                mnuFolder.Text = tabFiles.CurrentFolder.Title;
                Settings.LastFolder = tabFiles.CurrentFolder;
            }
        }

        private void ToggleMenu() {
            if (Settings.ShowToolbar == false) {
                mnu.Visible = !mnu.Visible;
            }
            if (mnu.Visible) {
                if (mnu.ContainsFocus) {
                    if (tabFiles.SelectedTab != null) { tabFiles.SelectedTab.Select(); }
                } else {
                    mnu.Select();
                    mnuNew.Select();
                }
            }
        }

    }
}
