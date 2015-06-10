using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace QText {
    internal partial class OptionsForm : Form {
        public OptionsForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            cmbSelectionDelimiters.Font = new Font("Courier New", SystemFonts.MessageBoxFont.SizeInPoints);

            SetWritableState(!Settings.NoRegistryWrites);
        }

        private void OptionsForm_Load(object sender, EventArgs e) {
            //Appearance
            chkDisplayURLs.Checked = Settings.DetectUrls;
            chkFollowURLs.Checked = Settings.FollowURLs;
            nudTabWidth.Value = Settings.DisplayTabWidth;
            txtFont.Text = GetFontText(Settings.DisplayFont);
            txtFont.Tag = Settings.DisplayFont;
            lblColorExample.BackColor = Settings.DisplayBackgroundColor;
            lblColorExample.ForeColor = Settings.DisplayForegroundColor;
            chkDisplayURLs_CheckedChanged(null, null);

            //Display
            chkShowInTaskbar.Checked = Settings.DisplayShowInTaskbar;
            chbShowMinimizeMaximizeButtons.Checked = Settings.DisplayMinimizeMaximizeButtons;
            chkShowToolbar.Checked = Settings.ShowToolbar;
            chbMultilineTabs.Checked = Settings.MultilineTabs;
            chbHorizontalScrollbar.Checked = (Settings.ScrollBars == ScrollBars.Horizontal) || (Settings.ScrollBars == ScrollBars.Both);
            chbVerticalScrollbar.Checked = (Settings.ScrollBars == ScrollBars.Vertical) || (Settings.ScrollBars == ScrollBars.Both);
            cmbSelectionDelimiters.Text = Settings.SelectionDelimiters;

            //Files
            chkPreloadFilesOnStartup.Checked = Settings.FilesPreload;
            chkDeleteToRecycleBin.Checked = Settings.FilesDeleteToRecycleBin;
            nudQuickSaveIntervalInSeconds.Value = Settings.QuickSaveInterval / 1000M;
            chbSavePlainWithLF.Checked = Settings.PlainLineEndsWithLf;

            //Behavior
            txtHotkey.Text = GetKeyString(Settings.ActivationHotkey);
            txtHotkey.Tag = Settings.ActivationHotkey;
            chkMinimizeToTray.Checked = Settings.TrayOnMinimize;
            chkSingleClickTrayActivation.Checked = Settings.TrayOneClickActivation;
            chkRunAtStartup.Checked = Settings.StartupRun;

            //Carbon copy
            chbUseCarbonCopy.Checked = Settings.CarbonCopyUse;
            txtCarbonCopyFolder.Text = Settings.CarbonCopyDirectory;
            chbCarbonCopyIgnoreCopyErrors.Checked = Settings.CarbonCopyIgnoreErrors;
            chbUseCarbonCopy_CheckedChanged(null, null);
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.DialogResult == DialogResult.OK) {
                if (Settings.ActivationHotkey != App.Hotkey.Key) {
                    if (App.Hotkey.IsRegistered) {
                        App.Hotkey.Unregister();
                    }
                    if (Settings.ActivationHotkey != Keys.None) {
                        try {
                            App.Hotkey.Register(Settings.ActivationHotkey);
                        } catch (InvalidOperationException) {
                            Medo.MessageBox.ShowWarning(null, "Hotkey is already in use.");
                        }
                    }
                }
            }
        }

        private void chkDisplayURLs_CheckedChanged(object sender, EventArgs e) {
            chkFollowURLs.Enabled = chkDisplayURLs.Checked;
        }

        private void btnFont_Click(object sender, EventArgs e) {
            using (var f = new FontDialog()) {
                f.AllowScriptChange = true;
                f.AllowSimulations = true;
                f.AllowVectorFonts = true;
                f.AllowVerticalFonts = false;
                f.FixedPitchOnly = false;
                f.FontMustExist = true;
                f.ShowApply = false;
                f.ShowColor = false;
                f.ShowEffects = false;
                f.Font = (Font)txtFont.Tag;
                if (f.ShowDialog(this) == DialogResult.OK) {
                    txtFont.Text = GetFontText(f.Font);
                    txtFont.Tag = f.Font;
                }
            }
        }

        private void btnColorBackground_Click(object sender, EventArgs e) {
            using (var f = new ColorDialog()) {
                f.AllowFullOpen = true;
                f.AnyColor = true;
                f.FullOpen = true;
                f.Color = lblColorExample.BackColor;
                if (f.ShowDialog(this) == DialogResult.OK) {
                    lblColorExample.BackColor = f.Color;
                }
            }
        }

        private void btnColorForeground_Click(object sender, EventArgs e) {
            using (var f = new ColorDialog()) {
                f.AllowFullOpen = true;
                f.AnyColor = true;
                f.FullOpen = true;
                f.Color = lblColorExample.ForeColor;
                if (f.ShowDialog(this) == DialogResult.OK) {
                    lblColorExample.ForeColor = f.Color;
                }
            }
        }

        private void chkShowInTaskbar_CheckedChanged(object sender, EventArgs e) {
            if (chkShowInTaskbar.Checked == false) {
                chbShowMinimizeMaximizeButtons.Checked = false;
            }
        }

        private void chbShowMinimizeMaximizeButtons_CheckedChanged(object sender, EventArgs e) {
            if (chbShowMinimizeMaximizeButtons.Checked) {
                chkShowInTaskbar.Checked = true;
            }
        }

        private void btnOpenLocationFolder_Click(object sender, EventArgs e) {
            try {
                Process.Start(Settings.FilesLocation, null);
            } catch (Win32Exception ex) {
                Medo.MessageBox.ShowWarning(this, ex.Message);
            }
        }

        private void btnChangeLocation_Click(object sender, EventArgs e) {
            using (var frm = new FolderOpenDialog() { InitialFolder = App.Document.RootPath }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    try {

                        var copier = new DocumentCopier(App.Document, frm.Folder);
                        //if (copier.DestinationRootAlreadyExisted) {
                        //    if (Medo.MessageBox.ShowQuestion(this, "Destination already exists. Are you sure you want to move there?", MessageBoxButtons.YesNo) == DialogResult.No) {
                        //        return;
                        //    }
                        //}

                        var alwaysOverwrite = false;
                        if (!copier.DestinationRootWasEmpty) {
                            switch (Medo.MessageBox.ShowQuestion(this, "Destination is not empty. Do you want to overwrite all files?\n\nIf you select No, you will be asked for each file existing at the destination.", MessageBoxButtons.YesNoCancel)) {
                                case DialogResult.Yes: alwaysOverwrite = true; break;
                                case DialogResult.No: alwaysOverwrite = false; break;
                                case DialogResult.Cancel: return;
                            }
                        }

                        copier.FolderOverwrite += delegate (object sender2, DocumentCopierOverwriteEventArgs e2) {
                            switch (Medo.MessageBox.ShowQuestion(this, "Folder \"" + e2.RelativePath + "\" already exists at the destination. Do you wish to overwrite it?", MessageBoxButtons.YesNoCancel)) {
                                case DialogResult.No: e2.Overwrite = false; break;
                                case DialogResult.Cancel: e2.Cancel = true; break;
                            }
                        };

                        copier.FileOverwrite += delegate (object sender2, DocumentCopierOverwriteEventArgs e2) {
                            switch (Medo.MessageBox.ShowQuestion(this, "File \"" + e2.RelativePath + "\" already exists at the destination. Do you wish to overwrite it?", MessageBoxButtons.YesNoCancel)) {
                                case DialogResult.No: e2.Overwrite = false; break;
                                case DialogResult.Cancel: e2.Cancel = true; break;
                            }
                        };

                        if (copier.CopyAll(alwaysOverwrite: alwaysOverwrite)) {
                            App.Document = copier.GetDestinationDocument();
                            Settings.FilesLocation = App.Document.RootPath;
                        }

                        Medo.MessageBox.ShowInformation(this, "Data location transfer succeeded.");
                        this.DialogResult = DialogResult.OK;

                    } catch (InvalidOperationException ex) {
                        Medo.MessageBox.ShowError(this, ex.Message);
                    }

                }
            }
        }


        private bool hadRegistered;

        private void txtHotkey_Enter(object sender, EventArgs e) {
            if (App.Hotkey.IsRegistered) {
                hadRegistered = true;
                App.Hotkey.Unregister();
            }
        }

        private void txtHotkey_Leave(object sender, EventArgs e) {
            if (hadRegistered && (Settings.ActivationHotkey != Keys.None)) {
                try {
                    App.Hotkey.Register(Settings.ActivationHotkey);
                } catch (InvalidOperationException) {
                    Medo.MessageBox.ShowWarning(this, "Cannot register hotkey.");
                }
            }
        }

        private void txtHotkey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            string str = GetKeyString(e.KeyData);
            if (!string.IsNullOrEmpty(str)) {
                txtHotkey.Text = str;
                txtHotkey.Tag = e.KeyData;
            } else {
                txtHotkey.Text = "Use Ctrl+Alt, Ctrl+Shift or Alt+Shift";
                txtHotkey.Tag = Keys.None;
            }
        }


        private void chbUseCarbonCopy_CheckedChanged(object sender, EventArgs e) {
            lblCarbonCopyFolder.Enabled = chbUseCarbonCopy.Checked;
            txtCarbonCopyFolder.Enabled = chbUseCarbonCopy.Checked;
            btnCarbonCopyFolderSelect.Enabled = chbUseCarbonCopy.Checked;
            chbCarbonCopyIgnoreCopyErrors.Enabled = chbUseCarbonCopy.Checked;
            btnCarbonCopyOpenFolder.Enabled = txtCarbonCopyFolder.Text.Length > 0;
        }

        private void txtCarbonCopyFolder_TextChanged(object sender, EventArgs e) {
            btnCarbonCopyOpenFolder.Enabled = txtCarbonCopyFolder.Text.Length > 0;
        }

        private void btnCarbonCopyFolderSelect_Click(object sender, EventArgs e) {
            using (var frm = new FolderOpenDialog()) {
                if (string.IsNullOrEmpty(Settings.CarbonCopyDirectory)) {
                    frm.InitialFolder = Settings.FilesLocation;
                } else {
                    frm.InitialFolder = Settings.CarbonCopyDirectory;
                }
                if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                    if (frm.Folder.StartsWith(Settings.FilesLocation, StringComparison.OrdinalIgnoreCase)) {
                        Medo.MessageBox.ShowWarning(this, "Carbon copy folder cannot be same as one used for main program storage.");
                    } else {
                        txtCarbonCopyFolder.Text = frm.Folder;
                    }
                }
            }
        }

        private void btnCarbonCopyOpenFolder_Click(object sender, EventArgs e) {
            try {
                Process.Start(txtCarbonCopyFolder.Text, null);
            } catch (System.ComponentModel.Win32Exception ex) {
                Medo.MessageBox.ShowWarning(this, ex.Message);
            }
        }


        private void btnExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void btnOk_Click(object sender, EventArgs e) {
            //Appearance
            Settings.DetectUrls = chkDisplayURLs.Checked;
            Settings.FollowURLs = chkFollowURLs.Checked;
            Settings.DisplayTabWidth = Convert.ToInt32(nudTabWidth.Value);
            Settings.DisplayFont = (System.Drawing.Font)txtFont.Tag;
            Settings.DisplayBackgroundColor = lblColorExample.BackColor;
            Settings.DisplayForegroundColor = lblColorExample.ForeColor;

            //Display
            Settings.DisplayShowInTaskbar = chkShowInTaskbar.Checked;
            Settings.DisplayMinimizeMaximizeButtons = chbShowMinimizeMaximizeButtons.Checked;
            Settings.ShowToolbar = chkShowToolbar.Checked;
            Settings.MultilineTabs = chbMultilineTabs.Checked;
            if ((chbHorizontalScrollbar.Checked && chbVerticalScrollbar.Checked)) {
                Settings.ScrollBars = ScrollBars.Both;
            } else if ((chbHorizontalScrollbar.Checked)) {
                Settings.ScrollBars = ScrollBars.Horizontal;
            } else if ((chbVerticalScrollbar.Checked)) {
                Settings.ScrollBars = ScrollBars.Vertical;
            } else {
                Settings.ScrollBars = ScrollBars.None;
            }
            Settings.SelectionDelimiters = cmbSelectionDelimiters.Text;

            //Files
            Settings.FilesPreload = chkPreloadFilesOnStartup.Checked;
            Settings.FilesDeleteToRecycleBin = chkDeleteToRecycleBin.Checked;
            Settings.QuickSaveInterval = Convert.ToInt32(nudQuickSaveIntervalInSeconds.Value * 1000);
            Settings.PlainLineEndsWithLf = chbSavePlainWithLF.Checked;

            //Behavior
            Settings.ActivationHotkey = (Keys)txtHotkey.Tag;
            Settings.TrayOnMinimize = chkMinimizeToTray.Checked;
            Settings.TrayOneClickActivation = chkSingleClickTrayActivation.Checked;
            Settings.StartupRun = chkRunAtStartup.Checked;

            //Carbon copy
            Settings.CarbonCopyUse = chbUseCarbonCopy.Checked;
            Settings.CarbonCopyDirectory = txtCarbonCopyFolder.Text;
            Settings.CarbonCopyIgnoreErrors = chbCarbonCopyIgnoreCopyErrors.Checked;
        }

        private void btnAllowSave_Click(object sender, EventArgs e) {
            if (Medo.MessageBox.ShowQuestion(this, "Do you allow this program use of registry in order to save its settings?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                btnAllowSave.Visible = false;
                Settings.NoRegistryWrites = false;
                SetWritableState(true);
            }
        }


        #region Helper

        private void SetWritableState(bool newState) {
            btnAllowSave.Visible = !newState;

            btnOk.Enabled = newState;
            foreach (Control control in tab.Controls) { control.Enabled = newState; }
        }

        private static string GetKeyString(Keys keyData) {
            if ((keyData & Keys.LWin) == Keys.LWin) { return string.Empty; }
            if ((keyData & Keys.RWin) == Keys.RWin) { return string.Empty; }

            var sb = new System.Text.StringBuilder();
            bool usesShift = false;
            bool usesCtrl = false;
            bool usesAlt = false;

            if ((keyData & Keys.Control) == Keys.Control) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Ctrl");
                keyData = keyData ^ Keys.Control;
                usesCtrl = true;
            }

            if ((keyData & Keys.Alt) == Keys.Alt) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Alt");
                keyData = keyData ^ Keys.Alt;
                usesAlt = true;
            }

            if ((keyData & Keys.Shift) == Keys.Shift) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Shift");
                keyData = keyData ^ Keys.Shift;
                usesShift = true;
            }

            switch (keyData) {
                case 0: return string.Empty;
                case Keys.ControlKey: return string.Empty;
                case Keys.Menu: return string.Empty;
                case Keys.ShiftKey: return string.Empty;
                default:
                    if (!((usesCtrl && usesAlt) || (usesCtrl && usesShift) || (usesAlt && usesShift))) {
                        return string.Empty;
                    }
                    if (sb.Length > 0) { sb.Append("+"); }
                    sb.Append(keyData.ToString());
                    return sb.ToString();
            }
        }

        private static string GetFontText(System.Drawing.Font font) {
            var sb = new System.Text.StringBuilder();
            sb.Append(font.Name);
            if (font.Bold) { sb.Append(", bold"); }
            if (font.Italic) { sb.Append(", italic"); }
            sb.Append(", " + font.SizeInPoints.ToString() + " pt");
            return sb.ToString();
        }

        #endregion

    }
}
