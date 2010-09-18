using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.ComponentModel;
using System;

namespace QTextAux {
    public static class App {

        public static Mutex SetupMutex;
        public static Form Form;
        public static Tray Tray;

        public static Medo.Windows.Forms.Hotkey Hotkey = new Medo.Windows.Forms.Hotkey();


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




        }


        private static void ApplicationExit(object sender, System.EventArgs e) {
            if (App.Tray != null) {
                App.Tray.Hide();
            }
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
