using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QText {
    internal class GotoResult {

        internal static IEnumerable<GotoResult> GetSuggestions(string suggestion, bool allowLineNumbers) {
            if (suggestion.Length == 0) {

                yield return new GotoResult(null, QText.Document.GetRootFolder(), null);

            } else {

                if (allowLineNumbers) {
                    int lineNumber;
                    if (int.TryParse(suggestion, NumberStyles.Integer, CultureInfo.CurrentCulture, out lineNumber)) {
                        if (lineNumber > 0) {
                            yield return new GotoResult(lineNumber, null, null);
                        }
                    }
                }

                foreach (var folder in QText.Document.GetSubFolders()) {
                    if (folder.Name.IndexOf(suggestion, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        yield return new GotoResult(null, folder, null);
                    }
                }

                foreach (var file in QText.Document.GetTitles("")) {
                    if (file.IndexOf(suggestion, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        yield return new GotoResult(null, QText.Document.GetRootFolder(), file);
                    }
                }
                foreach (var folder in QText.Document.GetSubFolders()) {
                    foreach (var file in QText.Document.GetTitles(folder.Name)) {
                        if (file.IndexOf(suggestion, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                            yield return new GotoResult(null, folder, file);
                        }
                    }
                }

            }
        }

        private GotoResult(int? lineNumber, DocumentFolder folder, string document) {
            this.LineNumber = lineNumber;
            this.Folder = folder;
            this.Document = document;
        }


        public Int32? LineNumber { get; private set; }
        public DocumentFolder Folder { get; private set; }
        public String Document { get; private set; }


        public int ImageIndex {
            get {
                if (LineNumber.HasValue) {
                    return -1;
                } else {
                    if (string.IsNullOrEmpty(this.Document)) {
                        return 0;
                    } else {
                        return 1;
                    }
                }
            }
        }

        public bool IsLineNumber {
            get { return this.LineNumber.HasValue; }
        }

        public bool IsDocument {
            get { return !this.IsLineNumber && !string.IsNullOrEmpty(this.Document); }
        }

        public bool IsFolder {
            get { return !this.IsLineNumber && !this.IsDocument && (this.Folder != null); }
        }

        public override string ToString() {
            if (LineNumber.HasValue) {
                return "Line " + LineNumber.Value.ToString() + " in current document";
            } else {
                if (string.IsNullOrEmpty(this.Document)) {
                    return this.Folder.Title;
                } else {
                    return this.Folder.IsRoot ? this.Document : this.Document + " (in " + this.Folder + ")";
                }
            }
        }

    }
}
