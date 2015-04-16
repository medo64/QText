using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace QText {
    public class Document {

        public Document(string path) {
            this.RootDirectory = new DirectoryInfo(path);

            this.Folders.Add(new DocumentFolder(this, string.Empty));
            foreach (var directory in this.RootDirectory.GetDirectories()) {
                this.Folders.Add(new DocumentFolder(this, directory.Name));
            }
            this.SortFolder();

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

        private void SortFolder() {
            this.Folders.Sort(delegate(DocumentFolder folder1, DocumentFolder folder2) {
                if (string.IsNullOrEmpty(folder1.Name) == string.IsNullOrEmpty(folder2.Name)) {
                    return string.Compare(folder1.Title, folder2.Title, StringComparison.OrdinalIgnoreCase);
                } else {
                    return string.IsNullOrEmpty(folder1.Name) ? -1 : +1;
                }
            });
        }

        #endregion


        #region Watcher

        private FileSystemWatcher Watcher;


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
            if (File.Exists(e.FullPath)) { //file - ignore directory changes
                this.OnChanged(e);
            }
        }

        void Watcher_Created(object sender, FileSystemEventArgs e) {
            if (Directory.Exists(e.FullPath)) { //directory
                var folder = new DocumentFolder(this, e.Name);
                this.Folders.Add(folder);
                this.SortFolder();
                this.OnFolderChanged(new DocumentFolderEventArgs(folder));
                this.OnChanged(e);
            } else if (File.Exists(e.FullPath)) { //file
                this.OnChanged(e);
            }
        }

        void Watcher_Deleted(object sender, FileSystemEventArgs e) {
            if (Directory.Exists(e.FullPath)) { //directory
                for (int i = 0; i < this.Folders.Count; i++) {
                    var folder = this.Folders[i];
                    if (!folder.IsRoot && string.Equals(folder.Info.FullName, e.FullPath, StringComparison.OrdinalIgnoreCase)) {
                        this.Folders.RemoveAt(i);
                        folder.Name = e.Name;
                        this.OnDocumentChanged(new EventArgs());
                        this.OnChanged(new FileSystemEventArgs(WatcherChangeTypes.Renamed, Path.GetDirectoryName(e.FullPath), Path.GetFileName(e.FullPath)));
                        break;
                    }
                }
            } else if (File.Exists(e.FullPath)) { //file
                this.OnChanged(e);
            }
        }

        void Watcher_Renamed(object sender, RenamedEventArgs e) {
            if (Directory.Exists(e.FullPath)) { //directory
                for (int i = 0; i < this.Folders.Count; i++) {
                    var folder = this.Folders[i];
                    if (!folder.IsRoot && string.Equals(folder.Info.FullName, e.OldFullPath, StringComparison.OrdinalIgnoreCase)) {
                        folder.Name = e.Name;
                        this.SortFolder();
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


        #region New folder

        public DocumentFolder NewFolder() {
            try {
                var newPath = Path.Combine(this.RootDirectory.FullName, Helper.EncodeFileName("New folder"));
                int n = 1;
                while (Directory.Exists(newPath)) {
                    var newTitle = string.Format(CultureInfo.CurrentUICulture, "New folder ({0})", n);
                    newPath = Path.Combine(this.RootDirectory.FullName, Helper.EncodeFileName(newTitle));
                    n += 1;
                }

                Directory.CreateDirectory(newPath);
                return new DocumentFolder(this, Helper.DecodeFileName(Path.GetFileName(newPath)));
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        #endregion

    }
}
