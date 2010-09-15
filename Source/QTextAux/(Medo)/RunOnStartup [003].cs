//Josip Medved <jmedved@jmedved.com>  http://www.jmedved.com

//2008-01-03: First version.
//2008-01-03: Added Resources.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2.


namespace Medo.Configuration {

	/// <summary>
	/// Controls defining application as startup process.
	/// </summary>
	public class RunOnStartup {

		private const string runSubkey = @"Software\Microsoft\Windows\CurrentVersion\Run";

		private static RunOnStartup _current = new RunOnStartup();
		/// <summary>
		/// Settings for current executable.
		/// </summary>
		public static RunOnStartup Current {
			get { return _current; }
		}


		/// <summary>
		/// Creates new instance with entry assembly as template.
		/// </summary>
		private RunOnStartup() : this(null,null) {
		}

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="title">Title of application. It will be used as name for value. If value is null or empty, entry assembly's title will be used.</param>
		/// <param name="executablePath">Full path of executable file. If null, current executable will be used.</param>
		public RunOnStartup(string title, string executablePath) {
			if (title == null) {
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
				object[] titleAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), true);
				if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
					this._title = ((System.Reflection.AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
				} else {
					this._title = assembly.GetName().Name;
				}
			} else {
				if (title.Length == 0) { throw new System.ArgumentException(Resources.ExceptionTitleCannotBeEmpty); }
				this._title = title;
			}

			if (executablePath == null) {
				this._executablePath = System.Windows.Forms.Application.ExecutablePath;
			} else {
				if (!System.IO.File.Exists(executablePath)) { throw new System.IO.FileNotFoundException(Resources.ExceptionExecutableCannotBeFound, executablePath); }
				_executablePath = executablePath;
			}
		}


		private string _title;
		/// <summary>
		/// Title of application. This will be used as value name in registry.
		/// </summary>
		public string Title {
			get { return _title; }
		}

		private string _executablePath;
		/// <summary>
		/// Full path of executable file.
		/// </summary>
		public string ExecutablePath {
			get { return _executablePath; }
		}

		/// <summary>
		/// Gets/sets whether this program is set as startup for current user.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Cannot open registry key.</exception>
		/// <exception cref="System.UnauthorizedAccessException">Attempted to perform an unauthorized operation.</exception>
		public bool RunForCurrentUser {
			get {
				using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runSubkey, false)) {
					if (rk != null) {
						object value = rk.GetValue(this.Title, null);
						if (value != null) {
							if (rk.GetValueKind(this.Title) == Microsoft.Win32.RegistryValueKind.String) {
								return string.Compare(_executablePath, (string)value, System.StringComparison.OrdinalIgnoreCase) == 0;
							}
						}
					}
				}
				return false;
			}
			set {
				if (value == true) { //add it to registry.
					if (this.RunForCurrentUser == false) {
						using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runSubkey, true)) {
							if (rk != null) {
								rk.SetValue(this.Title, this.ExecutablePath, Microsoft.Win32.RegistryValueKind.String);
							} else {
								throw new System.InvalidOperationException(Resources.ExceptionCannotOpenRegistryKey);
							}
						}
					}
				} else { //delete if from registry.
					if (this.RunForCurrentUser == true) {
						using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runSubkey, true)) {
							if (rk != null) {
								rk.DeleteValue(this.Title, false);
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
				using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runSubkey, false)) {
					if (rk != null) {
						object value = rk.GetValue(this.Title, null);
						if (value != null) {
							if (rk.GetValueKind(this.Title) == Microsoft.Win32.RegistryValueKind.String) {
								return string.Compare(_executablePath, (string)value, System.StringComparison.OrdinalIgnoreCase) == 0;
							}
						}
					}
				}
				return false;
			}
			set {
				if (value == true) { //add it to registry.
					if (this.RunForAllUsers == false) {
						using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runSubkey, true)) {
							if (rk != null) {
								rk.SetValue(this.Title, this.ExecutablePath, Microsoft.Win32.RegistryValueKind.String);
							} else {
								throw new System.InvalidOperationException(Resources.ExceptionCannotOpenRegistryKey);
							}
						}
					}
				} else { //delete if from registry.
					if (this.RunForAllUsers == true) {
						using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runSubkey, true)) {
							if (rk != null) {
								rk.DeleteValue(this.Title, false);
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
