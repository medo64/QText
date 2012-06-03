using System;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal partial class FileRenameForm : Form {
        public FileRenameForm(string basePath, string oldTitle) {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            erp.SetIconAlignment(txtTitle, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(txtTitle, 2);

            this.BasePath = basePath;
            this.OldTitle = oldTitle;

            txtTitle.Text = oldTitle;
        }


        private readonly string BasePath;

        public readonly string OldTitle;
        public string NewTitle { get; private set; }


        private void txtTitle_TextChanged(object sender, EventArgs e) {
            bool alreadyTaken = false;
            if (string.Equals(this.OldTitle, txtTitle.Text, StringComparison.OrdinalIgnoreCase) == false) {
                var newFileTitle = Helper.EncodeFileName(txtTitle.Text);
                alreadyTaken = QFileInfo.IsNameAlreadyTaken(this.BasePath, newFileTitle);
                if (alreadyTaken) { erp.SetError(txtTitle, "File with same name already exists."); } else { erp.SetError(txtTitle, null); }
            }
            btnOK.Enabled = (txtTitle.Text.Length > 0) && (alreadyTaken == false) && (txtTitle.Text.Equals(this.OldTitle, StringComparison.Ordinal) == false);
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.NewTitle = txtTitle.Text;
        }

    }
}
