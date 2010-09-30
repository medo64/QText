using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.ComponentModel;
using System;

namespace QText {

    public static class App {

        public static Mutex SetupMutex;
        public static MainForm Form;
        public static Tray Tray;

        public static Medo.Windows.Forms.Hotkey Hotkey = new Medo.Windows.Forms.Hotkey();


        [STAThread]
        public static void Main() {
            App.SetupMutex = new Mutex(false, @"Global\JosipMedved_QText");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Medo.Configuration.Settings.Read("CultureName", "en-US"));

            if (Settings.LegacySettingsCopied == false) {
                var currProcess = Process.GetCurrentProcess();
                var currProcessId = currProcess.Id;
                var currProcessName = currProcess.ProcessName;
                var currProcessFileName = currProcess.MainModule.FileName;
                foreach (var iProcess in Process.GetProcesses()) {
                    Debug.WriteLine(iProcess.ProcessName);
                    try {
                        if (string.CompareOrdinal(iProcess.ProcessName, "QText") == 0) {
                            if (iProcess.Id != currProcessId) {
                                iProcess.Kill();
                            }
                        }
                    } catch (Win32Exception) { }
                }
            }

            Medo.Application.UnhandledCatch.ThreadException += new System.EventHandler<ThreadExceptionEventArgs>(UnhandledException);
            Medo.Application.UnhandledCatch.Attach();

            Helper.LegacySettings.CopyLegacySettingsIfNeeded();

            Application.ApplicationExit += new System.EventHandler(ApplicationExit);


            App.Form = new MainForm();

            Medo.Application.SingleInstance.NewInstanceDetected += new EventHandler<Medo.Application.NewInstanceEventArgs>(SingleInstance_NewInstanceDetected);
            Medo.Application.SingleInstance.Attach();


            try {
                Helper.Path.CreatePath(Settings.FilesLocation);
            } catch (Exception ex) {
                switch (Medo.MessageBox.ShowQuestion(null, ex.Message + Environment.NewLine + "Do you wish to try using default location instead?", MessageBoxButtons.YesNo)) {
                    case DialogResult.Yes:
                        try {
                            string defaultPath = Settings.DefaultFilesLocation;
                            Helper.Path.CreatePath(defaultPath);
                            Settings.FilesLocation = defaultPath;
                        } catch (Exception ex2) {
                            global::Medo.MessageBox.ShowError(null, ex2.Message, MessageBoxButtons.OK);
                            Application.Exit();
                            System.Environment.Exit(0);
                        }
                        break;
                    case DialogResult.No:
                        break;
                }
            }

            App.Tray = new Tray(App.Form);
            App.Tray.Show();

            App.Hotkey.HotkeyActivated += new EventHandler<EventArgs>(Hotkey_HotkeyActivated);
            if ((Settings.ActivationHotkey != Keys.None)) {
                try {
                    App.Hotkey.Register(Settings.ActivationHotkey);
                } catch (InvalidOperationException) {
                    Medo.MessageBox.ShowWarning(null, "Hotkey is already in use.");
                }
            }


            Application.Run();


            System.GC.KeepAlive(App.SetupMutex);
        }


        private static void SingleInstance_NewInstanceDetected(object sender, Medo.Application.NewInstanceEventArgs e) {
            try {
                App.Form.CreateControl();
                App.Form.Handle.GetType();

                NewInstanceDetectedProcDelegate method = new NewInstanceDetectedProcDelegate(NewInstanceDetectedProc);
                App.Form.Invoke(method);
            } catch (Exception) { }
        }

        private delegate void NewInstanceDetectedProcDelegate();

        private static void NewInstanceDetectedProc() {
            App.Tray.Show();
            if (App.Form.WindowState == FormWindowState.Minimized) { App.Form.WindowState = FormWindowState.Normal; }
            App.Form.Show();
            App.Form.Activate();
        }

        private static void Hotkey_HotkeyActivated(object sender, EventArgs e) {
            NewInstanceDetectedProc();
        }


        private static void ApplicationExit(object sender, System.EventArgs e) {
            if (App.Tray != null) { App.Tray.Hide(); }
            Environment.Exit(0);
        }

        private static void UnhandledException(object sender, ThreadExceptionEventArgs e) {
            Medo.Diagnostics.ErrorReport.SaveToTemp(e.Exception);
#if !DEBUG
            Medo.Diagnostics.ErrorReport.ShowDialog(null, e.Exception, new Uri("http://jmedved.com/ErrorReport/"));
#else
            throw e.Exception;
#endif
        }

    }
}
