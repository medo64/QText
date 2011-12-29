using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace QText {

    internal static class Helper {

        public static void CreatePath(string path) {
            if ((!Directory.Exists(path))) {
                string currPath = path;
                var allPaths = new List<string>();
                while (!(Directory.Exists(currPath))) {
                    allPaths.Add(currPath);
                    currPath = System.IO.Path.GetDirectoryName(currPath);
                    if (string.IsNullOrEmpty(currPath)) {
                        throw new IOException("Path \"" + path + "\" can not be created.");
                    }
                }

                try {
                    for (int i = allPaths.Count - 1; i >= 0; i += -1) {
                        System.IO.Directory.CreateDirectory(allPaths[i]);
                    }
                } catch (Exception) {
                    throw new System.IO.IOException("Path \"" + path + "\" can not be created.");
                }
            }
        }

        public static void MovePath(string currentPath, string newPath) {
            if (currentPath.StartsWith(@"\\?\", StringComparison.Ordinal) == false) { currentPath = @"\\?\" + currentPath; }
            if (newPath.StartsWith(@"\\?\", StringComparison.Ordinal) == false) { newPath = @"\\?\" + newPath; }
            if (NativeMethods.MoveFileExW(currentPath, newPath, NativeMethods.MOVEFILE_COPY_ALLOWED | NativeMethods.MOVEFILE_WRITE_THROUGH) ==  false) {
                var ex = new Win32Exception();
                throw new IOException(ex.Message, ex);
            }
        }


        internal static class NativeMethods {

            public const uint MOVEFILE_COPY_ALLOWED = 0x02;
            public const uint MOVEFILE_WRITE_THROUGH = 0x08;


            [DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError=true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool MoveFileExW([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpExistingFileName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpNewFileName, uint dwFlags);

        }

    }
}
