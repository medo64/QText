﻿//Copyright (c) 2013 Josip Medved <jmedved@jmedved.com>

//2013-12-27: First version.


using System;
using System.Runtime.InteropServices;

namespace Medo.Application {

    /// <summary>
    /// Registering for Windows Error Reporting (WER) restart.
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373347(v=vs.85).aspx
    /// </remarks>
    public static class Restart {

        /// <summary>
        /// Returns true if this application successfully registered for restart.
        /// </summary>
        public static Boolean Register() {
            return Restart.Register(null, RestartModifiers.None);
        }

        /// <summary>
        /// Returns true if this application successfully registered for restart.
        /// </summary>
        /// <param name="arguments">Command-line arguments for the application when it is restarted.</param>
        public static Boolean Register(String arguments) {
            return Restart.Register(arguments, RestartModifiers.None);
        }

        /// <summary>
        /// Returns true if this application successfully registered for restart.
        /// </summary>
        /// <param name="arguments">Command-line arguments for the application when it is restarted.</param>
        /// <param name="modifiers">Special behaviour.</param>
        public static Boolean Register(String arguments, RestartModifiers modifiers) {
            try {
                var hResult = NativeMethods.RegisterApplicationRestart(arguments, (int)modifiers);
                if (NativeMethods.IsHResultSuccess(hResult)) {
                    Restart.IsRegistered = true;
                    return true;
                } else {
                    return false;
                }
            } catch (EntryPointNotFoundException) { //on Windows earlier than Vista
                return false;
            }
        }

        /// <summary>
        /// Gets whether restart is registered.
        /// </summary>
        public static Boolean IsRegistered { get; private set; }

        /// <summary>
        /// Returns true if this application has been successfully unregistered for restart.
        /// </summary>
        public static Boolean Unregister() {
            try {
                var hResult = NativeMethods.UnregisterApplicationRestart();
                if (NativeMethods.IsHResultSuccess(hResult)) {
                    Restart.IsRegistered = false;
                    return true;
                } else {
                    return false;
                }
            } catch (EntryPointNotFoundException) { //on Windows earlier than Vista
                return false;
            }
        }


        private static class NativeMethods {

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "This is bogus warning.")]
            internal static extern UInt32 RegisterApplicationRestart(String pwzCommandline, Int32 dwFlags);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "This is bogus warning.")]
            internal static extern UInt32 UnregisterApplicationRestart();


            internal static Boolean IsHResultSuccess(UInt32 hResult) {
                return (hResult & 0x80000000) != 0x80000000;
            }

        }

    }



    /// <summary>
    /// Restart behaviour.
    /// </summary>
    [Flags]
    public enum RestartModifiers {
        /// <summary>
        /// No special behavior.
        /// </summary>
        None = 0,
        /// <summary>
        /// Do not restart the process if it terminates due to an unhandled exception.
        /// </summary>
        NoCrash = 1,
        /// <summary>
        /// Do not restart the process if it terminates due to the application not responding.
        /// </summary>
        NoHang = 2,
        /// <summary>
        /// Do not restart the process if it terminates due to the installation of an update.
        /// </summary>
        NoPatch = 4,
        /// <summary>
        /// Do not restart the process if the computer is restarted as the result of an update.
        /// </summary>
        NoRestart = 8,
    }
}
