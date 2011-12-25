using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal class TabFiles : TabControl {

        public TabFiles()
            : base() {
        }


        internal string CurrentDirectory {
            get {
                if (string.IsNullOrEmpty(this.CurrentFolder)) { return Settings.FilesLocation; }
                return Path.Combine(Settings.FilesLocation, CurrentFolder);
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
            this.TabPages.Clear();
            this.CurrentFolder = folder;

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(this.CurrentDirectory, "*.txt"));
            files.AddRange(Directory.GetFiles(this.CurrentDirectory, "*.rtf"));

            string selectedTitle;
            IList<string> orderedTitles = ReadOrderedTitles(out selectedTitle);

            files.Sort(delegate(string file1, string file2) {
                var title1 = Path.GetFileNameWithoutExtension(file1);
                var title2 = Path.GetFileNameWithoutExtension(file2);
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
                if ((Settings.StartupRememberSelectedFile) && (tab.Title == selectedTitle)) {
                    selectedTab = tab;
                }
            }
            if (selectedTab != null) {
                this.SelectedTab = selectedTab;
            } else if (this.TabCount > 0) {
                this.SelectedTab = (TabFile)this.TabPages[0];
            }
            this.Visible = initialVisibility;
            if (this.SelectedTab != null) {
                this.SelectedTab.Reopen();
                this.SelectedTab.Focus();
            }
        }

        public void FolderSave() {
            foreach (TabFile file in this.TabPages) {
                file.Save();
            }
            WriteOrderedTitles();
        }

        internal static IEnumerable<string> GetSubFolders() {
            var folders = new List<string>();
            foreach (var directory in Directory.GetDirectories(Settings.FilesLocation)) {
                folders.Add(new DirectoryInfo(directory).Name);
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
            TabFile t = TabFile.Create(Path.Combine(this.CurrentDirectory, title + (isRichText ? ".rtf" : ".txt")));
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
            tab.Save();
            this.SelectedTab = GetNextTab();
            this.TabPages.Remove(tab);
            string destFolder = string.IsNullOrEmpty(newFolder) ? Settings.FilesLocation : Path.Combine(Settings.FilesLocation, newFolder);
            var oldFile = new FileInfo(tab.CurrentFile.FullName);
            oldFile.MoveTo(Path.Combine(destFolder, oldFile.Name));
            WriteOrderedTitles();
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
                    if ((base.TabPages.IndexOf(currTabPage) < base.TabPages.IndexOf(this._dragTabPage))) {
                        base.TabPages.Remove(this._dragTabPage);
                        base.TabPages.Insert(base.TabPages.IndexOf(currTabPage), this._dragTabPage);
                        base.SelectedTab = this._dragTabPage;
                    } else if ((base.TabPages.IndexOf(currTabPage) > base.TabPages.IndexOf(this._dragTabPage))) {
                        base.TabPages.Remove(this._dragTabPage);
                        base.TabPages.Insert(base.TabPages.IndexOf(currTabPage) + 1, this._dragTabPage);
                        base.SelectedTab = this._dragTabPage;
                    }
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
                                    var title = parts[0];
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

        #endregion


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
