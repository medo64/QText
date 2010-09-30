using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;

namespace QText {
    public partial class FindForm : Form {

        private TabControlDnD _tabFiles;

        public FindForm(TabControlDnD tabFiles) {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;

            this._tabFiles = tabFiles;
        }


        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) {
            switch (keyData) {
                case Keys.Control | Keys.F:
                    txtText.SelectAll();
                    txtText.Focus();

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
        }


        private void txtText_TextChanged(object sender, EventArgs e) {
            btnFind.Enabled = (txtText.Text.Length > 0);
        }

        private void btnFind_Click(object sender, EventArgs e) {
            if (txtText.Text.Length > 0) {
                SearchStatus.Text = txtText.Text;
                SearchStatus.CaseSensitive = chbCaseSensitive.Checked;
                if ((_tabFiles.SelectedTab != null) && (!string.IsNullOrEmpty(SearchStatus.Text))) {
                    TabFile tf = _tabFiles.SelectedTab;
                    //found characters, move window if needed
                    if ((tf.Find(SearchStatus.Text, SearchStatus.CaseSensitive))) {
                        Rectangle selRect = tf.GetSelectedRectangle();
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
                    } else {
                        Medo.MessageBox.ShowInformation(this, "Text \"" + SearchStatus.Text + "\" cannot be found.");
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }


    }
}
