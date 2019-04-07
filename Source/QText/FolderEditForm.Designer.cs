namespace QText {
    partial class FolderEditForm {
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
            this.lsv = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnu = new System.Windows.Forms.ToolStrip();
            this.mnuNewFolder = new System.Windows.Forms.ToolStripButton();
            this.mnuRename = new System.Windows.Forms.ToolStripButton();
            this.mnu0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuDelete = new System.Windows.Forms.ToolStripButton();
            this.mnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lsv
            // 
            this.lsv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lsv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsv.FullRowSelect = true;
            this.lsv.GridLines = true;
            this.lsv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsv.LabelEdit = true;
            this.lsv.Location = new System.Drawing.Point(0, 27);
            this.lsv.MultiSelect = false;
            this.lsv.Name = "lsv";
            this.lsv.Size = new System.Drawing.Size(322, 326);
            this.lsv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lsv.TabIndex = 0;
            this.lsv.UseCompatibleStateImageBehavior = false;
            this.lsv.View = System.Windows.Forms.View.Details;
            this.lsv.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lsv_AfterLabelEdit);
            this.lsv.ItemActivate += new System.EventHandler(this.lsv_ItemActivate);
            this.lsv.Resize += new System.EventHandler(this.lsv_Resize);
            // 
            // mnu
            // 
            this.mnu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mnu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNewFolder,
            this.mnuRename,
            this.mnu0,
            this.mnuDelete});
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.mnu.Size = new System.Drawing.Size(322, 27);
            this.mnu.TabIndex = 1;
            // 
            // mnuNewFolder
            // 
            this.mnuNewFolder.Image = global::QText.Properties.Resources.mnuFolder_16;
            this.mnuNewFolder.Name = "mnuNewFolder";
            this.mnuNewFolder.Size = new System.Drawing.Size(63, 24);
            this.mnuNewFolder.Tag = "mnuFolder";
            this.mnuNewFolder.Text = "New";
            this.mnuNewFolder.ToolTipText = "New (Ctrl+N)";
            this.mnuNewFolder.Click += new System.EventHandler(this.mnuNew_Click);
            // 
            // mnuRename
            // 
            this.mnuRename.Image = global::QText.Properties.Resources.mnuRename_16;
            this.mnuRename.Name = "mnuRename";
            this.mnuRename.Size = new System.Drawing.Size(87, 24);
            this.mnuRename.Text = "Rename";
            this.mnuRename.ToolTipText = "Rename (F2)";
            this.mnuRename.Click += new System.EventHandler(this.mnuRename_Click);
            // 
            // mnu0
            // 
            this.mnu0.Name = "mnu0";
            this.mnu0.Size = new System.Drawing.Size(6, 27);
            // 
            // mnuDelete
            // 
            this.mnuDelete.Image = global::QText.Properties.Resources.mnuDelete_16;
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(77, 24);
            this.mnuDelete.Text = "Delete";
            this.mnuDelete.ToolTipText = "Delete (Del)";
            this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
            // 
            // FolderEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 353);
            this.Controls.Add(this.lsv);
            this.Controls.Add(this.mnu);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 360);
            this.Name = "FolderEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit folders";
            this.Load += new System.EventHandler(this.Form_Load);
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lsv;
        private System.Windows.Forms.ToolStrip mnu;
        private System.Windows.Forms.ToolStripButton mnuNewFolder;
        private System.Windows.Forms.ToolStripButton mnuRename;
        private System.Windows.Forms.ToolStripSeparator mnu0;
        private System.Windows.Forms.ToolStripButton mnuDelete;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}