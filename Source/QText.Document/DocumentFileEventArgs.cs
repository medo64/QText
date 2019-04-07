using System;

namespace QText {
    public class DocumentFileEventArgs : EventArgs {

        public DocumentFileEventArgs(DocumentFile file) {
            File = file;
        }

        public DocumentFile File { get; private set; }

    }
}
