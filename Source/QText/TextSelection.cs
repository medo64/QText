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

        public Boolean IsEmpty { get { return (this.Start < 0); } }
        private static readonly TextSelection Empty = new TextSelection(-1, 0);


        public static TextSelection FindWord(RichTextBoxEx textBox, int start) {
            if (textBox.TextLength == 0) { return TextSelection.Empty; }

            var startIndex = start;
            if (startIndex >= textBox.TextLength) { startIndex -= 1; }
            if (textBox.Text[startIndex] == '\n') {
                var line1 = textBox.GetLineFromCharIndex(startIndex);
                var line2 = textBox.GetLineFromCharIndex(startIndex - 1);
                if (line1 == line2) {
                    startIndex -= 1;
                } else {
                    return TextSelection.Empty;
                }
            }

            var leftCount = 0;
            while (startIndex - leftCount >= 0) { //find non whitespace
                if (char.IsWhiteSpace(textBox.Text[startIndex - leftCount]) == false) { break; }
                leftCount += 1;
            }
            if (startIndex - leftCount < 0) { leftCount = int.MinValue; }

            var rightCount = 0;
            while (startIndex + rightCount < textBox.TextLength - 1) { //find non whitespace
                if (char.IsWhiteSpace(textBox.Text[startIndex + rightCount]) == false) { break; }
                rightCount += 1;
            }
            if (startIndex + rightCount >= textBox.TextLength - 1) { rightCount = int.MinValue; }

            if ((rightCount == int.MinValue) && (leftCount == int.MinValue)) {
                return TextSelection.Empty; //cannot select text if there is only whitespace
            } else if (leftCount == int.MinValue) {
                startIndex = startIndex + rightCount;
            } else if (rightCount == int.MinValue) {
                startIndex = startIndex - leftCount;
            } else {
                if (rightCount <= leftCount) {
                    startIndex = startIndex + rightCount;
                } else {
                    startIndex = startIndex - leftCount;
                }
            }

            var category = GetLikeUnicodeCategory(textBox.Text[startIndex]);
            while (startIndex >= 0) { //find start of word
                if (GetLikeUnicodeCategory(textBox.Text[startIndex]) != category) { break; }
                startIndex -= 1;
            }
            startIndex += 1;
            var endIndex = startIndex;
            while (endIndex < textBox.TextLength - 1) { //find end of word
                if (GetLikeUnicodeCategory(textBox.Text[endIndex]) != category) { break; }
                endIndex += 1;
            }
            if (endIndex < textBox.TextLength - 1) { endIndex -= 1; }

            return new TextSelection (startIndex ,endIndex - startIndex + 1);
        }


        private static UnicodeCategory GetLikeUnicodeCategory(char ch) {
            if (ch == '-') { return UnicodeCategory.LetterNumber; }
            if (char.IsWhiteSpace(ch)) {
                return UnicodeCategory.SpaceSeparator;
            } else if (char.IsPunctuation(ch)) {
                return UnicodeCategory.OtherSymbol;
            } else {
                return UnicodeCategory.LetterNumber;
            }
        }

    }
}
