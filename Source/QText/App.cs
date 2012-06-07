using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace QText {

    internal static class App {

        public static MainForm Form;
        public static Tray Tray;

        public static Medo.Windows.Forms.Hotkey Hotkey = new Medo.Windows.Forms.Hotkey();


        [STAThread]
        public static void Main() {
            bool createdNew;
            var mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow));
            using (var setupMutex = new Mutex(false, @"Global\JosipMedved_QText", out createdNew, mutexSecurity)) {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Medo.Configuration.Settings.Read("CultureName", "en-US"));

                if (Medo.Application.Args.Current.ContainsKey("setup")) {
                    Legacy.Settings.Upgrade();
                    Legacy.OrderedFiles.Upgrade();
                    Legacy.HiddenFiles.Upgrade();
                }

                Medo.Application.UnhandledCatch.ThreadException += new System.EventHandler<ThreadExceptionEventArgs>(UnhandledException);
                Medo.Application.UnhandledCatch.Attach();

                Application.ApplicationExit += new System.EventHandler(ApplicationExit);

                Medo.Configuration.Settings.NoRegistryWrites = !Settings.Installed;
                Medo.Windows.Forms.State.NoRegistryWrites = !Settings.Installed;

                App.Form = new MainForm();

                Medo.Application.SingleInstance.NewInstanceDetected += new EventHandler<Medo.Application.NewInstanceEventArgs>(SingleInstance_NewInstanceDetected);
                if (Medo.Application.SingleInstance.IsOtherInstanceRunning) {
                    var currProcess = Process.GetCurrentProcess();
                    foreach (var iProcess in Process.GetProcessesByName(currProcess.ProcessName)) {
                        try {
                            if (iProcess.Id != currProcess.Id) {
                                NativeMethods.AllowSetForegroundWindow(iProcess.Id);
                                break;
                            }
                        } catch (Win32Exception) { }
                    }
                }
                Medo.Application.SingleInstance.Attach();


                try {
                    Helper.CreatePath(Settings.FilesLocation);
                } catch (Exception ex) {
                    switch (Medo.MessageBox.ShowQuestion(null, ex.Message + Environment.NewLine + "Do you wish to try using default location instead?", MessageBoxButtons.YesNo)) {
                        case DialogResult.Yes:
                            try {
                                string defaultPath = Settings.DefaultFilesLocation;
                                Helper.CreatePath(defaultPath);
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
                if (Settings.ActivationHotkey != Keys.None) {
                    try {
                        App.Hotkey.Register(Settings.ActivationHotkey);
                    } catch (InvalidOperationException) {
                        Medo.MessageBox.ShowWarning(null, "Hotkey is already in use.");
                    }
                }

                if (Medo.Application.Args.Current.ContainsKey("hide") == false) {
                    Tray.ShowForm();
                }
                Application.Run();
            }
        }


        private static void SingleInstance_NewInstanceDetected(object sender, Medo.Application.NewInstanceEventArgs e) {
            try {
                if (App.Form.IsHandleCreated == false) {
                    App.Form.CreateControl();
                    App.Form.Handle.GetType();
                }

                NewInstanceDetectedProcDelegate method = new NewInstanceDetectedProcDelegate(NewInstanceDetectedProc);
                App.Form.Invoke(method);
            } catch (Exception) { }
        }

        private delegate void NewInstanceDetectedProcDelegate();

        private static void NewInstanceDetectedProc() {
            App.Tray.Show();
            App.Form.Show();
            if (App.Form.WindowState == FormWindowState.Minimized) { App.Form.WindowState = FormWindowState.Normal; }
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
            if (Settings.Installed) { Medo.Diagnostics.ErrorReport.SaveToTemp(e.Exception); }
#if !DEBUG
            Medo.Diagnostics.ErrorReport.ShowDialog(null, e.Exception, new Uri("http://jmedved.com/feedback/"));
#else
            throw e.Exception;
#endif
        }


        private static class NativeMethods {

            [DllImportAttribute("user32.dll", EntryPoint = "AllowSetForegroundWindow")]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool AllowSetForegroundWindow(int dwProcessId);

        }

    }
}
