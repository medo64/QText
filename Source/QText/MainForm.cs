using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;

namespace QText {

    internal partial class MainForm : Form {

        internal bool _suppressMenuKey = false;
        internal float _dpiX;
        internal float _dpiY;
        internal float _dpiRatioX;
        internal float _dpiRatioY;

        public MainForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            using (var g = this.CreateGraphics()) {
                _dpiX = g.DpiX;
                _dpiY = g.DpiY;
                _dpiRatioX = Convert.ToSingle(Math.Round(_dpiX / 96, 2));
                _dpiRatioY = Convert.ToSingle(Math.Round(_dpiY / 96, 2));
                System.Diagnostics.Trace.TraceInformation("DPI: {0}x{1}; Ratio:{2}x{3}", this._dpiX, this._dpiY, this._dpiRatioX, this._dpiRatioY);
            }
            if (Settings.ZoomToolbarWithDpiChange) {
                mnu.ImageScalingSize = new Size(Convert.ToInt32(16 * _dpiRatioX), Convert.ToInt32(16 * _dpiRatioY));
                mnu.Scale(new SizeF(_dpiRatioX, _dpiRatioY));
            }

            tmrQuickAutoSave.Interval = Settings.QuickAutoSaveSeconds * 1000;

            tabFiles.Multiline = Settings.DisplayMultilineTabHeader;
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
                    //this.Visible = false;
                    break;
            }
#endif

            try {
                tabFiles.DirectorySave();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(null, "QText : File saving failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
            }
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            Application.Exit();
        }


        private void Form_Activated(object sender, EventArgs e) {
            this.tmrUpdateToolbar.Enabled = Settings.ShowToolbar;
            if (tabFiles == null) { return; }

            if (tabFiles.SelectedTab != null) {
                if (tabFiles.SelectedTab.TextBox != null) {
                    tabFiles.SelectedTab.TextBox.Focus();
                }
            }
        }

        private void Form_Deactivate(object sender, EventArgs e) {
            this.tmrUpdateToolbar.Enabled = false;
        }


