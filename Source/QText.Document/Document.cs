using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace QText {
    public class Document {

        public Document(DirectoryInfo rootDirectory) {
            this.RootDirectory = rootDirectory;

            this.Watcher = new FileSystemWatcher(rootDirectory.FullName) { IncludeSubdirectories = true, InternalBufferSize = 32768 };
            this.Watcher.Changed += delegate(object sender, FileSystemEventArgs e) { this.OnChanged(e); };
            this.Watcher.Created += delegate(object sender, FileSystemEventArgs e) { this.OnChanged(e); };
            this.Watcher.Deleted += delegate(object sender, FileSystemEventArgs e) { this.OnChanged(e); };
            this.Watcher.Renamed += delegate(object sender, RenamedEventArgs e) { this.OnChanged(new FileSystemEventArgs(WatcherChangeTypes.Renamed, Path.GetDirectoryName(e.FullPath), Path.GetFileName(e.FullPath))); };
        }

        public DirectoryInfo RootDirectory { get; private set; }

        public bool DeleteToRecycleBin { get; set; }


        #region Watcher

        private FileSystemWatcher Watcher;


        public event EventHandler<FileSystemEventArgs> Changed;

        /// <summary>
        /// Raises Changed event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnChanged(FileSystemEventArgs e) {
            var eh = this.Changed;
            if (eh != null) { eh.Invoke(this, e); }
        }

        public void DisableWatcher() {
            this.Watcher.EnableRaisingEvents = false;
        }

        public void EnableWatcher() {
            this.Watcher.EnableRaisingEvents = true;
        }

        #endregion


        #region Enumerate

        public DocumentFolder GetRootFolder() {
            var path = new DirectoryInfo(this.RootDirectory.FullName);
            return new DocumentFolder(this, path, string.Empty);
        }

        public IEnumerable<DocumentFolder> GetFolders() {
            yield return this.GetRootFolder();
            foreach (var folder in this.GetSubFolders()) {
                yield return folder;
            }
        }

        public IEnumerable<DocumentFolder> GetSubFolders() {
            var root = this.GetRootFolder().Info;

            var directories = new List<DirectoryInfo>();
            foreach (var directory in root.GetDirectories()) {
                directories.Add(directory);
            }
            directories.Sort(delegate(DirectoryInfo item1, DirectoryInfo item2) {
                return string.Compare(item1.Name, item2.Name);
            });
            foreach (var directory in directories) {
                yield return new DocumentFolder(this, directory, directory.Name);
            }
        }


        public DocumentFolder GetFolder(string name) {
            foreach (var folder in this.GetFolders()) {
                if (folder.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    return folder;
                }
            }
            return null;
        }

        #endregion


        #region New folder

        public DocumentFolder NewFolder() {
            try {
                var newTitle = "New folder";
                var newPath = Path.Combine(this.RootDirectory.FullName, Helper.EncodeFileName(newTitle));
                if (Directory.Exists(newPath)) {
                    int n = 1;
                    while (true) {
                        newTitle = string.Format(CultureInfo.CurrentUICulture, "New folder ({0})", n);
                        newPath = Path.Combine(this.RootDirectory.FullName, Helper.EncodeFileName(newTitle));
                        if (!Directory.Exists(newPath)) { break; } //this folder is available
                        n += 1;
                    }
                }

                Directory.CreateDirectory(newPath);

                return new DocumentFolder(this, new DirectoryInfo(newPath), newTitle);
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion


        public IEnumerable<DocumentFile> GetTitles(DocumentFolder folder) {
            return folder.GetFiles();
        }


        //public static IEnumerable<String> GetFilePaths(DocumentFolder folder) {
        //    var files = new List<string>();
        //    foreach (var extension in QFileInfo.GetExtensions()) {
        //        files.AddRange(Directory.GetFiles(folder.Directory.FullName, "*" + extension));
        //    }

        //    string selectedTitle = null;
        //    var orderedTitles = ReadOrderedTitles(folder, out selectedTitle);
        //    files.Sort(delegate(string file1, string file2) {
        //        var title1 = Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file1));
        //        var title2 = Helper.DecodeFileName(QFileInfo.GetFileNameWithoutExtension(file2));
        //        if (orderedTitles != null) {
        //            var titleIndex1 = orderedTitles.IndexOf(title1);
        //            var titleIndex2 = orderedTitles.IndexOf(title2);
        //            if ((titleIndex1 != -1) && (titleIndex2 != -1)) { //both are ordered
        //                return (titleIndex1 < titleIndex2) ? -1 : 1;
        //            } else if (titleIndex1 != -1) { //first one is ordered
        //                return -1;
        //            } else if (titleIndex2 != -1) { //second one is ordered 
        //                return 1;
        //            }
        //        }
        //        return string.Compare(title1, title2); //just sort alphabetically
        //    });

        //    foreach (var file in files) {
        //        yield return file;
        //    }
        //}

    }
}
