using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace QText {
    internal partial class FolderEditForm : Form {

        public FolderEditForm(string currentFolder) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.CurrentFolder = currentFolder;
        }


        protected override bool ProcessDialogKey(Keys keyData) {
            Debug.WriteLine("ProcessDialogKey: " + keyData.ToString());
            switch (keyData) {
                case Keys.F10:
                case Keys.Apps:
                    if (mnu.ContainsFocus) {
                        this.SelectNextControl(mnu, true, true, true, true);
                    } else {
                        mnu.Select();
                        mnuNew.Select();
                    }
                    return true;

                case Keys.Control | Keys.N:
                    mnuNew.PerformClick();
                    return true;

                case Keys.F2:
                    mnuRename.PerformClick();
                    return true;

                case Keys.Delete:
                    mnuDelete.PerformClick();
                    return true;

                case Keys.Escape:
                    this.Close();
                    return true;

                default: return base.ProcessDialogKey(keyData);
            }
        }


        public string CurrentFolder { get; private set; }


        private void Form_Load(object sender, System.EventArgs e) {
            lsv.Columns[0].Width = lsv.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;

            foreach (var folder in TabFiles.GetSubFolders()) {
                lsv.Items.Add(folder);
            }
            foreach (ListViewItem item in lsv.Items) {
                if (string.Equals(item.Text, this.CurrentFolder, StringComparison.Ordinal)) {
                    item.Focused = true;
                    item.Selected = true;
                    break;
                }
            }
        }


        private void lsv_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (e.Label == null) { e.CancelEdit = true; return; }
            var oldName = lsv.Items[e.Item].Text;
            var newName = e.Label.Trim();
            try {
                if (string.Equals(oldName, newName, StringComparison.Ordinal)) {
                    e.CancelEdit = true;
                } else if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase)) {
                    var tempName = Guid.NewGuid().ToString();
                    try {
                        Directory.Move(Path.Combine(Settings.FilesLocation, oldName), Path.Combine(Settings.FilesLocation, tempName));
                    } catch (Exception) {
                        e.CancelEdit = true;
                        throw;
                    }
                    try {
                        Directory.Move(Path.Combine(Settings.FilesLocation, tempName), Path.Combine(Settings.FilesLocation, newName));
                    } catch (Exception) { //something is wrong - at least give current name to user
                        lsv.Items[e.Item].Text = tempName;
                        if (string.Equals(oldName, this.CurrentFolder, StringComparison.Ordinal)) {
                            this.CurrentFolder = tempName;
                        }
                        e.CancelEdit = true;
                        throw;
                    }
                } else {
                    try {
                        Directory.Move(Path.Combine(Settings.FilesLocation, oldName), Path.Combine(Settings.FilesLocation, newName));
                    } catch (Exception) {
                        e.CancelEdit = true;
                        throw;
                    }
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowError(this, string.Format(CultureInfo.CurrentUICulture, "Cannot rename folder.\n\n{0}", ex.Message));
            }
            if ((e.CancelEdit == false) && string.Equals(oldName, this.CurrentFolder, StringComparison.Ordinal)) {
                this.CurrentFolder = newName;
            }
        }

        private void lsv_ItemActivate(object sender, System.EventArgs e) {
            if (lsv.SelectedItems.Count == 1) {
                this.CurrentFolder = lsv.SelectedItems[0].Text;
                this.DialogResult = DialogResult.OK;
            }
        }

        #region Menu

        private void mnuNew_Click(object sender, System.EventArgs e) {
            var newFolder = "New folder";
            if (Directory.Exists(Path.Combine(Settings.FilesLocation, newFolder))) {
                int n = 1;
                while (true) {
                    newFolder = string.Format(CultureInfo.CurrentUICulture, "New folder ({0})", n);
                    if (Directory.Exists(Path.Combine(Settings.FilesLocation, newFolder)) == false) {
                        break;
                    }
                    n += 1;
                }
            }
            try {
                Directory.CreateDirectory(Path.Combine(Settings.FilesLocation, newFolder));
                var item = new ListViewItem(newFolder);
                lsv.Items.Add(item);
                item.Focused = true;
                item.Selected = true;
                item.BeginEdit();
            } catch (Exception ex) {
                Medo.MessageBox.ShowError(this, string.Format(CultureInfo.CurrentUICulture, "Cannot create folder.\n\n{0}", ex.Message));
            }
        }

        private void mnuRename_Click(object sender, System.EventArgs e) {
            if (lsv.SelectedItems.Count == 1) {
                lsv.SelectedItems[0].BeginEdit();
            }
        }

        private void mnuDelete_Click(object sender, System.EventArgs e) {
            if (lsv.SelectedItems.Count == 1) {
                var folder = lsv.SelectedItems[0].Text;
                if (Medo.MessageBox.ShowQuestion(this, string.Format(CultureInfo.CurrentUICulture, "Do you really want to delete folder \"{0}\"?", folder), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                    try {
                        if (Settings.FilesDeleteToRecycleBin) {
                            SHFile.DeleteDirectory(Path.Combine(Settings.FilesLocation, folder));
                        } else {
                            Directory.Delete(Path.Combine(Settings.FilesLocation, folder), true);
                        }
                        lsv.Items.RemoveAt(lsv.SelectedItems[0].Index);
                        if (lsv.FocusedItem != null) { lsv.FocusedItem.Selected = true; }
                        if (string.Equals(folder, this.CurrentFolder, StringComparison.Ordinal)) {
                            this.CurrentFolder = null;
                        }
                    } catch (Exception ex) {
                        Medo.MessageBox.ShowError(this, string.Format(CultureInfo.CurrentUICulture, "Cannot delete folder.\n\n{0}", ex.Message));
                    }
                }
            }
        }

        #endregion

    }
}
