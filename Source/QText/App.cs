using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Medo.Configuration;
using QText.Plugins;

namespace QText {

    internal static class App {

        public static TrayContext TrayContext;
        public static QText.Document Document;

        public static Medo.Windows.Forms.Hotkey Hotkey = new Medo.Windows.Forms.Hotkey();
        private static bool ArgHide;
        private static bool ArgRestart;
        private static bool ArgSetup;

        [STAThread]
        public static void Main() {
            var mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow));
            using (var setupMutex = new Mutex(false, @"Global\JosipMedved_QText", out var createdNew, mutexSecurity)) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Medo.Configuration.Settings.Read("CultureName", "en-US"));

                ArgHide = Medo.Application.Args.Current.ContainsKey("hide");
                ArgSetup = Medo.Application.Args.Current.ContainsKey("setup");
                ArgRestart = Medo.Application.Args.Current.ContainsKey("restart");

                if (ArgSetup) {
                    Legacy.OrderedFiles.Upgrade();
                    Legacy.HiddenFiles.Upgrade();
                }

                Medo.Application.UnhandledCatch.ThreadException += new System.EventHandler<ThreadExceptionEventArgs>(UnhandledException);
                Medo.Application.UnhandledCatch.Attach();
                Medo.Application.Restart.Register("/restart");

                Application.ApplicationExit += new System.EventHandler(ApplicationExit);

                Medo.Windows.Forms.State.NoRegistryWrites = Settings.Current.NoRegistryWrites;
                Medo.Diagnostics.ErrorReport.DisableAutomaticSaveToTemp = Settings.Current.NoRegistryWrites;

                if (!Config.IsAssumedInstalled) {
                    Medo.Windows.Forms.State.ReadState += delegate (object sender, Medo.Windows.Forms.StateReadEventArgs e) {
                        e.Value = Config.Read("State!" + e.Name.Replace("QText.", ""), e.DefaultValue);
                    };
                    Medo.Windows.Forms.State.WriteState += delegate (object sender, Medo.Windows.Forms.StateWriteEventArgs e) {
                        Config.Write("State!" + e.Name.Replace("QText.", ""), e.Value);
                    };
                }

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
                                var defaultPath = Settings.Current.DefaultFilesLocation;
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

                //initialize plugins
                foreach (var plugin in App.GetEnabledPlugins()) {
                    plugin.Initialize(App.TrayContext);
                }

                if (!ArgHide && !ArgRestart) {
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
            if (!ArgHide && !ArgRestart) {
                App.TrayContext.ShowForm();
            }
        }


        private static void Hotkey_HotkeyActivated(object sender, EventArgs e) {
            var doShow = true;
            if (Settings.Current.HotkeyTogglesVisibility) {
                var activeHwnd = NativeMethods.GetForegroundWindow();
                if (activeHwnd != IntPtr.Zero) {
                    NativeMethods.GetWindowThreadProcessId(activeHwnd, out var activeProcess);
                    doShow = !(activeProcess == Process.GetCurrentProcess().Id); //hide if already active
                }
            }

            if (doShow) {
                App.TrayContext.ShowForm();
            } else {
                App.TrayContext.HideForm();
            }
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


        #region Plugins

        private static ReadOnlyCollection<IPlugin> AllPlugins;

        public static IEnumerable<IPlugin> GetEnabledPlugins() {
            if (App.AllPlugins == null) { //TODO: Dynamic loading whenever I have more plugins
                var list = new List<IPlugin>();
                list.Add(new Plugins.Reminder.ReminderPlugin());
                App.AllPlugins = list.AsReadOnly();
            }

            foreach (var plugin in App.AllPlugins) {
                yield return plugin;
            }
        }

        #endregion Plugins


        private static class NativeMethods {

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

            [DllImport("user32.dll", EntryPoint = "AllowSetForegroundWindow")]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool AllowSetForegroundWindow(int dwProcessId);

        }

    }
}
