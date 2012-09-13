namespace QText {
    partial class FindForm {
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
            this.btnFind = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblFind = new System.Windows.Forms.Label();
            this.chbCaseSensitive = new System.Windows.Forms.CheckBox();
            this.txtText = new System.Windows.Forms.TextBox();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.groupScope = new System.Windows.Forms.GroupBox();
            this.radioFolders = new System.Windows.Forms.RadioButton();
            this.radioFolder = new System.Windows.Forms.RadioButton();
            this.radioFile = new System.Windows.Forms.RadioButton();
            this.GroupBox1.SuspendLayout();
            this.groupScope.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFind.Enabled = false;
            this.btnFind.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnFind.Location = new System.Drawing.Point(140, 148);
            this.btnFind.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(98, 29);
            this.btnFind.TabIndex = 4;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(244, 148);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(98, 29);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblFind
            // 
            this.lblFind.AutoSize = true;
            this.lblFind.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFind.Location = new System.Drawing.Point(12, 15);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(39, 17);
            this.lblFind.TabIndex = 0;
            this.lblFind.Text = "Find:";
            // 
            // chbCaseSensitive
            // 
            this.chbCaseSensitive.AutoSize = true;
            this.chbCaseSensitive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chbCaseSensitive.Location = new System.Drawing.Point(9, 24);
            this.chbCaseSensitive.Name = "chbCaseSensitive";
            this.chbCaseSensitive.Size = new System.Drawing.Size(121, 21);
            this.chbCaseSensitive.TabIndex = 0;
            this.chbCaseSensitive.Text = "Case sensitive";
            this.chbCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // txtText
            // 
            this.txtText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtText.Location = new System.Drawing.Point(82, 12);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(260, 22);
            this.txtText.TabIndex = 1;
            this.txtText.TextChanged += new System.EventHandler(this.txtText_TextChanged);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.chbCaseSensitive);
            this.GroupBox1.Location = new System.Drawing.Point(12, 40);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(160, 53);
            this.GroupBox1.TabIndex = 2;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Options";
            // 
            // groupScope
            // 
            this.groupScope.Controls.Add(this.radioFolders);
            this.groupScope.Controls.Add(this.radioFolder);
            this.groupScope.Controls.Add(this.radioFile);
            this.groupScope.Location = new System.Drawing.Point(182, 40);
            this.groupScope.Name = "groupScope";
            this.groupScope.Size = new System.Drawing.Size(160, 90);
            this.groupScope.TabIndex = 3;
            this.groupScope.TabStop = false;
            this.groupScope.Text = "Scope";
            // 
            // radioFolders
            // 
            this.radioFolders.AutoSize = true;
            this.radioFolders.Location = new System.Drawing.Point(6, 63);
            this.radioFolders.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.radioFolders.Name = "radioFolders";
            this.radioFolders.Size = new System.Drawing.Size(91, 21);
            this.radioFolders.TabIndex = 2;
            this.radioFolders.TabStop = true;
            this.radioFolders.Text = "All folders";
            this.radioFolders.UseVisualStyleBackColor = true;
            // 
            // radioFolder
            // 
            this.radioFolder.AutoSize = true;
            this.radioFolder.Location = new System.Drawing.Point(6, 42);
            this.radioFolder.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioFolder.Name = "radioFolder";
            this.radioFolder.Size = new System.Drawing.Size(128, 21);
            this.radioFolder.TabIndex = 1;
            this.radioFolder.TabStop = true;
            this.radioFolder.Text = "All files in folder";
            this.radioFolder.UseVisualStyleBackColor = true;
            // 
            // radioFile
            // 
            this.radioFile.AutoSize = true;
            this.radioFile.Location = new System.Drawing.Point(6, 21);
            this.radioFile.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.radioFile.Name = "radioFile";
            this.radioFile.Size = new System.Drawing.Size(98, 21);
            this.radioFile.TabIndex = 0;
            this.radioFile.TabStop = true;
            this.radioFile.Text = "Current file";
            this.radioFile.UseVisualStyleBackColor = true;
            // 
            // FindForm
            // 
            this.AcceptButton = this.btnFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(354, 189);
            this.Controls.Add(this.groupScope);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblFind);
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.GroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find";
            this.Load += new System.EventHandler(this.FindForm_Load);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.groupScope.ResumeLayout(false);
            this.groupScope.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnFind;
        internal System.Windows.Forms.Button btnClose;
        internal System.Windows.Forms.Label lblFind;
        internal System.Windows.Forms.CheckBox chbCaseSensitive;
        internal System.Windows.Forms.TextBox txtText;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.GroupBox groupScope;
        private System.Windows.Forms.RadioButton radioFolders;
        private System.Windows.Forms.RadioButton radioFolder;
        private System.Windows.Forms.RadioButton radioFile;
    }
}