using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QText {
    internal partial class FilesEditForm : Form {

        public FilesEditForm(TabFiles tabFiles) {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            TabFiles = tabFiles;
            mnu.Renderer = Helper.ToolstripRenderer;
            Helper.ScaleToolstrip(mnu);

            Medo.Windows.Forms.State.Attach(this);
        }


        internal bool SuppressMenuKey = false;

        protected override bool ProcessDialogKey(Keys keyData) {
            Debug.WriteLine("FilesEditForm_ProcessDialogKey: " + keyData.ToString());
            if (((keyData & Keys.Alt) == Keys.Alt) && (keyData != (Keys.Alt | Keys.Menu))) { SuppressMenuKey = true; }

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
                    Close();
                    return true;

                default: return base.ProcessDialogKey(keyData);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            Debug.WriteLine("FilesEditForm_OnKeyUp: " + e.KeyData.ToString());
            if ((e != null) && (e.KeyData == Keys.Menu)) {
                if (SuppressMenuKey) { SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyUp(e);
            }
        }


        public TabFiles TabFiles { get; private set; }


        private void Form_Load(object sender, System.EventArgs e) {
            foreach (var page in TabFiles.TabPages) {
                var file = (TabFile)page;
                lsv.Items.Add(new ListViewItem(file.Text) { Tag = file });
            }
            foreach (ListViewItem item in lsv.Items) {
                if (TabFiles.SelectedTab != null) {
                    if (string.Equals(item.Text, TabFiles.SelectedTab.Text, StringComparison.Ordinal)) {
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
            var newTitle = e.Label.Trim();
            if (string.IsNullOrEmpty(newTitle) || string.Equals(oldName, newTitle, StringComparison.Ordinal)) {
                e.CancelEdit = true;
            } else {
                try {
                    var tab = (TabFile)lsv.Items[e.Item].Tag;
                    if (tab.BaseFile.Folder.GetFileByTitle(newTitle) != null) {
                        e.CancelEdit = true;
                    } else {
                        tab.Rename(newTitle);
                    }
                } catch (InvalidOperationException ex) {
                    e.CancelEdit = true;
                    Medo.MessageBox.ShowError(this, string.Format(CultureInfo.CurrentUICulture, "Cannot rename file.\n\n{0}", ex.Message));
                }
            }
        }

        private void lsv_ItemActivate(object sender, System.EventArgs e) {
            if (lsv.SelectedItems.Count == 1) {
                TabFiles.SelectedTab = (TabFile)lsv.SelectedItems[0].Tag;
                DialogResult = DialogResult.OK;
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
                if (Helper.DeleteTabFile(this, TabFiles, tabFile)) {
                    lsv.Items.Remove(lsv.SelectedItems[0]);
                }
            }
        }

        private void mnuSort_Click(object sender, EventArgs e) {
            TabFiles.CurrentFolder.Sort();
            TabFiles.FolderOpen(TabFiles.CurrentFolder, saveBeforeOpen: false);
            DialogResult = DialogResult.OK;
        }

        #endregion

    }
}
