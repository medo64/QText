using System;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace QText {
    internal partial class FindForm : Form {

        private TabFiles _tabFiles;

        public FindForm(TabFiles tabFiles) {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;

            this._tabFiles = tabFiles;
        }


        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) {
            switch (keyData) {
                case Keys.Control | Keys.F:
                    txtText.SelectAll();
                    txtText.Select();

                    return true;
                case Keys.F3:
                    btnFind_Click(null, null);

                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void FindForm_Load(object sender, EventArgs e) {
            if (_tabFiles.SelectedTab != null) {
                TabFile tf = _tabFiles.SelectedTab;
                if (tf.TextBox.SelectedText.Length > 0) {
                    SearchStatus.Text = tf.TextBox.SelectedText;
                }
            }

            txtText.Text = SearchStatus.Text;
            chbCaseSensitive.Checked = SearchStatus.CaseSensitive;
            switch (SearchStatus.Scope) {
                case SearchScope.Folders: radioFolders.Checked = true; break;
                case SearchScope.Folder: radioFolder.Checked = true; break;
                default: radioFile.Checked = true; break;
            }
        }


        private void txtText_TextChanged(object sender, EventArgs e) {
            btnFind.Enabled = (txtText.Text.Length > 0);
        }

        private void btnFind_Click(object sender, EventArgs e) {
            SearchStatus.Text = txtText.Text;
            SearchStatus.CaseSensitive = chbCaseSensitive.Checked;
            SearchStatus.Scope = SearchStatus.GetScope(radioFile.Checked, radioFolder.Checked, radioFolders.Checked);

            try {
                Cursor.Current = Cursors.WaitCursor;

                if (Search.FindNext(this, this._tabFiles, this._tabFiles.SelectedTab)) {
                    Rectangle selRect = this._tabFiles.SelectedTab.GetSelectedRectangle();
                    var thisRect = this.Bounds;
                    if ((thisRect.IntersectsWith(selRect))) {
                        Rectangle screenRect = Screen.GetWorkingArea(selRect.Location);
                        int rightSpace = screenRect.Right - selRect.Right;
                        int leftSpace = selRect.Left - screenRect.Left;
                        int topSpace = selRect.Top - screenRect.Top;
                        int bottomSpace = screenRect.Bottom - selRect.Bottom;

                        if ((bottomSpace >= thisRect.Height)) {
                            this.Location = new Point(thisRect.Left, selRect.Bottom);
                        } else if ((topSpace >= thisRect.Height)) {
                            this.Location = new Point(thisRect.Left, selRect.Top - thisRect.Height);
                        } else if ((rightSpace >= thisRect.Width)) {
                            this.Location = new Point(selRect.Right, thisRect.Top);
                        } else if ((leftSpace >= thisRect.Width)) {
                            this.Location = new Point(selRect.Left - thisRect.Width, thisRect.Top);
                        }
                    }
                }
            } finally {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }


        internal void FindNext() {
            btnFind_Click(null, null);
        }

    }
}
