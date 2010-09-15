namespace QTextAux {
    public static class SearchStatus {

        private static string _text;
        public static string Text {
            get { return _text; }
            set {
                if ((!string.IsNullOrEmpty(value))) {
                    _text = value;
                }
            }
        }

        public static bool CaseSensitive { get; set; }

    }
}