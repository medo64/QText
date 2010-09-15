using System;
using System.IO;
using System.Windows.Forms;

namespace QTextAux {
    internal partial class NewFileForm : Form {
        public NewFileForm(string fileName) {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;

            this.FileName = fileName;
            txtFileName.Text = fileName;
            if (Settings.IsRichTextFileDefault) {
                radRichText.Checked = true;
            }
        }

        public string FileName {
            get;
            private set;
        }

        public bool IsRichText {
            get;
            private set;
        }


        private void txtFileName_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar.ToString().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                e.Handled = !Char.IsControl(e.KeyChar);
            }
        }

        private void txtFileName_TextChanged(object sender, EventArgs e) {
            if (txtFileName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                var sb = new System.Text.StringBuilder(txtFileName.Text);
                var ic = System.IO.Path.GetInvalidFileNameChars();
                for (int i = 0; i < ic.Length; ++i) {
                    sb.Replace(ic[i].ToString(), "");
                }
                txtFileName.Text = sb.ToString();
            }
            btnOK.Enabled = (txtFileName.Text.Length > 0);
        }

        private void btnOK_Click(object sender, EventArgs e) {
            this.FileName = txtFileName.Text;
            this.IsRichText = radRichText.Checked;
            Settings.IsRichTextFileDefault = radRichText.Checked;
        }

    }
}
