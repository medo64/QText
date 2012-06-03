using System;
using System.Drawing;
using System.Windows.Forms;

namespace QText {
    internal partial class ChangePasswordForm : Form {
        public ChangePasswordForm(string title) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            erp.SetIconAlignment(txtVerify, ErrorIconAlignment.MiddleLeft);

            this.Text = "QText: " + title;
        }


        public string Password { get; private set; }


        private void txt_TextChanged(object sender, EventArgs e) {
            var isValid = txtPassword.Text.Equals(txtVerify.Text, StringComparison.Ordinal);
            btnOK.Enabled = isValid;
            if (isValid) {
                erp.SetError(txtVerify, null);
            } else {
                erp.SetError(txtVerify, "Passwords do not match.");
            }
        }

        private void txt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyData == Keys.Enter) { e.IsInputKey = false; }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                e.Handled = true;
                txtVerify.Select();
            }
        }

        private void txtVerify_KeyDown(object sender, KeyEventArgs e) {
            if ((e.KeyData == Keys.Enter) && btnOK.Enabled) {
                e.Handled = true;
                btnOK.PerformClick();
            }
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.Password = txtPassword.Text;
            this.DialogResult = DialogResult.OK;
        }

    }
}
