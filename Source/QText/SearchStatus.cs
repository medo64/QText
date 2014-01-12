namespace QText {
    internal static class SearchStatus {

        private static string _text;
        public static string Text {
            get { return _text; }
            set {
                if ((!string.IsNullOrEmpty(value))) {
                    _text = value;
                }
            }
        }

        public static bool CaseSensitive {
            get { return Settings.SearchCaseSensitive; }
            set { Settings.SearchCaseSensitive = value; }
        }

        public static SearchScope Scope {
            get { return Settings.SearchScope; }
            set { Settings.SearchScope = value; }
        }


        internal static SearchScope GetScope(bool file, bool folder, bool folders) {
            if (folders) { return SearchScope.Folders; }
            if (folder) { return SearchScope.Folder; }
            return SearchScope.File;
        }

        internal static SearchScope GetScope(int scope) {
            if (scope == 2) { return SearchScope.Folders; }
            if (scope == 1) { return SearchScope.Folder; }
            return SearchScope.File;
        }
    }

    internal enum SearchScope {
        File = 0,
        Folder = 1,
        Folders = 2
    }
}