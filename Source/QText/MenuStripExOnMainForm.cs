using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace QText {
    internal class MenuStripExOnMainForm : MenuStrip {

        public MenuStripExOnMainForm() {
            this.KeyUp += new KeyEventHandler(MenuStripExOnMainForm_KeyUp);
            this.MenuDeactivate += new EventHandler(MenuStripExOnMainForm_MenuDeactivate);
        }


        private void MenuStripExOnMainForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
            Debug.WriteLine("MenuStripEx.KeyUp: " + e.KeyData.ToString());
            MainForm mainForm = this.Parent as MainForm;
            if (mainForm == null) { return; }
            TabControlDnD tabFiles = mainForm.tabFiles;

            switch (e.KeyData) {
                case Keys.Menu:
                    if (this.Visible) {
                        if (!Settings.ShowMenu) {
                            this.Visible = false;
                        }
                        if (tabFiles.SelectedTab != null) {
                            TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                            txt.Focus();
                        }
                    }

                    break;
            }
        }

        private void MenuStripExOnMainForm_MenuDeactivate(object sender, System.EventArgs e) {
            Debug.WriteLine("MenuStripEx.Deactivate");
            MainForm mainForm = this.Parent as MainForm;
            if (mainForm == null) { return; }
            TabControlDnD tabFiles = mainForm.tabFiles;

            this.Visible = Settings.ShowMenu;

            if (tabFiles.SelectedTab != null) {
                TextBoxBase txt = tabFiles.SelectedTab.TextBox;
                txt.Focus();
            }
        }

    }
}
