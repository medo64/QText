namespace QText {
    partial class OptionsAdvancedForm {
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
            this.prgSettings = new System.Windows.Forms.PropertyGrid();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // prgSettings
            // 
            this.prgSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgSettings.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.prgSettings.CommandsVisibleIfAvailable = false;
            this.prgSettings.Location = new System.Drawing.Point(12, 12);
            this.prgSettings.Name = "prgSettings";
            this.prgSettings.Size = new System.Drawing.Size(438, 448);
            this.prgSettings.TabIndex = 1;
            this.prgSettings.ToolbarVisible = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(375, 478);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // OptionsAdvancedForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(462, 513);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.prgSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsAdvancedForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced options";
            this.Load += new System.EventHandler(this.OptionsAdvancedForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid prgSettings;
        private System.Windows.Forms.Button btnClose;
    }
}