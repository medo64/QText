using System;
using System.Windows.Forms;

namespace QText {
    internal partial class FileNewForm : Form {

        public FileNewForm(DocumentFolder folder) {
            InitializeComponent();
            Font = System.Drawing.SystemFonts.MessageBoxFont;
            erp.SetIconAlignment(txtTitle, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(txtTitle, 2);

            Folder = folder;

            if (Settings.Current.IsRichTextFileDefault) {
                radRichText.Checked = true;
            }
        }


        private readonly DocumentFolder Folder;
        public string Title { get; private set; }
        public bool IsRichText { get; private set; }


        private void txtTitle_TextChanged(object sender, EventArgs e) {
            var newFileTitle = txtTitle.Text.Trim();
            var canCreate = Folder.CanNewFile(newFileTitle);
            if (canCreate) {
                erp.SetError(txtTitle, null);
            } else {
                erp.SetError(txtTitle, "File with same name already exists.");
            }
            btnOK.Enabled = (newFileTitle.Length > 0) && canCreate;
        }


        private void btnOK_Click(object sender, EventArgs e) {
            Title = txtTitle.Text.Trim();
            IsRichText = radRichText.Checked;
            Settings.Current.IsRichTextFileDefault = radRichText.Checked;
        }

    }
}
