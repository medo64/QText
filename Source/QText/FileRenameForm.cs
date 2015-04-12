using System;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal partial class FileRenameForm : Form {
        public FileRenameForm(DocumentFile file) {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            erp.SetIconAlignment(txtTitle, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(txtTitle, 2);

            this.File = file;

            txtTitle.Text = this.File.Title;
        }


        private readonly string BasePath;

        public readonly DocumentFile File;
        public string NewTitle { get; private set; }


        private void txtTitle_TextChanged(object sender, EventArgs e) {
            bool alreadyTaken = false;
            if (string.Equals(this.File.Title, txtTitle.Text, StringComparison.OrdinalIgnoreCase) == false) {
                var newFileTitle = txtTitle.Text.Trim();
                alreadyTaken = (this.File.Folder.GetFileByTitle(newFileTitle) != null);
                if (alreadyTaken) { erp.SetError(txtTitle, "File with same name already exists."); } else { erp.SetError(txtTitle, null); }
            }
            btnOK.Enabled = (txtTitle.Text.Length > 0) && (alreadyTaken == false) && (txtTitle.Text.Equals(this.File.Title, StringComparison.Ordinal) == false);
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.NewTitle = txtTitle.Text;
        }

    }
}
