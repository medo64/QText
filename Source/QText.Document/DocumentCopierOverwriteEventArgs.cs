using System;

namespace QText {
    /// <summary>
    /// Event arguments of overwrite events.
    /// </summary>
    public class DocumentCopierOverwriteEventArgs : EventArgs {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="relativePath">Relative path.</param>
        internal DocumentCopierOverwriteEventArgs(string relativePath) {
            RelativePath = relativePath;
            Overwrite = true;
            Cancel = false;
        }


        /// <summary>
        /// Gets relative path of the item.
        /// </summary>
        public string RelativePath { get; private set; }


        /// <summary>
        /// Gets/sets if all events are to be cancelled.
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Gets/sets if event is cancelled.
        /// </summary>
        public bool Cancel { get; set; }

    }
}
