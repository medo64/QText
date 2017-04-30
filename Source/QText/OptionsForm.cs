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
        }

        private void OptionsForm_Load(object sender, EventArgs e) {
            //Appearance
            chkDisplayURLs.Checked = Settings.Current.DetectUrls;
            chkFollowURLs.Checked = Settings.Current.FollowURLs;
            nudTabWidth.Value = Settings.Current.DisplayTabWidth;
            txtFont.Text = GetFontText(Settings.Current.DisplayFont);
            txtFont.Tag = Settings.Current.DisplayFont;
            lblColorExample.BackColor = Settings.Current.DisplayBackgroundColor;
            lblColorExample.ForeColor = Settings.Current.DisplayForegroundColor;
            chkDisplayURLs_CheckedChanged(null, null);

            //Display
            chkShowInTaskbar.Checked = Settings.Current.DisplayShowInTaskbar;
            chbShowMinimizeMaximizeButtons.Checked = Settings.Current.DisplayMinimizeMaximizeButtons;
            chkShowToolbar.Checked = Settings.Current.ShowToolbar;
            chbMultilineTabs.Checked = Settings.Current.MultilineTabs;
            chbHorizontalScrollbar.Checked = (Settings.Current.ScrollBars == ScrollBars.Horizontal) || (Settings.Current.ScrollBars == ScrollBars.Both);
            chbVerticalScrollbar.Checked = (Settings.Current.ScrollBars == ScrollBars.Vertical) || (Settings.Current.ScrollBars == ScrollBars.Both);
            cmbSelectionDelimiters.Text = Settings.Current.SelectionDelimiters;

            //Files
            chkPreloadFilesOnStartup.Checked = Settings.Current.FilesPreload;
            chkDeleteToRecycleBin.Checked = Settings.Current.FilesDeleteToRecycleBin;
            nudQuickSaveIntervalInSeconds.Value = Settings.Current.QuickSaveInterval / 1000M;
            chbSavePlainWithLF.Checked = Settings.Current.PlainLineEndsWithLf;

            //Behavior
            txtHotkey.Text = GetKeyString(Settings.Current.ActivationHotkey);
            txtHotkey.Tag = Settings.Current.ActivationHotkey;
            chkMinimizeToTray.Checked = Settings.Current.TrayOnMinimize;
            chkSingleClickTrayActivation.Checked = Settings.Current.TrayOneClickActivation;
            chkRunAtStartup.Checked = Settings.Current.StartupRun;

            //Carbon copy
            chbUseCarbonCopy.Checked = Settings.Current.CarbonCopyUse;
            txtCarbonCopyFolder.Text = Settings.Current.CarbonCopyDirectory;
            chbCarbonCopyIgnoreCopyErrors.Checked = Settings.Current.CarbonCopyIgnoreErrors;
            chbUseCarbonCopy_CheckedChanged(null, null);
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.DialogResult == DialogResult.OK) {
                if (Settings.Current.ActivationHotkey != App.Hotkey.Key) {
                    if (App.Hotkey.IsRegistered) {
                        App.Hotkey.Unregister();
                    }
                    if (Settings.Current.ActivationHotkey != Keys.None) {
                        try {
                            App.Hotkey.Register(Settings.Current.ActivationHotkey);
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
                Process.Start(Settings.Current.FilesLocation, null);
            } catch (Win32Exception ex) {
                Medo.MessageBox.ShowWarning(this, ex.Message);
            }
        }

        private void btnChangeLocation_Click(object sender, EventArgs e) {
            using (var frm = new FolderOpenDialog() { InitialFolder = App.Document.RootPath }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    try {
                        var copier = new DocumentCopier(App.Document, frm.Folder);

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
                            Settings.Current.FilesLocation = App.Document.RootPath;
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
            if (hadRegistered && (Settings.Current.ActivationHotkey != Keys.None)) {
                try {
                    App.Hotkey.Register(Settings.Current.ActivationHotkey);
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
                if (string.IsNullOrEmpty(Settings.Current.CarbonCopyDirectory)) {
                    frm.InitialFolder = Settings.Current.FilesLocation;
                } else {
                    frm.InitialFolder = Settings.Current.CarbonCopyDirectory;
                }
                if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                    if (frm.Folder.StartsWith(Settings.Current.FilesLocation, StringComparison.OrdinalIgnoreCase)) {
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
            Settings.Current.DetectUrls = chkDisplayURLs.Checked;
            Settings.Current.FollowURLs = chkFollowURLs.Checked;
            Settings.Current.DisplayTabWidth = Convert.ToInt32(nudTabWidth.Value);
            Settings.Current.DisplayFont = (System.Drawing.Font)txtFont.Tag;
            Settings.Current.DisplayBackgroundColor = lblColorExample.BackColor;
            Settings.Current.DisplayForegroundColor = lblColorExample.ForeColor;

            //Display
            Settings.Current.DisplayShowInTaskbar = chkShowInTaskbar.Checked;
            Settings.Current.DisplayMinimizeMaximizeButtons = chbShowMinimizeMaximizeButtons.Checked;
            Settings.Current.ShowToolbar = chkShowToolbar.Checked;
            Settings.Current.MultilineTabs = chbMultilineTabs.Checked;
            if ((chbHorizontalScrollbar.Checked && chbVerticalScrollbar.Checked)) {
                Settings.Current.ScrollBars = ScrollBars.Both;
            } else if ((chbHorizontalScrollbar.Checked)) {
                Settings.Current.ScrollBars = ScrollBars.Horizontal;
            } else if ((chbVerticalScrollbar.Checked)) {
                Settings.Current.ScrollBars = ScrollBars.Vertical;
            } else {
                Settings.Current.ScrollBars = ScrollBars.None;
            }
            Settings.Current.SelectionDelimiters = cmbSelectionDelimiters.Text;

            //Files
            Settings.Current.FilesPreload = chkPreloadFilesOnStartup.Checked;
            Settings.Current.FilesDeleteToRecycleBin = chkDeleteToRecycleBin.Checked;
            Settings.Current.QuickSaveInterval = Convert.ToInt32(nudQuickSaveIntervalInSeconds.Value * 1000);
            Settings.Current.PlainLineEndsWithLf = chbSavePlainWithLF.Checked;

            //Behavior
            Settings.Current.ActivationHotkey = (Keys)txtHotkey.Tag;
            Settings.Current.TrayOnMinimize = chkMinimizeToTray.Checked;
            Settings.Current.TrayOneClickActivation = chkSingleClickTrayActivation.Checked;
            Settings.Current.StartupRun = chkRunAtStartup.Checked;

            //Carbon copy
            Settings.Current.CarbonCopyUse = chbUseCarbonCopy.Checked;
            Settings.Current.CarbonCopyDirectory = txtCarbonCopyFolder.Text;
            Settings.Current.CarbonCopyIgnoreErrors = chbCarbonCopyIgnoreCopyErrors.Checked;
        }


        private void btnAdvanced_Click(object sender, EventArgs e) {
            using (var frm = new OptionsAdvancedForm()) {
                frm.ShowDialog(this);
            }
        }


        #region Helper

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
