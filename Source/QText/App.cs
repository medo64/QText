using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace QText {

    internal static class App {

        public static TrayContext TrayContext;
        public static QText.Document Document;

        public static Medo.Windows.Forms.Hotkey Hotkey = new Medo.Windows.Forms.Hotkey();


        [STAThread]
        public static void Main() {
            var mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow));
            using (var setupMutex = new Mutex(false, @"Global\JosipMedved_QText", out var createdNew, mutexSecurity)) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Medo.Configuration.Settings.Read("CultureName", "en-US"));

                if (Medo.Application.Args.Current.ContainsKey("setup")) {
                    Legacy.OrderedFiles.Upgrade();
                    Legacy.HiddenFiles.Upgrade();
                }

                Medo.Application.UnhandledCatch.ThreadException += new System.EventHandler<ThreadExceptionEventArgs>(UnhandledException);
                Medo.Application.UnhandledCatch.Attach();
                Medo.Application.Restart.Register("/restart");

                Application.ApplicationExit += new System.EventHandler(ApplicationExit);

                Medo.Windows.Forms.State.NoRegistryWrites = Settings.Current.NoRegistryWrites;
                Medo.Diagnostics.ErrorReport.DisableAutomaticSaveToTemp = Settings.Current.NoRegistryWrites;

                #region Init document

                try {
                    OpenDocument();
                } catch (DirectoryNotFoundException) { //try to create it
                    try {
                        Helper.CreatePath(Settings.Current.FilesLocation);
                        OpenDocument();
                    } catch (DirectoryNotFoundException) { //try to create it
                        if (Medo.MessageBox.ShowError(null, "Directory " + Settings.Current.FilesLocation + " cannot be found.\n\nDo you wish to use default location at " + Settings.Current.DefaultFilesLocation + "?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                            Settings.Current.FilesLocation = Settings.Current.DefaultFilesLocation;
                            Helper.CreatePath(Settings.Current.FilesLocation);
                            OpenDocument();
                        } else {
                            return;
                        }
                    } catch (InvalidOperationException) { //try to create it
                        if (Medo.MessageBox.ShowError(null, "Directory " + Settings.Current.FilesLocation + " cannot be used.\n\nDo you wish to use default location at " + Settings.Current.DefaultFilesLocation + "?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                            Settings.Current.FilesLocation = Settings.Current.DefaultFilesLocation;
                            Helper.CreatePath(Settings.Current.FilesLocation);
                            OpenDocument();
                        } else {
                            return;
                        }
                    }
                } catch (NotSupportedException) { //path is invalid
                    if (Medo.MessageBox.ShowError(null, "Directory " + Settings.Current.FilesLocation + " is invalid.\n\nDo you wish to use default location at " + Settings.Current.DefaultFilesLocation + "?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        Settings.Current.FilesLocation = Settings.Current.DefaultFilesLocation;
                        Helper.CreatePath(Settings.Current.FilesLocation);
                        OpenDocument();
                    } else {
                        return;
                    }
                } catch (InvalidOperationException ex) {
                    Medo.MessageBox.ShowError(null, "Fatal startup error: " + ex.Message);
                    return;
                }

                #endregion

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
                    Helper.CreatePath(Settings.Current.FilesLocation);
                } catch (Exception ex) {
                    switch (Medo.MessageBox.ShowQuestion(null, ex.Message + Environment.NewLine + "Do you wish to try using default location instead?", MessageBoxButtons.YesNo)) {
                        case DialogResult.Yes:
                            try {
                                string defaultPath = Settings.Current.DefaultFilesLocation;
                                Helper.CreatePath(defaultPath);
                                Settings.Current.FilesLocation = defaultPath;
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

                App.TrayContext = new TrayContext(new MainForm());
                App.TrayContext.ShowIcon();

                App.Hotkey.HotkeyActivated += new EventHandler<EventArgs>(Hotkey_HotkeyActivated);
                if (Settings.Current.ActivationHotkey != Keys.None) {
                    try {
                        App.Hotkey.Register(Settings.Current.ActivationHotkey);
                    } catch (InvalidOperationException) {
                        Medo.MessageBox.ShowWarning(null, "Hotkey is already in use.");
                    }
                }

                if (Medo.Application.Args.Current.ContainsKey("hide") == false) {
                    App.TrayContext.ShowForm();
                }
                Application.Run(App.TrayContext);
            }
        }

        private static void OpenDocument() {
            App.Document = new Document(Settings.Current.FilesLocation) {
                DeleteToRecycleBin = Settings.Current.FilesDeleteToRecycleBin,
                CarbonCopyRootPath = Settings.Current.CarbonCopyUse ? Settings.Current.CarbonCopyDirectory : null,
                CarbonCopyIgnoreErrors = Settings.Current.CarbonCopyIgnoreErrors
            };
        }

        private static void SingleInstance_NewInstanceDetected(object sender, Medo.Application.NewInstanceEventArgs e) {
            App.TrayContext.ShowForm();
        }


        private static void Hotkey_HotkeyActivated(object sender, EventArgs e) {
            App.TrayContext.ShowForm();
        }


        private static void ApplicationExit(object sender, System.EventArgs e) {
            if (App.Hotkey.IsRegistered) { App.Hotkey.Unregister(); }
            Environment.Exit(0);
        }

        private static void UnhandledException(object sender, ThreadExceptionEventArgs e) {
#if !DEBUG
            foreach (Form form in Application.OpenForms) {
                if (form.TopMost) {
                    Medo.Diagnostics.ErrorReport.TopMost = true;
                    break;
                }
            }
            Medo.Diagnostics.ErrorReport.ShowDialog(null, e.Exception, new Uri("https://medo64.com/feedback/"));

            if (Medo.Application.Restart.IsRegistered) { throw e.Exception; }
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
