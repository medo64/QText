using System;
using System.Drawing;
using System.Windows.Forms;

namespace QText {
    internal partial class FileRenameForm : Form {
        public FileRenameForm(DocumentFile file) {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
            erp.SetIconAlignment(txtTitle, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(txtTitle, 2);

            File = file;

            txtTitle.Text = File.Title;
        }


        public readonly DocumentFile File;
        public string NewTitle { get; private set; }


        private void txtTitle_TextChanged(object sender, EventArgs e) {
            var alreadyTaken = false;
            var newFileTitle = txtTitle.Text.Trim();
            if (string.Equals(File.Title, newFileTitle, StringComparison.OrdinalIgnoreCase) == false) {
                alreadyTaken = (File.Folder.GetFileByTitle(newFileTitle) != null);
                if (alreadyTaken) { erp.SetError(txtTitle, "File with same name already exists."); } else { erp.SetError(txtTitle, null); }
            }
            btnOK.Enabled = (newFileTitle.Length > 0) && (alreadyTaken == false) && (newFileTitle.Equals(File.Title, StringComparison.Ordinal) == false);
        }


        private void btnOK_Click(object sender, EventArgs e) {
            NewTitle = txtTitle.Text;
        }

    }
}
