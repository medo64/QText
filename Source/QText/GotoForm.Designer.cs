namespace QText {
    partial class GotoForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GotoForm));
            this.lsvSuggestions = new System.Windows.Forms.ListView();
            this.lsvSuggestions_colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblWhere = new System.Windows.Forms.Label();
            this.btnGoto = new System.Windows.Forms.Button();
            this.txtWhere = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.imlSuggestions = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // lsvSuggestions
            // 
            this.lsvSuggestions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvSuggestions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lsvSuggestions_colTitle});
            this.lsvSuggestions.FullRowSelect = true;
            this.lsvSuggestions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvSuggestions.HideSelection = false;
            this.lsvSuggestions.Location = new System.Drawing.Point(12, 40);
            this.lsvSuggestions.MultiSelect = false;
            this.lsvSuggestions.Name = "lsvSuggestions";
            this.lsvSuggestions.Size = new System.Drawing.Size(278, 162);
            this.lsvSuggestions.SmallImageList = this.imlSuggestions;
            this.lsvSuggestions.TabIndex = 2;
            this.lsvSuggestions.UseCompatibleStateImageBehavior = false;
            this.lsvSuggestions.View = System.Windows.Forms.View.Details;
            this.lsvSuggestions.SelectedIndexChanged += new System.EventHandler(this.lsvSuggestions_SelectedIndexChanged);
            // 
            // lsvSuggestions_colTitle
            // 
            this.lsvSuggestions_colTitle.Text = "Title";
            // 
            // lblWhere
            // 
            this.lblWhere.AutoSize = true;
            this.lblWhere.Location = new System.Drawing.Point(12, 15);
            this.lblWhere.Name = "lblWhere";
            this.lblWhere.Size = new System.Drawing.Size(54, 17);
            this.lblWhere.TabIndex = 0;
            this.lblWhere.Text = "Where:";
            // 
            // btnGoto
            // 
            this.btnGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGoto.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGoto.Enabled = false;
            this.btnGoto.Location = new System.Drawing.Point(134, 220);
            this.btnGoto.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnGoto.Name = "btnGoto";
            this.btnGoto.Size = new System.Drawing.Size(75, 23);
            this.btnGoto.TabIndex = 3;
            this.btnGoto.Text = "Go to";
            this.btnGoto.UseVisualStyleBackColor = true;
            this.btnGoto.Click += new System.EventHandler(this.btnGoto_Click);
            // 
            // txtWhere
            // 
            this.txtWhere.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWhere.Location = new System.Drawing.Point(87, 12);
            this.txtWhere.Name = "txtWhere";
            this.txtWhere.Size = new System.Drawing.Size(200, 22);
            this.txtWhere.TabIndex = 1;
            this.txtWhere.TextChanged += new System.EventHandler(this.txtWhere_TextChanged);
            this.txtWhere.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtWhere_KeyDown);
            this.txtWhere.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtWhere_PreviewKeyDown);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(215, 220);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // imlSuggestions
            // 
            this.imlSuggestions.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlSuggestions.ImageStream")));
            this.imlSuggestions.TransparentColor = System.Drawing.Color.Transparent;
            this.imlSuggestions.Images.SetKeyName(0, "Folder (16x16).png");
            this.imlSuggestions.Images.SetKeyName(1, "Document (16x16).png");
            // 
            // GotoForm
            // 
            this.AcceptButton = this.btnGoto;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(302, 255);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtWhere);
            this.Controls.Add(this.btnGoto);
            this.Controls.Add(this.lblWhere);
            this.Controls.Add(this.lsvSuggestions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GotoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Go to";
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lsvSuggestions;
        private System.Windows.Forms.ColumnHeader lsvSuggestions_colTitle;
        private System.Windows.Forms.Label lblWhere;
        private System.Windows.Forms.Button btnGoto;
        private System.Windows.Forms.TextBox txtWhere;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ImageList imlSuggestions;
    }
}