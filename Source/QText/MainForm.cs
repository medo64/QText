using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.IO;

namespace QText {
    public partial class MainForm : Form {

        internal bool _suppressMenuKey = false;
        internal float _dpiX;
        internal float _dpiY;
        internal float _dpiRatioX;
        internal float _dpiRatioY;
        internal FindForm _findForm;
        internal FileOrder FileOrder = new FileOrder();


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
                tls.ImageScalingSize = new Size(Convert.ToInt32(16 * _dpiRatioX), Convert.ToInt32(16 * _dpiRatioY));
                tls.Scale(new SizeF(_dpiRatioX, _dpiRatioY));
            }

            // Add any initialization after the InitializeComponent() call.
            tmrQuickAutoSave.Interval = Settings.QuickAutoSaveSeconds * 1000;

            mnuFileClose.Visible = true;
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


        private void MainForm_Activated(object sender, EventArgs e) {
            this.tmrUpdateToolbar.Enabled = Settings.DisplayShowToolbar;
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.TextBox.Focus();
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e) {
            mnu.Visible = Settings.ShowMenu; //TODO: Check this on XP when compiled
            Medo.Windows.Forms.State.Save(this);
            this.tmrUpdateToolbar.Enabled = false;
        }

        private void MainForm_Load(object sender, EventArgs e) {
            mnu.Visible = Settings.ShowMenu;
            mnuViewRefresh_Click(null, null);
            MainForm_Resize(null, null);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            Medo.Windows.Forms.State.Save(this);

            mnu_Leave(null, null);
            try {
                if (Settings.SaveOnHide) { SaveAllChanged(); }
                tabFiles.SaveAll();
                this.FileOrder.Save(tabFiles, e.CloseReason == CloseReason.WindowsShutDown);
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
            }

            if (e.Cancel) { return; }

#if !DEBUG 
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                this.Hide();
            } else {
                try {
                    Application.Exit();
                } catch (Exception) { }
            }
#else
            Application.Exit();
#endif
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            Debug.WriteLine("MainForm.KeyDown:" + e.KeyData.ToString());
            tmrQuickAutoSave.Enabled = false;

