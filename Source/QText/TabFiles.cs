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
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
        }

        private readonly StringFormat StringFormat = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

        protected override void OnDrawItem(DrawItemEventArgs e) {
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
            get {
                if (string.IsNullOrEmpty(this.CurrentFolder)) { return Settings.FilesLocation; }
                return Path.Combine(Settings.FilesLocation, Helper.EncodeFileName(CurrentFolder));
            }
        }
        public string CurrentFolder { get; private set; }

        public ContextMenuStrip TabContextMenuStrip { get; set; }


        public void FolderOpen(string folder, bool saveBeforeOpen = true) {
            if (folder == null) { throw new ArgumentNullException("folder", "Folder cannot be null."); }
            if ((this.CurrentFolder != null) && (saveBeforeOpen)) { FolderSave(); }

            //check if it exists
            string newFolder = "";
            foreach (var iFolder in TabFiles.GetSubFolders()) {
                if (string.Equals(iFolder, folder, StringComparison.OrdinalIgnoreCase)) {
                    newFolder = iFolder;
                    break;
                }
            }
            folder = newFolder;

            var initialVisibility = this.Visible;
            this.Visible = false;
            this.SelectedTab = null;
            this.TabPages.Clear();
            this.CurrentFolder = folder;

            var files = new List<string>();
            foreach (var extension in QFileInfo.GetExtensions()) {
                files.AddRange(Directory.GetFiles(this.CurrentDirectory, "*" + extension));
            }

            string selectedTitle;
            IList<string> orderedTitles = ReadOrderedTitles(out selectedTitle);

            files.Sort(delegate(string file1, string file2) {
                var title1 = Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file1));
                var title2 = Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file2));
                if (orderedTitles != null) {
                    var titleIndex1 = orderedTitles.IndexOf(title1);
                    var titleIndex2 = orderedTitles.IndexOf(title2);
                    if ((titleIndex1 != -1) && (titleIndex2 != -1)) { //both are ordered
                        return (titleIndex1 < titleIndex2) ? -1 : 1;
                    } else if (titleIndex1 != -1) { //first one is ordered
                        return -1;
                    } else if (titleIndex2 != -1) { //second one is ordered 
                        return 1;
                    }
                }
                return string.Compare(title1, title2); //just sort alphabetically
            });

            TabFile selectedTab = null;
            foreach (var file in files) {
                var tab = new TabFile(file);
                tab.ContextMenuStrip = this.TabContextMenuStrip;
                this.TabPages.Add(tab);
                if (tab.Title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase)) {
                    selectedTab = tab;
                }
            }
            if ((selectedTab == null) && (this.TabCount > 0)) {
                selectedTab = (TabFile)this.TabPages[0];
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
            WriteOrderedTitles();
        }

        internal static IEnumerable<string> GetSubFolders() {
            var folders = new List<string>();
            foreach (var directory in Directory.GetDirectories(Settings.FilesLocation)) {
                folders.Add(Helper.DecodeFileName(new DirectoryInfo(directory).Name));
            }
            folders.Sort();
            foreach (var folder in folders) {
                yield return folder;
            }
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
            WriteOrderedTitles();
        }

        public void RemoveTab(TabFile tab) {
            this.SelectedTab = GetNextTab();
            this.TabPages.Remove(tab);
            if (Settings.FilesDeleteToRecycleBin) {
                SHFile.Delete(tab.CurrentFile.FullName);
            } else {
                File.Delete(tab.CurrentFile.FullName);
            }
            WriteOrderedTitles();
        }

        public void MoveTab(TabFile tab, string newFolder) {
            string oldPath, newPath;
            MoveTabPreview(tab, newFolder, out oldPath, out newPath);
            tab.Save();
            this.SelectedTab = GetNextTab();
            this.TabPages.Remove(tab);
            Helper.MovePath(oldPath, newPath);
            WriteOrderedTitles();
        }

        public void MoveTabPreview(TabFile tab, string newFolder, out string oldPath, out string newPath) {
            string destFolder = string.IsNullOrEmpty(newFolder) ? Settings.FilesLocation : Path.Combine(Settings.FilesLocation, newFolder);
            var oldFile = new QFileInfo(tab.CurrentFile.FullName);
            oldPath = oldFile.FullName;
            newPath = Path.Combine(destFolder, oldFile.Name);
        }

        #endregion

        #region Drag & drop

        private TabPage _dragTabPage = null;

        protected override void OnMouseDown(MouseEventArgs e) {
            if ((e.Button == MouseButtons.Left) && (base.SelectedTab != null) && (!base.GetTabRect(base.SelectedIndex).IsEmpty)) {
                this._dragTabPage = base.SelectedTab;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if ((e.Button == MouseButtons.Left) && (this._dragTabPage != null)) {
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
            if ((e.Button == MouseButtons.Left) && (this._dragTabPage != null)) {
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
                    this.WriteOrderedTitles();
                }
            }
            this._dragTabPage = null;
            base.Cursor = Cursors.Default;
            base.OnMouseUp(e);
        }

        #endregion

        #region Ordering

        private string OrderFile { get { return Path.Combine(this.CurrentDirectory, ".qtext"); } }

        private IList<string> ReadOrderedTitles(out string selectedTitle) {
            selectedTitle = null;
            string currentSelectedTitle = null;
            IList<string> orderedTitles = null;
            IList<string> currentOrderedTitles = null;
            try {
                using (var fs = new FileStream(this.OrderFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    using (var sr = new StreamReader(fs)) {
                        while (sr.EndOfStream == false) {
                            var line = sr.ReadLine();
                            if (line.Equals("/[")) { //start of block
                                currentOrderedTitles = new List<string>();
                                currentSelectedTitle = null;
                            } else if (line.Equals("]/")) { //end of block
                                orderedTitles = currentOrderedTitles;
                                selectedTitle = currentSelectedTitle;
                            } else {
                                if (currentOrderedTitles != null) {
                                    var parts = line.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                                    var title = Helper.DecodeFileName(parts[0]);
                                    var attrs = parts.Length > 1 ? parts[1] : null;
                                    if ("selected".Equals(attrs)) { currentSelectedTitle = title; }
                                    currentOrderedTitles.Add(title);
                                }
                            }
                        }
                    }
                }
            } catch (IOException) { }
            return orderedTitles;
        }

        public void WriteOrderedTitles() {
            try {
                var fi = new QFileInfo(this.OrderFile);
                if (fi.Exists == false) {
                    fi.Create();
                }
                fi.Attributes = FileAttributes.Hidden;
                using (var fs = new FileStream(this.OrderFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
                    try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                    fs.Position = fs.Length;
                    using (var sw = new StreamWriter(fs)) {
                        sw.WriteLine("/["); //always start block with /[
                        foreach (TabFile tab in this.TabPages) {
                            if (tab.Equals(this.SelectedTab)) { //selected file is written with //selected
                                sw.WriteLine(tab.Title + "//selected");
                            } else {
                                sw.WriteLine(tab.Title);
                            }
                        }
                        sw.WriteLine("]/"); //always end block with ]/
                    }
                }
            } catch (IOException) { }
        }

        public void CleanOrderedTitles() {
            try {
                var fi = new QFileInfo(this.OrderFile);
                if (fi.Exists == false) {
                    fi.Create();
                }
                fi.Attributes = FileAttributes.Hidden;
                using (var fs = new FileStream(this.OrderFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
                    try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                    fs.Position = fs.Length;
                    using (var sw = new StreamWriter(fs)) {
                        sw.WriteLine("/["); //always start block with /[
                        sw.WriteLine("]/"); //always end block with ]/
                    }
                }
            } catch (IOException) { }
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
