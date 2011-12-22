using System;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace QText {
    internal class TabFiles : TabControl {

        public TabFiles()
            : base() {
        }


        internal string CurrentDirectory { get; private set; }

        public void DirectoryOpen() { //Reopens
            this.DirectoryOpen(this.CurrentDirectory);
        }

        public void DirectoryOpen(string directory) {
            if (directory == null) { directory = Settings.FilesLocation; }
            if (this.CurrentDirectory != null) { DirectorySave(); }

            this.CurrentDirectory = directory;

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(Settings.FilesLocation, "*.txt"));
            files.AddRange(Directory.GetFiles(Settings.FilesLocation, "*.rtf"));

            string selectedTitle;
            IList<string> orderedTitles = ReadOrderedTitles(out selectedTitle);

            files.Sort(delegate(string file1, string file2) {
                var title1 = Helper.Path.GetTitle(file1);
                var title2 = Helper.Path.GetTitle(file2);
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

            this.Visible = false;
            this.TabPages.Clear();
            TabFile selectedTab = null;
            foreach (var file in files) {
                var tab = new TabFile(file, App.Form.mnxText);
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
            this.Visible = true;
        }

        public void DirectorySave() {
            this.SaveAll();
            WriteOrderedTitles();
        }


        private TabPage GetTabPageFromXY(int x, int y) {
            for (int i = 0; i <= base.TabPages.Count - 1; i++) {
                if (base.GetTabRect(i).Contains(x, y)) {
                    return base.TabPages[i];
                }
            }
            return null;
        }

        public void SaveAll() {
            foreach (TabFile file in this.TabPages) {
                file.Save();
            }
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

        private void WriteOrderedTitles() {
            try {
                using (var fs = new FileStream(this.OrderFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
                    try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                    fs.Position = fs.Length;
                    using (var sw = new StreamWriter(fs)) {
                        sw.WriteLine("/["); //always start block with /[
                        foreach (TabFile tab in this.TabPages) {
                            if (this.SelectedTab.Equals(tab)) { //selected file is written with //selected
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

    }
}
