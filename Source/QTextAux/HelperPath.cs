using System;
using System.Collections.Generic;
using System.IO;

namespace QTextAux {
    namespace Helper {

        public static class Path {

            public static void Create(string path) {
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

        }

    }
}
