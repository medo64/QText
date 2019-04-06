/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2008-01-03: First version.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2.
//2010-12-18: Added Arguments property.


using System;
using System.Globalization;

namespace Medo.Configuration {

    /// <summary>
    /// Controls defining application as startup process.
    /// </summary>
    public class RunOnStartup {

        private const string runSubkey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Settings for current executable.
        /// </summary>
        public static RunOnStartup Current { get; } = new RunOnStartup();


        /// <summary>
        /// Creates new instance with entry assembly as template.
        /// </summary>
        private RunOnStartup()
            : this(null, null, null) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="title">Title of application. It will be used as name for value. If value is null or empty, entry assembly's title will be used.</param>
        /// <param name="executablePath">Full path of executable file. If null, current executable will be used.</param>
        public RunOnStartup(string title, string executablePath)
            : this(title, executablePath, null) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="title">Title of application. It will be used as name for value. If value is null or empty, entry assembly's title will be used.</param>
        /// <param name="executablePath">Full path of executable file. If null, current executable will be used.</param>
        /// <param name="arguments">Arguments for executable file.</param>
        public RunOnStartup(string title, string executablePath, string arguments) {
            if (title == null) {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                var titleAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), true);
                if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                    Title = ((System.Reflection.AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                } else {
                    Title = assembly.GetName().Name;
                }
            } else {
                if (title.Length == 0) { throw new System.ArgumentException(Resources.ExceptionTitleCannotBeEmpty); }
                Title = title;
            }

            if (executablePath == null) {
                ExecutablePath = System.Windows.Forms.Application.ExecutablePath;
            } else {
                if (!System.IO.File.Exists(executablePath)) { throw new System.IO.FileNotFoundException(Resources.ExceptionExecutableCannotBeFound, executablePath); }
                ExecutablePath = executablePath;
            }
            Arguments = arguments;
        }


        /// <summary>
        /// Title of application. This will be used as value name in registry.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Full path of executable file.
        /// </summary>
        public string ExecutablePath { get; private set; }

        /// <summary>
        /// Gets arguments for executable.
        /// </summary>
        public string Arguments { get; private set; }


        private string ExecutablePathWithQuotes {
            get {
                return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", ExecutablePath);
            }
        }

        private string ExecutablePathWithQuotesAndArguments {
            get {
                if (string.IsNullOrEmpty(Arguments)) {
                    return ExecutablePathWithQuotes;
                } else {
                    return ExecutablePathWithQuotes + " " + Arguments;
                }
            }
        }

        private bool IsExecutableInside(string value) {
            if ((string.Compare(ExecutablePath, value, StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(ExecutablePathWithQuotes, value, System.StringComparison.OrdinalIgnoreCase) == 0)) {
                return true;
            } else if (value.StartsWith(ExecutablePathWithQuotes + " ", System.StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Gets/sets whether this program is set as startup for current user.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot open registry key.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Attempted to perform an unauthorized operation.</exception>
        public bool RunForCurrentUser {
            get {
                using (var rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runSubkey, false)) {
                    if (rk != null) {
                        var value = rk.GetValue(Title, null);
                        if (value != null) {
                            if (rk.GetValueKind(Title) == Microsoft.Win32.RegistryValueKind.String) {
                                return IsExecutableInside(value.ToString());
                            }
                        }
                    }
                }
                return false;
            }
            set {
                if (value == true) { //add it to registry.
                    if (RunForCurrentUser == false) {
                        using (var rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runSubkey, true)) {
                            if (rk != null) {
                                rk.SetValue(Title, ExecutablePathWithQuotesAndArguments, Microsoft.Win32.RegistryValueKind.String);
                            } else {
                                throw new System.InvalidOperationException(Resources.ExceptionCannotOpenRegistryKey);
                            }
                        }
                    }
                } else { //delete if from registry.
                    if (RunForCurrentUser == true) {
                        using (var rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runSubkey, true)) {
                            if (rk != null) {
                                rk.DeleteValue(Title, false);
                            } else {
                                throw new System.InvalidOperationException(Resources.ExceptionCannotOpenRegistryKey);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets/sets whether this program is set as startup for all users.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot open registry key.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Attempted to perform an unauthorized operation.</exception>
        public bool RunForAllUsers {
            get {
                using (var rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runSubkey, false)) {
                    if (rk != null) {
                        var value = rk.GetValue(Title, null);
                        if (value != null) {
                            if (rk.GetValueKind(Title) == Microsoft.Win32.RegistryValueKind.String) {
                                return IsExecutableInside(value.ToString());
                            }
                        }
                    }
                }
                return false;
            }
            set {
                if (value == true) { //add it to registry.
                    if (RunForAllUsers == false) {
                        using (var rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runSubkey, true)) {
                            if (rk != null) {
                                rk.SetValue(Title, ExecutablePathWithQuotesAndArguments, Microsoft.Win32.RegistryValueKind.String);
                            } else {
                                throw new System.InvalidOperationException(Resources.ExceptionCannotOpenRegistryKey);
                            }
                        }
                    }
                } else { //delete if from registry.
                    if (RunForAllUsers == true) {
                        using (var rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runSubkey, true)) {
                            if (rk != null) {
                                rk.DeleteValue(Title, false);
                            } else {
                                throw new System.InvalidOperationException(Resources.ExceptionCannotOpenRegistryKey);
                            }
                        }
                    }
                }
            }
        }


        private static class Resources {

            internal static string ExceptionTitleCannotBeEmpty { get { return "Title cannot be empty."; } }

            internal static string ExceptionExecutableCannotBeFound { get { return "Executable cannot be found."; } }

            internal static string ExceptionCannotOpenRegistryKey { get { return "Cannot open registry key."; } }

        }

    }
}
