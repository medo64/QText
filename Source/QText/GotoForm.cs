using System;
using System.Drawing;
using System.Windows.Forms;

namespace QText {
    internal partial class GotoForm : Form {
        public GotoForm(bool hasText) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            Helper.ScaleGotoImageList(this, imlSuggestions);

            this.HasText = hasText;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);
        }

        private readonly Boolean HasText;


        private void Form_Load(object sender, EventArgs e) {
            txtWhere_TextChanged(null, null);
        }

        private void txtWhere_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            switch (e.KeyData) {
                case Keys.Up:
                case Keys.Down:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void txtWhere_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Up:
                    if (lsvSuggestions.Items.Count > 0) {
                        var index = lsvSuggestions.SelectedItems[lsvSuggestions.SelectedItems.Count - 1].Index - 1;
                        if (index >= 0) {
                            lsvSuggestions.Items[index].Selected = true;
                            lsvSuggestions.Items[index].EnsureVisible();
                        }
                    }
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Down:
                    if (lsvSuggestions.Items.Count > 0) {
                        var index = lsvSuggestions.SelectedItems[lsvSuggestions.SelectedItems.Count - 1].Index + 1;
                        if (index < lsvSuggestions.Items.Count) {
                            lsvSuggestions.Items[index].Selected = true;
                            lsvSuggestions.Items[index].EnsureVisible();
                        }
                    }
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void txtWhere_TextChanged(object sender, EventArgs e) {
            lsvSuggestions.BeginUpdate();
            lsvSuggestions.Items.Clear();
            foreach (var result in GotoResult.GetSuggestions(txtWhere.Text.Trim(), this.HasText)) {
                lsvSuggestions.Items.Add(new ListViewItem(result.ToString()) { Tag = result, ImageIndex = result.ImageIndex });
            }
            lsvSuggestions.EndUpdate();
            if (lsvSuggestions.Items.Count > 0) {
                lsvSuggestions.Items[0].Selected = true;
                btnGoto.Enabled = true;
            } else {
                btnGoto.Enabled = false;
            }
            txtWhere.Select();
        }


        private void lsvSuggestions_ItemActivate(object sender, EventArgs e) {
            if (btnGoto.Enabled) { this.AcceptButton.PerformClick(); }
        }

        private void lsvSuggestions_SelectedIndexChanged(object sender, EventArgs e) {
            btnGoto.Enabled = (lsvSuggestions.SelectedItems.Count > 0);
        }

        private void lsvSuggestions_Resize(object sender, EventArgs e) {
            lsvSuggestions_colTitle.Width = lsvSuggestions.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;
        }


        private void btnGoto_Click(object sender, EventArgs e) {
            if (lsvSuggestions.SelectedItems.Count > 0) {
                this.SelectedItem = (GotoResult)(lsvSuggestions.SelectedItems[0].Tag);
            }
        }

        public GotoResult SelectedItem { get; private set; }

    }
}