            switch (e.KeyData) {

                case Keys.Control | Keys.D1:
                    if (tabFiles.TabPages.Count >= 1) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.D2:
                    if (tabFiles.TabPages.Count >= 2) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[1];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.D3:
                    if (tabFiles.TabPages.Count >= 3) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[2];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.D4:
                    if (tabFiles.TabPages.Count >= 4) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[3];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.D5:
                    if (tabFiles.TabPages.Count >= 5) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[4];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.D6:
                    if (tabFiles.TabPages.Count >= 6) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[5];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

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
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.D9:
                    if (tabFiles.TabPages.Count >= 9) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[8];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.D0:
                    if (tabFiles.TabPages.Count >= 10) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[9];
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Escape:
                    mnuFileClose_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.Back:
                    mnuEditUndo_Click(null, null);
                    this._suppressMenuKey = true;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.Shift | Keys.Back:
                    mnuEditRedo_Click(null, null);
                    this._suppressMenuKey = true;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

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
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

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
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.F:
                    mnu.Visible = true;
                    mnuFile.ShowDropDown();
                    this._suppressMenuKey = true;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.E:
                    mnu.Visible = true;
                    mnuEdit.ShowDropDown();
                    this._suppressMenuKey = true;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.V:
                    mnu.Visible = true;
                    mnuView.ShowDropDown();
                    this._suppressMenuKey = true;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.O:
                    mnu.Visible = true;
                    mnuFormat.ShowDropDown();
                    this._suppressMenuKey = true;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.T:
                    mnu.Visible = true;
                    mnuTools.ShowDropDown();
                    this._suppressMenuKey = true;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Alt | Keys.H:
                    mnu.Visible = true;
                    mnuHelp.ShowDropDown();
                    this._suppressMenuKey = true;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

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

        private void MainForm_KeyUp(object sender, KeyEventArgs e) {
            tmrQuickAutoSave.Enabled = false;

            Debug.WriteLine("MainForm.KeyUp: " + e.KeyData.ToString());
            switch (e.KeyData) {

                case Keys.Menu:
                    if (this._suppressMenuKey) {
                        Debug.WriteLine("Suppress.");
                        this._suppressMenuKey = false;
                        return;
                    }
                    if (!Settings.ShowMenu) {
                        if (mnu.Visible == true) { return; }
                        mnu.Visible = true;
                        mnu.Select();
                        mnuFile.Select();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Control | Keys.B:
                    break;

            }

            if (Settings.EnableQuickAutoSave) {
                tmrQuickAutoSave.Enabled = true;
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e) {
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

        private bool frmMain_Resize_Reentry = false;
        private void MainForm_Resize(object sender, EventArgs e) {
            if (frmMain_Resize_Reentry) { return; }
            frmMain_Resize_Reentry = true;

            if (this.Visible) { Medo.Windows.Forms.State.Save(this); }
            if (this.WindowState != FormWindowState.Minimized) {
                tls.Visible = Settings.DisplayShowToolbar;

                int newTop = 0;
                if (Settings.DisplayShowToolbar) {
                    newTop += tls.Height;
                }
                if (Settings.ShowMenu || mnu.Visible) {
                    newTop += mnu.Height;
                }

                if (Settings.DisplayShowToolbar == true) {
                    if (mnu.Visible) {
                        tabFiles.Left = this.ClientRectangle.Left;
                        tabFiles.Top = newTop;
                        tabFiles.Width = this.ClientRectangle.Width;
                        tabFiles.Height = this.ClientRectangle.Height - tabFiles.Top;
                    } else {
                        tabFiles.Left = this.ClientRectangle.Left;
                        tabFiles.Top = newTop;
                        tabFiles.Width = this.ClientRectangle.Width;
                        tabFiles.Height = this.ClientRectangle.Height - tabFiles.Top;
                    }
                } else {
                    tabFiles.Left = this.ClientRectangle.Left;
                    tabFiles.Top = newTop;
                    tabFiles.Width = this.ClientRectangle.Width;
                    tabFiles.Height = this.ClientRectangle.Height - tabFiles.Top;
                }
                //window has been minimized
            } else if (Settings.TrayOnMinimize) {
                if (Settings.SaveOnHide) { SaveAllChanged(); }
                this.Hide();
                tabFiles.SaveAll();
                this.FileOrder.Save(tabFiles, true);
            }

            frmMain_Resize_Reentry = false;
        }

        private void MainForm_VisibleChanged(object sender, EventArgs e) {
            if ((_findForm == null) || (_findForm.IsDisposed) || (_findForm.Visible == false)) {
            } else {
                _findForm.Close();
            }
        }


        private void tabFiles_ChangedOrder(object sender, EventArgs e) {
            this.FileOrder.Save(tabFiles, true);
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
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                txt.Refresh();
                txt.Select();
                txt.Focus();
            }
            SetSelectedTab(tabFiles.SelectedTab);
        }



        #region Menu

        private void mnu_Leave(object sender, EventArgs e) {
            if (!Settings.ShowMenu) {
                if (mnu.Visible != false) {
                    mnu.Visible = false;
                    MainForm_Resize(null, null);
                }
            }
        }

        private void mnu_VisibleChanged(object sender, EventArgs e) {
            MainForm_Resize(null, null);
        }


        private void mnuFile_DropDownOpening(object sender, EventArgs e) {
            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;

            mnuFileNew.Enabled = true;
            mnuFileReopen.Enabled = isTabSelected;
            mnuFileConvertToPlainText.Enabled = isTabSelected && isTabRichText;
            mnuFileConvertToRichText.Enabled = isTabSelected && (isTabRichText == false);
            mnuFileSaveNow.Enabled = isTabSelected;
            mnuFileSaveAll.Enabled = isTabSelected;
            mnuFileDelete.Enabled = isTabSelected;
            mnuFileRename.Enabled = isTabSelected;
            mnuFileHide.Enabled = isTabSelected;
            mnuFileUnhide.Enabled = true;
            mnuFilePrintPreview.Enabled = isTabSelected;
            mnuFilePrint.Enabled = isTabSelected;
            mnuFileClose.Enabled = true;
            mnuFileExit.Enabled = true;
        }

        private void mnuFileNew_Click(object sender, EventArgs e) {
            this.FileOrder.Save(tabFiles, true);

            using (NewFileForm frm = new NewFileForm("")) {
                if ((frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)) {
                    try {
                        TabFile t = default(TabFile);
                        if ((frm.IsRichText)) {
                            t = new TabFile(System.IO.Path.Combine(Settings.FilesLocation, frm.FileName) + ".rtf", App.Form.mnxTextBox, true);
                        } else {
                            t = new TabFile(System.IO.Path.Combine(Settings.FilesLocation, frm.FileName) + ".txt", App.Form.mnxTextBox, true);
                        }
                        tabFiles.TabPages.Add(t);
                        tabFiles.SelectedTab = t;
                    } catch (Exception ex) {
                        Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                    }
                }
            }

            this.FileOrder.Save(tabFiles, true);
        }

        private void mnuFileReopen_Click(object sender, EventArgs e) {
            this.FileOrder.Save(tabFiles, true);

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

        private void mnuFileConvertToPlainText_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                if (Medo.MessageBox.ShowQuestion(this, "Conversion will remove all formating (font, style, etc.). Do you want to continue?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    tabFiles.SelectedTab.ConvertToPlainText(this.FileOrder);
                    this.FileOrder.Save(tabFiles, true);
                }
            }
        }

        private void mnuFileConvertToRichText_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.ConvertToRichText(this.FileOrder);
                this.FileOrder.Save(tabFiles, true);
            }
        }

        private void mnuFileSaveNow_Click(object sender, EventArgs e) {
            this.FileOrder.Save(tabFiles, true);

            if (tabFiles.SelectedTab != null) {
                try {
                    tabFiles.SelectedTab.Save();
                    this.FileOrder.Save(tabFiles, true);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }

        private void mnuFileSaveAll_Click(object sender, EventArgs e) {
            this.FileOrder.Save(tabFiles, true);

            try {
                tabFiles.SaveAll();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnuFileDelete_Click(object sender, EventArgs e) {
            this.FileOrder.Save(tabFiles, true);
            if (tabFiles.SelectedTab != null) {
                var currTab = tabFiles.SelectedTab;
                if (currTab.TextBox.Text.Length > 0) {
                    if (Medo.MessageBox.ShowQuestion(this, "Are you sure?", Medo.Reflection.EntryAssembly.Title + " : Delete", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                        return;
                    }
                }
                try {
                    int tindex = tabFiles.TabPages.IndexOf(tabFiles.SelectedTab) + 1; //select next tab
                    if (tindex >= tabFiles.TabPages.Count) {
                        tindex -= 2; //go to one in front of it
                    }
                    if ((tindex > 0) && (tindex < tabFiles.TabPages.Count)) {
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[tindex];
                    }

                    currTab.Delete();
                    tabFiles.TabPages.Remove(currTab);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }

        private void mnuFileRename_Click(object sender, EventArgs e) {
            this.FileOrder.Save(tabFiles, true);

            if (tabFiles.SelectedTab != null) {
                try {
                    using (var frm = new RenameFileForm(tabFiles.SelectedTab.Title)) {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            tabFiles.SelectedTab.Rename(frm.Title);
                        }
                    }
                    this.FileOrder.Save(tabFiles, true);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }

        private void mnuFileHide_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                var currTab = tabFiles.SelectedTab;
                try {
                    var currAttributes = File.GetAttributes(tabFiles.SelectedTab.FullFileName);
                    File.SetAttributes(tabFiles.SelectedTab.FullFileName, currAttributes | FileAttributes.Hidden);
                    tabFiles.TabPages.Remove(currTab);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }

        private void mnuFileUnhide_Click(object sender, EventArgs e) {
            using (var frm = new UnhideFileForm()) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    mnuViewRefresh_Click(null, null);
                }
            }
        }

        private void mnuFilePrintPreview_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                try {
                    Medo.Drawing.Printing.FullText ol = new Medo.Drawing.Printing.FullText(tabFiles.SelectedTab.Title, 10.0f, 10.0f, 20.0f, 10.0f);
                    ol.BeginPrint += Document_BeginPrint;
                    ol.StartPrintPage += Document_PrintPage;
                    ol.Font = Settings.DisplayFont;
                    ol.Text = tabFiles.SelectedTab.TextBox.Text;
                    Medo.Windows.Forms.PrintPreviewDialog x = new Medo.Windows.Forms.PrintPreviewDialog(ol.Document);
                    x.ShowDialog(this);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }

        private void mnuFilePrint_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                try {
                    Medo.Drawing.Printing.FullText ol = new Medo.Drawing.Printing.FullText(tabFiles.SelectedTab.Title, 10.0f, 10.0f, 20.0f, 10.0f);
                    ol.BeginPrint += Document_BeginPrint;
                    ol.StartPrintPage += Document_PrintPage;
                    ol.Font = Settings.DisplayFont;
                    ol.Text = tabFiles.SelectedTab.TextBox.Text;
                    ol.Print();
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }
        }

        private void mnuFileClose_Click(object sender, EventArgs e) {
            if (Settings.SaveOnHide) { SaveAllChanged(); }
            this.Hide();
            this.FileOrder.Save(tabFiles, true);
        }

        private void mnuFileExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }


        private void mnuEdit_DropDownOpening(object sender, EventArgs e) {
            bool isTabSelected = tabFiles.SelectedTab != null;
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;
            bool isTabPlainText = isTabSelected && (tabFiles.SelectedTab.IsRichTextFormat == false);
            bool canUndo = isTabSelected && tabFiles.SelectedTab.CanUndo;
            bool canRedo = isTabSelected && tabFiles.SelectedTab.CanRedo;
            bool canCut = isTabSelected && tabFiles.SelectedTab.CanCopy;
            bool canCopy = isTabSelected && tabFiles.SelectedTab.CanCopy;
            bool canPaste = isTabSelected && tabFiles.SelectedTab.CanPaste;
            bool canDelete = isTabSelected && ((tabFiles.SelectedTab.TextBox.SelectedText.Length > 0) || (tabFiles.SelectedTab.TextBox.SelectionStart < tabFiles.SelectedTab.TextBox.Text.Length));
            bool canSelectAll = isTabSelected && (tabFiles.SelectedTab.TextBox.Text.Length > 0);

            mnuEditUndo.Enabled = canUndo;
            mnuEditRedo.Enabled = canRedo;
            mnuEditCut.Enabled = canCopy;
            mnuEditCopy.Enabled = canCopy;
            mnuEditPaste.Enabled = canPaste;
            mnuEditDelete.Enabled = canDelete;
            mnuEditSelectAll.Enabled = canSelectAll;
            mnuEditFind.Enabled = isTabSelected;
            mnuEditFindNext.Enabled = isTabSelected && (string.IsNullOrEmpty(SearchStatus.Text) == false);
        }

        private void mnuEditUndo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.Undo();
            }
        }

        private void mnuEditRedo_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.Redo();
            }
        }

        private void mnuEditCut_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Cut(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnuEditCopy_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Copy(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnuEditPaste_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Paste(Settings.ForceTextCopyPaste);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnuEditDelete_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                if (txt.SelectedText.Length > 0) {
                    txt.SelectedText = "";
                } else if (txt.SelectionStart < txt.Text.Length) {
                    txt.SelectionLength = 1;
                    txt.SelectedText = "";
                }
            }
        }

        private void mnuEditSelectAll_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                txt.SelectAll();
            }
        }

        private void mnuEditFind_Click(object sender, EventArgs e) {
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

        private void mnuEditFindNext_Click(object sender, EventArgs e) {
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


        private void mnuView_DropDownOpening(object sender, EventArgs e) {
            mnuViewAlwaysOnTop.Checked = Settings.DisplayAlwaysOnTop;
            mnuViewMenu.Checked = Settings.ShowMenu;
            mnuViewToolbar.Checked = Settings.DisplayShowToolbar;
        }

        private void mnuViewAlwaysOnTop_Click(object sender, EventArgs e) {
            Settings.DisplayAlwaysOnTop = mnuViewAlwaysOnTop.Checked;
            this.TopMost = mnuViewAlwaysOnTop.Checked;
        }

        private void mnuViewMenu_Click(object sender, EventArgs e) {
            Settings.ShowMenu = mnuViewMenu.Checked;
            mnu.Visible = Settings.ShowMenu;
            MainForm_Resize(null, null);
        }

        private void mnuViewToolbar_Click(object sender, EventArgs e) {
            Settings.DisplayShowToolbar = mnuViewToolbar.Checked;
            tls.Visible = Settings.DisplayShowToolbar;
            MainForm_Resize(null, null);
        }

        private void mnuViewZoomIn_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.ZoomIn();
            }
        }

        private void mnuViewZoomOut_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.ZoomOut();
            }
        }

        private void mnuViewZoomReset_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                tabFiles.SelectedTab.ZoomReset();
            }
        }

        private void mnuViewRefresh_Click(object sender, EventArgs e) {
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

            if (sender != null) { this.FileOrder.Save(tabFiles, true); }

            string selectedTitle = "*";
            if (tabFiles.SelectedTab != null) { selectedTitle = tabFiles.SelectedTab.Title; }

            tabFiles.Visible = false;
            tabFiles.TabPages.Clear();
            try { //if files cannot be found
                this.FileOrder.Reload();
                if ((Settings.StartupRememberSelectedFile) && ((string.IsNullOrEmpty(selectedTitle)) || (selectedTitle == "*"))) { selectedTitle = this.FileOrder.SelectedTitle; }
                string[] fs = this.FileOrder.Files;
                for (int i = 0; i <= fs.Length - 1; i++) {
                    TabFile t = new TabFile(fs[i], App.Form.mnxTextBox);
                    tabFiles.TabPages.Add(t);
                }

                tabFiles.Visible = true;
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Files could not be loaded." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
            }

            if (tabFiles.TabPages.Count > 0) {
                for (int i = 0; i <= tabFiles.TabPages.Count - 1; i++) {
                    if ((string.Compare(tabFiles.TabPages[i].Text, selectedTitle) == 0)) {
                        selectedTitle = "";
                        tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[i];

                    }
                }
                if (!string.IsNullOrEmpty(selectedTitle)) { tabFiles.SelectedTab = (TabFile)tabFiles.TabPages[0]; }
                tabFiles_SelectedIndexChanged(null, null);
            }

            this.TopMost = Settings.DisplayAlwaysOnTop;
        }


        private void mnuFormat_DropDownOpening(object sender, EventArgs e) {
            bool isTabSelected = tabFiles.SelectedTab != null;
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;
            bool isTextSelected = isTabSelected && (tabFiles.SelectedTab.TextBox.SelectedText.Length > 0);
            bool canFont = isTabRichText;
            bool isBoldChecked = canFont && (tabFiles.SelectedTab.TextBox.SelectionFont != null) && (tabFiles.SelectedTab.TextBox.SelectionFont.Bold);
            bool isItalicChecked = canFont && (tabFiles.SelectedTab.TextBox.SelectionFont != null) && (tabFiles.SelectedTab.TextBox.SelectionFont.Italic);
            bool isUnderlineChecked = canFont && (tabFiles.SelectedTab.TextBox.SelectionFont != null) && (tabFiles.SelectedTab.TextBox.SelectionFont.Underline);
            bool isStrikeoutChecked = canFont && (tabFiles.SelectedTab.TextBox.SelectionFont != null) && (tabFiles.SelectedTab.TextBox.SelectionFont.Strikeout);

            mnuFormatFont.Visible = isTabRichText;
            mnuFormatBold.Visible = isTabRichText;
            mnuFormatItalic.Visible = isTabRichText;
            mnuFormatUnderline.Visible = isTabRichText;
            mnuFormatStrikeout.Visible = isTabRichText;
            mnuFormatRtfSeparator.Visible = isTabRichText;

            mnuFormatSortAscending.Enabled = isTextSelected;
            mnuFormatSortDescending.Enabled = isTextSelected;
            mnuFormatConvertToLower.Enabled = isTextSelected;
            mnuFormatConvertToUpper.Enabled = isTextSelected;
            mnuFormatConvertToTitleCase.Enabled = isTextSelected;
            mnuFormatConvertToTitleCaseDrGrammar.Enabled = isTextSelected;

            mnuFormatBold.Checked = isBoldChecked;
            mnuFormatItalic.Checked = isItalicChecked;
            mnuFormatUnderline.Checked = isUnderlineChecked;
            mnuFormatStrikeout.Checked = isStrikeoutChecked;
        }

        private void mnuFormatFont_Click(object sender, EventArgs e) {
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

        private void mnuFormatBold_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    ToogleStyle(tf.TextBox, FontStyle.Bold);
                }
            }
        }

        private void mnuFormatItalic_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    ToogleStyle(tf.TextBox, FontStyle.Italic);
                }
            }
        }

        private void mnuFormatUnderline_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    if (tf.TextBox.SelectionFont != null) {
                        ToogleStyle(tf.TextBox, FontStyle.Underline);
                    }
                }
            }
        }

        private void mnuFormatStrikeout_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                if (tf.IsRichTextFormat) {
                    if (tf.TextBox.SelectionFont != null) {
                        ToogleStyle(tf.TextBox, FontStyle.Strikeout);
                    }
                }
            }
        }

        private void mnuFormatSortAscending_Click(object sender, EventArgs e) {
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

        private void mnuFormatSortDescending_Click(object sender, EventArgs e) {
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

        private void mnuFormatConvertToLower_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = txt.SelectedText.ToLower();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnuFormatConvertToUpper_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = txt.SelectedText.ToUpper();

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnuFormatConvertToTitleCase_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                int ss = txt.SelectionStart;
                int sl = txt.SelectionLength;

                txt.SelectedText = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToLower(txt.SelectedText));

                txt.SelectionStart = ss;
                txt.SelectionLength = sl;
            }
        }

        private void mnuFormatConvertToTitleCaseDrGrammar_Click(object sender, EventArgs e) {
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


        private void mnuToolsOptions_Click(object sender, EventArgs e) {
            using (var frm = new OptionsForm()) {
                tmrQuickAutoSave.Enabled = false;
                SaveAllChanged();
                this.FileOrder.Save(tabFiles, true);
                this.tmrUpdateToolbar.Enabled = false;
                mnuViewRefresh_Click(null, null);
                if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                    mnu.Visible = Settings.ShowMenu;
                    tabFiles.Multiline = Settings.DisplayMultilineTabHeader;
                    if (Settings.DisplayMinimizeMaximizeButtons) {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    } else {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                    }
                    if (Settings.ZoomToolbarWithDpiChange) {
                        tls.ImageScalingSize = new Size(Convert.ToInt32(16 * _dpiRatioX), Convert.ToInt32(16 * _dpiRatioY));
                        tls.Scale(new SizeF(_dpiRatioX, _dpiRatioY));
                    } else {
                        float rx = Convert.ToSingle(16 / tls.ImageScalingSize.Width);
                        float ry = Convert.ToSingle(16 / tls.ImageScalingSize.Height);
                        tls.ImageScalingSize = new Size(16, 16);
                        tls.Scale(new SizeF(rx, ry));
                    }
                    this.ShowInTaskbar = Settings.DisplayShowInTaskbar;
                    this.TopMost = Settings.DisplayAlwaysOnTop;
                    mnuViewRefresh_Click(null, null);
                    MainForm_Resize(null, null);
                    this.tmrUpdateToolbar.Enabled = Settings.DisplayShowToolbar;
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


        private void mnuHelpReportABug_Click(object sender, EventArgs e) {
            Medo.Diagnostics.ErrorReport.TopMost = this.TopMost;
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("http://jmedved.com/ErrorReport/"));
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e) {
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("http://www.jmedved.com/?page=qtext"), global::Medo.Reflection.EntryAssembly.Product + " (beta 3)");

            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                txt.Focus();
            }
        }

        #endregion

        #region Toolbar

        private void tls_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                ToolStripItem tsi = tls.Items[tls.Items.Count - 1];
                if (e.X > tsi.Bounds.Right) {
                    //mnu.Show(tls, e.Location)
                }
            }
        }

        private void tls_btnAlwaysOnTop_Click(object sender, EventArgs e) {
            Settings.DisplayAlwaysOnTop = tls_btnAlwaysOnTop.Checked;
            this.TopMost = Settings.DisplayAlwaysOnTop;
        }

        #endregion


        #region Tabs menu

        private void mnxTab_Opening(object sender, CancelEventArgs e) {
            mnuFile_DropDownOpening(null, null);

            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;

            mnxTabConvertToPlainText.Visible = isTabRichText;
            mnxTabConvertToRichText.Visible = (isTabRichText == false);

            mnxTabNew.Enabled = mnuFileNew.Enabled;
            mnxTabReopen.Enabled = mnuFileReopen.Enabled;
            mnxTabSaveNow.Enabled = mnuFileSaveNow.Enabled;
            mnxTabDelete.Enabled = mnuFileDelete.Enabled;
            mnxTabRename.Enabled = mnuFileRename.Enabled;
            mnxTabPrintPreview.Enabled = mnuFilePrintPreview.Enabled;
            mnxTabPrint.Enabled = mnuFilePrint.Enabled;
            mnxTabHide.Enabled = mnuFileHide.Enabled;
            mnxTabUnhide.Enabled = mnuFileUnhide.Enabled;
            mnxTabPrintPreview.Enabled = mnuFilePrintPreview.Enabled;
            mnxTabPrint.Enabled = mnuFilePrint.Enabled;
            mnxTabOpenContainingFolder.Enabled = mnuFileReopen.Enabled;
        }

        private void mnxTabOpenContainingFolder_Click(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                string file = tabFiles.SelectedTab.FullFileName;
                var exe = new ProcessStartInfo("explorer.exe", "/select,\"" + file + "\"");
                Process.Start(exe);
            }
        }

        #endregion


        #region TextBox menu

        private void mnxTextBox_Opening(object sender, CancelEventArgs e) {
            mnuEdit_DropDownOpening(null, null);
            mnuFormat_DropDownOpening(null, null);

            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;

            mnxTextBoxCutAsText.Visible = isTabRichText && (Settings.ForceTextCopyPaste == false);
            mnxTextBoxCopyAsText.Visible = isTabRichText && (Settings.ForceTextCopyPaste == false);
            mnxTextBoxPasteAsText.Visible = isTabRichText && (Settings.ForceTextCopyPaste == false);
            mnxTextBoxCutCopyPasteAsTextSeparator.Visible = isTabRichText && (Settings.ForceTextCopyPaste == false);

            mnxTextBoxFont.Visible = isTabRichText;
            mnxTextBoxBold.Visible = isTabRichText;
            mnxTextBoxItalic.Visible = isTabRichText;
            mnxTextBoxUnderline.Visible = isTabRichText;
            mnxTextBoxStrikeout.Visible = isTabRichText;
            mnxTextBoxRtfSeparator.Visible = isTabRichText;

            mnxTextBoxUndo.Enabled = mnuEditUndo.Enabled;
            mnxTextBoxRedo.Enabled = mnuEditRedo.Enabled;
            mnxTextBoxCut.Enabled = mnuEditCut.Enabled;
            mnxTextBoxCopy.Enabled = mnuEditCopy.Enabled;
            mnxTextBoxPaste.Enabled = mnuEditPaste.Enabled;
            mnxTextBoxCutAsText.Enabled = mnuEditCut.Enabled;
            mnxTextBoxCopyAsText.Enabled = mnuEditCopy.Enabled;
            mnxTextBoxPasteAsText.Enabled = mnuEditPaste.Enabled;
            mnxTextBoxSelectAll.Enabled = mnuEditSelectAll.Enabled;
            mnxTextBoxFont.Enabled = mnuFormatFont.Enabled;
            mnxTextBoxBold.Enabled = mnuFormatBold.Enabled;
            mnxTextBoxItalic.Enabled = mnuFormatItalic.Enabled;
            mnxTextBoxUnderline.Enabled = mnuFormatUnderline.Enabled;
            mnxTextBoxStrikeout.Enabled = mnuFormatStrikeout.Enabled;
            mnxTextBoxFormat.Enabled = mnuFormatConvertToLower.Enabled;

            mnxTextBoxBold.Checked = mnuFormatBold.Checked;
            mnxTextBoxItalic.Checked = mnuFormatItalic.Checked;
            mnxTextBoxUnderline.Checked = mnuFormatUnderline.Checked;
            mnxTextBoxStrikeout.Checked = mnuFormatStrikeout.Checked;
        }

        private void mnxTextBoxCutAsText_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Cut(true);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnxTextBoxCopyAsText_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Copy(true);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void mnxTextBoxPasteAsText_Click(object sender, EventArgs e) {
            try {
                if (tabFiles.SelectedTab != null) {
                    tabFiles.SelectedTab.Paste(true);
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation could not be completed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        #endregion


        private void fswLocationTxt_Changed(object sender, System.IO.FileSystemEventArgs e) {
            //mnuViewRefresh_Click(null, null);
        }


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
                tabFiles.SaveAll();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
            }
            this.FileOrder.Save(tabFiles, true);
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
            mnuFile_DropDownOpening(null, null);
            mnuEdit_DropDownOpening(null, null);
            mnuView_DropDownOpening(null, null);
            mnuFormat_DropDownOpening(null, null);

            bool isTabSelected = (tabFiles.SelectedTab != null);
            bool isTabRichText = isTabSelected && tabFiles.SelectedTab.IsRichTextFormat;

            tls_btnFont.Visible = isTabRichText;
            tls_btnBold.Visible = isTabRichText;
            tls_btnItalic.Visible = isTabRichText;
            tls_btnUnderline.Visible = isTabRichText;
            tls_btnStrikeout.Visible = isTabRichText;
            tls_RtfSeparator.Visible = isTabRichText;

            tls_btnNew.Enabled = mnuFileNew.Enabled;
            tls_btnSaveNow.Enabled = mnuFileSaveNow.Enabled;
            tls_btnRename.Enabled = mnuFileRename.Enabled;
            tls_btnPrintPreview.Enabled = mnuFilePrintPreview.Enabled;
            tls_btnPrint.Enabled = mnuFilePrint.Enabled;

            tls_btnCut.Enabled = mnuEditCut.Enabled;
            tls_btnCopy.Enabled = mnuEditCopy.Enabled;
            tls_btnPaste.Enabled = mnuEditPaste.Enabled;

            tls_btnFont.Enabled = mnuFormatFont.Enabled;
            tls_btnBold.Enabled = mnuFormatBold.Enabled;
            tls_btnItalic.Enabled = mnuFormatItalic.Enabled;
            tls_btnUnderline.Enabled = mnuFormatUnderline.Enabled;
            tls_btnStrikeout.Enabled = mnuFormatStrikeout.Enabled;

            tls_btnUndo.Enabled = mnuEditUndo.Enabled;
            tls_btnRedo.Enabled = mnuEditRedo.Enabled;

            tls_btnFind.Enabled = mnuEditFind.Enabled;
            tls_btnAlwaysOnTop.Enabled = mnuViewAlwaysOnTop.Enabled;

            tls_btnBold.Checked = mnuFormatBold.Checked;
            tls_btnItalic.Checked = mnuFormatItalic.Checked;
            tls_btnUnderline.Checked = mnuFormatUnderline.Checked;
            tls_btnStrikeout.Checked = mnuFormatStrikeout.Checked;
            tls_btnAlwaysOnTop.Checked = mnuViewAlwaysOnTop.Checked;
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

    }
}
