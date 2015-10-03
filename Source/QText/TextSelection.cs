using System;
using System.Globalization;

namespace QText {
    internal class TextSelection {

        private TextSelection(int start, int length) {
            this.Start = start;
            this.Length = length;
        }


        public Int32 Start { get; private set; }
        public Int32 Length { get; private set; }
        public Int32 End { get { return (this.Start >= 0) ? this.Start + this.Length : -1; } }

        public Boolean IsEmpty { get { return (this.Start < 0); } }
        public Boolean IsNotEmpty { get { return !this.IsEmpty; } }
        private static readonly TextSelection Empty = new TextSelection(-1, 0);


        public static TextSelection FindWordStart(RichTextBoxEx textBox, int startAt, bool ignoreWhitespace = true) {
            if (textBox.TextLength == 0) { return TextSelection.Empty; }
            if ((startAt < 0) || (startAt > textBox.TextLength)) { return TextSelection.Empty; }

            var i = startAt - 1;

            if (ignoreWhitespace) {
                while ((i > 0) && (GetLikeUnicodeCategory(textBox.Text[i]) == UnicodeCategory.SpaceSeparator)) {
                    i--;
                }
            }
            if (i < 0) { return new TextSelection(0, 0); }

            var category = GetLikeUnicodeCategory(textBox.Text[i]);
            while (i >= 0) {
                var currCategory = GetLikeUnicodeCategory(textBox.Text[i]);
                if (currCategory != category) { i++; break; }
                if (currCategory == UnicodeCategory.ParagraphSeparator) { break; }
                i--;
            }
            if (i < 0) { return new TextSelection(0, 0); }

            return new TextSelection(i, 0);
        }

        public static TextSelection FindWordEnd(RichTextBoxEx textBox, int startAt, bool ignoreWhitespace = true) {
            if (textBox.TextLength == 0) { return TextSelection.Empty; }
            if ((startAt < 0) || (startAt >= textBox.TextLength)) { return TextSelection.Empty; }

            var i = startAt;
            var category = GetLikeUnicodeCategory(textBox.Text[i]);
            while (i < textBox.TextLength) {
                var currCategory = GetLikeUnicodeCategory(textBox.Text[i]);
                if (currCategory != category) { break; }
                if (currCategory == UnicodeCategory.ParagraphSeparator) { i++; break; }
                i++;
            }
            if (ignoreWhitespace) {
                while ((i < textBox.TextLength) && (GetLikeUnicodeCategory(textBox.Text[i]) == UnicodeCategory.SpaceSeparator)) {
                    i++;
                }
            }

            return new TextSelection(i, 0);
        }

        public static TextSelection FindWord(RichTextBoxEx textBox, int start) {
            var left = FindWordStart(textBox, start + 1, false);
            var right = FindWordEnd(textBox, start, false);
            if (left.IsEmpty || right.IsEmpty) { return TextSelection.Empty; }
            return new TextSelection(left.Start, right.Start - left.Start);
        }


        private static UnicodeCategory GetLikeUnicodeCategory(char ch) {
            if (ch == '\n') { return UnicodeCategory.ParagraphSeparator; }
            if (Settings.Current.SelectionDelimiters.Contains(ch.ToString())) { return UnicodeCategory.OtherSymbol; }
            if (char.IsWhiteSpace(ch)) {
                return UnicodeCategory.SpaceSeparator;
            } else {
                return UnicodeCategory.LetterNumber;
            }
        }

    }
}
