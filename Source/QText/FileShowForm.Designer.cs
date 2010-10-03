namespace QText {
    partial class FileShowForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.listHiddenFiles = new System.Windows.Forms.CheckedListBox();
            this.lblNoHiddenFiles = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(244, 326);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 29);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(140, 326);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 29);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // listHiddenFiles
            // 
            this.listHiddenFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHiddenFiles.IntegralHeight = false;
            this.listHiddenFiles.Location = new System.Drawing.Point(12, 12);
            this.listHiddenFiles.Name = "listHiddenFiles";
            this.listHiddenFiles.Size = new System.Drawing.Size(330, 296);
            this.listHiddenFiles.TabIndex = 0;
            this.listHiddenFiles.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listHiddenFiles_ItemCheck);
            // 
            // lblNoHiddenFiles
            // 
            this.lblNoHiddenFiles.AutoSize = true;
            this.lblNoHiddenFiles.BackColor = System.Drawing.SystemColors.Window;
            this.lblNoHiddenFiles.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNoHiddenFiles.Location = new System.Drawing.Point(14, 14);
            this.lblNoHiddenFiles.Name = "lblNoHiddenFiles";
            this.lblNoHiddenFiles.Size = new System.Drawing.Size(102, 17);
            this.lblNoHiddenFiles.TabIndex = 3;
            this.lblNoHiddenFiles.Text = "No hidden files";
            this.lblNoHiddenFiles.Visible = false;
            // 
            // FileShowForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 367);
            this.Controls.Add(this.lblNoHiddenFiles);
            this.Controls.Add(this.listHiddenFiles);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileShowForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Show";
            this.Load += new System.EventHandler(this.FileShowForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckedListBox listHiddenFiles;
        private System.Windows.Forms.Label lblNoHiddenFiles;
    }
}