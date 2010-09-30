using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;

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



        private void mnuFile_DropDownOpening(object sender, EventArgs e) {
            if (tabFiles.SelectedTab != null) {
                TabFile tf = tabFiles.SelectedTab;
                mnuFileNew.Enabled = true;
                mnuFileReopen.Enabled = true;
                mnuFileConvertToPlainText.Enabled = tf.IsRichTextFormat;
                mnuFileConvertToRichText.Enabled = !tf.IsRichTextFormat;
                mnuFileSaveNow.Enabled = true;
                mnuFileSaveAll.Enabled = true;
                mnuFileDelete.Enabled = true;
                mnuFileRename.Enabled = true;
                mnuFilePrintPreview.Enabled = true;
                mnuFilePrint.Enabled = true;
                mnuFileClose.Enabled = true;
                mnuFileExit.Enabled = true;
            } else {
                mnuFileNew.Enabled = true;
                mnuFileReopen.Enabled = false;
                mnuFileConvertToPlainText.Enabled = false;
                mnuFileConvertToRichText.Enabled = false;
                mnuFileSaveNow.Enabled = false;
                mnuFileSaveAll.Enabled = false;
                mnuFileDelete.Enabled = false;
                mnuFileRename.Enabled = false;
                mnuFilePrintPreview.Enabled = false;
                mnuFilePrint.Enabled = false;
                mnuFileClose.Enabled = true;
                mnuFileExit.Enabled = true;
            }
        }

        private void mnuFileNew_Click(object sender, EventArgs e) {
            fswLocationTxt.EnableRaisingEvents = false;
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
            fswLocationTxt.EnableRaisingEvents = true;
        }

        private void mnuFileReopen_Click(object sender, EventArgs e) {
            fswLocationTxt.EnableRaisingEvents = false;
            this.FileOrder.Save(tabFiles, true);

            if ((tabFiles.SelectedTab != null)) {
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
                }
            }

            fswLocationTxt.EnableRaisingEvents = true;
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
            fswLocationTxt.EnableRaisingEvents = false;
            this.FileOrder.Save(tabFiles, true);

            if (tabFiles.SelectedTab != null) {
                try {
                    tabFiles.SelectedTab.Save();
                    this.FileOrder.Save(tabFiles, true);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }

            fswLocationTxt.EnableRaisingEvents = true;
        }

        private void mnuFileSaveAll_Click(object sender, EventArgs e) {
            fswLocationTxt.EnableRaisingEvents = false;
            this.FileOrder.Save(tabFiles, true);

            try {
                tabFiles.SaveAll();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }

            fswLocationTxt.EnableRaisingEvents = true;
        }

        private void mnuFileDelete_Click(object sender, EventArgs e) {
            fswLocationTxt.EnableRaisingEvents = false;
            this.FileOrder.Save(tabFiles, true);

            if (tabFiles.SelectedTab != null) {
                if (tabFiles.SelectedTab.TextBox.Text.Length > 0) {
                    switch (Medo.MessageBox.ShowQuestion(this, "Are you sure?", global::Medo.Reflection.EntryAssembly.Title + " : Delete", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2)) {
                        case DialogResult.Yes: break;
                        case DialogResult.No: return;
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

                    tabFiles.SelectedTab.Delete();
                    tabFiles.TabPages.Remove(tabFiles.SelectedTab);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }

            fswLocationTxt.EnableRaisingEvents = true;
        }

        private void mnuFileRename_Click(object sender, EventArgs e) {
            fswLocationTxt.EnableRaisingEvents = false;
            this.FileOrder.Save(tabFiles, true);

            if ((tabFiles.SelectedTab != null)) {
                try {
                    using (RenameFileForm frm = new RenameFileForm(tabFiles.SelectedTab.Title)) {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            tabFiles.SelectedTab.Rename(frm.Title);
                        }
                    }
                    this.FileOrder.Save(tabFiles, true);
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                }
            }

            fswLocationTxt.EnableRaisingEvents = true;
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
            fswLocationTxt.EnableRaisingEvents = false;
            try {
                tabFiles.SaveAll();
            } catch (Exception ex) {
                Medo.MessageBox.ShowWarning(this, "Operation failed." + Environment.NewLine + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
            }
            this.FileOrder.Save(tabFiles, true);
            fswLocationTxt.EnableRaisingEvents = true;
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

    }
}
