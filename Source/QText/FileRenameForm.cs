﻿using System;
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


        private void txtTitle_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar.ToString().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                e.Handled = !Char.IsControl(e.KeyChar);
            }
        }

        private void txtTitle_TextChanged(object sender, EventArgs e) {
            if (txtTitle.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                var sb = new System.Text.StringBuilder(txtTitle.Text);
                var ic = System.IO.Path.GetInvalidFileNameChars();
                for (int i = 0; i < ic.Length; ++i) {
                    sb.Replace(ic[i].ToString(), "");
                }
                txtTitle.Text = sb.ToString();
            }
            bool alreadyTaken = false;
            if (string.Equals(this.OldTitle, txtTitle.Text, StringComparison.OrdinalIgnoreCase) == false) {
                alreadyTaken |= File.Exists(Path.Combine(this.BasePath, txtTitle.Text + ".txt"));
                alreadyTaken |= File.Exists(Path.Combine(this.BasePath, txtTitle.Text + ".rtf"));
                alreadyTaken |= Directory.Exists(Path.Combine(this.BasePath, txtTitle.Text + ".txt"));
                alreadyTaken |= Directory.Exists(Path.Combine(this.BasePath, txtTitle.Text + ".rtf"));
                if (alreadyTaken) { erp.SetError(txtTitle, "File with same name already exists."); } else { erp.SetError(txtTitle, null); }
            }
            btnOK.Enabled = (txtTitle.Text.Length > 0) && (alreadyTaken == false) && (txtTitle.Text.Equals(this.OldTitle, StringComparison.Ordinal) == false);
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.NewTitle = txtTitle.Text;
        }

    }
}
