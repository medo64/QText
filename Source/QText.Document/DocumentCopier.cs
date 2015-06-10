using System;
using System.IO;

namespace QText {
    /// <summary>
    /// Performs copying of all files to another location.
    /// </summary>
    public class DocumentCopier {

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="destinationPath">Destination path.</param>
        /// <exception cref="InvalidOperationException">Cannot copy into the current storage directory tree. -or- Cannot create destination's root direcory.</exception>
        public DocumentCopier(Document document, string destinationPath) {
            this.Document = document;

            var destinationRoot = Path.GetFullPath(destinationPath);
            if (destinationRoot.StartsWith(this.Document.RootPath, StringComparison.OrdinalIgnoreCase)) {
                throw new InvalidOperationException("Cannot copy into the current storage directory tree.");
            }
            if ((this.Document.CarbonCopyRootPath != null) && (destinationRoot.StartsWith(this.Document.CarbonCopyRootPath, StringComparison.OrdinalIgnoreCase))) {
                throw new InvalidOperationException("Cannot copy into the carbon copy directory tree.");
            }
            this.DestinationRootPath = destinationRoot;

            if (Directory.Exists(this.DestinationRootPath)) {
                this.DestinationRootAlreadyExisted = true;
            } else {
                try {
                    Helper.CreatePath(this.DestinationRootPath);
                } catch (Exception ex) {
                    throw new InvalidOperationException("Cannot create destination's root direcory.", ex);
                }
            }

            this.DestinationRootWasEmpty = (Directory.GetDirectories(this.DestinationRootPath).Length == 0) && (Directory.GetFiles(this.DestinationRootPath).Length == 0);
        }

        private readonly Document Document;
        private readonly string DestinationRootPath;

        /// <summary>
        /// Gets if destination root already existed before copier was instantiated.
        /// </summary>
        public bool DestinationRootAlreadyExisted { get; private set; }

        /// <summary>
        /// Gets if destination root was empty when copier was instantiated.
        /// </summary>
        public bool DestinationRootWasEmpty { get; private set; }


        /// <summary>
        /// Copies whole directory structure and returns true if copy was successful.
        /// </summary>
        public bool CopyAll() {
            return this.CopyAll(false);
        }

        /// <summary>
        /// Copies whole directory structure and returns true if copy was successful.
        /// </summary>
        /// <param name="alwaysOverwrite">If true, files will be overwritten without raising the event.</param>
        public bool CopyAll(bool alwaysOverwrite) {
            return CopyDirectory(this.Document.RootPath, this.DestinationRootPath, "", alwaysOverwrite, 0);
        }


        private bool CopyDirectory(string sourcePath, string destinationPath, string relativePath, bool alwaysOverwrite, int level) {
            foreach (var filePath in Directory.GetFiles(sourcePath)) {
                var fileName = Path.GetFileName(filePath);

                var destinationFilePath = Path.Combine(destinationPath, fileName);
                bool canOverwrite = true;
                if (File.Exists(destinationFilePath) && !alwaysOverwrite) {
                    if ((level == 0) && fileName.Equals(".qtext", StringComparison.OrdinalIgnoreCase)) {
                        canOverwrite = false; //if there is a .qtext at destination, leave it be
                    } else {
                        var relativeFilePath = string.IsNullOrEmpty(relativePath) ? fileName : relativePath + "\\" + fileName;
                        var e = new DocumentCopierOverwriteEventArgs(relativeFilePath);
                        this.OnFileOverwrite(e);
                        if (e.Cancel) { return false; }
                        canOverwrite = e.Overwrite;
                    }
                }
                if (canOverwrite) { File.Copy(filePath, destinationFilePath, true); }
            }

            foreach (var directoryPath in Directory.GetDirectories(sourcePath)) {
                var directoryName = Path.GetFileName(directoryPath); //GetDirectoryName would return root directory path

                var destinationDirectoryPath = Path.Combine(destinationPath, directoryName);
                var relativeDirectoryPath = string.IsNullOrEmpty(relativePath) ? directoryName : relativePath + "\\" + directoryName;
                bool canOverwrite = true;
                if (Directory.Exists(destinationDirectoryPath) && !alwaysOverwrite) {
                    var e = new DocumentCopierOverwriteEventArgs(relativeDirectoryPath);
                    this.OnFolderOverwrite(e);
                    if (e.Cancel) { return false; }
                    canOverwrite = !e.Overwrite;
                }
                if (canOverwrite) {
                    Directory.CreateDirectory(destinationDirectoryPath);
                    var cancelled = !CopyDirectory(directoryPath, destinationDirectoryPath, relativeDirectoryPath, alwaysOverwrite, level++); //recurse
                    if (cancelled) { return false; }
                }
            }

            return true;
        }


        /// <summary>
        /// Returns document that uses destination as root directory.
        /// </summary>
        public Document GetDestinationDocument() {
            return new Document(this.DestinationRootPath);
        }


        #region Events

        /// <summary>
        /// Folder overwrite is about to occur.
        /// </summary>
        public event EventHandler<DocumentCopierOverwriteEventArgs> FolderOverwrite;

        /// <summary>
        /// Raises FolderOverwrite event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnFolderOverwrite(DocumentCopierOverwriteEventArgs e) {
            var eh = this.FolderOverwrite;
            if (eh != null) { eh.Invoke(this, e); }
        }


        /// <summary>
        /// File overwrite is about to occur.
        /// </summary>
        public event EventHandler<DocumentCopierOverwriteEventArgs> FileOverwrite;

        /// <summary>
        /// Raises FileOverwrite event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnFileOverwrite(DocumentCopierOverwriteEventArgs e) {
            var eh = this.FileOverwrite;
            if (eh != null) { eh.Invoke(this, e); }
        }

        #endregion

    }
}
