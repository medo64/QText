using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace QText {
    public class Document {

        public Document(string path) {
            try {
                this.RootPath = Path.GetFullPath(path);

                this.Folders.Add(new DocumentFolder(this, string.Empty));
                foreach (var directory in Directory.GetDirectories(this.RootPath)) {
                    this.Folders.Add(new DocumentFolder(this, Path.GetFileName(directory)));
                }
                this.SortFolders();

                foreach (var folder in this.Folders) {
                    foreach (var extension in FileExtensions.All) {
                        foreach (var fileName in Directory.GetFiles(folder.FullPath, "*" + extension, SearchOption.TopDirectoryOnly)) {
                            if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) { //need this check because *.txt matches A.txt2 too.
                                this.Files.Add(new DocumentFile(folder, Path.GetFileName(fileName)));
                            }
                        }
                    }
                }

                var orderFilePath = Path.Combine(this.RootPath, ".qtext");
                string[] lines = new string[] { };
                try {
                    if (File.Exists(orderFilePath)) {
                        lines = File.ReadAllLines(orderFilePath, Document.Encoding);
                    }
                } catch (IOException) {
                } catch (UnauthorizedAccessException) { }

                var selectedFiles = new List<DocumentFile>();
                this.Files.Sort(delegate (DocumentFile file1, DocumentFile file2) {
                    var index1 = Array.IndexOf(lines, file1.Folder.Name + "|" + file1.Name + "|");
                    if (index1 < 0) {
                        index1 = Array.IndexOf(lines, file1.Folder.Name + "|" + file1.Name + "|*");
                        if ((index1 >= 0) && !selectedFiles.Contains(file1)) {
                            selectedFiles.Add(file1);
                        }
                    }

                    var index2 = Array.IndexOf(lines, file2.Folder.Name + "|" + file2.Name + "|");
                    if (index2 < 0) {
                        index2 = Array.IndexOf(lines, file2.Folder.Name + "|" + file2.Name + "|*");
                        if ((index2 >= 0) && !selectedFiles.Contains(file2)) {
                            selectedFiles.Add(file2);
                        }
                    }

                    if ((index1 < 0) && (index2 < 0)) {
                        return string.Compare(file1.Title, file2.Title, StringComparison.OrdinalIgnoreCase);
                    } else if (index1 < 0) {
                        return -1;
                    } else if (index2 < 0) {
                        return +1;
                    } else {
                        return (index1 < index2) ? -1 : +1;
                    }
                });
                foreach (var selectedFile in selectedFiles) {
                    selectedFile.Selected = true;
                }

                this.Watcher = new FileSystemWatcher(this.RootPath) { IncludeSubdirectories = true, InternalBufferSize = 32768 };
                this.Watcher.Changed += Watcher_Changed;
                this.Watcher.Created += Watcher_Created;
                this.Watcher.Deleted += Watcher_Deleted;
                this.Watcher.Renamed += Watcher_Renamed;

                this.EnableWatcher();
            } catch (DirectoryNotFoundException) {
                throw; //just rethrow it
            } catch (NotSupportedException) {
                throw; //just rethrow it
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }


        /// <summary>
        /// Gets root path.
        /// </summary>
        public String RootPath { get; private set; }

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

        #endregion


        #region Carbon copy

        private String _carbonCopyRootPath;
        /// <summary>
        /// Gets root path for carbon copy.
        /// Can be empty.
        /// </summary>
        public String CarbonCopyRootPath {
            get {
                return this._carbonCopyRootPath;
            }
            set {
                if (value == null) {
                    this._carbonCopyRootPath = value;
                } else {
                    var path = Path.GetFullPath(value);
                    if (value.StartsWith(this.RootPath, StringComparison.OrdinalIgnoreCase)) {
                        this._carbonCopyRootPath = null; //cannot be subfolder of root directory
                    } else {
                        this._carbonCopyRootPath = path;
                    }
                }
            }
        }

        /// <summary>
        /// Gets/sets if carbon copy errors are to be ignored.
        /// </summary>
        public bool CarbonCopyIgnoreErrors { get; set; }


        /// <summary>
        /// Writes all carbon copies.
        /// </summary>
        public void WriteAllCarbonCopies() {
            if (this.CarbonCopyRootPath != null) {
                foreach (var folder in this.GetFolders()) {
                    foreach (var file in folder.GetFiles()) {
                        file.WriteCarbonCopy();
                    }
                }
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


        void Watcher_Changed(object sender, FileSystemEventArgs e) {
            Debug.WriteLine("FileSystemWatcher.Changed: " + e.FullPath);
            if (File.Exists(e.FullPath)) { //file - ignore directory changes
                foreach (var file in this.Files) {
                    if (string.Equals(file.FullPath, e.FullPath, StringComparison.OrdinalIgnoreCase)) {
                        this.OnFileExternallyChanged(new DocumentFileEventArgs(file));
                        break;
                    }
                }
            }
        }

        void Watcher_Created(object sender, FileSystemEventArgs e) {
            Debug.WriteLine("FileSystemWatcher.Created: " + e.FullPath);
            if (Directory.Exists(e.FullPath)) { //directory

                var folder = new DocumentFolder(this, e.Name);
                this.Folders.Add(folder);
                this.SortFolders();
                this.OnExternallyChanged(new EventArgs());

            } else if (File.Exists(e.FullPath)) { //file

                var directoryPath = Path.GetDirectoryName(e.FullPath);
                foreach (var folder in this.Folders) {
                    if (folder.FullPath.Equals(directoryPath, StringComparison.OrdinalIgnoreCase)) {
                        var newExtension = Path.GetExtension(e.Name);
                        var newName = Path.GetFileNameWithoutExtension(e.Name);

                        DocumentFile newFile;
                        if (newExtension.Equals(FileExtensions.PlainText, StringComparison.OrdinalIgnoreCase)) {
                            newFile = new DocumentFile(folder, newName + newExtension);
                        } else if (newExtension.Equals(FileExtensions.RichText, StringComparison.OrdinalIgnoreCase)) {
                            newFile = new DocumentFile(folder, newName + newExtension);
                        } else if (newExtension.Equals(FileExtensions.EncryptedPlainText, StringComparison.OrdinalIgnoreCase)) {
                            newFile = new DocumentFile(folder, newName + newExtension);
                        } else if (newExtension.Equals(FileExtensions.EncryptedRichText, StringComparison.OrdinalIgnoreCase)) {
                            newFile = new DocumentFile(folder, newName + newExtension);
                        } else {
                            break; //ignore because it has unsupported extension
                        }

                        this.Files.Add(newFile);

                        this.OnFolderExternallyChanged(new DocumentFolderEventArgs(folder));
                        break;
                    }
                }

            }
        }

        void Watcher_Deleted(object sender, FileSystemEventArgs e) {
            Debug.WriteLine("FileSystemWatcher.Deleted: " + e.FullPath);
            ProcessDelete(e.FullPath);
            this.OnExternallyChanged(new EventArgs());
        }

        internal void ProcessDelete(string fullPath) {
            for (int i = this.Files.Count - 1; i >= 0; i--) {
                var file = this.Files[i];
                if (string.Equals(file.FullPath, fullPath, StringComparison.OrdinalIgnoreCase) || string.Equals(file.Folder.FullPath, fullPath, StringComparison.OrdinalIgnoreCase)) {
                    this.Files.RemoveAt(i);
                }
            }
            for (int i = 0; i < this.Folders.Count; i++) {
                var folder = this.Folders[i];
                if (!folder.IsRoot && string.Equals(folder.FullPath, fullPath, StringComparison.OrdinalIgnoreCase)) {
                    this.Folders.RemoveAt(i);
                    break;
                }
            }
        }

        void Watcher_Renamed(object sender, RenamedEventArgs e) {
            Debug.WriteLine("FileSystemWatcher.Renamed: " + e.OldFullPath + " -> " + e.FullPath);
            if (Directory.Exists(e.FullPath)) { //directory

                for (int i = 0; i < this.Folders.Count; i++) {
                    var folder = this.Folders[i];
                    if (!folder.IsRoot && string.Equals(folder.FullPath, e.OldFullPath, StringComparison.OrdinalIgnoreCase)) {
                        folder.Name = e.Name;
                        this.SortFolders();
                        this.OnFolderChanged(new DocumentFolderEventArgs(folder));
                        break;
                    }
                }

            } else if (File.Exists(e.FullPath)) { //file

                for (int i = 0; i < this.Files.Count; i++) {
                    var file = this.Files[i];
                    var oldName = file.Name;
                    var oldKind = file.Style;
                    var oldFolder = file.Folder;

                    if (string.Equals(file.FullPath, e.OldFullPath, StringComparison.OrdinalIgnoreCase)) {
                        var newExtension = Path.GetExtension(e.Name);
                        var newName = Path.GetFileNameWithoutExtension(e.Name);

                        if (!newExtension.Equals(file.Extension, StringComparison.OrdinalIgnoreCase)) { //extension has changed
                            if (newExtension.Equals(FileExtensions.PlainText, StringComparison.OrdinalIgnoreCase)) {
                                file.Kind = DocumentKind.PlainText;
                            } else if (newExtension.Equals(FileExtensions.RichText, StringComparison.OrdinalIgnoreCase)) {
                                file.Kind = DocumentKind.RichText;
                            } else if (newExtension.Equals(FileExtensions.EncryptedPlainText, StringComparison.OrdinalIgnoreCase)) {
                                file.Kind = DocumentKind.EncryptedPlainText;
                            } else if (newExtension.Equals(FileExtensions.EncryptedRichText, StringComparison.OrdinalIgnoreCase)) {
                                file.Kind = DocumentKind.EncryptedRichText;
                            } else {
                                this.Files.RemoveAt(i); //remove because it has unsupported extension
                                break;
                            }
                        }

                        if (!newName.Equals(file.Name, StringComparison.OrdinalIgnoreCase)) {
                            file.Name = newName;
                        }

                        this.OnFileExternallyChanged(new DocumentFileEventArgs(file));
                        return;
                    }
                }

                var directoryPath = Path.GetDirectoryName(e.FullPath);
                foreach (var folder in this.Folders) {
                    if (string.Equals(folder.FullPath, directoryPath, StringComparison.OrdinalIgnoreCase)) {
                        Watcher_Created(null, e);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Change affecting all entries has occurred.
        /// Includes folder deletion and creation.
        /// </summary>
        public event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Raises Changed event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal void OnChanged(EventArgs e) {
            var eh = this.Changed;
            if (eh != null) { eh.Invoke(this, e); }
        }

        /// <summary>
        /// Change affecting all entries has occurred.
        /// Includes folder deletion and creation.
        /// </summary>
        public event EventHandler<EventArgs> ExternallyChanged;

        /// <summary>
        /// Raises ExternallyChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnExternallyChanged(EventArgs e) {
            var eh = this.ExternallyChanged;
            if (eh != null) { eh.Invoke(this, e); }
            this.OnChanged(e);
        }


        /// <summary>
        /// Folder change has occurred.
        /// Includes folder renames and file creation and deletion.
        /// </summary>
        public event EventHandler<DocumentFolderEventArgs> FolderChanged;

        /// <summary>
        /// Raises FolderChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal void OnFolderChanged(DocumentFolderEventArgs e) {
            var eh = this.FolderChanged;
            if (eh != null) { eh.Invoke(this, e); }
        }

        /// <summary>
        /// Folder change has occurred.
        /// Includes folder renames and file creation and deletion.
        /// </summary>
        public event EventHandler<DocumentFolderEventArgs> FolderExternallyChanged;

        /// <summary>
        /// Raises FolderExternallyChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnFolderExternallyChanged(DocumentFolderEventArgs e) {
            var eh = this.FolderExternallyChanged;
            if (eh != null) { eh.Invoke(this, e); }
            this.OnFolderChanged(e);
        }


        /// <summary>
        /// File change has occurred.
        /// Includes file renames, style changes, and encryption/decryption.
        /// </summary>
        public event EventHandler<DocumentFileEventArgs> FileChanged;

        /// <summary>
        /// Raises FileChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal void OnFileChanged(DocumentFileEventArgs e) {
            var eh = this.FileChanged;
            if (eh != null) { eh.Invoke(this, e); }
        }

        /// <summary>
        /// File change has occurred.
        /// Includes file renames, style changes, and encryption/decryption.
        /// </summary>
        public event EventHandler<DocumentFileEventArgs> FileExternallyChanged;

        /// <summary>
        /// Raises FileExternallyChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnFileExternallyChanged(DocumentFileEventArgs e) {
            var eh = this.FileExternallyChanged;
            if (eh != null) { eh.Invoke(this, e); }
            this.OnFileChanged(e);
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
                var newPath = Path.Combine(this.RootPath, Helper.EncodeTitle("New folder"));
                int n = 1;
                while (Directory.Exists(newPath)) {
                    var newTitle = string.Format(CultureInfo.CurrentUICulture, "New folder ({0})", n);
                    newPath = Path.Combine(this.RootPath, Helper.EncodeTitle(newTitle));
                    n += 1;
                }

                Debug.WriteLine("Create: " + Path.GetFileName(newPath));

                using (var watcher = new Helper.FileSystemToggler(this.Watcher)) {
                    Directory.CreateDirectory(newPath);
                }

                var folder = new DocumentFolder(this, Path.GetFileName(newPath));
                this.Folders.Add(folder);
                this.SortFolders();

                this.OnChanged(new EventArgs());
                return folder;
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public DocumentFolder CreateFolder(string title) {
            try {
                var newPath = Path.Combine(this.RootPath, Helper.EncodeTitle(title));

                Debug.WriteLine("Create: " + Path.GetFileName(newPath));

                using (var watcher = new Helper.FileSystemToggler(this.Watcher)) {
                    Directory.CreateDirectory(newPath);
                }

                var folder = new DocumentFolder(this, Path.GetFileName(newPath));
                this.Folders.Add(folder);
                this.SortFolders();

                this.OnChanged(new EventArgs());
                return folder;
            } catch (Exception ex) {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }


        internal bool CanNewFile(DocumentFolder folder, string title) {
            foreach (var file in folder.GetFiles()) {
                if (string.Equals(file.Title, title, StringComparison.OrdinalIgnoreCase)) { return false; }
            }
            return true;
        }

        internal DocumentFile NewFile(DocumentFolder folder, string title, DocumentKind kind) {
            var name = Helper.EncodeTitle(title);

            DocumentFile newFile;
            switch (kind) {
                case DocumentKind.PlainText: newFile = new DocumentFile(folder, name + FileExtensions.PlainText); break;
                case DocumentKind.RichText: newFile = new DocumentFile(folder, name + FileExtensions.RichText); break;
                default: throw new InvalidOperationException("Unrecognized file kind.");
            }

            Debug.WriteLine("File.New: " + name);
            using (var watcher = new Helper.FileSystemToggler(this.Watcher)) {
                File.WriteAllText(newFile.FullPath, "");
                this.Files.Add(newFile);
            }

            this.OnFolderChanged(new DocumentFolderEventArgs(newFile.Folder));
            return newFile;
        }

        #endregion


        #region Order

        internal void OrderBefore(DocumentFile file, DocumentFile pivotFile) {
            var index = this.Files.IndexOf(file);
            var indexPivot = (pivotFile == null) ? this.Files.Count : this.Files.IndexOf(pivotFile);
            if (index != indexPivot) {
                this.Files.RemoveAt(index);
                this.Files.Insert((index < indexPivot) ? indexPivot - 1 : indexPivot, file);
                this.WriteOrder();
            }
        }

        internal void OrderAfter(DocumentFile file, DocumentFile pivotFile) {
            var index = this.Files.IndexOf(file);
            var indexPivot = (pivotFile == null) ? -1 : this.Files.IndexOf(pivotFile);
            if (index != indexPivot) {
                this.Files.RemoveAt(index);
                this.Files.Insert((index > indexPivot) ? indexPivot + 1 : indexPivot, file);
                this.WriteOrder();
            }
        }

        #endregion


        #region Write metadata

        private static readonly UTF8Encoding Encoding = new UTF8Encoding(false);
        private string LastOrderText = null;

        /// <summary>
        /// Writes ordering information.
        /// </summary>
        internal void WriteOrder() {
            var sb = new StringBuilder(65536);
            foreach (var file in this.Files) {
                sb.AppendLine(file.Folder.Name + "|" + file.Name + "|" + (file.Selected ? "*" : ""));
            }
            var orderText = sb.ToString();

            if (this.LastOrderText != orderText) {
                this.LastOrderText = orderText;
                try {
                    var orderFilePath = Path.Combine(this.RootPath, ".qtext");
                    var attributes = FileAttributes.Normal;
                    if (File.Exists(orderFilePath)) {
                        attributes = File.GetAttributes(orderFilePath);
                        File.SetAttributes(orderFilePath, attributes & ~FileAttributes.Hidden);
                    }
                    File.WriteAllText(orderFilePath, orderText, Document.Encoding);
                    File.SetAttributes(orderFilePath, attributes | FileAttributes.Hidden);
                } catch (IOException) {
                } catch (UnauthorizedAccessException) { }
            }
        }

        #endregion

    }
}
