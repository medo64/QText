using System;

namespace QText {
    public class DocumentFolderEventArgs : EventArgs {

        public DocumentFolderEventArgs(DocumentFolder folder) {
            this.Folder = folder;
        }

        public DocumentFolder Folder { get; private set; }

    }
}
