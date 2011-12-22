using System;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal partial class NewFileForm : Form {

        public NewFileForm() {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;

            if (Settings.IsRichTextFileDefault) {
                radRichText.Checked = true;
            }
        }


        public string Title { get; private set; }
        public bool IsRichText { get; private set; }


        private void txtTitle_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar.ToString().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                e.Handled = !Char.IsControl(e.KeyChar);
            }
        }

        private void txtTitle_TextChanged(object sender, EventArgs e) {
            if (txtTitle.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                var sb = new System.Text.StringBuilder(txtTitle.Text);
                var ic = System.IO.Path.GetInvalidFileNameChars();
                for (int i = 0; i < ic.Length; ++i) {
                    sb.Replace(ic[i].ToString(), "");
                }
                txtTitle.Text = sb.ToString();
            }
            btnOK.Enabled = (txtTitle.Text.Length > 0);
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.Title = txtTitle.Text;
            this.IsRichText = radRichText.Checked;
            Settings.IsRichTextFileDefault = radRichText.Checked;
        }

    }
}
