using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace QText {
    public class Document {

        public Document(string path) {
            this.RootDirectory = new DirectoryInfo(path);

            this.Folders.Add(new DocumentFolder(this, string.Empty));
            foreach (var directory in this.RootDirectory.GetDirectories()) {
                this.Folders.Add(new DocumentFolder(this, directory.Name));
            }
            this.SortFolders();

            foreach (var folder in this.Folders) {
                foreach (var extension in FileExtensions.All) {
                    foreach (var fileName in Directory.GetFiles(folder.FullPath, "*" + extension, SearchOption.TopDirectoryOnly)) {
                        this.Files.Add(new DocumentFile(folder, Path.GetFileName(fileName)));
                    }
                }
            }
            this.Files.Sort(delegate (DocumentFile file1, DocumentFile file2) {
                return string.Compare(file1.Title, file2.Title, StringComparison.OrdinalIgnoreCase);
            });

            this.Watcher = new FileSystemWatcher(this.RootDirectory.FullName) { IncludeSubdirectories = true, InternalBufferSize = 32768 };
            this.Watcher.Changed += Watcher_Changed;
            this.Watcher.Created += Watcher_Created;
            this.Watcher.Deleted += Watcher_Deleted;
            this.Watcher.Renamed += Watcher_Renamed;
        }


        /// <summary>
        /// Gets root directory.
        /// </summary>
        public DirectoryInfo RootDirectory { get; private set; }

        /// <summary>
        /// Gets/sets if file deletion is going to be performed to recycle bin
        /// </summary>
        public bool DeleteToRecycleBin { get; set; }


        #region Folders

        private readonly List<DocumentFolder> Folders = new List<DocumentFolder>();

        internal void SortFolders() {
            this.Folders.Sort(delegate (DocumentFolder folder1, DocumentFolder folder2) {
                if (string.IsNullOrEmpty(folder1.Name) == string.IsNullOrEmpty(folder2.Name)) {
                    return string.Compare(folder1.Title, folder2.Title, StringComparison.OrdinalIgnoreCase);
                } else {
                    return string.IsNullOrEmpty(folder1.Name) ? -1 : +1;
                }
            });
        }

        #endregion


        #region Files

        private readonly List<DocumentFile> Files = new List<DocumentFile>();

        internal IEnumerable<DocumentFile> GetFiles(DocumentFolder folder) {
            foreach (var file in this.Files) {
                if (file.Folder.Equals(folder)) {
                    yield return file;
                }
            }
        }

        internal IEnumerable<DocumentFile> GetAllFiles() {
            foreach (var file in this.Files) {
                yield return file;
            }
        }

        #endregion


        #region Watcher

        internal readonly FileSystemWatcher Watcher;


        public void DisableWatcher() {
            this.Watcher.EnableRaisingEvents = false;
        }

        public void EnableWatcher() {
            this.Watcher.EnableRaisingEvents = true;
        }


        public event EventHandler<FileSystemEventArgs> Changed;

        /// <summary>
        /// Raises Changed event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnChanged(FileSystemEventArgs e) {
            var eh = this.Changed;
            if (eh != null) { eh.Invoke(this, e); }
        }


        void Watcher_Changed(object sender, FileSystemEventArgs e) {
            Debug.WriteLine("FileSystemWatcher.Changed: " + e.FullPath);
            if (File.Exists(e.FullPath)) { //file - ignore directory changes
                this.OnChanged(e);
            }
        }

        void Watcher_Created(object sender, FileSystemEventArgs e) {
            Debug.WriteLine("FileSystemWatcher.Created: " + e.FullPath);
            if (Directory.Exists(e.FullPath)) { //directory
                var folder = new DocumentFolder(this, e.Name);
                this.Folders.Add(folder);
                this.SortFolders();
                this.OnFolderChanged(new DocumentFolderEventArgs(folder));
                this.OnChanged(e);
            } else if (File.Exists(e.FullPath)) { //file
                this.OnChanged(e);
            }
        }

        void Watcher_Deleted(object sender, FileSystemEventArgs e) {
            Debug.WriteLine("FileSystemWatcher.Deleted: " + e.FullPath);
            ProcessFolderDelete(e.FullPath);
            this.OnChanged(e);
        }

        internal void ProcessFolderDelete(string fullPath) {
            for (int i = 0; i < this.Files.Count; i++) {
                var file = this.Files[i];
                if (string.Equals(file.Folder.FullPath, fullPath, StringComparison.OrdinalIgnoreCase)) {
                    this.Files.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < this.Folders.Count; i++) {
                var folder = this.Folders[i];
                if (!folder.IsRoot && string.Equals(folder.FullPath, fullPath, StringComparison.OrdinalIgnoreCase)) {
                    this.Folders.RemoveAt(i);
                    break;
                }
            }
            this.OnDocumentChanged(new EventArgs());
        }

        void Watcher_Renamed(object sender, RenamedEventArgs e) {
            Debug.WriteLine("FileSystemWatcher.Renamed: " + e.OldFullPath + " -> " + e.FullPath);
            if (Directory.Exists(e.FullPath)) { //directory
                for (int i = 0; i < this.Folders.Count; i++) {
                    var folder = this.Folders[i];
                    if (!folder.IsRoot && string.Equals(folder.FullPath, e.OldFullPath, StringComparison.OrdinalIgnoreCase)) {
                        folder.Name = e.Name;
                        this.SortFolders();
                        this.OnChanged(new FileSystemEventArgs(WatcherChangeTypes.Renamed, Path.GetDirectoryName(e.FullPath), Path.GetFileName(e.FullPath)));
                        this.OnFolderChanged(new DocumentFolderEventArgs(folder));
                        break;
                    }
                }
            } else if (File.Exists(e.FullPath)) { //file
                this.OnChanged(new FileSystemEventArgs(WatcherChangeTypes.Renamed, Path.GetDirectoryName(e.FullPath), Path.GetFileName(e.FullPath)));
            }
        }


        /// <summary>
        /// Change affecting all entries has occurred, e.g. folder deletion.
        /// </summary>
        public event EventHandler<EventArgs> DocumentChanged;

        /// <summary>
        /// Raises FolderChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnDocumentChanged(EventArgs e) {
            var eh = this.DocumentChanged;
            if (eh != null) { eh.Invoke(this, e); }
        }


        /// <summary>
        /// Folder change has occurred, e.g. folder creation or rename.
        /// </summary>
        public event EventHandler<DocumentFolderEventArgs> FolderChanged;

        /// <summary>
        /// Raises FolderChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnFolderChanged(DocumentFolderEventArgs e) {
            var eh = this.FolderChanged;
            if (eh != null) { eh.Invoke(this, e); }
        }

        #endregion


        #region Enumerate

        public DocumentFolder RootFolder {
            get {
                return this.Folders[0];
            }
        }

        public IEnumerable<DocumentFolder> GetFolders() {
            foreach (var folder in this.Folders) {
                yield return folder;
            }
        }

        public IEnumerable<DocumentFolder> GetSubFolders() {
            foreach (var folder in this.Folders) {
                if (!folder.IsRoot) { yield return folder; }
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


        #region New

        public DocumentFolder CreateFolder() {
            try {
                var newPath = Path.Combine(this.RootDirectory.FullName, Helper.EncodeTitle("New folder"));
                int n = 1;
                while (Directory.Exists(newPath)) {
                    var newTitle = string.Format(CultureInfo.CurrentUICulture, "New folder ({0})", n);
                    newPath = Path.Combine(this.RootDirectory.FullName, Helper.EncodeTitle(newTitle));
                    n += 1;
                }

                Debug.WriteLine("Create: " + Path.GetFileName(newPath));

                using (var watcher = new Helper.FileSystemToggler(this.Watcher)) {
                    Directory.CreateDirectory(newPath);
                }

                var folder = new DocumentFolder(this, Path.GetFileName(newPath));
                this.Folders.Add(folder);
                this.SortFolders();
                return folder;
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public DocumentFolder CreateFolder(string title) {
            try {
                var newPath = Path.Combine(this.RootDirectory.FullName, Helper.EncodeTitle(title));

                Debug.WriteLine("Create: " + Path.GetFileName(newPath));

                using (var watcher = new Helper.FileSystemToggler(this.Watcher)) {
                    Directory.CreateDirectory(newPath);
                }

                var folder = new DocumentFolder(this, Path.GetFileName(newPath));
                this.Folders.Add(folder);
                this.SortFolders();
                return folder;
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion


        #region Write metadata

        /// <summary>
        /// Writes ordering information.
        /// </summary>
        internal void WriteOrder() {
            try {
                var sb = new StringBuilder();
                foreach (var file in this.Files) {
                    sb.AppendLine(file.Folder.Name + "|" + file.Name);
                }
                File.WriteAllText(Path.Combine(this.RootDirectory.FullName, ".qtext"), sb.ToString());
            } catch (IOException) {
            }
        }

        #endregion

    }
}
