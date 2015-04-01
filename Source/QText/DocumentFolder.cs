using System;
using System.IO;

namespace QText {
    internal class DocumentFolder {

        public DocumentFolder(DirectoryInfo directory, string name, string title) {
            this.Directory = directory;
            this.Name = name;
            this.Title = title;
        }


        /// <summary>
        /// Gets directory.
        /// </summary>
        public DirectoryInfo Directory { get; private set; }

        /// <summary>
        /// Gets name of folder - for internal use.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets title to display to user.
        /// </summary>
        public string Title { get; private set; }

    }
}
