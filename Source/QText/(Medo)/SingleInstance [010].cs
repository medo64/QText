/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2012-11-24: Suppressing bogus CA5122 warning (http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical).
//2010-10-07: Added IsOtherInstanceRunning method.
//2008-11-14: Reworked code to use SafeHandle.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (SpecifyMarshalingForPInvokeStringArguments, NestedTypesShouldNotBeVisible).
//2008-04-10: NewInstanceEventArgs is not nested class anymore.
//2008-01-26: AutoExit parameter changed to NoAutoExit
//2008-01-08: Main method is now called Attach.
//2008-01-06: System.Environment.Exit returns E_ABORT (0x80004004).
//2008-01-03: Added Resources.
//2007-12-29: New version.


using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Medo.Application {

    /// <summary>
    /// Handles detection and communication of programs multiple instances.
    /// This class is thread safe.
    /// </summary>
    public static class SingleInstance {

        private static Mutex _mtxFirstInstance;
        private static Thread _thread;
        private static readonly object _syncRoot = new object();



        /// <summary>
        /// Returns true if this application is not already started.
        /// Another instance is contacted via named pipe.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">API call failed.</exception>
        public static bool Attach() {
            return Attach(false);
        }

        /// <summary>
        /// Returns true if this application is not already started.
        /// Another instance is contacted via named pipe.
        /// </summary>
        /// <param name="noAutoExit">If true, application will exit after informing another instance.</param>
        /// <exception cref="System.InvalidOperationException">API call failed.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Needs to be cought all in order not to break in any case.")]
        public static bool Attach(bool noAutoExit) {
            lock (_syncRoot) {
                NativeMethods.FileSafeHandle handle = null;
                var isFirstInstance = false;
                try {
                    _mtxFirstInstance = new Mutex(true, MutexName, out isFirstInstance);
                    if (isFirstInstance == false) { //we need to contact previous instance.
                        _mtxFirstInstance = null;

                        byte[] buffer;
                        using (var ms = new System.IO.MemoryStream()) {
                            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            bf.Serialize(ms, new NewInstanceEventArgs(System.Environment.CommandLine, System.Environment.GetCommandLineArgs()));
                            ms.Flush();
                            buffer = ms.GetBuffer();
                        }

                        //open pipe
                        if (!NativeMethods.WaitNamedPipe(NamedPipeName, NativeMethods.NMPWAIT_USE_DEFAULT_WAIT)) { throw new System.InvalidOperationException(Resources.ExceptionWaitNamedPipeFailed); }
                        handle = NativeMethods.CreateFile(NamedPipeName, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, 0, System.IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, System.IntPtr.Zero);
                        if (handle.IsInvalid) {
                            throw new System.InvalidOperationException(Resources.ExceptionCreateFileFailed);
                        }

                        //send bytes
                        uint written = 0;
                        var overlapped = new NativeOverlapped();
                        if (!NativeMethods.WriteFile(handle, buffer, (uint)buffer.Length, ref written, ref overlapped)) {
                            throw new System.InvalidOperationException(Resources.ExceptionWriteFileFailed);
                        }
                        if (written != buffer.Length) { throw new System.InvalidOperationException(Resources.ExceptionWriteFileWroteUnexpectedNumberOfBytes); }

                    } else {  //there is no application already running.

                        _thread = new Thread(Run) {
                            Name = "Medo.Application.SingleInstance.0",
                            IsBackground = true
                        };
                        _thread.Start();

                    }

                } catch (System.Exception ex) {
                    System.Diagnostics.Trace.TraceWarning(ex.Message + "  {Medo.Application.SingleInstance}");

                } finally {
                    //if (handle != null && (!(handle.IsClosed || handle.IsInvalid))) {
                    //    handle.Close();
                    //}
                    if (handle != null) {
                        handle.Dispose();
                    }
                }

                if ((isFirstInstance == false) && (noAutoExit == false)) {
                    System.Diagnostics.Trace.TraceInformation("Exit(E_ABORT): Another instance is running.  {Medo.Application.SingleInstance}");
                    System.Environment.Exit(unchecked((int)0x80004004)); //E_ABORT(0x80004004)
                }

                return isFirstInstance;
            }
        }

        private static string _mutexName;
        private static string MutexName {
            get {
                lock (_syncRoot) {
                    if (_mutexName == null) {
                        var sbComponents = new System.Text.StringBuilder();
                        sbComponents.AppendLine(System.Environment.MachineName);
                        sbComponents.AppendLine(System.Environment.UserName);
                        sbComponents.AppendLine(System.Reflection.Assembly.GetEntryAssembly().FullName);
                        sbComponents.AppendLine(System.Reflection.Assembly.GetEntryAssembly().CodeBase);
                        
                        byte[] hash;
                        using (var sha1 =System.Security.Cryptography.SHA1Managed.Create()){
                            hash = sha1.ComputeHash(System.Text.Encoding.Unicode.GetBytes(sbComponents.ToString()));
                        }

                        var sbFinal = new System.Text.StringBuilder();
                        var assName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                        sbFinal.Append(assName, 0, System.Math.Min(assName.Length, 64));
                        sbFinal.Append('.');
                        for (var i = 0; i < hash.Length; ++i) {
                            sbFinal.AppendFormat("{0:X2}", hash[i]);
                        }
                        _mutexName = sbFinal.ToString();
                    }
                    return _mutexName;
                }
            }
        }
        private static readonly string NamedPipeName = @"\\.\pipe\" + MutexName;

        /// <summary>
        /// Gets whether there is another instance running.
        /// It temporary creates mutex.
        /// </summary>
        public static bool IsOtherInstanceRunning {
            get {
                lock (_syncRoot) {
                    if (_mtxFirstInstance != null) {
                        return false; //no other instance is running
                    } else {
                        var tempInstance = new Mutex(true, MutexName, out var isFirstInstance);
                        tempInstance.Close();
                        return (isFirstInstance == false);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs in first instance when new instance is detected.
        /// </summary>
        public static event System.EventHandler<NewInstanceEventArgs> NewInstanceDetected;


        /// <summary>
        /// Thread function.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Needs to be cought all in order not to break in any case.")]
        private static void Run() {
            while (_mtxFirstInstance != null) {
                var handle = IntPtr.Zero;
                try {
                    handle = NativeMethods.CreateNamedPipe(NamedPipeName, NativeMethods.PIPE_ACCESS_DUPLEX, NativeMethods.PIPE_TYPE_BYTE | NativeMethods.PIPE_READMODE_BYTE | NativeMethods.PIPE_WAIT, NativeMethods.PIPE_UNLIMITED_INSTANCES, 4096, 4096, NativeMethods.NMPWAIT_USE_DEFAULT_WAIT, System.IntPtr.Zero);
                    if (handle.Equals(IntPtr.Zero)) { throw new System.InvalidOperationException(Resources.ExceptionCreateNamedPipeFailed); }
                    var connected = NativeMethods.ConnectNamedPipe(handle, System.IntPtr.Zero);
                    if (!connected) { throw new System.InvalidOperationException(Resources.ExceptionConnectNamedPipeFailed); }

                    uint available = 0;
                    while (available == 0) {
                        uint bytesRead = 0, thismsg = 0;
                        if (!NativeMethods.PeekNamedPipe(handle, null, 0, ref bytesRead, ref available, ref thismsg)) {
                            Thread.Sleep(100);
                            available = 0;
                        }
                    }
                    var buffer = new byte[available];
                    uint read = 0;
                    var overlapped = new NativeOverlapped();
                    if (!NativeMethods.ReadFile(handle, buffer, (uint)buffer.Length, ref read, ref overlapped)) {
                        throw new System.InvalidOperationException(Resources.ExceptionReadFileFailed);
                    }
                    if (read != available) {
                        throw new System.InvalidOperationException(Resources.ExceptionReadFileReturnedUnexpectedNumberOfBytes);
                    }

                    using (var ms = new System.IO.MemoryStream(buffer)) {
                        var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        NewInstanceDetected?.Invoke(null, (NewInstanceEventArgs)bf.Deserialize(ms));
                    }

                } catch (System.Exception ex) {
                    System.Diagnostics.Trace.TraceWarning(ex.Message + "  {Medo.Application.SingleInstance");
                    Thread.Sleep(1000);
                } finally { //closing native resources.
                    if (!handle.Equals(System.IntPtr.Zero)) {
                        NativeMethods.CloseHandle(handle);
                    }
                }//try
            }//while
        }

        private static class Resources {

            internal static string ExceptionWaitNamedPipeFailed { get { return "WaitNamedPipe failed."; } }

            internal static string ExceptionCreateFileFailed { get { return "CreateFile failed."; } }

            internal static string ExceptionWriteFileFailed { get { return "WriteFile failed."; } }

            internal static string ExceptionWriteFileWroteUnexpectedNumberOfBytes { get { return "WriteFile wrote unexpected number of bytes."; } }

            internal static string ExceptionCreateNamedPipeFailed { get { return "CreateNamedPipe failed."; } }

            internal static string ExceptionConnectNamedPipeFailed { get { return "ConnectNamedPipe failed."; } }

            internal static string ExceptionReadFileFailed { get { return "ReadFile failed."; } }

            internal static string ExceptionReadFileReturnedUnexpectedNumberOfBytes { get { return "ReadFile returned unexpected number of bytes."; } }

        }

        private static class NativeMethods {
#pragma warning disable IDE0049 // Simplify Names

            public const UInt32 FILE_ATTRIBUTE_NORMAL = 0;
            public const UInt32 GENERIC_READ = 0x80000000;
            public const UInt32 GENERIC_WRITE = 0x40000000;
            public const Int32 INVALID_HANDLE_VALUE = -1;
            public const UInt32 NMPWAIT_USE_DEFAULT_WAIT = 0x00000000;
            public const UInt32 OPEN_EXISTING = 3;
            public const UInt32 PIPE_ACCESS_DUPLEX = 0x00000003;
            public const UInt32 PIPE_READMODE_BYTE = 0x00000000;
            public const UInt32 PIPE_TYPE_BYTE = 0x00000000;
            public const UInt32 PIPE_UNLIMITED_INSTANCES = 255;
            public const UInt32 PIPE_WAIT = 0x00000000;


            public class FileSafeHandle : SafeHandle {
                private static readonly IntPtr minusOne = new IntPtr(-1);


                public FileSafeHandle()
                    : base(minusOne, true) { }


                public override bool IsInvalid {
                    get { return (IsClosed) || (base.handle == minusOne); }
                }

                protected override bool ReleaseHandle() {
                    return CloseHandle(base.handle);
                }

                public override string ToString() {
                    return base.handle.ToString();
                }

            }


            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean CloseHandle(IntPtr hObject);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean ConnectNamedPipe(IntPtr hNamedPipe, IntPtr lpOverlapped);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern FileSafeHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CreateNamedPipe(String lpName, UInt32 dwOpenMode, UInt32 dwPipeMode, UInt32 nMaxInstances, UInt32 nOutBufferSize, UInt32 nInBufferSize, UInt32 nDefaultTimeOut, IntPtr lpSecurityAttributes);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean PeekNamedPipe(IntPtr hNamedPipe, Byte[] lpBuffer, UInt32 nBufferSize, ref UInt32 lpBytesRead, ref UInt32 lpTotalBytesAvail, ref UInt32 lpBytesLeftThisMessage);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean ReadFile(IntPtr hFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToRead, ref UInt32 lpNumberOfBytesRead, ref NativeOverlapped lpOverlapped);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean WriteFile(FileSafeHandle hFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, ref UInt32 lpNumberOfBytesWritten, ref NativeOverlapped lpOverlapped);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean WaitNamedPipe(String lpNamedPipeName, UInt32 nTimeOut);

#pragma warning restore IDE0049 // Simplify Names
        }

    }

}


namespace Medo.Application {

    /// <summary>
    /// Arguments for newly detected application instance.
    /// </summary>
    [System.Serializable()]
    public class NewInstanceEventArgs : System.EventArgs {
        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="commandLine">Command line.</param>
        /// <param name="commandLineArgs">String array containing the command line arguments.</param>
        public NewInstanceEventArgs(string commandLine, string[] commandLineArgs) {
            CommandLine = commandLine;
            _commandLineArgs = commandLineArgs;
        }

        /// <summary>
        /// Gets the command line.
        /// </summary>
        public string CommandLine { get; private set; }

        private readonly string[] _commandLineArgs;
        /// <summary>
        /// Returns a string array containing the command line arguments.
        /// </summary>
        public string[] GetCommandLineArgs() {
            return _commandLineArgs;
        }

    }
}
