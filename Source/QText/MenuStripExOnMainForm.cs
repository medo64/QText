using System;
using System.Windows.Forms;

namespace QText {
    internal class MenuStripExOnMainForm : MenuStrip {

        public MenuStripExOnMainForm() {
            this.KeyUp += new KeyEventHandler(MenuStripExOnMainForm_KeyUp);
            this.MenuDeactivate += new EventHandler(MenuStripExOnMainForm_MenuDeactivate);
        }


        private void MenuStripExOnMainForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
            MainForm mainForm = this.Parent as MainForm;
            if (mainForm == null) { return; }
            TabFiles tabFiles = mainForm.tabFiles;

            switch (e.KeyData) {
                case Keys.Menu:
                    if (this.Visible) {
                        if (tabFiles.SelectedTab != null) {
                            TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                            txt.Focus();
                        }
                    }

                    break;
            }
        }

        private void MenuStripExOnMainForm_MenuDeactivate(object sender, System.EventArgs e) {
            MainForm mainForm = this.Parent as MainForm;
            if (mainForm == null) { return; }
            TabFiles tabFiles = mainForm.tabFiles;

            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                txt.Focus();
            }
        }

    }
}
