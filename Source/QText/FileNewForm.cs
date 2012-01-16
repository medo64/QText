using System;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal partial class FileNewForm : Form {

        public FileNewForm(string basePath) {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            erp.SetIconAlignment(txtTitle, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(txtTitle, 2);

            this.BasePath = basePath;

            if (Settings.IsRichTextFileDefault) {
                radRichText.Checked = true;
            }
        }


        private readonly string BasePath;
        public string Title { get; private set; }
        public bool IsRichText { get; private set; }


        private void txtTitle_TextChanged(object sender, EventArgs e) {
            bool alreadyTaken = false;
            var newFileTitle = Helper.EncodeFileName(txtTitle.Text);
            alreadyTaken |= File.Exists(Path.Combine(this.BasePath, newFileTitle + ".txt"));
            alreadyTaken |= File.Exists(Path.Combine(this.BasePath, newFileTitle + ".rtf"));
            alreadyTaken |= Directory.Exists(Path.Combine(this.BasePath, newFileTitle + ".txt"));
            alreadyTaken |= Directory.Exists(Path.Combine(this.BasePath, newFileTitle + ".rtf"));
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