        private void Form_KeyDown(object sender, KeyEventArgs e) {
            tmrQuickAutoSave.Enabled = false;

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


                case Keys.Control | Keys.D1:
                    if (tabFiles.TabPages.Count >= 1) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                    }
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.D2:
                    if (tabFiles.TabPages.Count >= 2) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[1];
                    }
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.D3:
                    if (tabFiles.TabPages.Count >= 3) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[2];
                    }
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.D4:
                    if (tabFiles.TabPages.Count >= 4) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[3];
                    }
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.D5:
                    if (tabFiles.TabPages.Count >= 5) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[4];
                    }
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.D6:
                    if (tabFiles.TabPages.Count >= 6) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[5];
                    }
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.D7:
                    if (tabFiles.TabPages.Count >= 7) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[6];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.D8:
                    if (tabFiles.TabPages.Count >= 8) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[7];
                    }
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.D9:
                    if (tabFiles.TabPages.Count >= 9) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[8];
                    }
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Control | Keys.D0:
                    if (tabFiles.TabPages.Count >= 10) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[9];
                    }
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

                case Keys.Alt | Keys.Left:
                case Keys.Control | Keys.PageDown:
                    if ((tabFiles.TabPages.Count >= 1) && (tabFiles.SelectedTab == null)) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                    }
                    if (tabFiles.TabPages.Count >= 2) {
                        int i = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab);
                        i = (tabFiles.TabPages.Count + i - 1) % tabFiles.TabPages.Count;
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[i];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;

                case Keys.Alt | Keys.Right:
                case Keys.Control | Keys.PageUp:
                    if ((tabFiles.TabPages.Count >= 1) && (tabFiles.SelectedTab == null)) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                    }
                    if ((tabFiles.TabPages.Count >= 2)) {
                        int i = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab);
                        i = (i + 1) % tabFiles.TabPages.Count;
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[i];
                    }
                    this._suppressMenuKey = true;
                    e.Handled = true; e.SuppressKeyPress = true; break;



                case Keys.Alt | Keys.Menu: //just to prevent suppressing menu key
                    break;

                default:
                    if (e.Alt) {
                        this._suppressMenuKey = true;
                    }
                    break;
            }

            if (Settings.EnableQuickAutoSave) {
                tmrQuickAutoSave.Enabled = true;
            }
        }

        private void Form_KeyUp(object sender, KeyEventArgs e) {
            tmrQuickAutoSave.Enabled = false;

            switch (e.KeyData) {

                case Keys.Menu:
                    if (this._suppressMenuKey) {
                        this._suppressMenuKey = false;
                        return;
                    }
                    mnu.Visible = (Settings.ShowToolbar) ? true : !mnu.Visible;
                    break;

                case Keys.Control | Keys.B:
                    break;

            }

            if (Settings.EnableQuickAutoSave) { tmrQuickAutoSave.Enabled = true; }
        }


        private void Form_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                Rectangle rect = tabFiles.ClientRectangle;
                if (tabFiles.TabPages.Count > 0) { rect = tabFiles.GetTabRect(tabFiles.TabCount - 1); }
                rect = new Rectangle(tabFiles.Left + rect.Left, tabFiles.Top + rect.Top, rect.Width, rect.Height);
                if ((e.Y >= rect.Top) && (e.Y <= rect.Bottom) && (e.X >= rect.Right)) {
                    tabFiles.SelectedTab = null;
                    mnxTab.Show(tabFiles, e.X - tabFiles.Left, e.Y - tabFiles.Top);
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

            tmrAutoSave.Enabled = this.Visible;
            tmrCheckFileUpdate.Enabled = this.Visible;
            tmrQuickAutoSave.Enabled = this.Visible;
            tmrUpdateToolbar.Enabled = this.Visible;
        }


        private void tabFiles_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                    if ((tabFiles.GetTabRect(i).Contains(e.X, e.Y))) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[i];
                        mnxTab.Show(tabFiles, e.X, e.Y);
                        return;
                    }
                }

            }
        }

        private void tabFiles_SelectedIndexChanged(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                try {
                    TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                    txt.Refresh();
                    txt.Select();
                    txt.Focus();
                    SetSelectedTab(tabFiles.SelectedTab);
                } catch (IOException ex) {
                    Medo.MessageBox.ShowError(this, "Cannot open file!\n\n" + ex.Message);
                    tabFiles.SelectedTab = this._CurrSelectedTab;
                }
            }
        }



        #region Menu

        private void mnuNew_Click(object sender, EventArgs e) {
            using (NewFileForm frm = new NewFileForm()) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    try {
                        tabFiles.NewTab(frm.Title, frm.IsRichText);
                    } catch (Exception ex) {
                        Medo.MessageBox.ShowWarning(this, string.Format(CultureInfo.CurrentUICulture, "Operation failed.\n\n{0}", ex.Message), MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void mnuSaveNow_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                try {
                    tabFiles.SelectedTab.Save();
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }

        private void mnuRename_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                try {
                    using (var frm = new RenameFileForm(tabFiles.SelectedTab.Title)) {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            tabFiles.SelectedTab.Rename(frm.Title);
                        }
                    }
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }


        private void mnuPrintPreview_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
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
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
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
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }


        private void mnuCut_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Cut(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnuCopy_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Copy(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnuPaste_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Paste(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }


        private void mnuFont_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
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
            }
        }

        private void mnuBold_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    ToogleStyle(tf.TextBox, FontStyle.Bold);
                }
            }
        }

        private void mnuItalic_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    ToogleStyle(tf.TextBox, FontStyle.Italic);
                }
            }
        }

        private void mnuUnderline_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    if (tf.TextBox.SelectionFont != null) {
                        ToogleStyle(tf.TextBox, FontStyle.Underline);
                    }
                }
            }
        }

        private void mnuStrikeout_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    if (tf.TextBox.SelectionFont != null) {
                        ToogleStyle(tf.TextBox, FontStyle.Strikeout);
                    }
                }
            }
        }


        private void mnuUndo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.Undo();
            }
        }

        private void mnuRedo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.Redo();
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


        private void mnuOptions_Click(object sender, EventArgs e) {
            using (var frm = new OptionsForm()) {
                tmrQuickAutoSave.Enabled = false;
                SaveAllChanged();
                this.tmrUpdateToolbar.Enabled = false;
                RefreshAll(null, null);
                if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                    tabFiles.Multiline = Settings.DisplayMultilineTabHeader;
                    if (Settings.DisplayMinimizeMaximizeButtons) {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    } else {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                    }
                    if (Settings.ZoomToolbarWithDpiChange) {
                        mnu.ImageScalingSize = new Size(Convert.ToInt32(16 * _dpiRatioX), Convert.ToInt32(16 * _dpiRatioY));
                        mnu.Scale(new SizeF(_dpiRatioX, _dpiRatioY));
                    } else {
                        float rx = Convert.ToSingle(16 / mnu.ImageScalingSize.Width);
                        float ry = Convert.ToSingle(16 / mnu.ImageScalingSize.Height);
                        mnu.ImageScalingSize = new Size(16, 16);
                        mnu.Scale(new SizeF(rx, ry));
                    }
                    this.ShowInTaskbar = Settings.DisplayShowInTaskbar;
                    this.TopMost = Settings.DisplayAlwaysOnTop;
                    RefreshAll(null, null);
                    Form_Resize(null, null);
                    this.tmrUpdateToolbar.Enabled = Settings.ShowToolbar;
                    tmrQuickAutoSave.Interval = Settings.QuickAutoSaveSeconds * 1000;

                    if ((Settings.CarbonCopyUse)) {
                        for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                            TabFile currTab = (TabFile)tabFiles.TabPages[i];
                            currTab.SaveCarbonCopy();
                        }
                    }
                }
            }
        }


        private void mnuAppFeedback_Click(object sender, EventArgs e) {
            Medo.Diagnostics.ErrorReport.TopMost = this.TopMost;
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("http://jmedved.com/ErrorReport/"));
        }

        private void mnuAppUpgrade_Click(object sender, EventArgs e) {
            using (var frm = new UpgradeForm()) {
                frm.ShowDialog(this);
            }
        }

        private void mnuAppDonate_Click(object sender, EventArgs e) {
            Process.Start("http://www.jmedved.com/donate/");
        }

        private void mnuAppAbout_Click(object sender, EventArgs e) {
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("http://www.jmedved.com/qtext/"));

            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                txt.Focus();
            }
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
            mnxTabPrintPreview.Enabled = isTabSelected;
            mnxTabPrint.Enabled = isTabSelected;
            mnxTabOpenContainingFolder.Enabled = isTabSelected;
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
                                Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
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
                    if (Medo.MessageBox.ShowQuestion(this, "Are you sure?", Medo.Reflection.EntryAssembly.Title + " : Delete", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                        return;
                    }
                }
                try {
                    tabFiles.SelectedTab = GetNextTab();
                    currTab.Delete();
                    tabFiles.TabPages.Remove(currTab);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
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

        private void mnxTabOpenContainingFolder_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                string file = tabFiles.SelectedTab.FullFileName;
                var exe = new ProcessStartInfo("explorer.exe", "/select,\"" + file + "\"");
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
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnxTextCopyPlain_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Copy(true);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnxTextPastePlain_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Paste(true);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
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

        private TabFile GetNextTab() {
            int tindex = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab) + 1; //select next tab
            if (tindex >= tabFiles.TabPages.Count) {
                tindex -= 2; //go to one in front of it
            }
            if ((tindex > 0) && (tindex < tabFiles.TabPages.Count)) {
                return (TabFile)tabFiles.TabPages[tindex];
            }
            return null;
        }

        private void SaveAllChanged() {
            try {
                tabFiles.SaveAll();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
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

            try {
                tabFiles.DirectoryOpen();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Files could not be loaded." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
            }
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

        private void tmrAutoSave_Tick(object sender, EventArgs e) {
            for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                try {
                    TabFile tf = (TabFile)tabFiles.TabPages[i];
                    if (tf.GetIsEligibleForSave(Settings.FilesAutoSaveInterval)) {
                        tf.Save();
                    }
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
            //TODO fswLocationTxt.EnableRaisingEvents = (tabFiles.TabPages.Count > 0);
        }

        private void tmrQuickAutoSave_Tick(object sender, EventArgs e) {
            tmrQuickAutoSave.Enabled = false;
            Debug.WriteLine("QText: QuickAutoSave");
            for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                try {
                    TabFile tf = (TabFile)tabFiles.TabPages[i];
                    if (tf.IsChanged) {
                        tf.Save();
                        Trace.WriteLine("QText: QuickAutoSave: Saved " + tf.Title + ".");
                        tmrQuickAutoSave.Enabled = true;
                        break;
                    }
                } catch (Exception) { }
            }
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
            tmrCheckFileUpdate.Enabled = false;
            if (tabFiles.TabCount > 0) {
                nextIndexToCheck = nextIndexToCheck % tabFiles.TabPages.Count;
                var currTab = (TabFile)tabFiles.TabPages[nextIndexToCheck];
                var fi = new FileInfo(currTab.FullFileName);
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



        //private void mnuViewZoomIn_Click(object sender, EventArgs e) {
        //    if (tabFiles.SelectedTab != null) {
        //        tabFiles.SelectedTab.ZoomIn();
        //    }
        //}

        //private void mnuViewZoomOut_Click(object sender, EventArgs e) {
        //    if (tabFiles.SelectedTab != null) {
        //        tabFiles.SelectedTab.ZoomOut();
        //    }
        //}

        //private void mnuViewZoomReset_Click(object sender, EventArgs e) {
        //    if (tabFiles.SelectedTab != null) {
        //        tabFiles.SelectedTab.ZoomReset();
        //    }
        //}


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
