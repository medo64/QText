using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QText {
    internal class GotoResult {

        internal static IEnumerable<GotoResult> GetSuggestions(string suggestion, bool allowLineNumbers) {
            if (suggestion.Length == 0) {

                yield return new GotoResult(null, App.Document.RootFolder, null);

            } else if (!Settings.Current.GotoSortResults) {

                foreach (var result in GetSuggestionsRaw(suggestion, allowLineNumbers)) {
                    yield return result;
                }

            } else { //sort

                var list = new List<GotoResult>(GetSuggestionsRaw(suggestion, allowLineNumbers));

                list.Sort(delegate(GotoResult item1, GotoResult item2) {
                    int initialCompare;
                    if (!Settings.Current.GotoSortPreferFolders || (item1.IsFolder == item2.IsFolder)) {
                        initialCompare = 0;
                    } else if (item1.IsFolder && !item2.IsFolder) {
                        initialCompare = -1;
                    } else {
                        initialCompare = +1;
                    }

                    if (initialCompare == 0) {
                        var title1 = item1.ToString();
                        var title2 = item2.ToString();
                        var starts1 = title1.StartsWith(suggestion, StringComparison.CurrentCultureIgnoreCase);
                        var starts2 = title2.StartsWith(suggestion, StringComparison.CurrentCultureIgnoreCase);
                        if (!Settings.Current.GotoSortPreferPrefix || (starts1 == starts2)) {
                            return string.Compare(title1, title2, StringComparison.CurrentCultureIgnoreCase);
                        } else if (starts1 && !starts2) {
                            return -1;
                        } else {
                            return +1;
                        }
                    } else {
                        return initialCompare;
                    }
                });

                foreach (var item in list) {
                    yield return item;
                }

            }
        }

        internal static IEnumerable<GotoResult> GetSuggestionsRaw(string suggestion, bool allowLineNumbers) {
            if (allowLineNumbers) {
                int lineNumber;
                if (int.TryParse(suggestion, NumberStyles.Integer, CultureInfo.CurrentCulture, out lineNumber)) {
                    if (lineNumber > 0) {
                        yield return new GotoResult(lineNumber, null, null);
                    }
                }
            }

            foreach (var folder in App.Document.GetSubFolders()) {
                if (folder.Title.IndexOf(suggestion, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                    yield return new GotoResult(null, folder, null);
                }
            }

            foreach (var folder in App.Document.GetFolders()) {
                foreach (var file in folder.GetFiles()) {
                    if (file.Title.IndexOf(suggestion, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        yield return new GotoResult(null, folder, file);
                    }
                }
            }

        }

        private GotoResult(int? lineNumber, DocumentFolder folder, DocumentFile file) {
            this.LineNumber = lineNumber;
            this.Folder = folder;
            this.File = file;
        }


        public Int32? LineNumber { get; private set; }
        public DocumentFolder Folder { get; private set; }
        public DocumentFile File { get; private set; }


        public int ImageIndex {
            get {
                if (LineNumber.HasValue) {
                    return -1;
                } else {
                    if (this.File == null) {
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

        public bool IsFile {
            get { return !this.IsLineNumber && (this.File != null); }
        }

        public bool IsFolder {
            get { return !this.IsLineNumber && !this.IsFile && (this.Folder != null); }
        }

        public override string ToString() {
            if (LineNumber.HasValue) {
                return "Line " + LineNumber.Value.ToString() + " in current file";
            } else {
                if (this.File == null) {
                    return this.Folder.Title;
                } else {
                    return this.Folder.IsRoot ? this.File.Title : this.File.Title + " (in " + this.Folder.Title + ")";
                }
            }
        }

    }
}
