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


        public readonly DocumentFile File;
        public string NewTitle { get; private set; }


        private void txtTitle_TextChanged(object sender, EventArgs e) {
            var alreadyTaken = false;
            var newFileTitle = txtTitle.Text.Trim();
            if (string.Equals(this.File.Title, newFileTitle, StringComparison.OrdinalIgnoreCase) == false) {
                alreadyTaken = (this.File.Folder.GetFileByTitle(newFileTitle) != null);
                if (alreadyTaken) { erp.SetError(txtTitle, "File with same name already exists."); } else { erp.SetError(txtTitle, null); }
            }
            btnOK.Enabled = (newFileTitle.Length > 0) && (alreadyTaken == false) && (newFileTitle.Equals(this.File.Title, StringComparison.Ordinal) == false);
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.NewTitle = txtTitle.Text;
        }

    }
}
