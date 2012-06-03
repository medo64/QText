using System;
using System.Drawing;
using System.Windows.Forms;

namespace QText {
    internal partial class PasswordForm : Form {
        public PasswordForm(string title) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Text = "QText: " + title;
        }


        public string Password { get; private set; }


        private void btnOK_Click(object sender, EventArgs e) {
            this.Password = txtPassword.Text;
        }

    }
}
