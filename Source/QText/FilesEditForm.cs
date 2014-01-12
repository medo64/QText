using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QText {
    internal partial class FilesEditForm : Form {

        public FilesEditForm(TabFiles tabFiles) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.TabFiles = tabFiles;
            mnu.Renderer = Helper.ToolstripRenderer;

            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);
        }


        internal bool SuppressMenuKey = false;

        protected override bool ProcessDialogKey(Keys keyData) {
            Debug.WriteLine("FilesEditForm_ProcessDialogKey: " + keyData.ToString());
            if (((keyData & Keys.Alt) == Keys.Alt) && (keyData != (Keys.Alt | Keys.Menu))) { this.SuppressMenuKey = true; }

            switch (keyData) {
                case Keys.F10:
                case Keys.Apps:
                    ToggleMenu();
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

        protected override void OnKeyUp(KeyEventArgs e) {
            Debug.WriteLine("FilesEditForm_OnKeyUp: " + e.KeyData.ToString());
            if ((e != null) && (e.KeyData == Keys.Menu)) {
                if (this.SuppressMenuKey) { this.SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyUp(e);
            }
        }


        public TabFiles TabFiles { get; private set; }


        private void Form_Load(object sender, System.EventArgs e) {
            foreach (var page in this.TabFiles.TabPages) {
                var file = (TabFile)page;
                lsv.Items.Add(new ListViewItem(file.Text) { Tag = file });
            }
            foreach (ListViewItem item in lsv.Items) {
                if (this.TabFiles.SelectedTab != null) {
                    if (string.Equals(item.Text, this.TabFiles.SelectedTab.Text, StringComparison.Ordinal)) {
                        item.Focused = true;
                        item.Selected = true;
                        break;
                    }
                }
            }
        }


        private void lsv_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (e.Label == null) { e.CancelEdit = true; return; }
            var oldName = lsv.Items[e.Item].Text;
            var newName = e.Label.Trim();
            try {
                if (string.IsNullOrEmpty(newName) || string.Equals(oldName, newName, StringComparison.Ordinal)) {
                    e.CancelEdit = true;
                } else {
                    try {
                        var tab = (TabFile)lsv.Items[e.Item].Tag;
                        if (QFileInfo.IsNameAlreadyTaken(tab.CurrentFile.DirectoryName, newName)) {
                            e.CancelEdit = true;
                        } else {
                            tab.Rename(newName);
                        }
                    } catch (Exception) {
                        e.CancelEdit = true;
                        throw;
                    }
                }
            } catch (Exception ex) {
                Medo.MessageBox.ShowError(this, string.Format(CultureInfo.CurrentUICulture, "Cannot rename folder.\n\n{0}", ex.Message));
            }
        }

        private void lsv_ItemActivate(object sender, System.EventArgs e) {
            if (lsv.SelectedItems.Count == 1) {
                this.TabFiles.SelectedTab = (TabFile)lsv.SelectedItems[0].Tag;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void lsv_Resize(object sender, EventArgs e) {
            lsv.Columns[0].Width = lsv.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;
        }


        private void ToggleMenu() {
            if (mnu.ContainsFocus) {
                lsv.Select();
            } else {
                mnu.Select();
                mnuRename.Select();
            }
        }


        #region Menu

        private void mnuRename_Click(object sender, System.EventArgs e) {
            if (lsv.SelectedItems.Count == 1) {
                lsv.SelectedItems[0].BeginEdit();
            }
        }

        private void mnuDelete_Click(object sender, System.EventArgs e) {
            if (lsv.SelectedItems.Count == 1) {
                var tabFile = (TabFile)lsv.SelectedItems[0].Tag;
                if (Helper.DeleteTabFile(this, this.TabFiles, tabFile)) {
                    lsv.Items.Remove(lsv.SelectedItems[0]);
                }
            }
        }

        private void mnuSort_Click(object sender, EventArgs e) {
            DocumentFolder.CleanOrderedTitles(this.TabFiles.CurrentFolder);
            this.TabFiles.FolderOpen(this.TabFiles.CurrentFolder, false);
        }

        #endregion

    }
}
