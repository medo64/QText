﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace QText {
    public partial class FileShowForm : Form {
        public FileShowForm() {
            InitializeComponent();
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
        }

        private void FileShowForm_Load(object sender, EventArgs e) {
            foreach (var file in FileOrder.GetSortedFileNames(true)) {
                var fi = new FileInfo(Path.Combine(Settings.FilesLocation, file));
                if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) {
                    listHiddenFiles.Items.Add(new HiddenFile(fi));
                }
            }
            lblNoHiddenFiles.Visible = (listHiddenFiles.Items.Count == 0);
        }

        private void listHiddenFiles_ItemCheck(object sender, ItemCheckEventArgs e) {
            var checkedCount = listHiddenFiles.CheckedItems.Count;
            if ((e.CurrentValue == CheckState.Unchecked) && (e.NewValue == CheckState.Checked)) { checkedCount += 1; }
            if ((e.CurrentValue == CheckState.Checked) && (e.NewValue == CheckState.Unchecked)) { checkedCount -= 1; }
            btnOK.Enabled = (checkedCount > 0);
        }


        private class HiddenFile {

            public FileInfo FileInfo { get; private set; }

            public HiddenFile(FileInfo fileInfo) {
                this.FileInfo = fileInfo;
            }

            public override string ToString() {
                return this.FileInfo.Name.Substring(0, this.FileInfo.Name.Length - this.FileInfo.Extension.Length);
            }

        }

        private void btnOK_Click(object sender, EventArgs e) {
            foreach (HiddenFile file in listHiddenFiles.CheckedItems) {
                var currAttributes = File.GetAttributes(file.FileInfo.FullName);
                File.SetAttributes(file.FileInfo.FullName, currAttributes ^ FileAttributes.Hidden);
            }
        }

    }
}
