using System;
using System.Drawing;
using System.Windows.Forms;

namespace QText {
    internal class Tray : IDisposable {

        private NotifyIcon notMain = new NotifyIcon();
        public Form Form { get; private set; }


        private static readonly object SyncRoot = new object();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This program is not intended to be localized.")]
        public Tray(Form form) {
            this.Form = form;

            this.Form.CreateControl();
            this.Form.Handle.GetType();

            notMain.Icon = Medo.Resources.ManifestResources.GetIcon("QText.Properties.App.ico", 16, 16);
            notMain.Text = Medo.Reflection.EntryAssembly.Title;

            notMain.MouseClick += delegate(object sender, MouseEventArgs e) {
                if ((e.Button == MouseButtons.Left) && (Settings.TrayOneClickActivation)) {
                    ShowForm();
                }
            };
            notMain.MouseDoubleClick += delegate(object sender, MouseEventArgs e) {
                if (e.Button == MouseButtons.Left) {
                    ShowForm();
                }
            };

            notMain.ContextMenu = new ContextMenu();
            notMain.ContextMenu.Popup += delegate(object sender, EventArgs e) {
                var showItem = new MenuItem("&Show") { DefaultItem = true };
                if (App.Hotkey.IsRegistered) { showItem.Text += "\t" + Helper.GetKeyString(App.Hotkey.Key); }
                showItem.Click += delegate(object sender2, EventArgs e2) {
                    ShowForm();
                };

                var showOnPrimaryItem = new MenuItem("Show on primary screen");
                showItem.Click += delegate(object sender2, EventArgs e2) {
                    ShowForm(true);
                };

                var exitItem = new MenuItem("E&xit");
                exitItem.Click += delegate(object sender2, EventArgs e2) {
                    this.Hide();
                    Application.Exit();
                };

                notMain.ContextMenu.MenuItems.Clear();
                notMain.ContextMenu.MenuItems.Add(showItem);
                notMain.ContextMenu.MenuItems.Add(showOnPrimaryItem);
                notMain.ContextMenu.MenuItems.Add(new MenuItem("-"));
                notMain.ContextMenu.MenuItems.Add(exitItem);
            };
        }


        public void Show() {
            notMain.Visible = true;
        }

        public void Hide() {
            notMain.Visible = false;
        }

        public void ShowForm(bool showOnPrimary = false) {
            lock (Tray.SyncRoot) {
                this.Form.Show();
                if (this.Form.WindowState == FormWindowState.Minimized) { this.Form.WindowState = FormWindowState.Normal; }

                if (showOnPrimary) {
                    Rectangle priBounds = Screen.PrimaryScreen.WorkingArea;
                    Rectangle currBounds = this.Form.Bounds;
                    Rectangle normalBounds = default(Rectangle);
                    if ((this.Form.WindowState == FormWindowState.Normal)) {
                        normalBounds = this.Form.Bounds;
                    } else {
                        normalBounds = this.Form.RestoreBounds;
                    }

                    if (!((currBounds.Left >= priBounds.Left) && (currBounds.Right <= priBounds.Right) && (currBounds.Top >= priBounds.Top) && (currBounds.Bottom <= priBounds.Bottom))) {
                        FormWindowState oldState = this.Form.WindowState;

                        if (oldState != FormWindowState.Normal) {
                            this.Form.WindowState = FormWindowState.Normal;
                        }

                        if ((normalBounds.Width > priBounds.Width)) {
                            this.Form.Width = priBounds.Width;
                        }
                        if ((normalBounds.Left < priBounds.Left)) {
                            this.Form.Left = priBounds.Left;
                        }
                        if ((normalBounds.Right > priBounds.Right)) {
                            this.Form.Left = priBounds.Right - normalBounds.Width;
                        }

                        if ((normalBounds.Height > priBounds.Height)) {
                            this.Form.Height = priBounds.Height;
                        }
                        if ((normalBounds.Top < priBounds.Top)) {
                            this.Form.Top = priBounds.Top;
                        }
                        if ((normalBounds.Bottom > priBounds.Bottom)) {
                            this.Form.Top = priBounds.Bottom - normalBounds.Height;
                        }

                        this.Form.WindowState = oldState;
                    }
                }

                this.Form.Activate();
            }
        }

        internal void ShowBalloonOnMinimize() {
            if (Settings.ShowBalloonOnNextMinimize) {
                Settings.ShowBalloonOnNextMinimize = false;
                var text = "Program continues to run in background.";
                if (App.Hotkey.IsRegistered) {
                    text += "\n\nPress " + Helper.GetKeyString(App.Hotkey.Key) + " to show window again.";
                }
                notMain.ShowBalloonTip(0, "QText", text, ToolTipIcon.Info);
            }
        }


        #region Disposing

        public void Dispose() {
            this.Dispose(true);
        }

        protected void Dispose(bool disposing) {
            if (disposing) {
                notMain.Dispose();
            }
        }

        #endregion

    }
}
