using System;
using System.Drawing;
using System.Windows.Forms;

namespace QText {
    internal class TrayContext : ApplicationContext {

        private NotifyIcon notMain = new NotifyIcon();
        public Form Form { get; private set; }


        private static readonly object SyncRoot = new object();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This program is not intended to be localized.")]
        public TrayContext(Form form) : base() {
            this.Form = form;

            this.Form.CreateControl();
            this.Form.Handle.GetType();

            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 10)) {
                notMain.Icon = QText.Properties.Resources.TrayIcon_White;
            } else {
                notMain.Icon = QText.Properties.Resources.TrayIcon;
            }
            notMain.Text = Medo.Reflection.EntryAssembly.Title;

            notMain.MouseClick += delegate (object sender, MouseEventArgs e) {
                if ((e.Button == MouseButtons.Left) && (Settings.TrayOneClickActivation)) {
                    this.ShowForm();
                }
            };
            notMain.MouseDoubleClick += delegate (object sender, MouseEventArgs e) {
                if (e.Button == MouseButtons.Left) {
                    this.ShowForm();
                }
            };

            notMain.ContextMenu = new ContextMenu();
            notMain.ContextMenu.Popup += delegate (object sender, EventArgs e) {
                var showItem = new MenuItem("&Show") { DefaultItem = true };
                if (App.Hotkey.IsRegistered) { showItem.Text += "\t" + Helper.GetKeyString(App.Hotkey.Key); }
                showItem.Click += delegate (object sender2, EventArgs e2) {
                    this.ShowForm();
                };

                var showOnPrimaryItem = new MenuItem("Show on primary screen");
                showItem.Click += delegate (object sender2, EventArgs e2) {
                    this.ShowForm(true);
                };

                var exitItem = new MenuItem("E&xit");
                exitItem.Click += delegate (object sender2, EventArgs e2) {
                    this.HideIcon();
                    Application.Exit();
                };

                notMain.ContextMenu.MenuItems.Clear();
                notMain.ContextMenu.MenuItems.Add(showItem);
                notMain.ContextMenu.MenuItems.Add(showOnPrimaryItem);
                notMain.ContextMenu.MenuItems.Add(new MenuItem("-"));
                notMain.ContextMenu.MenuItems.Add(exitItem);
            };
        }


        public void ShowIcon() {
            notMain.Visible = true;
        }

        public void HideIcon() {
            notMain.Visible = false;
        }


        #region ShowForm

        private delegate void ShowFormProcDelegate(bool showOnPrimary);

        public void ShowForm(bool showOnPrimary = false) {
            try {
                if (this.Form.InvokeRequired) {
                    var method = new ShowFormProcDelegate(ShowFormProc);
                    this.Form.Invoke(method, showOnPrimary);
                } else {
                    ShowFormProc(showOnPrimary);
                }
            } catch (Exception) { }
        }

        private void ShowFormProc(bool showOnPrimary) {
            this.ShowIcon();

            lock (TrayContext.SyncRoot) {
                if (this.Form.IsHandleCreated == false) {
                    this.Form.CreateControl();
                    this.Form.Handle.GetType();
                }

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

        #endregion

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


        protected override void ExitThreadCore() {
            this.HideIcon();
            base.ExitThreadCore();
        }


        #region Disposing

        protected override void Dispose(bool disposing) {
            if (disposing) {
                notMain.Dispose();
            }
        }

        #endregion

    }
}
