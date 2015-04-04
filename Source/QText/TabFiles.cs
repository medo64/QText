using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal class TabFiles : TabControl {

        public TabFiles()
            : base() {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.Appearance = TabAppearance.Normal;
            this.DrawMode = TabDrawMode.Normal;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
        }

        private readonly StringFormat StringFormat = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

        protected override void OnDrawItem(DrawItemEventArgs e) {
            if (e == null) { return; }
            base.OnDrawItem(e);

            var tab = (TabFile)this.TabPages[e.Index];
            var x = e.Bounds.Left;
            var y = e.Bounds.Top + e.Bounds.Height / 2;

            e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
                var selectionColor = Color.FromArgb(128, SystemColors.Highlight.R, SystemColors.Highlight.G, SystemColors.Highlight.B);
                using (var selectionBrush = new LinearGradientBrush(e.Bounds, selectionColor, SystemColors.Control, LinearGradientMode.Vertical) { Blend = new Blend() { Positions = new float[] { 0.75F, 1 } } }) {
                    e.Graphics.FillRectangle(selectionBrush, e.Bounds);
                }
                x += SystemInformation.SizingBorderWidth;
            } else {
                x += SystemInformation.Border3DSize.Width;
                y += SystemInformation.Border3DSize.Height;
            }
            if (tab.CurrentFile.IsEncrypted) {
                e.Graphics.DrawString(tab.Text, this.Font, Brushes.DarkGreen, x, y, this.StringFormat);
            } else {
                e.Graphics.DrawString(tab.Text, this.Font, SystemBrushes.ControlText, x, y, this.StringFormat);
            }
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            if (this.SelectedTab != null) { this.SelectedTab.Select(); }
        }


        internal string CurrentDirectory {
            get { return CurrentFolder.Directory.FullName; }
        }

        public DocumentFolder CurrentFolder { get; private set; }

        public ContextMenuStrip TabContextMenuStrip { get; set; }


        public void FolderOpen(DocumentFolder folder, bool saveBeforeOpen = true) {
            if (folder == null) { throw new ArgumentNullException("folder", "Folder cannot be null."); }
            if ((this.CurrentFolder != null) && (saveBeforeOpen)) { FolderSave(); }

            var initialVisibility = this.Visible;
            this.Visible = false;
            this.SelectedTab = null;
            this.TabPages.Clear();
            this.CurrentFolder = folder;

            foreach (var tab in Document.GetTabs(Document.GetFilePaths(this.CurrentFolder), this.TabContextMenuStrip)) {
                this.TabPages.Add(tab);
            }

            string selectedTitle;
            Document.ReadOrderedTitles(this.CurrentFolder, out selectedTitle);
            TabFile selectedTab = (this.TabCount > 0) ? (TabFile)this.TabPages[0] : null;
            foreach (TabFile tab in this.TabPages) {
                if (tab.Title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase)) {
                    selectedTab = tab;
                }
            }

            SelectNextTab(selectedTab);
            if (this.SelectedTab != null) {
                this.SelectedTab.Select();
                this.OnSelectedIndexChanged(new EventArgs());
            }

            this.Visible = initialVisibility;
            if (this.SelectedTab != null) {
                this.SelectedTab.Select();
                this.OnSelectedIndexChanged(new EventArgs());
            }
        }

        public void SelectNextTab(TabFile preferredTab) {
            if (preferredTab == null) { return; }
            if (preferredTab.CurrentFile.IsEncrypted) {
                var currIndex = this.TabPages.IndexOf(preferredTab);
                preferredTab = null; //remove it in case that no unencrypted tab is found
                for (int i = 0; i < this.TabPages.Count; i++) {
                    var nextIndex = (currIndex + i) % this.TabPages.Count;
                    var nextTab = (TabFile)this.TabPages[nextIndex];
                    if (nextTab.CurrentFile.IsEncrypted == false) {
                        preferredTab = nextTab;
                        break;
                    }
                }
            }
            this.SelectedTab = preferredTab;
        }

        public void FolderSave() {
            foreach (TabFile file in this.TabPages) {
                if (file.IsChanged) { file.Save(); }
            }
            Document.WriteOrderedTitles(this);
        }

        private TabPage GetTabPageFromXY(int x, int y) {
            for (int i = 0; i <= base.TabPages.Count - 1; i++) {
                if (base.GetTabRect(i).Contains(x, y)) {
                    return base.TabPages[i];
                }
            }
            return null;
        }

        internal new TabFile SelectedTab {
            get {
                try {
                    if (base.SelectedTab == null) { return null; }
                    return base.SelectedTab as QText.TabFile;
                } catch (NullReferenceException) { //work around for bug (Bajus)
                    return null;
                }
            }
            set {
                base.SelectedTab = value;
            }
        }


        #region File operations

        public void AddTab(string title, bool isRichText) {
            TabFile t = TabFile.Create(Path.Combine(this.CurrentDirectory, Helper.EncodeFileName(title) + (isRichText ? QFileInfo.Extensions.Rich : QFileInfo.Extensions.Plain)));
            t.ContextMenuStrip = this.TabContextMenuStrip;
            if (this.SelectedTab != null) {
                this.TabPages.Insert(this.TabPages.IndexOf(this.SelectedTab) + 1, t);
            } else {
                this.TabPages.Add(t);
            }
            this.SelectedTab = t;
            Document.WriteOrderedTitles(this);
        }

        public void RemoveTab(TabFile tab) {
            this.SelectedTab = GetNextTab();
            this.TabPages.Remove(tab);
            if (Settings.FilesDeleteToRecycleBin) {
                SHFile.Delete(tab.CurrentFile.Path);
            } else {
                File.Delete(tab.CurrentFile.Path);
            }
            if (this.SelectedTab != null) {
                this.SelectedTab.Open();
            }
            Document.WriteOrderedTitles(this);
        }

        public void MoveTab(TabFile tab, string newFolder) {
            string oldPath, newPath;
            MoveTabPreview(tab, newFolder, out oldPath, out newPath);
            tab.Save();
            this.SelectedTab = GetNextTab();
            this.TabPages.Remove(tab);
            Helper.MovePath(oldPath, newPath);
            Document.WriteOrderedTitles(this);
        }

        public void MoveTabPreview(TabFile tab, string newFolder, out string oldPath, out string newPath) {
            string destFolder = string.IsNullOrEmpty(newFolder) ? Settings.FilesLocation : Path.Combine(Settings.FilesLocation, newFolder);
            var oldFile = new QFileInfo(tab.CurrentFile.Path);
            oldPath = oldFile.FullName;
            newPath = Path.Combine(destFolder, oldFile.Name);
        }

        #endregion

        #region Drag & drop

        private TabPage _dragTabPage = null;

        protected override void OnMouseDown(MouseEventArgs e) {
            if ((e != null) && (e.Button == MouseButtons.Left) && (base.SelectedTab != null) && (!base.GetTabRect(base.SelectedIndex).IsEmpty)) {
                this._dragTabPage = base.SelectedTab;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if ((e != null) && (e.Button == MouseButtons.Left) && (this._dragTabPage != null)) {
                var currTabPage = GetTabPageFromXY(e.X, e.Y);
                if ((currTabPage != null)) {
                    var currRect = base.GetTabRect(base.TabPages.IndexOf(currTabPage));
                    if ((base.TabPages.IndexOf(currTabPage) < base.TabPages.IndexOf(this._dragTabPage))) {
                        base.Cursor = Cursors.PanWest;
                    } else if ((base.TabPages.IndexOf(currTabPage) > base.TabPages.IndexOf(this._dragTabPage))) {
                        base.Cursor = Cursors.PanEast;
                    } else {
                        base.Cursor = Cursors.Default;
                    }
                } else {
                    this.Cursor = Cursors.No;
                }
            } else {
                this.Cursor = Cursors.Default;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            if ((e != null) && (e.Button == MouseButtons.Left) && (this._dragTabPage != null)) {
                TabPage currTabPage = GetTabPageFromXY(e.X, e.Y);
                if ((currTabPage != null) && (!currTabPage.Equals(this._dragTabPage))) {
                    var currRect = base.GetTabRect(base.TabPages.IndexOf(currTabPage));
                    base.Enabled = false;
                    if ((base.TabPages.IndexOf(currTabPage) < base.TabPages.IndexOf(this._dragTabPage))) {
                        base.TabPages.Remove(this._dragTabPage);
                        base.TabPages.Insert(base.TabPages.IndexOf(currTabPage), this._dragTabPage);
                        base.SelectedTab = this._dragTabPage;
                    } else if ((base.TabPages.IndexOf(currTabPage) > base.TabPages.IndexOf(this._dragTabPage))) {
                        base.TabPages.Remove(this._dragTabPage);
                        base.TabPages.Insert(base.TabPages.IndexOf(currTabPage) + 1, this._dragTabPage);
                        base.SelectedTab = this._dragTabPage;
                    }
                    base.Enabled = true;
                    Document.WriteOrderedTitles(this);
                }
            }
            this._dragTabPage = null;
            base.Cursor = Cursors.Default;
            base.OnMouseUp(e);
        }

        #endregion


        public void SaveCarbonCopies(IWin32Window owner) {
            var directories = new List<string>();
            directories.Add(Settings.FilesLocation);
            directories.AddRange(Directory.GetDirectories(Settings.FilesLocation));
            foreach (var directory in directories) {
                foreach (var extension in QFileInfo.GetExtensions()) {
                    foreach (var file in Directory.GetFiles(directory, "*" + extension)) {
                        TabFile.SaveCarbonCopy(owner, file);
                    }
                }
            }
        }

        private TabFile GetNextTab() {
            int tindex = this.TabPages.IndexOf(this.SelectedTab) + 1; //select next tab
            if (tindex >= this.TabPages.Count) {
                tindex -= 2; //go to one in front of it
            }
            if ((tindex > 0) && (tindex < this.TabPages.Count)) {
                return (TabFile)this.TabPages[tindex];
            }
            return null;
        }

    }
}
