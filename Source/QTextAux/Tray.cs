using System.Drawing;
using System.Windows.Forms;

namespace QTextAux {
    internal class Tray {

        private MenuItem mnxNotifyShow = new MenuItem("&Show");
        private MenuItem mnxNotifyShowOnPrimary = new MenuItem("&Show on primary screen");
        private MenuItem mnxNotifyExit = new MenuItem("E&xit");
        private NotifyIcon notMain = new NotifyIcon();
        public Form Form { get; private set; }


        private static readonly object SyncRoot = new object();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This program is not intended to be localized.")]
        public Tray(Form form) {
            this.Form = form;

            this.Form.CreateControl();
            this.Form.Handle.GetType();

            mnxNotifyShow.DefaultItem = true;
            mnxNotifyShow.Click += mnxNotifyShow_Click;
            mnxNotifyShowOnPrimary.Click += mnxNotifyShowOnPrimary_Click;
            mnxNotifyExit.Click += mnxNotifyExit_Click;

            notMain.ContextMenu = new ContextMenu(new MenuItem[] {
			    mnxNotifyShow,
			    mnxNotifyShowOnPrimary,
			    new MenuItem("-"),
			    mnxNotifyExit
		    });
            notMain.Icon = global::Medo.Resources.ManifestResources.GetIcon("QText.App.ico", 16, 16);
            notMain.Text = global::Medo.Reflection.EntryAssembly.Title;
            notMain.MouseClick += notMain_MouseClick;
            notMain.MouseDoubleClick += notMain_MouseDoubleClick;
        }


        public void Show() {
            notMain.Visible = true;
        }

        public void Hide() {
            notMain.Visible = false;
        }

        private void mnxNotifyShow_Click(object sender, System.EventArgs e) {
            lock (Tray.SyncRoot) {
                this.Form.Show();
                this.Form.Activate();
            }
        }

        private void mnxNotifyShowOnPrimary_Click(object sender, System.EventArgs e) {
            lock (Tray.SyncRoot) {
                this.Form.Show();

                Rectangle priBounds = Screen.PrimaryScreen.WorkingArea;
                Rectangle currBounds = this.Form.Bounds;
                Rectangle normalBounds = default(Rectangle);
                if ((this.Form.WindowState == FormWindowState.Normal)) {
                    normalBounds = this.Form.Bounds;
                } else {
                    normalBounds = this.Form.RestoreBounds;
                }

                if ((currBounds.Left >= priBounds.Left) && (currBounds.Right <= priBounds.Right) && (currBounds.Top >= priBounds.Top) && (currBounds.Bottom <= priBounds.Bottom)) {
                } else {
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

                this.Form.Activate();
            }
        }

        private void mnxNotifyExit_Click(object sender, System.EventArgs e) {
            this.Hide();
            Application.Exit();
        }

        private void notMain_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            if ((e.Button == MouseButtons.Left) && (Settings.TrayOneClickActivation)) {
                mnxNotifyShow_Click(sender, new System.EventArgs());
            }
        }

        private void notMain_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            if ((e.Button == MouseButtons.Left) && (!Settings.TrayOneClickActivation)) {
                mnxNotifyShow_Click(sender, new System.EventArgs());
            }
        }

    }
}
