using System;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal partial class FileNewForm : Form {

        public FileNewForm(DocumentFolder folder) {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            erp.SetIconAlignment(txtTitle, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(txtTitle, 2);

            this.Folder = folder;

            if (Settings.IsRichTextFileDefault) {
                radRichText.Checked = true;
            }
        }


        private readonly DocumentFolder Folder;
        public string Title { get; private set; }
        public bool IsRichText { get; private set; }


        private void txtTitle_TextChanged(object sender, EventArgs e) {
            var newFileTitle = Helper.EncodeFileName(txtTitle.Text);
            bool alreadyTaken = (this.Folder.GetFileByTitle(newFileTitle) != null);
            if (alreadyTaken) { erp.SetError(txtTitle, "File with same name already exists."); } else { erp.SetError(txtTitle, null); }
            btnOK.Enabled = (txtTitle.Text.Length > 0) && (alreadyTaken == false);
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.Title = txtTitle.Text;
            this.IsRichText = radRichText.Checked;
            Settings.IsRichTextFileDefault = radRichText.Checked;
        }

    }
}
