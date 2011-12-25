using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QText {

    internal partial class MainForm : Form {

        internal bool _suppressMenuKey = false;

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

            Medo.Windows.Forms.State.Load(this);
        }


        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) {
            switch (keyData) {

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

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }


        private void Form_Load(object sender, EventArgs e) {
            RefreshAll(null, null);
            Form_Resize(null, null);
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
#if !DEBUG
            switch (e.CloseReason) {
                case CloseReason.ApplicationExitCall:
                case CloseReason.TaskManagerClosing:
                case CloseReason.WindowsShutDown:
                    break;

                default:
                    e.Cancel = true;
                    this.Hide();
                    break;
            }
#endif

            var failedTitles = new List<string>();
            var failedExceptions = new List<Exception>();
            foreach (TabFile file in tabFiles.TabPages) {
                try {
                    file.Save();
                } catch (Exception ex) {
                    failedTitles.Add(file.Title);
                    failedExceptions.Add(ex);
                }
            }
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

            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.Focus();
            }
        }

        private void Form_Deactivate(object sender, EventArgs e) {
            this.tmrUpdateToolbar.Enabled = false;
        }


        private void Form_KeyDown(object sender, KeyEventArgs e) {
            tmrQuickSave.Enabled = false;

            switch (e.KeyData) {

                case Keys.Control | Keys.N:
                    mnuNew_Click(null, null);
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.S:
                    mnuSaveNow_Click(null, null);
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.F2:
                    mnuRename_Click(null, null);
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.P:
                    mnuPrint_Click(null, null);
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.F:
                    mnuFind_Click(null, null);
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.F3:
                    FindNext();
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.T:
                    mnuAlwaysOnTop_Click(null, null);
                    e.Handled = true; e.SuppressKeyPress = true; break;


                case Keys.Escape:
                    this.Close();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.Back:
                    mnuUndo_Click(null, null);
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.Shift | Keys.Back:
                    mnuRedo_Click(null, null);
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;


                case Keys.Alt | Keys.D1:
                    if (tabFiles.TabPages.Count >= 1) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D2:
                    if (tabFiles.TabPages.Count >= 2) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[1];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D3:
                    if (tabFiles.TabPages.Count >= 3) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[2];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D4:
                    if (tabFiles.TabPages.Count >= 4) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[3];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D5:
                    if (tabFiles.TabPages.Count >= 5) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[4];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D6:
                    if (tabFiles.TabPages.Count >= 6) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[5];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D7:
                    if (tabFiles.TabPages.Count >= 7) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[6];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D8:
                    if (tabFiles.TabPages.Count >= 8) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[7];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D9:
                    if (tabFiles.TabPages.Count >= 9) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[8];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.D0:
                    if (tabFiles.TabPages.Count >= 10) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[9];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;


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
                        this._suppressMenuKey = true;
                        e.Handled = true; e.SuppressKeyPress = true;
                    } break;

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
                        this._suppressMenuKey = true;
                        e.Handled = true; e.SuppressKeyPress = true;
                    } break;


                case Keys.Alt | Keys.Home:
                    tabFiles.FolderOpen("");
                    mnuFolder.Text = string.IsNullOrEmpty(tabFiles.CurrentFolder) ? "(Default)" : tabFiles.CurrentFolder;
                    Settings.LastFolder = tabFiles.CurrentFolder;
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.Up: {
                        var currFolder = tabFiles.CurrentFolder;
                        var list = new List<string>(new string[] { "" });
                        list.AddRange(TabFiles.GetSubFolders());
                        var index = list.FindIndex(delegate(string folder) { return string.Equals(folder, currFolder, StringComparison.OrdinalIgnoreCase); });
                        if (index > 0) {
                            tabFiles.FolderOpen(list[index - 1]);
                            mnuFolder.Text = string.IsNullOrEmpty(tabFiles.CurrentFolder) ? "(Default)" : tabFiles.CurrentFolder;
                            Settings.LastFolder = tabFiles.CurrentFolder;
                        }
                        this._suppressMenuKey = true;
                        e.Handled = true; e.SuppressKeyPress = true;
                    } break;

                case Keys.Alt | Keys.Down: {
                        var currFolder = tabFiles.CurrentFolder;
                        var list = new List<string>(new string[] { "" });
                        list.AddRange(TabFiles.GetSubFolders());
                        var index = list.FindIndex(delegate(string folder) { return string.Equals(folder, currFolder, StringComparison.OrdinalIgnoreCase); });
                        if (index < list.Count - 1) {
                            tabFiles.FolderOpen(list[index + 1]);
                            mnuFolder.Text = string.IsNullOrEmpty(tabFiles.CurrentFolder) ? "(Default)" : tabFiles.CurrentFolder;
                            Settings.LastFolder = tabFiles.CurrentFolder;
                        }
                        this._suppressMenuKey = true;
                        e.Handled = true; e.SuppressKeyPress = true;
                    } break;


                case Keys.Alt | Keys.Menu: //just to prevent suppressing menu key
                    break;

                default:
                    if (e.Alt) {
                        this._suppressMenuKey = true;
                    } break;
            }

            tmrQuickSave.Enabled = true;
        }

        private void Form_KeyUp(object sender, KeyEventArgs e) {
            tmrQuickSave.Enabled = false;

            switch (e.KeyData) {

                case Keys.Menu:
                    if (this._suppressMenuKey) {
                        this._suppressMenuKey = false;
                        return;
                    }
                    if (Settings.ShowToolbar == false) {
                        mnu.Visible = !mnu.Visible;
                    }
                    if (mnu.Visible) {
                        mnu.Select();
                        mnu.Items[0].Select();
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | (Keys)93:
                    tabFiles.ContextMenuStrip.Show(tabFiles, tabFiles.Width / 2, tabFiles.Height / 2);
                    break;

                case Keys.Control | Keys.B:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

            }

            tmrQuickSave.Enabled = true;
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
            } else if (Settings.TrayOnMinimize) { //Window has been minimized.
                this.Visible = false;
                this.Close();
            }

            _form_ResizeReentry = false;
        }

        private void Form_VisibleChanged(object sender, EventArgs e) {
            if ((_findForm == null) || (_findForm.IsDisposed) || (_findForm.Visible == false)) {
            } else {
                _findForm.Close();
            }

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
                    TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                    txt.Refresh();
                    txt.Select();
                    txt.Focus();
                    SetSelectedTab(tabFiles.SelectedTab);
                } catch (IOException ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot open file.\n\n{0}", ex.Message));
                    tabFiles.SelectedTab = this._CurrSelectedTab;
                }
                tmrQuickSave.Enabled = true;
            }
        }


        #region Menu

        private void mnuNew_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            using (FileNewForm frm = new FileNewForm(tabFiles.CurrentDirectory)) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    try {
                        tabFiles.AddTab(frm.Title, frm.IsRichText);
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
                using (var frm = new FileRenameForm(tabFiles.CurrentDirectory, tabFiles.SelectedTab.Title)) {
                    try {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            tabFiles.SelectedTab.Rename(frm.NewTitle);
                            tabFiles.WriteOrderedTitles();
                        }
                    } catch (Exception ex) {
                        Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot rename file.\n\n{0}", ex.Message));
                    }
                }
                tmrQuickSave.Enabled = true;
            }
        }


        private void mnuPrintPreview_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                try {
                    using (var ol = new Medo.Drawing.Printing.FullText(tabFiles.SelectedTab.Title, 10.0f, 10.0f, 20.0f, 10.0f)) {
                        ol.BeginPrint += Document_BeginPrint;
                        ol.StartPrintPage += Document_PrintPage;
                        ol.Font = Settings.DisplayFont;
                        ol.Text = tabFiles.SelectedTab.TextBox.Text;
                        using (var f = new Medo.Windows.Forms.PrintPreviewDialog(ol.Document)) {
                            f.ShowDialog(this);
                        }
                    }
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation failed.\n\n{0}", ex.Message));
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuPrint_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                try {
                    using (var ol = new Medo.Drawing.Printing.FullText(tabFiles.SelectedTab.Title, 10.0f, 10.0f, 20.0f, 10.0f)) {
                        ol.BeginPrint += Document_BeginPrint;
                        ol.StartPrintPage += Document_PrintPage;
                        ol.Font = Settings.DisplayFont;
                        ol.Text = tabFiles.SelectedTab.TextBox.Text;
                        ol.Print();
                    }
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format("Operation failed.\n\n{0}", ex.Message));
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
                if (tf.IsRichTextFormat) {
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
                        if (f.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
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
                if (tf.IsRichTextFormat) {
                    ToogleStyle(tf.TextBox, FontStyle.Bold);
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuItalic_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    ToogleStyle(tf.TextBox, FontStyle.Italic);
                }
                tmrQuickSave.Enabled = true;
            }
        }

        private void mnuUnderline_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tmrQuickSave.Enabled = false;
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
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
                if (tf.IsRichTextFormat) {
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


        private void mnuFind_Click(object sender, EventArgs e) {
            FindFirst();
        }

        private void mnuAlwaysOnTop_Click(object sender, EventArgs e) {
            mnuAlwaysOnTop.Checked = !mnuAlwaysOnTop.Checked;
            Settings.DisplayAlwaysOnTop = mnuAlwaysOnTop.Checked;
            this.TopMost = Settings.DisplayAlwaysOnTop;
        }


        private void mnuFolder_DropDownOpening(object sender, EventArgs e) {
            mnuFolder.DropDownItems.Clear();
            mnuFolder.DropDownItems.Add(new ToolStripMenuItem("(Default)", null, mnuFolder_Click) { Tag = "" });
            foreach (var folder in TabFiles.GetSubFolders()) {
                mnuFolder.DropDownItems.Add(new ToolStripMenuItem(folder, null, mnuFolder_Click) { Tag = folder });
            }
            foreach (ToolStripMenuItem item in mnuFolder.DropDownItems) {
                item.Enabled = !string.Equals(tabFiles.CurrentFolder, (string)item.Tag, StringComparison.OrdinalIgnoreCase);
            }
            mnuFolder.DropDownItems.Add(new ToolStripSeparator());
            mnuFolder.DropDownItems.Add(new ToolStripMenuItem("Edit folders", null, mnuFolderEdit_Click));
        }

        private void mnuFolder_Click(object sender, EventArgs e) {
            try {
                tabFiles.FolderSave();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format("Cannot save folder.\n\n{0}", ex.Message));
                return;
            }
            var oldFolder = tabFiles.CurrentFolder;
            var newFolder = ((ToolStripMenuItem)sender).Tag as string;
            if (string.Equals(oldFolder, newFolder, StringComparison.OrdinalIgnoreCase) == false) {
                tabFiles.FolderOpen(newFolder);
                mnuFolder.Text = string.IsNullOrEmpty(tabFiles.CurrentFolder) ? "(Default)" : tabFiles.CurrentFolder;
                Settings.LastFolder = tabFiles.CurrentFolder;
            }
        }

        private void mnuFolderEdit_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            tabFiles.Enabled = false;
            tabFiles.FolderSave();
            using (var frm = new FolderEditForm(tabFiles.CurrentFolder)) {
                frm.ShowDialog(this);
                if (string.Equals(tabFiles.CurrentFolder, frm.CurrentFolder, StringComparison.Ordinal) == false) {
                    tabFiles.FolderOpen(frm.CurrentFolder, false);
                    mnuFolder.Text = string.IsNullOrEmpty(tabFiles.CurrentFolder) ? "(Default)" : tabFiles.CurrentFolder;
                    Settings.LastFolder = tabFiles.CurrentFolder;
                }
            }
            tabFiles.Enabled = true;
            tmrQuickSave.Enabled = true;
        }


        private void mnuOptions_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            using (var frm = new OptionsForm()) {
                SaveAllChanged();
                this.tmrUpdateToolbar.Enabled = false;
                RefreshAll(null, null);
                if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                    if (Settings.DisplayMinimizeMaximizeButtons) {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    } else {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                    }
                    this.ShowInTaskbar = Settings.DisplayShowInTaskbar;
                    this.TopMost = Settings.DisplayAlwaysOnTop;
                    RefreshAll(null, null);
                    Form_Resize(null, null);
                    this.tmrUpdateToolbar.Enabled = Settings.ShowToolbar;
                    tmrQuickSave.Interval = Settings.QuickSaveInterval;

                    if ((Settings.CarbonCopyUse)) {
                        for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                            TabFile currTab = (TabFile)tabFiles.TabPages[i];
                            currTab.SaveCarbonCopy();
                        }
                    }
                }
            }
            tmrQuickSave.Enabled = true;
        }


        private void mnuAppFeedback_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            Medo.Diagnostics.ErrorReport.TopMost = this.TopMost;
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("http://jmedved.com/ErrorReport/"));
            tmrQuickSave.Enabled = true;
        }

        private void mnuAppUpgrade_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            using (var frm = new UpgradeForm()) {
                frm.ShowDialog(this);
            }
            tmrQuickSave.Enabled = true;
        }

        private void mnuAppDonate_Click(object sender, EventArgs e) {
            Process.Start("http://www.jmedved.com/donate/");
        }

        private void mnuAppAbout_Click(object sender, EventArgs e) {
            tmrQuickSave.Enabled = false;
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("http://www.jmedved.com/qtext/"));

            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                txt.Focus();
            }
            tmrQuickSave.Enabled = true;
        }

        #endregion


        #region Tabs menu

        private void mnxTab_Opening(object sender, CancelEventArgs e) {
            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;
            bool isTabPlainText = isTabSelected && (tabFiles.SelectedTab.IsRichTextFormat == false);

            mnxTabReopen.Enabled = isTabSelected;
            mnxTabSaveNow.Enabled = isTabSelected;
            mnxTabDelete.Enabled = isTabSelected;
            mnxTabRename.Enabled = isTabSelected;
            mnxTabConvertPlain.Enabled = isTabRichText;
            mnxTabConvertRich.Enabled = isTabPlainText;
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
                var currTab = tabFiles.SelectedTab;
                if (currTab.TextBox.Text.Length > 0) {
                    if (Medo.MessageBox.ShowQuestion(this, string.Format(CultureInfo.CurrentUICulture, "Do you really want to delete \"{0}\"?", tabFiles.SelectedTab.Title), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                        return;
                    }
                }
                try {
                    tabFiles.RemoveTab(tabFiles.SelectedTab);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot delete file.\n\n{0}", ex.Message));
                }
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

        private void mnxTabMoveTo_DropDownOpening(object sender, EventArgs e) {
            mnxTabMoveTo.DropDownItems.Clear();
            mnxTabMoveTo.DropDownItems.Add(new ToolStripMenuItem("(Default)", null, mnxTabMoveTo_Click) { Tag = null });
            foreach (var folder in TabFiles.GetSubFolders()) {
                mnxTabMoveTo.DropDownItems.Add(new ToolStripMenuItem(folder, null, mnxTabMoveTo_Click) { Tag = folder });
            }
            foreach (ToolStripMenuItem item in mnxTabMoveTo.DropDownItems) {
                item.Enabled = !string.Equals(tabFiles.CurrentFolder, (string)item.Tag, StringComparison.OrdinalIgnoreCase);
            }
        }

        private void mnxTabMoveTo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var folder = ((ToolStripMenuItem)sender).Tag as string;
                tabFiles.MoveTab(tabFiles.SelectedTab, folder);
            }
        }

        private void mnxTabOpenContainingFolder_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                string file = tabFiles.SelectedTab.CurrentFile.FullName;
                var exe = new ProcessStartInfo("explorer.exe", "/select,\"" + file + "\"");
                Process.Start(exe);
            } else {
                var exe = new ProcessStartInfo("explorer.exe", "\"" + tabFiles.CurrentDirectory + "\"");
                Process.Start(exe);
            }
        }

        #endregion


        #region Text menu

        private void mnxText_Opening(object sender, CancelEventArgs e) {
            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;
            bool isTabPlainText = isTabSelected && (tabFiles.SelectedTab.IsRichTextFormat == false);
            bool isTextSelected = isTabSelected && (tabFiles.SelectedTab.TextBox.SelectedText.Length > 0);

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

            mnxTextFormatSortAsc.Enabled = isTextSelected;
            mnxTextFormatSortDesc.Enabled = isTextSelected;
            mnxTextFormatConvertLower.Enabled = isTextSelected;
            mnxTextFormatConvertUpper.Enabled = isTextSelected;
            mnxTextFormatConvertTitleCase.Enabled = isTextSelected;
            mnxTextFormatConvertDrGrammar.Enabled = isTextSelected;
            mnxTextFormat.Enabled = mnxTextFormatSortAsc.Enabled || mnxTextFormatSortDesc.Enabled || mnxTextFormatConvertLower.Enabled || mnxTextFormatConvertUpper.Enabled || mnxTextFormatConvertTitleCase.Enabled || mnxTextFormatConvertDrGrammar.Enabled;
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

        private void mnxTextSortAsc_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = ((TabFile)tabFiles.SelectedTab).TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                string[] selSplit = txt.SelectedText.Split(new string[] { new string(new char[] { System.Convert.ToChar(13), System.Convert.ToChar(10) }), System.Convert.ToChar(13).ToString(), System.Convert.ToChar(10).ToString() }, StringSplitOptions.None);
                Array.Sort(selSplit, new SortAZ());
                txt.SelectedText = string.Join(Environment.NewLine, selSplit);

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }

        }

        private void mnxTextSortDesc_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                string[] selSplit = txt.SelectedText.Split(new string[] { new string(new char[] { System.Convert.ToChar(13), System.Convert.ToChar(10) }), System.Convert.ToChar(13).ToString(), System.Convert.ToChar(10).ToString() }, StringSplitOptions.None);
                Array.Sort(selSplit, new SortZA());
                txt.SelectedText = string.Join(Environment.NewLine, selSplit);

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextConvertLower_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = txt.SelectedText.ToLower();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextConvertUpper_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = txt.SelectedText.ToUpper();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextConvertTitle_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToLower(txt.SelectedText));

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnxTextConvertDrGrammar_Click(object sender, EventArgs e) {
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

            var currentFolder = tabFiles.CurrentFolder;
            try {
                tabFiles.FolderOpen(Settings.LastFolder);
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Cannot load folder.\n\n{0}", ex.Message));
                tabFiles.FolderOpen(currentFolder);
            }
            mnuFolder.Text = string.IsNullOrEmpty(tabFiles.CurrentFolder) ? "(Default)" : tabFiles.CurrentFolder;
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

        private int _PageNumber;

        private void Document_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e) {
            this._PageNumber = 1;
        }

        private void Document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            try {
                using (System.Drawing.Font font = new System.Drawing.Font("Tahoma", 10)) {
                    e.Graphics.DrawString(((Medo.Drawing.Printing.FullText)sender).Document.DocumentName, font, System.Drawing.Brushes.Black, 0, -10 / 25.4f * 100);

                    //used now only for height measurements
                    string textC = global::Medo.Reflection.EntryAssembly.Title;
                    System.Drawing.SizeF sizeC = e.Graphics.MeasureString(textC, font);
                    if (Settings.PrintApplicationName) {
                        e.Graphics.DrawString(textC, font, System.Drawing.Brushes.Black, (e.MarginBounds.Width - sizeC.Width) / 2, -10 / 25.4f * 100);
                    }

                    string textR = this._PageNumber.ToString();
                    System.Drawing.SizeF sizeR = e.Graphics.MeasureString(textR, font);
                    e.Graphics.DrawString(textR, font, System.Drawing.Brushes.Black, e.MarginBounds.Right - sizeR.Width, -10 / 25.4f * 100);

                    e.Graphics.DrawLine(System.Drawing.Pens.Black, 0.0f, Convert.ToSingle(-10 / 25.4 * 100 + sizeC.Height), e.MarginBounds.Right, Convert.ToSingle(-10 / 25.4 * 100 + sizeC.Height));
                }

                this._PageNumber += 1;
            } catch (ArgumentException) {
                Medo.MessageBox.ShowWarning(this, "Print preview cannot be shown.\n\nIs there a printer connected and up?");
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
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;
            bool isTabPlainText = isTabSelected && (tabFiles.SelectedTab.IsRichTextFormat == false);

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
                mnuBold.Checked = (tabFiles.SelectedTab.TextBox.SelectionFont != null) && (tabFiles.SelectedTab.TextBox.SelectionFont.Bold);
                mnuItalic.Checked = (tabFiles.SelectedTab.TextBox.SelectionFont != null) && (tabFiles.SelectedTab.TextBox.SelectionFont.Italic);
                mnuUnderline.Checked = (tabFiles.SelectedTab.TextBox.SelectionFont != null) && (tabFiles.SelectedTab.TextBox.SelectionFont.Underline);
                mnuStrikeout.Checked = (tabFiles.SelectedTab.TextBox.SelectionFont != null) && (tabFiles.SelectedTab.TextBox.SelectionFont.Strikeout);
            }
        }


        int nextIndexToCheck = 0;

        private void tmrCheckFileUpdate_Tick(object sender, EventArgs e) {
            if (tabFiles.Enabled == false) { return; }
            tmrCheckFileUpdate.Enabled = false;
            if (tabFiles.TabCount > 0) {
                nextIndexToCheck = nextIndexToCheck % tabFiles.TabPages.Count;
                var currTab = (TabFile)tabFiles.TabPages[nextIndexToCheck];
                var fi = new FileInfo(currTab.CurrentFile.FullName);
                if (fi.LastWriteTimeUtc != currTab.LastWriteTimeUtc) { //change
                    if (currTab.IsChanged) {
                        currTab.Save();
                    } else {
                        currTab.Reopen();
                    }
                }
                nextIndexToCheck += 1;
            } else {
                nextIndexToCheck = 0;
            }
            tmrCheckFileUpdate.Enabled = true;
        }

        #endregion


        private FindForm _findForm;

        private void FindFirst() {
            if ((_findForm == null) || (_findForm.IsDisposed)) {
                _findForm = new FindForm(this.tabFiles);
                _findForm.Left = this.Left + (this.Width - _findForm.Width) / 2;
                _findForm.Top = this.Top + (this.Height - _findForm.Height) / 2;
            }
            if (!_findForm.Visible) {
                _findForm.Show(this);
            }
            _findForm.Activate();
        }

        private void FindNext() {
            if ((_findForm == null) || (_findForm.IsDisposed) || (!_findForm.Visible)) {
                if ((tabFiles.SelectedTab != null) && (!string.IsNullOrEmpty(SearchStatus.Text))) {
                    TabFile tf = tabFiles.SelectedTab;
                    if (tf.Find(SearchStatus.Text, SearchStatus.CaseSensitive) == false) {
                        Medo.MessageBox.ShowInformation(this, "Text \"" + SearchStatus.Text + "\" cannot be found.");
                    }
                }
            } else {
                _findForm.FindNext();
            }
        }

    }
}
