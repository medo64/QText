namespace QText {
    partial class SpellingForm {
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
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtSpelling = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblInput = new System.Windows.Forms.Label();
            this.lblSpelling = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Location = new System.Drawing.Point(101, 12);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(361, 22);
            this.txtInput.TabIndex = 1;
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // txtSpelling
            // 
            this.txtSpelling.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpelling.Location = new System.Drawing.Point(101, 40);
            this.txtSpelling.Multiline = true;
            this.txtSpelling.Name = "txtSpelling";
            this.txtSpelling.ReadOnly = true;
            this.txtSpelling.Size = new System.Drawing.Size(361, 197);
            this.txtSpelling.TabIndex = 3;
            this.txtSpelling.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSpelling_KeyDown);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(364, 246);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 29);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(12, 15);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(43, 17);
            this.lblInput.TabIndex = 0;
            this.lblInput.Text = "Input:";
            // 
            // lblSpelling
            // 
            this.lblSpelling.AutoSize = true;
            this.lblSpelling.Location = new System.Drawing.Point(12, 43);
            this.lblSpelling.Name = "lblSpelling";
            this.lblSpelling.Size = new System.Drawing.Size(58, 17);
            this.lblSpelling.TabIndex = 2;
            this.lblSpelling.Text = "Spelling";
            // 
            // SpellingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(474, 287);
            this.Controls.Add(this.lblSpelling);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtSpelling);
            this.Controls.Add(this.txtInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpellingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Spelling";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtSpelling;
        internal System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.Label lblSpelling;
    }
}