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

        }

        private void mnuFileNew_Click(object sender, EventArgs e) {

        }

        private void mnuFileReopen_Click(object sender, EventArgs e) {

        }

        private void mnuFileConvertToPlainText_Click(object sender, EventArgs e) {

        }

        private void mnuFileConvertToRichText_Click(object sender, EventArgs e) {

        }

        private void mnuFileSaveNow_Click(object sender, EventArgs e) {

        }

        private void mnuFileSaveAll_Click(object sender, EventArgs e) {

        }

        private void mnuFileDelete_Click(object sender, EventArgs e) {

        }

        private void mnuFileRename_Click(object sender, EventArgs e) {

        }

        private void mnuFilePrintPreview_Click(object sender, EventArgs e) {

        }

        private void mnuFilePrint_Click(object sender, EventArgs e) {

        }

        private void mnuFileClose_Click(object sender, EventArgs e) {

        }

        private void mnuFileExit_Click(object sender, EventArgs e) {

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

        #endregion

    }
}
