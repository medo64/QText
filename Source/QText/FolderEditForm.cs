using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QText {
    internal partial class FolderEditForm : Form {

        public FolderEditForm(DocumentFolder currentFolder) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            mnu.Renderer = Helper.ToolstripRenderer;
            Helper.ScaleToolstrip(mnu);

            this.CurrentFolder = currentFolder;

            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);
        }


        internal bool SuppressMenuKey = false;

        protected override bool ProcessDialogKey(Keys keyData) {
            Debug.WriteLine("FolderEditForm_ProcessDialogKey: " + keyData.ToString());
            if (((keyData & Keys.Alt) == Keys.Alt) && (keyData != (Keys.Alt | Keys.Menu))) { this.SuppressMenuKey = true; }

            switch (keyData) {
                case Keys.F10:
                case Keys.Apps:
                    ToggleMenu();
                    return true;

                case Keys.Control | Keys.N:
                    mnuNewFolder.PerformClick();
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
            Debug.WriteLine("FolderEditForm_OnKeyUp: " + e.KeyData.ToString());
            if ((e != null) && (e.KeyData == Keys.Menu)) {
                if (this.SuppressMenuKey) { this.SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyUp(e);
            }
        }


        public DocumentFolder CurrentFolder { get; private set; }


        private void Form_Load(object sender, System.EventArgs e) {
            foreach (var folder in App.Document.GetSubFolders()) {
                lsv.Items.Add(new ListViewItem(folder.Title) { Tag = folder });
            }
            foreach (ListViewItem item in lsv.Items) {
                var folder = (DocumentFolder)item.Tag;
                if (this.CurrentFolder.Equals(folder)) {
                    item.Focused = true;
                    item.Selected = true;
                    break;
                }
            }
        }


        private void lsv_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (e.Label == null) { e.CancelEdit = true; return; }
            var oldFolder = (DocumentFolder)lsv.Items[e.Item].Tag;

            var newTitle = e.Label.Trim();
            if (string.Equals(oldFolder.Title, newTitle, StringComparison.Ordinal)) {
                e.CancelEdit = true;
            } else {
                try {
                    oldFolder.Rename(newTitle);
                } catch (InvalidOperationException ex) {
                    e.CancelEdit = true;
                    Medo.MessageBox.ShowError(this, string.Format(CultureInfo.CurrentUICulture, "Cannot rename folder.\n\n{0}", ex.Message));
                }
            }
        }

        private void lsv_ItemActivate(object sender, System.EventArgs e) {
            if (lsv.SelectedItems.Count == 1) {
                this.CurrentFolder = (DocumentFolder)lsv.SelectedItems[0].Tag;
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
                mnuNewFolder.Select();
            }
        }


        #region Menu

        private void mnuNew_Click(object sender, System.EventArgs e) {
            try {
                var newFolder = App.Document.CreateFolder();

                var item = new ListViewItem(newFolder.Title) { Tag = newFolder };
                lsv.Items.Add(item);
                item.Focused = true;
                item.Selected = true;
                item.BeginEdit();
            } catch (InvalidOperationException ex) {
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
                var folder = (DocumentFolder)lsv.SelectedItems[0].Tag;
                if (Medo.MessageBox.ShowQuestion(this, string.Format(CultureInfo.CurrentUICulture, "Do you really want to delete folder \"{0}\"?", folder), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                    if (!folder.IsEmpty && Medo.MessageBox.ShowQuestion(this, string.Format(CultureInfo.CurrentUICulture, "Folder \"{0}\" is not empty. Do you really want to delete it?", folder), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) != DialogResult.Yes) {
                        return;
                    }

                    try {
                        folder.Delete();
                        lsv.Items.RemoveAt(lsv.SelectedItems[0].Index);
                        if (lsv.FocusedItem != null) { lsv.FocusedItem.Selected = true; }
                        if (this.CurrentFolder.Equals(folder)) {
                            this.CurrentFolder = App.Document.RootFolder;
                        }
                    } catch (InvalidOperationException ex) {
                        Medo.MessageBox.ShowError(this, string.Format(CultureInfo.CurrentUICulture, "Cannot delete folder.\n\n{0}", ex.Message));
                    }
                }
            }
        }

        #endregion

    }
}
