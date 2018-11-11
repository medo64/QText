using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QText {
    internal partial class SpellingForm : Form {
        public SpellingForm(string defaultText = "") {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            txtInput.Text = defaultText;
            txtInput.SelectionStart = txtInput.Text.Length;
        }


        private void txtInput_TextChanged(object sender, EventArgs e) {
            var sb = new StringBuilder();
            var noSpace = true;
            foreach (var ch in txtInput.Text.ToUpperInvariant()) {
                if (noSpace) { noSpace = false; } else { sb.Append(" "); }
                if (char.IsLetterOrDigit(ch)) {
                    sb.Append(Transcribe(ch));
                } else if (ch == ' ') {
                    noSpace = true;
                    sb.AppendLine();
                } else {
                    sb.Append(ch);
                }
            }
            txtSpelling.Text = sb.ToString();
            txtSpelling.SelectAll();
        }

        private void txtSpelling_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Control | Keys.A: {
                        txtSpelling.SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
            }
        }


        private static string Transcribe(char ch) {
            switch (ch) {
                case 'A': return "Alfa";
                case 'B': return "Bravo";
                case 'C': return "Charlie";
                case 'D': return "Delta";
                case 'E': return "Echo";
                case 'F': return "Foxtrot";
                case 'G': return "Golf";
                case 'H': return "Hotel";
                case 'I': return "India";
                case 'J': return "Juliett";
                case 'K': return "Kilo";
                case 'L': return "Lima";
                case 'M': return "Mike";
                case 'N': return "November";
                case 'O': return "Oscar";
                case 'P': return "Papa";
                case 'Q': return "Quebec";
                case 'R': return "Romeo";
                case 'S': return "Sierra";
                case 'T': return "Tango";
                case 'U': return "Uniform";
                case 'V': return "Victor";
                case 'W': return "Whiskey";
                case 'X': return "X-ray";
                case 'Y': return "Yankee";
                case 'Z': return "Zulu";
                case '0': return "Zero";
                case '1': return "One";
                case '2': return "Two";
                case '3': return "Three";
                case '4': return "Four";
                case '5': return "Five";
                case '6': return "Six";
                case '7': return "Seven";
                case '8': return "Eight";
                case '9': return "Nine";
                default: return "[?]";
            }
        }

    }
}
