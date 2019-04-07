using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QText {
    internal class TabFiles : TabControl {

        public TabFiles()
            : base() {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Appearance = TabAppearance.Normal;
            DrawMode = TabDrawMode.Normal;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
        }

        private readonly StringFormat StringFormat = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

        protected override void OnDrawItem(DrawItemEventArgs e) {
            if (e == null) { return; }
            base.OnDrawItem(e);

            var tab = (TabFile)TabPages[e.Index];
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
            if (tab.BaseFile.IsEncrypted) {
                e.Graphics.DrawString(tab.Text, Font, Brushes.DarkGreen, x, y, StringFormat);
            } else {
                e.Graphics.DrawString(tab.Text, Font, SystemBrushes.ControlText, x, y, StringFormat);
            }
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            if (SelectedTab != null) { SelectedTab.Select(); }
        }

        protected override void OnSelected(TabControlEventArgs e) {
            if (e.TabPage is TabFile tabFile) { tabFile.BaseFile.Selected = true; }
        }


        public DocumentFolder CurrentFolder { get; private set; }

        public ContextMenuStrip TabContextMenuStrip { get; set; }


        public void FolderOpen(DocumentFolder folder, bool saveBeforeOpen = true) {
            if ((CurrentFolder != null) && (saveBeforeOpen)) { FolderSave(); }

            var initialVisibility = Visible;
            Visible = false;
            SelectedTab = null;
            TabPages.Clear();
            CurrentFolder = folder ?? throw new ArgumentNullException("folder", "Folder cannot be null.");

            foreach (var tab in Helper.GetTabs(CurrentFolder.GetFiles(), TabContextMenuStrip)) {
                TabPages.Add(tab);
            }

            var selectedTab = (TabCount > 0) ? (TabFile)TabPages[0] : null;
            foreach (TabFile tab in TabPages) {
                if (tab.BaseFile.Equals(folder.SelectedFile)) {
                    selectedTab = tab;
                }
            }

            SelectNextTab(selectedTab);
            if (SelectedTab != null) {
                SelectedTab.Select();
                OnSelectedIndexChanged(new EventArgs());
            }

            Visible = initialVisibility;
            if (SelectedTab != null) {
                SelectedTab.Select();
                OnSelectedIndexChanged(new EventArgs());
            }
        }

        public void SelectNextTab(TabFile preferredTab) {
            if (preferredTab == null) { return; }
            if (preferredTab.BaseFile.IsEncrypted) {
                var currIndex = TabPages.IndexOf(preferredTab);
                preferredTab = null; //remove it in case that no unencrypted tab is found
                for (var i = 0; i < TabPages.Count; i++) {
                    var nextIndex = (currIndex + i) % TabPages.Count;
                    var nextTab = (TabFile)TabPages[nextIndex];
                    if (nextTab.BaseFile.IsEncrypted == false) {
                        preferredTab = nextTab;
                        break;
                    }
                }
            }
            SelectedTab = preferredTab;
        }

        public void FolderSave() {
            foreach (TabFile file in TabPages) {
                if (file.IsChanged) { file.Save(); }
            }
        }

        private TabPage GetTabPageFromXY(int x, int y) {
            for (var i = 0; i <= base.TabPages.Count - 1; i++) {
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
                if (value != null) { value.BaseFile.Selected = true; }
                base.SelectedTab = value;
            }
        }


        #region File operations

        public void AddTab(string title, bool isRichText) {
            var t = TabFile.Create(CurrentFolder, title, isRichText ? DocumentKind.RichText : DocumentKind.PlainText);
            t.ContextMenuStrip = TabContextMenuStrip;
            if (SelectedTab != null) {
                TabPages.Insert(TabPages.IndexOf(SelectedTab) + 1, t);
            } else {
                TabPages.Add(t);
            }
            SelectedTab = t;
        }

        public void DeleteTab(TabFile tab) {
            RemoveTab(tab);
            tab.BaseFile.Delete();
        }

        public void RemoveTab(TabFile tab) {
            SelectedTab = GetNextTab();
            TabPages.Remove(tab);
            if (SelectedTab != null) {
                SelectedTab.Open();
            }
        }

        #endregion

        #region Drag & drop

        private TabPage _dragTabPage = null;

        protected override void OnMouseDown(MouseEventArgs e) {
            if ((e != null) && (e.Button == MouseButtons.Left) && (base.SelectedTab != null) && (!base.GetTabRect(base.SelectedIndex).IsEmpty)) {
                _dragTabPage = base.SelectedTab;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if ((e != null) && (e.Button == MouseButtons.Left) && (_dragTabPage != null)) {
                var currTabPage = GetTabPageFromXY(e.X, e.Y);
                if ((currTabPage != null)) {
                    var currRect = base.GetTabRect(base.TabPages.IndexOf(currTabPage));
                    if ((base.TabPages.IndexOf(currTabPage) < base.TabPages.IndexOf(_dragTabPage))) {
                        base.Cursor = Cursors.PanWest;
                    } else if ((base.TabPages.IndexOf(currTabPage) > base.TabPages.IndexOf(_dragTabPage))) {
                        base.Cursor = Cursors.PanEast;
                    } else {
                        base.Cursor = Cursors.Default;
                    }
                } else {
                    Cursor = Cursors.No;
                }
            } else {
                Cursor = Cursors.Default;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            if ((e != null) && (e.Button == MouseButtons.Left) && (_dragTabPage != null)) {
                var currTabPage = GetTabPageFromXY(e.X, e.Y);
                if ((currTabPage != null) && (!currTabPage.Equals(_dragTabPage))) {
                    var currRect = base.GetTabRect(base.TabPages.IndexOf(currTabPage));
                    base.Enabled = false;
                    if ((base.TabPages.IndexOf(currTabPage) < base.TabPages.IndexOf(_dragTabPage))) {
                        base.TabPages.Remove(_dragTabPage);
                        base.TabPages.Insert(base.TabPages.IndexOf(currTabPage), _dragTabPage);
                        base.SelectedTab = _dragTabPage;
                        var pivotTab = currTabPage as TabFile;
                        var movedTab = _dragTabPage as TabFile;
                        movedTab.BaseFile.OrderBefore(pivotTab.BaseFile);
                    } else if ((base.TabPages.IndexOf(currTabPage) > base.TabPages.IndexOf(_dragTabPage))) {
                        base.TabPages.Remove(_dragTabPage);
                        base.TabPages.Insert(base.TabPages.IndexOf(currTabPage) + 1, _dragTabPage);
                        base.SelectedTab = _dragTabPage;
                        var pivotTab = currTabPage as TabFile;
                        var movedTab = _dragTabPage as TabFile;
                        movedTab.BaseFile.OrderAfter(pivotTab.BaseFile);
                    }
                    base.Enabled = true;
                }
            }
            _dragTabPage = null;
            base.Cursor = Cursors.Default;
            base.OnMouseUp(e);
        }

        #endregion


        private TabFile GetNextTab() {
            var tindex = TabPages.IndexOf(SelectedTab) + 1; //select next tab
            if (tindex >= TabPages.Count) {
                tindex -= 2; //go to one in front of it
            }
            if ((tindex > 0) && (tindex < TabPages.Count)) {
                return (TabFile)TabPages[tindex];
            }
            return null;
        }

    }
}
