using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal partial class OptionsForm : Form {
        public OptionsForm() {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
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
            chkShowToolbar.Checked = Settings.ShowToolbar;
            chkShowInTaskbar.Checked = Settings.DisplayShowInTaskbar;
            chbMultilineTabHeaders.Checked = Settings.DisplayMultilineTabHeader;
            chbShowMinimizeMaximizeButtons.Checked = Settings.DisplayMinimizeMaximizeButtons;
            chbHorizontalScrollbar.Checked = (Settings.ScrollBars == ScrollBars.Horizontal) || (Settings.ScrollBars == ScrollBars.Both);
            chbVerticalScrollbar.Checked = (Settings.ScrollBars == ScrollBars.Vertical) || (Settings.ScrollBars == ScrollBars.Both);
            chbZoomToolbarWithDpi.Checked = Settings.ZoomToolbarWithDpiChange;

            //Files
            chkRememberSelectedFile.Checked = Settings.StartupRememberSelectedFile;
            chkPreloadFilesOnStartup.Checked = Settings.FilesPreload;
            chkDeleteToRecycleBin.Checked = Settings.FilesDeleteToRecycleBin;
            nudQuickSaveIntervalInSeconds.Value = Settings.QuickSaveInterval / 1000;

            //Behavior
            txtHotkey.Text = GetKeyString(Settings.ActivationHotkey);
            txtHotkey.Tag = Settings.ActivationHotkey;
            chkMinimizeToTray.Checked = Settings.TrayOnMinimize;
            chkSingleClickTrayActivation.Checked = Settings.TrayOneClickActivation;
            chkRunAtStartup.Checked = Settings.StartupRun;

            //Carbon copy
            chbUseCarbonCopy.Checked = Settings.CarbonCopyUse;
            txtCarbonCopyFolder.Text = Settings.CarbonCopyFolder;
            chkCarbonCopyFolderCreate.Checked = Settings.CarbonCopyCreateFolder;
            chbCarbonCopyIgnoreCopyErrors.Checked = Settings.CarbonCopyIgnoreErrors;
            chbUseCarbonCopy_CheckedChanged(null, null);

            if (App.Hotkey.IsRegistered) {
                App.Hotkey.Unregister();
            }
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e) {
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
            using (var fd = new SaveFileDialog()) {
                fd.CheckFileExists = false;
                fd.CheckPathExists = true;
                fd.CreatePrompt = false;
                fd.Filter = "All files|" + Guid.Empty.ToString();
                fd.FileName = "any";
                fd.InitialDirectory = Settings.FilesLocation;
                fd.OverwritePrompt = false;
                fd.Title = "Please select folder for storage of files";
                fd.ValidateNames = false;
                if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                    System.IO.FileInfo selectedFile = new System.IO.FileInfo(fd.FileName);
                    string selectedPath = selectedFile.DirectoryName;
                    if ((string.Compare(selectedPath, Settings.CarbonCopyFolder, true) == 0)) {
                        global::Medo.MessageBox.ShowWarning(this, "This folder is currenly used for carbon copy. Move will be aborted.");
                    } else if ((string.Compare(selectedPath, Settings.FilesLocation, true) == 0)) {
                        global::Medo.MessageBox.ShowWarning(this, "This folder is already used for storage. Move will be aborted.");
                    } else {
                        string newPath = selectedPath;

                        bool isOK = true;
                        try {
                            Helper.CreatePath(newPath);
                            try {
                                var oldFiles = new List<string>();
                                oldFiles.AddRange(System.IO.Directory.GetFiles(Settings.FilesLocation, "*.txt"));
                                oldFiles.AddRange(System.IO.Directory.GetFiles(Settings.FilesLocation, "*.rtf"));
                                if ((oldFiles.Count > 0)) {
                                    switch (Medo.MessageBox.ShowQuestion(this, "Do you want to copy existing files to new location?", MessageBoxButtons.YesNo)) {
                                        case DialogResult.Yes:
                                            try {
                                                System.IO.File.Copy(System.IO.Path.Combine(Settings.FilesLocation, "QText.xml"), System.IO.Path.Combine(newPath, "QText.xml"));
                                            } catch (Exception) { }
                                            for (int i = 0; i <= oldFiles.Count - 1; i++) {
                                                try {
                                                    string oldFile = oldFiles[i];
                                                    System.IO.FileInfo oldFI = new System.IO.FileInfo(oldFile);
                                                    string newFile = System.IO.Path.Combine(newPath, oldFI.Name);
                                                    if (System.IO.File.Exists(newFile)) {
                                                        switch (Medo.MessageBox.ShowQuestion(this, "File \"" + oldFI.Name + "\" already exists at destination. Do you want to overwrite?", MessageBoxButtons.YesNoCancel)) {
                                                            case DialogResult.Yes:
                                                                var oldAttr = File.GetAttributes(oldFile);
                                                                File.SetAttributes(newFile, FileAttributes.Normal);
                                                                File.Copy(oldFile, newFile, true);
                                                                File.SetAttributes(newFile, oldAttr);
                                                                break;
                                                            case DialogResult.No:
                                                                continue;
                                                            case DialogResult.Cancel:
                                                                return;
                                                        }
                                                    } else {
                                                        System.IO.File.Copy(oldFile, newFile);
                                                    }
                                                } catch (Exception ex) {
                                                    Medo.MessageBox.ShowWarning(this, "Error copying files." + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                                                    isOK = false;
                                                }
                                            }

                                            break;
                                    }
                                }
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            } catch (Exception ex) {
                                Medo.MessageBox.ShowWarning(this, "Error retrieving old path." + Environment.NewLine + ex.Message, MessageBoxButtons.OK);
                                isOK = false;
                            }
                            if (isOK) {
                                Settings.FilesLocation = newPath;
                                Medo.MessageBox.ShowInformation(this, "Data location transfer succeeded.");
                            } else {
                                Medo.MessageBox.ShowWarning(this, "Data location transfer succeeded with some errors.");
                            }
                        } catch (Exception ex) {
                            Medo.MessageBox.ShowWarning(this, ex.Message, MessageBoxButtons.OK);
                        }

                    }
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
            chkCarbonCopyFolderCreate.Enabled = chbUseCarbonCopy.Checked;
            chbCarbonCopyIgnoreCopyErrors.Enabled = chbUseCarbonCopy.Checked;
            btnCarbonCopyOpenFolder.Enabled = txtCarbonCopyFolder.Text.Length > 0;
        }

        private void txtCarbonCopyFolder_TextChanged(object sender, EventArgs e) {
            btnCarbonCopyOpenFolder.Enabled = txtCarbonCopyFolder.Text.Length > 0;
        }

        private void btnCarbonCopyFolderSelect_Click(object sender, EventArgs e) {
            using (var fd = new FolderBrowserDialog()) {
                fd.RootFolder = Environment.SpecialFolder.Desktop;
                fd.Description = "Please select folder for carbon copy. It cannot be same folder that is used for main storage.";
                fd.ShowNewFolderButton = true;
                if (string.IsNullOrEmpty(Settings.CarbonCopyFolder)) {
                    fd.SelectedPath = Settings.FilesLocation;
                } else {
                    fd.SelectedPath = Settings.CarbonCopyFolder;
                }
                if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                    if ((string.Compare(fd.SelectedPath, Settings.FilesLocation, true) == 0)) {
                        Medo.MessageBox.ShowWarning(this, "Carbon copy folder cannot be same as one used for main program storage.");
                    } else {
                        txtCarbonCopyFolder.Text = fd.SelectedPath;
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


        private void btnOk_Click(object sender, EventArgs e) {
            //Appearance
            Settings.DetectUrls = chkDisplayURLs.Checked;
            Settings.FollowURLs = chkFollowURLs.Checked;
            Settings.DisplayTabWidth = Convert.ToInt32(nudTabWidth.Value);
            Settings.DisplayFont = (System.Drawing.Font)txtFont.Tag;
            Settings.DisplayBackgroundColor = lblColorExample.BackColor;
            Settings.DisplayForegroundColor = lblColorExample.ForeColor;

            //Display
            Settings.ShowToolbar = chkShowToolbar.Checked;
            Settings.DisplayShowInTaskbar = chkShowInTaskbar.Checked;
            Settings.DisplayMultilineTabHeader = chbMultilineTabHeaders.Checked;
            Settings.DisplayMinimizeMaximizeButtons = chbShowMinimizeMaximizeButtons.Checked;
            if ((chbHorizontalScrollbar.Checked && chbVerticalScrollbar.Checked)) {
                Settings.ScrollBars = ScrollBars.Both;
            } else if ((chbHorizontalScrollbar.Checked)) {
                Settings.ScrollBars = ScrollBars.Horizontal;
            } else if ((chbVerticalScrollbar.Checked)) {
                Settings.ScrollBars = ScrollBars.Vertical;
            } else {
                Settings.ScrollBars = ScrollBars.None;
            }
            Settings.ZoomToolbarWithDpiChange = chbZoomToolbarWithDpi.Checked;

            //Files
            Settings.StartupRememberSelectedFile = chkRememberSelectedFile.Checked;
            Settings.FilesPreload = chkPreloadFilesOnStartup.Checked;
            Settings.FilesDeleteToRecycleBin = chkDeleteToRecycleBin.Checked;
            Settings.QuickSaveInterval = Convert.ToInt32(nudQuickSaveIntervalInSeconds.Value) * 1000;

            //Behavior
            Settings.ActivationHotkey = (Keys)txtHotkey.Tag;
            Settings.TrayOnMinimize = chkMinimizeToTray.Checked;
            Settings.TrayOneClickActivation = chkSingleClickTrayActivation.Checked;
            Settings.StartupRun = chkRunAtStartup.Checked;

            //Carbon copy
            Settings.CarbonCopyUse = chbUseCarbonCopy.Checked;
            Settings.CarbonCopyFolder = txtCarbonCopyFolder.Text;
            Settings.CarbonCopyCreateFolder = chkCarbonCopyFolderCreate.Checked;
            Settings.CarbonCopyIgnoreErrors = chbCarbonCopyIgnoreCopyErrors.Checked;
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
