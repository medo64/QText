using System;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace QText {
    internal static class SHFile {

        /// <summary>
        /// Deletes specified file.
        /// </summary>
        /// <param name="path">The name of file to be deleted.</param>
        public static void Delete(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("Cannot delete " + path + ": Cannot find the specified file.");
            }

            var fileOp = new NativeMethods.SHFILEOPSTRUCTW();
            fileOp.hwnd = IntPtr.Zero;
            fileOp.wFunc = NativeMethods.FO_DELETE;
            fileOp.pFrom = path + "\0";
            fileOp.pTo = "\0";
            fileOp.fFlags = NativeMethods.FOF_NOCONFIRMATION | NativeMethods.FOF_ALLOWUNDO;
            fileOp.lpszProgressTitle = "\0";
            if (NativeMethods.SHFileOperation(ref fileOp) != 0) {
                throw new Win32Exception();
            }
        }

        public static void DeleteDirectory(string path) {
            if (!Directory.Exists(path)) {
                throw new FileNotFoundException("Cannot delete " + path + ": Cannot find the specified file.");
            }

            var fileOp = new NativeMethods.SHFILEOPSTRUCTW();
            fileOp.hwnd = IntPtr.Zero;
            fileOp.wFunc = NativeMethods.FO_DELETE;
            fileOp.pFrom = path + "\0";
            fileOp.pTo = "\0";
            fileOp.fFlags = NativeMethods.FOF_NOCONFIRMATION | NativeMethods.FOF_ALLOWUNDO;
            fileOp.lpszProgressTitle = "\0";
            if (NativeMethods.SHFileOperation(ref fileOp) != 0) {
                throw new Win32Exception();
            }
        }


        private class NativeMethods {

            private NativeMethods() {
            }


            internal const int FO_COPY = 0x2;              // Copy File/Folder
            internal const int FO_DELETE = 0x3;            // Delete File/Folder
            internal const int FO_MOVE = 0x1;              // Move File/Folder
            internal const int FO_RENAME = 0x4;            // Rename File/Folder
            internal const int FOF_ALLOWUNDO = 0x40;       // Allow to undo rename, delete ie sends to recycle bin
            internal const int FOF_FILESONLY = 0x80;       // Only allow files
            internal const int FOF_NOCONFIRMATION = 0x10;  // No File Delete or Overwrite Confirmation Dialog
            internal const int FOF_SILENT = 0x4;           // No copy/move dialog
            internal const int FOF_SIMPLEPROGRESS = 0x100; // Does not display file names


            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct SHFILEOPSTRUCTW {
                public IntPtr hwnd;
                public uint wFunc;
                [MarshalAsAttribute(UnmanagedType.LPWStr)]
                public string pFrom;
                [MarshalAsAttribute(UnmanagedType.LPWStr)]
                public string pTo;
                public ushort fFlags;
                public int fAnyOperationsAborted;
                public IntPtr hNameMappings;
                [MarshalAsAttribute(UnmanagedType.LPWStr)]
                public string lpszProgressTitle;
            }

            [DllImportAttribute("shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int SHFileOperation(ref SHFILEOPSTRUCTW lpFileOp);

        }

    }
}
