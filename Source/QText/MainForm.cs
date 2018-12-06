using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QText {

    internal partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            tmrQuickSave.Interval = Settings.Current.QuickSaveInterval;

            if (Settings.Current.DisplayMinimizeMaximizeButtons) {
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            } else {
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            }
            ShowInTaskbar = Settings.Current.DisplayShowInTaskbar;
            mnu.Renderer = Helper.ToolstripRenderer;
            Helper.ScaleToolstrip(mnu, mnxTab, mnxText);

            tabFiles.Multiline = Settings.Current.MultilineTabs;

            mnuAppUpgrade.Visible = Medo.Configuration.Config.IsAssumedInstalled;
        }


        internal bool SuppressMenuKey = false;

        protected override bool ProcessDialogKey(Keys keyData) {
            Debug.WriteLine("MainForm_ProcessDialogKey: " + keyData.ToString());
            if (((keyData & Keys.Alt) == Keys.Alt) && (keyData != (Keys.Alt | Keys.Menu))) { SuppressMenuKey = true; }

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
                    Close();
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
                    }
                    return true;


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
                    }
                    return true;

                case Keys.Alt | Keys.Home: {
                        if (!tabFiles.CurrentFolder.IsRoot) {
                            tabFiles.FolderOpen(App.Document.RootFolder);
                            mnuFolder.Text = tabFiles.CurrentFolder.Title;
                            Settings.Current.LastFolder = tabFiles.CurrentFolder;
                        }
                    }
                    return true;

                case Keys.Alt | Keys.PageUp:
                case Keys.Alt | Keys.Up: {
                        var currFolder = tabFiles.CurrentFolder;
                        var list = new List<DocumentFolder>(App.Document.GetFolders());
                        var index = list.FindIndex(delegate (DocumentFolder folder) { return folder.Equals(currFolder); });
                        if (index > 0) {
                            tabFiles.FolderOpen(list[index - 1]);
                            mnuFolder.Text = tabFiles.CurrentFolder.Title;
                            Settings.Current.LastFolder = tabFiles.CurrentFolder;
                        }
                    }
                    return true;

                case Keys.Alt | Keys.PageDown:
                case Keys.Alt | Keys.Down: {
                        var currFolder = tabFiles.CurrentFolder;
                        var list = new List<DocumentFolder>(App.Document.GetFolders());
                        var index = list.FindIndex(delegate (DocumentFolder folder) { return folder.Equals(currFolder); });
                        if (index < list.Count - 1) {
                            tabFiles.FolderOpen(list[index + 1]);
                            mnuFolder.Text = tabFiles.CurrentFolder.Title;
                            Settings.Current.LastFolder = tabFiles.CurrentFolder;
                        }
                    }
                    return true;

                case Keys.Alt | Keys.Shift | Keys.D: {
                        if ((tabFiles.SelectedTab != null) && (tabFiles.SelectedTab.IsOpened)) {
                            tabFiles.SelectedTab.TextBox.SelectedText = DateTime.Now.ToShortDateString();
                        }
                    }
                    return true;

                case Keys.Alt | Keys.Shift | Keys.T: {
                        if ((tabFiles.SelectedTab != null) && (tabFiles.SelectedTab.IsOpened)) {
                            tabFiles.SelectedTab.TextBox.SelectedText = DateTime.Now.ToShortTimeString();
                        }
                    }
                    return true;

            }

            return base.ProcessDialogKey(keyData);
        }


        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) {
            Debug.WriteLine("MainForm_ProcessCmdKey: " + keyData.ToString());
            switch (keyData) {

                case Keys.Shift | Keys.F1: {
                        mnxTextSelectionSpelling_Click(null, null);
                    }
                    return true;

                case Keys.F5: {
                        mnxTextInsertDateTimeBoth_Click(null, null);
                    }
                    return true;

                case Keys.Control | Keys.OemSemicolon: {
                        mnxTextInsertDate_Click(null, null);
                    }
                    return true;

                case Keys.Control | Keys.Shift | Keys.OemSemicolon: {
                        mnxTextInsertTime_Click(null, null);
                    }
                    return true;


                case Keys.Control | Keys.Tab:
                case Keys.Control | Keys.Shift | Keys.Tab:
                    if ((tabFiles.TabPages.Count >= 1) && (tabFiles.SelectedTab == null)) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                    }
                    if ((tabFiles.TabPages.Count >= 2)) {
                        var tp = GetPreviousSelectedTab();
                        if ((tp != null)) {
                            tabFiles.SelectedTab = tp;
                        } else {
                            var i = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab);
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
                if (SuppressMenuKey) { SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyUp(e);
            }

            tmrQuickSave.Enabled = true;
        }

        private void Form_Load(object sender, EventArgs e) {
            Medo.Windows.Forms.State.Load(this);

            TopMost = Settings.Current.DisplayAlwaysOnTop;
            mnuAlwaysOnTop.Checked = TopMost;

            //get plugin buttons
            ToolStripItem lastPluginItem = new ToolStripSeparator();
            mnu.Items.Insert(mnu.Items.IndexOf(mnuAlwaysOnTop) + 1, lastPluginItem);
            foreach (var plugin in App.GetEnabledPlugins()) {
                foreach (var item in plugin.GetToolStripItems()) {
                    if ((item is ToolStripSeparator) && (lastPluginItem is ToolStripSeparator)) {
                        //no double separators
                    } else {
                        mnu.Items.Insert(mnu.Items.IndexOf(lastPluginItem) + 1, item);
                        lastPluginItem = item;
                    }
                }
            }

            RefreshAll(null, null);
            Form_Resize(null, null);

            App.Document.FileExternallyChanged += Document_FileExternallyChanged;
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
#if !DEBUG
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Hide();
                App.TrayContext.ShowBalloonOnMinimize();
            }
#endif

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
                for (var i = 0; i < failedTitles.Count; i++) {
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
            bwCheckForUpgrade.CancelAsync();

            foreach (var plugin in App.GetEnabledPlugins()) {
                plugin.Terminate();
            }

            Application.Exit();
        }


        private void Form_Activated(object sender, EventArgs e) {
            tmrUpdateToolbar.Enabled = Settings.Current.ShowToolbar;
            if (tabFiles == null) { return; }

            if ((tabFiles.SelectedTab != null) && (tabFiles.SelectedTab.IsOpened)) {
                tabFiles.SelectedTab.Select();
            }
        }

        private void Form_Deactivate(object sender, EventArgs e) {
            tmrUpdateToolbar.Enabled = false;
            tmrQuickSave_Tick(null, null);
        }


        private void Form_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                if (GetNonTabLocation(e.Location) != null) {
                    if (mnuNew.Enabled) { mnuNew.PerformClick(); }
                }
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                var location = GetNonTabLocation(e.Location);
                if (location != null) {
                    tabFiles.ContextMenuStrip.Show(tabFiles, location.Value.X, location.Value.Y);
                }
            }
        }

        private void Form_Move(object sender, EventArgs e) {
            if (Visible && (WindowState == FormWindowState.Normal)) { Medo.Windows.Forms.State.Save(this); }
        }

        private bool _form_ResizeReentry = false;
        private void Form_Resize(object sender, EventArgs e) {
            if (_form_ResizeReentry) { return; }
            _form_ResizeReentry = true;

            if (WindowState != FormWindowState.Minimized) {
                if (Visible) { Medo.Windows.Forms.State.Save(this); }
                mnu.Visible = Settings.Current.ShowToolbar;
            } else if (Settings.Current.TrayOnMinimize) {
                Visible = false;
                Close();
            }

            _form_ResizeReentry = false;
        }

        private void Form_VisibleChanged(object sender, EventArgs e) {
            var isVisible = Visible;

            tmrQuickSave.Enabled = isVisible;
            tmrUpdateToolbar.Enabled = isVisible;
        }


        private void Form_Shown(object sender, EventArgs e) {
            var version = Assembly.GetExecutingAssembly().GetName().Version; //don't auto-check for development builds
            if ((version.Major != 0) || (version.Minor != 0)) { bwCheckForUpgrade.RunWorkerAsync(); }
        }


        private void tabFiles_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                for (var i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
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
                        Focus();
                        if (Visible == false) { return; }
                        using (var frm = new PasswordForm(tabFiles.SelectedTab.Title)) {
                            frm.TopMost = TopMost;
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
                    var errorTab = tabFiles.SelectedTab;
                    tabFiles.SelectedTab = _CurrSelectedTab;
                    if (!errorTab.BaseFile.Exists) { tabFiles.TabPages.Remove(errorTab); }
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
            using (var frm = new FileNewForm(tabFiles.CurrentFolder)) {
                frm.TopMost = TopMost;
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
                    frm.TopMost = TopMost;
                    try {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            tabFiles.SelectedTab.Rename(frm.NewTitle);
                        }
                    } catch (Exception ex) {
                        Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot rename file.\n\n{0}", ex.Message));
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }


        private void mnuPrint_Click(object sender, EventArgs e) {
            if (PrinterSettings.InstalledPrinters.Count == 0) { return; }

            SaveAllChanged();
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                Print(tabFiles.SelectedTab);
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuPrintPreview_Click(object sender, EventArgs e) {
            if (PrinterSettings.InstalledPrinters.Count == 0) { return; }

            SaveAllChanged();
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                Print(tabFiles.SelectedTab, true);
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuPrintSetup_Click(object sender, EventArgs e) {
            if (PrinterSettings.InstalledPrinters.Count == 0) { return; }

            var pageSettings = new PageSettings();
            try { pageSettings.PaperSize.PaperName = Settings.Current.PrintPaperName; } catch (ArgumentException) { }
            try { pageSettings.PaperSource.SourceName = Settings.Current.PrintPaperSource; } catch (ArgumentException) { }
            pageSettings.Landscape = Settings.Current.PrintIsPaperLandscape;
            pageSettings.Margins = Settings.Current.PrintMargins;

            using (var frm = new PageSetupDialog() { PageSettings = pageSettings }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    Settings.Current.PrintPaperName = pageSettings.PaperSize.PaperName;
                    Settings.Current.PrintPaperSource = pageSettings.PaperSource.SourceName;
                    Settings.Current.PrintIsPaperLandscape = pageSettings.Landscape;
                    Settings.Current.PrintMargins = pageSettings.Margins;
                }
            }
        }


        private void mnuCut_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Cut(Settings.Current.ForceTextCopyPaste);
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
                    tabFiles.SelectedTab.Copy(Settings.Current.ForceTextCopyPaste);
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
                    tabFiles.SelectedTab.Paste(Settings.Current.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation could not be completed.\n\n{0}", ex.Message));
            }
            tmrQuickSave.Enabled = true;
        }


        private void mnuFont_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                var tf = tabFiles.SelectedTab;
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
                var tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    ToogleStyle(tf.TextBox, FontStyle.Bold);
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuItalic_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                var tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    ToogleStyle(tf.TextBox, FontStyle.Italic);
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuUnderline_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                var tf = tabFiles.SelectedTab;
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
                var tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    if (tf.TextBox.SelectionFont != null) {
                        ToogleStyle(tf.TextBox, FontStyle.Strikeout);
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuListBullets_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                var tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    if (tf.TextBox.SelectionFont != null) {
                        tf.TextBox.SelectionBullet = !tf.TextBox.SelectionBullet;
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuListNumbers_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                var tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    if (tf.TextBox.SelectionFont != null) {
                        tf.TextBox.SelectionNumbered = !tf.TextBox.SelectionNumbered;
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuResetFont_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                var tf = tabFiles.SelectedTab;
                if (tf.BaseFile.IsRichText) {
                    tf.TextBox.Cursor = Cursors.WaitCursor;
                    tf.TextBox.ResetSelectionFont();
                    tf.TextBox.ResetSelectionParagraphSpacing();
                    tf.TextBox.ResetSelectionParagraphIndent();
                    tf.TextBox.Cursor = Cursors.Default;
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
            SaveAllChanged();
            FindFirst();
        }

        private void mnuFindFindNext_Click(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(SearchStatus.Text)) {
                SaveAllChanged();
                Search.FindNext(this, tabFiles, tabFiles.SelectedTab);
            }
        }

        private void mnuFindGoto_Click(object sender, EventArgs e) {
            var hasText = false;
            if (tabFiles.SelectedTab != null) {
                var tf = tabFiles.SelectedTab;
                hasText = tf.IsOpened;
            }

            SaveAllChanged();
            using (var frm = new GotoForm(hasText)) {
                frm.TopMost = TopMost;
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    var destination = frm.SelectedItem;
                    if (destination.IsLineNumber) {
                        if (tabFiles.SelectedTab != null) {
                            var tf = tabFiles.SelectedTab;
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
                            foreach (TabFile tab in tabFiles.TabPages) {
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
            Settings.Current.DisplayAlwaysOnTop = mnuAlwaysOnTop.Checked;
            TopMost = Settings.Current.DisplayAlwaysOnTop;
        }


        private void mnuFolder_DropDownOpening(object sender, EventArgs e) {
            mnuFolder.DropDownItems.Clear();

            foreach (var folder in App.Document.GetFolders()) {
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
                frm.TopMost = TopMost;
                frm.ShowDialog(this);
                if (!tabFiles.CurrentFolder.Equals(frm.CurrentFolder)) {
                    tabFiles.Enabled = true;
                    tabFiles.FolderOpen(frm.CurrentFolder, false);
                }
                mnuFolder.Text = tabFiles.CurrentFolder.Title;
                Settings.Current.LastFolder = tabFiles.CurrentFolder;
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
                frm.TopMost = TopMost;
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
                frm.TopMost = TopMost;
                SaveAllChanged();
                tmrUpdateToolbar.Enabled = false;
                RefreshAll(null, null);
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    if (Settings.Current.DisplayMinimizeMaximizeButtons) {
                        FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    } else {
                        FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                    }
                    ShowInTaskbar = Settings.Current.DisplayShowInTaskbar;
                    tabFiles.Multiline = Settings.Current.MultilineTabs;
                    TopMost = Settings.Current.DisplayAlwaysOnTop;
                    RefreshAll(null, null);
                    Form_Resize(null, null);
                    tmrUpdateToolbar.Enabled = Settings.Current.ShowToolbar;
                    tmrQuickSave.Interval = Settings.Current.QuickSaveInterval;

                    App.Document.CarbonCopyRootPath = Settings.Current.CarbonCopyUse ? Settings.Current.CarbonCopyDirectory : null;
                    App.Document.CarbonCopyIgnoreErrors = Settings.Current.CarbonCopyIgnoreErrors;
                    try {
                        App.Document.WriteAllCarbonCopies();
                    } catch (InvalidOperationException ex) {
                        Medo.MessageBox.ShowError(this, ex.Message);
                    }
                }
            }
            tmrQuickSave.Enabled = true;
        }


        private void mnuAppFeedback_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            Medo.Diagnostics.ErrorReport.TopMost = TopMost;
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("https://medo64.com/feedback/"));
            tmrQuickSave.Enabled = true;
        }

        private void mnuAppUpgrade_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            Medo.Services.Upgrade.ShowDialog(this, new Uri("https://medo64.com/upgrade/"));
            tmrQuickSave.Enabled = true;
        }

        private void mnuAppAbout_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("https://www.medo64.com/qtext/"));

            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.TextBox.Select();
            }
            tmrQuickSave.Enabled = true;
        }

        #endregion


        #region Tabs menu

        private void mnxTab_Opening(object sender, CancelEventArgs e) {
            var isTabSelected = (tabFiles.SelectedTab != null);
            var isTabRich = isTabSelected && tabFiles.SelectedTab.BaseFile.IsRichText;
            var isTabPlain = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsRichText == false);
            var isTabEncryptable = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsEncrypted == false);
            var isTabDecryptable = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsEncrypted);
            var isZoomResetable = isTabSelected && tabFiles.SelectedTab.TextBox.HasZoom;

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
                var tf = tabFiles.SelectedTab;
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

            foreach (var folder in App.Document.GetFolders()) {
                var item = new ToolStripMenuItem(folder.Title, null, mnxTabMoveTo_Click) { Tag = folder };
                item.AutoToolTip = false;
                item.Enabled = !folder.Equals(tabFiles.CurrentFolder) && tabFiles.SelectedTab.BaseFile.CanMove(folder);
                if (!item.Enabled) {
                    item.ToolTipText = folder.Equals(tabFiles.CurrentFolder) ? "File is already in this folder." : "File with same name already exists in destination folder.";
                }
                mnxTabMoveTo.DropDownItems.Add(item);
            }
        }

        private void mnxTabMoveTo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var folder = (DocumentFolder)((ToolStripMenuItem)sender).Tag;
                try {
                    tabFiles.SelectedTab.BaseFile.Move(folder);
                    tabFiles.RemoveTab(tabFiles.SelectedTab);
                } catch (InvalidOperationException ex) {
                    Medo.MessageBox.ShowError(this, "Error moving file!\n\n" + ex.Message);
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
                using (var frm = new ChangePasswordForm(Text)) {
                    frm.TopMost = TopMost;
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        tabFiles.SelectedTab.Encrypt(frm.Password);
                    }
                }
            }
        }

        private void mnxTabChangePassword_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                using (var frm = new ChangePasswordForm(Text)) {
                    frm.TopMost = TopMost;
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        tabFiles.SelectedTab.BaseFile.Password = frm.Password;
                        tabFiles.SelectedTab.Save();
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
                tabFiles.SelectedTab.BaseFile.OpenInExplorer();
            } else {
                tabFiles.CurrentFolder.OpenInExplorer();
            }
        }

        #endregion


        #region Text menu

        private void mnxText_Opening(object sender, CancelEventArgs e) {
            var isTabSelected = (tabFiles.SelectedTab != null);
            var isTabRichText = isTabSelected && tabFiles.SelectedTab.BaseFile.IsRichText;
            var isTabPlainText = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsRichText == false);
            var isTextSelected = isTabSelected && (tabFiles.SelectedTab.TextBox.SelectedText.Length > 0);
            var hasText = isTabSelected && (tabFiles.SelectedTab.TextBox.Text.Length > 0);
            var isZoomResetable = isTabSelected && (tabFiles.SelectedTab.TextBox.ZoomFactor != 1);

            mnxTextUndo.Enabled = isTabSelected && tabFiles.SelectedTab.CanUndo;
            mnxTextRedo.Enabled = isTabSelected && tabFiles.SelectedTab.CanRedo;

            mnxTextCut.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnxTextCopy.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnxTextPaste.Enabled = isTabSelected && tabFiles.SelectedTab.CanPaste();

            mnxTextCutPlain.Visible = isTabRichText && (Settings.Current.ForceTextCopyPaste == false);
            mnxTextCopyPlain.Visible = isTabRichText && (Settings.Current.ForceTextCopyPaste == false);
            mnxTextPastePlain.Visible = isTabRichText && (Settings.Current.ForceTextCopyPaste == false);
            mnxTextBoxCutCopyPasteAsTextSeparator.Visible = isTabRichText && (Settings.Current.ForceTextCopyPaste == false);
            mnxTextCutPlain.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnxTextCopyPlain.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnxTextPastePlain.Enabled = isTabSelected && tabFiles.SelectedTab.CanPaste(forceText: true);

            mnxTextSelectAll.Enabled = isTabSelected && (tabFiles.SelectedTab.Text.Length > 0);

            mnxTextFont.Visible = isTabRichText;
            mnxTextBold.Visible = isTabRichText;
            mnxTextItalic.Visible = isTabRichText;
            mnxTextUnderline.Visible = isTabRichText;
            mnxTextStrikeout.Visible = isTabRichText;
            mnxTextRtfSeparator.Visible = isTabRichText;
            mnxTextResetFont.Visible = isTabRichText;

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
                var ss = txt.SelectionStart;
                var sl = txt.SelectionLength;

                txt.SelectedText = txt.SelectedText.ToLower();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextSelectionUpper_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                var ss = txt.SelectionStart;
                var sl = txt.SelectionLength;

                txt.SelectedText = txt.SelectedText.ToUpper();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextSelectionTitle_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                var ss = txt.SelectionStart;
                var sl = txt.SelectionLength;

                txt.SelectedText = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToLower(txt.SelectedText));

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextSelectionDrGrammar_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                var ss = txt.SelectionStart;
                var sl = txt.SelectionLength;

                var sb = new StringBuilder(txt.SelectedText.ToLower());
                var startOfWord = -1;
                for (var i = 0; i <= sb.Length; i++) {
                    if ((i < sb.Length) && (char.IsLetter(sb[i]) || (sb[i] == '\''))) {
                        if (startOfWord == -1) { startOfWord = i; }
                    } else {
                        if (startOfWord > -1) {
                            var word = txt.SelectedText.Substring(startOfWord, i - startOfWord).ToLower();
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
                    frm.TopMost = TopMost;
                    frm.ShowDialog(this);
                }
            } else {
                using (var frm = new SpellingForm()) {
                    frm.TopMost = TopMost;
                    frm.ShowDialog(this);
                }
            }
        }


        private void mnxTextLinesSortAsc_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var txt = tabFiles.SelectedTab.TextBox;
                txt.SelectLineBlock();

                var ss = txt.SelectionStart;
                var sl = txt.SelectionLength;

                var text = txt.SelectedText;
                var addLf = false;
                if (text.EndsWith("\n")) {
                    addLf = true;
                    text = text.Remove(text.Length - 1);
                }
                var selSplit = text.Split(new string[] { "\n" }, StringSplitOptions.None);
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

                var ss = txt.SelectionStart;
                var sl = txt.SelectionLength;

                var text = txt.SelectedText;
                var addLf = false;
                if (text.EndsWith("\n")) {
                    addLf = true;
                    text = text.Remove(text.Length - 1);
                }
                var selSplit = text.Split(new string[] { "\n" }, StringSplitOptions.None);
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

                var ss = txt.SelectionStart;
                var sl = txt.SelectionLength;

                var text = txt.SelectedText;
                var addLf = false;
                if (text.EndsWith("\n")) {
                    addLf = true;
                    text = text.Remove(text.Length - 1);
                }
                var selSplit = text.Split(new string[] { "\n" }, StringSplitOptions.None);
                var lenDiff = 0;
                for (var i = 0; i < selSplit.Length; i++) {
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

        private void mnxTextInsertDateTimeBoth_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var txt = tabFiles.SelectedTab.TextBox;
                txt.SelectedText = DateTime.Now.ToString(Settings.Current.DateTimeFormat) + Settings.Current.DateTimeSeparator;
            }
        }

        private void mnxTextInsertTime_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var txt = tabFiles.SelectedTab.TextBox;
                txt.SelectedText = DateTime.Now.ToString("t") + Settings.Current.DateTimeSeparator;
            }
        }

        private void mnxTextInsertDate_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var txt = tabFiles.SelectedTab.TextBox;
                txt.SelectedText = DateTime.Now.ToString("d") + Settings.Current.DateTimeSeparator;
            }
        }

        #endregion


        #region Helpers

        private TabFile _CurrSelectedTab;
        private TabFile _PrevSelectedTab;

        private Point? GetNonTabLocation(Point e) { //location of point next to tabs (due to transparency tab click is received on form)
            if (tabFiles.TabPages.Count > 0) {
                var rect = tabFiles.GetTabRect(tabFiles.TabCount - 1);
                rect.Offset(tabFiles.Left, tabFiles.Top);
                if ((e.Y >= rect.Top) && (e.Y <= rect.Bottom) && (e.X >= rect.Right)) {
                    tabFiles.SelectedTab = null;
                    return new Point(e.X - tabFiles.Left, e.Y - tabFiles.Top);
                }
            } else {
                var rect = tabFiles.ClientRectangle;
                if ((e.Y >= rect.Top) && (e.Y <= rect.Bottom)) {
                    return new Point(e.X - tabFiles.Left, e.Y - tabFiles.Top);
                }
            }
            return null;
        }

        private void SetSelectedTab(TabFile tabPage) {
            _PrevSelectedTab = _CurrSelectedTab;
            _CurrSelectedTab = tabPage;
        }

        private TabFile GetPreviousSelectedTab() {
            if ((_PrevSelectedTab != null) && (!_PrevSelectedTab.Equals(_CurrSelectedTab))) {
                if (tabFiles.TabPages.Contains(_PrevSelectedTab)) {
                    return _PrevSelectedTab;
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
            var unsaved = false;
            for (var i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                var tf = (TabFile)tabFiles.TabPages[i];
                if (tf.IsChanged) {
                    unsaved = true;
                    break;
                }
            }

            if (unsaved) {
                switch (Medo.MessageBox.ShowQuestion(this, "Content of unsaved files will be lost if not saved. Do you with to save it now?", MessageBoxButtons.YesNo)) {
                    case DialogResult.Yes:
                        for (var i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                            var tf = (TabFile)tabFiles.TabPages[i];
                            if (tf.IsChanged) {
                                tf.Save();
                            }
                        }

                        break;
                }
            }

            tabFiles.FolderOpen(Settings.Current.LastFolder);

            try {
                SetSelectedTab(tabFiles.SelectedTab);
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot load folder.\n\n{0}", ex.Message));
                tabFiles.FolderOpen(tabFiles.CurrentFolder ?? App.Document.RootFolder);
            }

            mnuFolder.Text = tabFiles.CurrentFolder.Title;
        }

        private static void ToogleStyle(RichTextBoxEx richTextBox, FontStyle fontStyle) {
            if (richTextBox.SelectionFont != null) {
                richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, richTextBox.SelectionFont.Style ^ fontStyle);
            } else {
                richTextBox.BeginUpdate();
                var selStart = richTextBox.SelectionStart;
                var selLength = richTextBox.SelectionLength;
                var toRegular = false;
                for (var i = selStart; i <= selStart + selLength - 1; i++) {
                    richTextBox.SelectionStart = i;
                    richTextBox.SelectionLength = 1;
                    var isStyleOn = (richTextBox.SelectionFont.Style & fontStyle) == fontStyle;
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

                try { printDocument.DefaultPageSettings.PaperSize.PaperName = Settings.Current.PrintPaperName; } catch (ArgumentException) { }
                try { printDocument.DefaultPageSettings.PaperSource.SourceName = Settings.Current.PrintPaperSource; } catch (ArgumentException) { }
                printDocument.DefaultPageSettings.Landscape = Settings.Current.PrintIsPaperLandscape;
                printDocument.DefaultPageSettings.Margins = Settings.Current.PrintMargins;

                if (preview) {
                    using (var frm = new Medo.Windows.Forms.PrintPreviewDialog(printDocument)) {
                        frm.TopMost = TopMost;
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
            var isTabSelected = (tabFiles.SelectedTab != null);
            var isTabRichText = isTabSelected && tabFiles.SelectedTab.BaseFile.IsRichText;
            var isTabPlainText = isTabSelected && (tabFiles.SelectedTab.BaseFile.IsRichText == false);

            var hasPrinters = false;
            try {
                hasPrinters = (PrinterSettings.InstalledPrinters.Count > 0);
            } catch (Win32Exception) { }

            mnuSaveNow.Enabled = isTabSelected;
            mnuRename.Enabled = isTabSelected;

            mnuPrintDefault.Enabled = hasPrinters;
            mnuPrintDefault.ToolTipText = hasPrinters ? "Print (Ctrl+P)" : "No printers installed";
            mnuPrint.Enabled = isTabSelected && hasPrinters;
            mnuPrintPreview.Enabled = isTabSelected && hasPrinters;
            mnuPrintSetup.Enabled = isTabSelected && hasPrinters;

            mnuCut.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnuCopy.Enabled = isTabSelected && tabFiles.SelectedTab.CanCopy;
            mnuPaste.Enabled = isTabSelected && tabFiles.SelectedTab.CanPaste();

            mnuFont.Visible = isTabRichText;
            mnuBold.Visible = isTabRichText;
            mnuItalic.Visible = isTabRichText;
            mnuUnderline.Visible = isTabRichText;
            mnuStrikeout.Visible = isTabRichText;
            mnuRtfSeparator1.Visible = isTabRichText;
            mnuListBullets.Visible = isTabRichText;
            mnuListNumbers.Visible = isTabRichText;
            mnuRtfSeparator2.Visible = isTabRichText;

            mnuUndo.Enabled = isTabSelected && tabFiles.SelectedTab.CanUndo;
            mnuRedo.Enabled = isTabSelected && tabFiles.SelectedTab.CanRedo;

            mnuFind.Enabled = isTabSelected;


            if (isTabRichText) {
                mnuBold.Checked = tabFiles.SelectedTab.IsTextBold;
                mnuItalic.Checked = tabFiles.SelectedTab.IsTextItalic;
                mnuUnderline.Checked = tabFiles.SelectedTab.IsTextUnderline;
                mnuStrikeout.Checked = tabFiles.SelectedTab.IsTextStrikeout;
                mnuListBullets.Checked = tabFiles.SelectedTab.IsTextBulleted;
                mnuListNumbers.Checked = tabFiles.SelectedTab.IsTextNumbered;
            }
        }


        private void Document_FileExternallyChanged(object sender, DocumentFileEventArgs e) {
            if (tabFiles.Enabled == false) { return; }

            try {
                Invoke((MethodInvoker)delegate () {
                    foreach (TabFile tab in tabFiles.TabPages) {
                        if (tab.BaseFile.Equals(e.File)) {
                            if (!tab.IsChanged) { //don't reopen tabs that were changed
                                try {
                                    tab.Reopen();
                                } catch (InvalidOperationException) {
                                    Thread.Sleep(100);
                                    try {
                                        tab.Reopen();
                                    } catch (InvalidOperationException) { }
                                }
                            }
                        }
                    }
                });
            } catch (ObjectDisposedException) { }
        }

        #endregion


        private void FindFirst() {
            using (var frm = new FindForm(tabFiles)) {
                frm.TopMost = TopMost;
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
                Settings.Current.LastFolder = tabFiles.CurrentFolder;
            }
        }

        private void ToggleMenu() {
            if (Settings.Current.ShowToolbar == false) {
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

        private void bwCheckForUpgrade_DoWork(object sender, DoWorkEventArgs e) {
            e.Cancel = true;

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 3000) { //wait for three seconds
                Thread.Sleep(100);
                if (bwCheckForUpgrade.CancellationPending) { return; }
            }

            var file = Medo.Services.Upgrade.GetUpgradeFile(new Uri("https://medo64.com/upgrade/"));
            if (file != null) {
                if (bwCheckForUpgrade.CancellationPending) { return; }
                e.Cancel = false;
            }
        }

        private void bwCheckForUpgrade_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (!e.Cancelled) {
                Helper.ScaleToolstripItem(mnuApp, "mnuAppUpgrade");
                mnuAppUpgrade.Text = "Upgrade is available";

                var locationButton = PointToScreen(new Point(mnu.Left + mnuApp.Bounds.Left, mnu.Top + mnuApp.Bounds.Top));
                var locationForm = Location;
                var tipX = locationButton.X - locationForm.X + mnuApp.Bounds.Width / 2;
                var tipY = locationButton.Y - locationForm.Y + mnuApp.Bounds.Height - SystemInformation.Border3DSize.Height;

                tip.ToolTipIcon = ToolTipIcon.Info;
                tip.ToolTipTitle = "Upgrade";
                tip.Show("Upgrade is available.", this, tipX, tipY, 1729);
            }
        }

    }
}
