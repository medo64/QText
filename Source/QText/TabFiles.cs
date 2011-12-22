using System;
using System.Windows.Forms;

namespace QText {
    internal class TabFiles : TabControl {

        internal FileOrder FileOrder = new FileOrder();

        public TabFiles()
            : base() {
        }

        internal string CurrentDirectory { get; private set; }

        public void Open(string directory) {
        }

        public void Save() {
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
            try {
                foreach (TabFile file in this.TabPages) {
                    file.Save();
                }
            } finally {
                this.FileOrder.Save(this, true);
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
                    //bigger then
                    if ((base.TabPages.IndexOf(currTabPage) < base.TabPages.IndexOf(this._dragTabPage))) {
                        base.TabPages.Remove(this._dragTabPage);
                        base.TabPages.Insert(base.TabPages.IndexOf(currTabPage), this._dragTabPage);
                        base.SelectedTab = this._dragTabPage;
                    } else if ((base.TabPages.IndexOf(currTabPage) > base.TabPages.IndexOf(this._dragTabPage))) {
                        base.TabPages.Remove(this._dragTabPage);
                        base.TabPages.Insert(base.TabPages.IndexOf(currTabPage) + 1, this._dragTabPage);
                        base.SelectedTab = this._dragTabPage;
                    }
                    this.FileOrder.Save(this, true);
                }
            }
            this._dragTabPage = null;
            base.Cursor = Cursors.Default;
            base.OnMouseUp(e);
        }

        #endregion

    }
}
