using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace QText {

    internal static class Helper {

        public static void CreatePath(string path) {
            if ((!Directory.Exists(path))) {
                var currPath = path;
                var allPaths = new List<string>();
                while (!(Directory.Exists(currPath))) {
                    allPaths.Add(currPath);
                    currPath = System.IO.Path.GetDirectoryName(currPath);
                    if (string.IsNullOrEmpty(currPath)) {
                        throw new InvalidOperationException("Path \"" + path + "\" can not be created.");
                    }
                }

                try {
                    for (var i = allPaths.Count - 1; i >= 0; i += -1) {
                        System.IO.Directory.CreateDirectory(allPaths[i]);
                    }
                } catch (Exception ex) {
                    throw new InvalidOperationException("Path \"" + path + "\" can not be created.", ex);
                }
            }
        }

        public static void MovePath(string currentPath, string newPath) {
            if (!currentPath.StartsWith(@"\\", StringComparison.Ordinal)) { currentPath = @"\\?\" + currentPath; }
            if (!newPath.StartsWith(@"\\", StringComparison.Ordinal)) { newPath = @"\\?\" + newPath; }
            if (NativeMethods.MoveFileExW(currentPath, newPath, NativeMethods.MOVEFILE_COPY_ALLOWED | NativeMethods.MOVEFILE_WRITE_THROUGH) == false) {
                var ex = new Win32Exception();
                throw new IOException(ex.Message, ex);
            }
        }

        public static bool DeleteTabFile(IWin32Window owner, TabFiles tabFiles, TabFile tabToDelete) {
            if (tabFiles == null) { throw new ArgumentNullException("tabFiles", "Tab parent cannot be null."); }
            if (tabToDelete == null) { throw new ArgumentNullException("tabToDelete", "Tab cannot be null."); }

            var askForConfirmation = false;
            if (tabToDelete.BaseFile.IsEncrypted) {
                askForConfirmation = true;
            } else {
                tabToDelete.Open();
                if (tabToDelete.TextBox.Text.Length > 0) { askForConfirmation |= true; }
            }
            if (askForConfirmation) {
                if (Medo.MessageBox.ShowQuestion(owner, string.Format(CultureInfo.CurrentUICulture, "Do you really want to delete \"{0}\"?", tabToDelete), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                    return false;
                }
            }
            try {
                tabFiles.Enabled = false;
                try {
                    tabFiles.DeleteTab(tabToDelete);
                    return true;
                } catch (Exception ex) {
                    Medo.MessageBox.ShowWarning(owner, string.Format(CultureInfo.CurrentUICulture, "Cannot delete file.\n\n{0}", ex.Message));
                    return false;
                }
            } finally {
                tabFiles.Enabled = true;
            }
        }


        #region Encode/decode file name

        public static string EncodeFileName(string fileName) {
            if (fileName == null) { throw new ArgumentNullException("fileName"); }
            var invalidChars = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder();
            foreach (var ch in fileName) {
                if (Array.IndexOf(invalidChars, ch) >= 0) {
                    var value = (int)ch;
                    if ((value < 0) || (value > 255)) { throw new InvalidOperationException("Expected single byte."); }
                    sb.Append("~");
                    sb.Append(value.ToString("x2"));
                    sb.Append("~");
                } else {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        #endregion


        public static string GetKeyString(Keys keyData) {
            if ((keyData & Keys.LWin) == Keys.LWin) { return string.Empty; }
            if ((keyData & Keys.RWin) == Keys.RWin) { return string.Empty; }

            var sb = new System.Text.StringBuilder();
            if ((keyData & Keys.Control) == Keys.Control) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Ctrl");
                keyData ^= Keys.Control;
            }

            if ((keyData & Keys.Alt) == Keys.Alt) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Alt");
                keyData ^= Keys.Alt;
            }

            if ((keyData & Keys.Shift) == Keys.Shift) {
                if (sb.Length > 0) { sb.Append("+"); }
                sb.Append("Shift");
                keyData ^= Keys.Shift;
            }

            switch (keyData) {
                case 0:
                    return string.Empty;
                case Keys.ControlKey:
                    return string.Empty;
                case Keys.Menu:
                    return string.Empty;
                case Keys.ShiftKey:
                    return string.Empty;
                default:
                    if (sb.Length > 0) { sb.Append("+"); }
                    sb.Append(keyData.ToString());
                    return sb.ToString();
            }
        }


        public static readonly ToolStripProfessionalRenderer ToolstripRenderer = new ToolStripBorderlessProfessionalRenderer();
        private class ToolStripBorderlessProfessionalRenderer : ToolStripProfessionalRenderer {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
                //base.OnRenderToolStripBorder(e);
            }
        }


        internal static class NativeMethods {

            public const uint MOVEFILE_COPY_ALLOWED = 0x02;
            public const uint MOVEFILE_WRITE_THROUGH = 0x08;


            [DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool MoveFileExW([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpExistingFileName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpNewFileName, uint dwFlags);

        }


        #region Toolstrip images

        internal static void ScaleToolstrip(params ToolStrip[] toolstrips) {
            var sizeAndSet = GetSizeAndSet(toolstrips);
            var size = sizeAndSet.Key;
            var set = sizeAndSet.Value;

            var resources = QText.Properties.Resources.ResourceManager;
            foreach (var toolstrip in toolstrips) {
                toolstrip.ImageScalingSize = new Size(size, size);
                foreach (ToolStripItem item in toolstrip.Items) {
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    if (item.Image != null) { //update only those already having image
                        Bitmap bitmap = null;
                        if (!string.IsNullOrEmpty(item.Name)) {
                            bitmap = resources.GetObject(item.Name + set) as Bitmap;
                        }
                        if ((bitmap == null) && !string.IsNullOrEmpty(item.Tag as string)) {
                            bitmap = resources.GetObject(item.Tag + set) as Bitmap;
                        }

                        item.ImageScaling = ToolStripItemImageScaling.None;
#if DEBUG
                        item.Image = (bitmap != null) ? new Bitmap(bitmap, size, size) : new Bitmap(size, size, PixelFormat.Format8bppIndexed);
#else
                        if (bitmap != null) { item.Image = new Bitmap(bitmap, size, size); }
#endif
                    }

                    if (item is ToolStripSplitButton toolstripSplitButton) {
                        ScaleToolstrip(toolstripSplitButton.DropDown);
                    }
                }
            }
        }

        internal static void ScaleGotoImageList(Form form, ImageList imageList) {
            var sizeAndSet = GetSizeAndSet(form);
            var size = sizeAndSet.Key;
            var set = sizeAndSet.Value;

            var resources = QText.Properties.Resources.ResourceManager;

            imageList.Images.Clear();
            imageList.ImageSize = new Size(size, size);
            imageList.Images.Add(resources.GetObject("staFolder" + set) as Bitmap);
            imageList.Images.Add(resources.GetObject("staFile" + set) as Bitmap);
        }

        internal static void ScaleToolstripItem(ToolStripItem item, string name) {
            var sizeAndSet = GetSizeAndSet(item.GetCurrentParent());
            var size = sizeAndSet.Key;
            var set = sizeAndSet.Value;

            var resources = QText.Properties.Resources.ResourceManager;
            item.ImageScaling = ToolStripItemImageScaling.None;
            if (resources.GetObject(name + set) is Bitmap bitmap) {
                item.Image = new Bitmap(bitmap, size, size);
            } else {
#if DEBUG
                item.Image = new Bitmap(size, size, PixelFormat.Format8bppIndexed);
#endif
            }
        }

        private static KeyValuePair<int, string> GetSizeAndSet(params Control[] controls) {
            using (var g = controls[0].CreateGraphics()) {
                var scale = Math.Max(Math.Max(g.DpiX, g.DpiY), 96.0) / 96.0;
                scale += Settings.Current.ScaleBoost;

                if (scale < 1.5) {
                    return new KeyValuePair<int, string>(16, "_16");
                } else if (scale < 2) {
                    return new KeyValuePair<int, string>(24, "_24");
                } else if (scale < 3) {
                    return new KeyValuePair<int, string>(32, "_32");
                } else {
                    var base32 = 16 * scale / 32;
                    var base48 = 16 * scale / 48;
                    if ((base48 - (int)base48) < (base32 - (int)base32)) {
                        return new KeyValuePair<int, string>(48 * (int)base48, "_48");
                    } else {
                        return new KeyValuePair<int, string>(32 * (int)base32, "_32");
                    }
                }
            }
        }

        #endregion



        public static IEnumerable<TabFile> GetTabs(IEnumerable<DocumentFile> files, ContextMenuStrip contextMenuStrip) {
            foreach (var file in files) {
                var tab = new TabFile(file) {
                    ContextMenuStrip = contextMenuStrip
                };
                yield return tab;
            }
        }

        #region RichText

        private enum RichTextParseState { None, Text, Escape, Command, Argument }
        [DebuggerDisplay("{Text.ToString()}")]
        private class RichTextParseResult {
            public bool IsVisible = true;
            public StringBuilder Text = new StringBuilder();
        }

        internal static string FilterRichText(string richText) {
            var sbCommand = new StringBuilder();
            var sbArgument = new StringBuilder();
            var groupStack = new Stack<RichTextParseResult>(new RichTextParseResult[] { new RichTextParseResult() });

            //Debug.WriteLine(new string('-', 80));
            //Debug.WriteLine(richText);
            //Debug.WriteLine(new string('-', 80));

            var state = RichTextParseState.None;
            foreach (var ch in richText) {
                switch (state) {
                    case RichTextParseState.None:
                        if (ch == '{') { //begin group
                            groupStack.Push(new RichTextParseResult());
                            state = RichTextParseState.Text;
                        }
                        break;

                    case RichTextParseState.Text:
                        if (ch == '\\') {
                            state = RichTextParseState.Escape;
                        } else if (ch == '{') {
                            groupStack.Push(new RichTextParseResult());
                        } else if (ch == '}') {
                            ProcessRichTextGroupPop(groupStack);
                        } else {
                            groupStack.Peek().Text.Append(ch);
                        }
                        break;

                    case RichTextParseState.Escape:
                        if (char.IsLetter(ch) || (ch == '*')) {
                            sbCommand.Clear();
                            sbArgument.Clear();
                            sbCommand.Append(ch);
                            state = RichTextParseState.Command;
                        } else {
                            groupStack.Peek().Text.Append('\\');
                            groupStack.Peek().Text.Append(ch);
                            state = RichTextParseState.Text;
                        }
                        break;

                    case RichTextParseState.Command:
                        if (char.IsLetter(ch)) {
                            sbCommand.Append(ch);
                        } else if (char.IsDigit(ch)) {
                            sbArgument.Append(ch);
                            state = RichTextParseState.Argument;
                        } else if (ch == '\\') {
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            state = RichTextParseState.Escape;
                        } else if (ch == '{') {
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            groupStack.Push(new RichTextParseResult());
                            state = RichTextParseState.Text;
                        } else if (ch == '}') {
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            ProcessRichTextGroupPop(groupStack);
                            state = RichTextParseState.Text;
                        } else if (ch == ' ') {
                            sbArgument.Append(ch);
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            state = RichTextParseState.Text;
                        } else {
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            groupStack.Peek().Text.Append(ch);
                            state = RichTextParseState.Text;
                        }

                        break;

                    case RichTextParseState.Argument:
                        if (ch == '\\') {
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            state = RichTextParseState.Escape;
                        } else if (ch == '{') {
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            groupStack.Push(new RichTextParseResult());
                            state = RichTextParseState.Text;
                        } else if (ch == '}') {
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            ProcessRichTextGroupPop(groupStack);
                            state = RichTextParseState.Text;
                        } else if (ch == ' ') {
                            sbArgument.Append(ch);
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            state = RichTextParseState.Text;
                        } else if (char.IsDigit(ch)) {
                            sbArgument.Append(ch);
                        } else {
                            ProcessRichTextCommand(groupStack, sbCommand.ToString(), sbArgument.ToString());
                            groupStack.Peek().Text.Append(ch);
                            state = RichTextParseState.Text;
                        }
                        break;
                }
            }

            if (groupStack.Count == 1) {
                var newRichText = groupStack.Peek().Text.ToString();
                //Debug.WriteLine(new string('=', 80));
                //Debug.WriteLine(newRichText);
                //Debug.WriteLine(new string('=', 80));
                return newRichText;
            } else {
                return null;
            }
        }

        private static void ProcessRichTextCommand(Stack<RichTextParseResult> stack, string command, string argument) {
            var currentStack = stack.Peek();
            if (currentStack.IsVisible) {
                switch (command) {
                    case "rtf": //header
                    case "plain": //reset format
                    case "ansi": //character set
                    case "mac": //character set
                    case "pc": //character set
                    case "pca": //character set
                    case "ansicpg": //character set
                    case "fcharset": //character set
                    case "deff": //default font
                    case "adeff": //default font
                    case "deflang": //default language
                    case "deflangfe": //default language
                    case "adeflang": //default language
                    case "fonttbl": //font table
                    case "f": //font
                    case "falt": //alternate font
                    case "fnil": //font family
                    case "froman": //font family
                    case "fswiss": //font family
                    case "fmodern": //font family
                    case "fscript": //font family
                    case "fdecor": //font family
                    case "ftech": //font family
                    case "fbidi": //font family
                    case "fprq": //font pitch
                    case "lang": //lanugage
                    case "alang": //lanugage ID
                    case "langfe": //lanugage formatting
                    case "stshfdbch": //scripts
                    case "stshfloch": //scripts
                    case "stshfhich": //scripts
                    case "stshfbi": //scripts
                    case "uc": //unicode text
                    case "u": //unicode character
                    case "colortbl": //color table
                    case "red": //red component
                    case "green": //green component
                    case "blue": //blue component
                    case "fs": //Font size in half-points
                    case "qc": //centered
                    case "qj": //justified
                    case "ql": //left-aligned(the default).
                    case "qr": //right-aligned.
                    case "qd": //distributed.
                    case "qk": //percentage of line occupied by Kashida justification(0 – low, 10 – medium, 20 – high).
                    case "qt": //for Thai distributed justification.
                    case "rtlch": //right-to-left
                    case "ltrch": //left-to-right
                    case "par": //paragraph
                    case "pard": //default paragraph
                    case "defchp": //default character properties
                    case "defpap": //default paragraph properties
                    case "af": //font number
                    case "afs": //font size
                    case "stylesheet": //stylesheet
                    case "styrsid": //stylesheet id
                        //Debug.WriteLine("Known RichText command " + command);
                        currentStack.Text.Append(@"\" + command + argument);
                        break;

                    //ignore these
                    case "snext": //next stylesheet
                    case "author": //author info
                    case "ri": //right indent
                    case "rin": //right indent
                    case "sb": //space before
                    case "lisb": //space before
                    case "sa": //space after
                    case "lisa": //space after
                    case "slmult": //spacing multiple
                    case "sl": //space between lines
                    case "linex": //line number left distance
                    case "cb": //background color
                    case "highlight": //highlight color
                        break;

                    default:
                        currentStack.Text.Append(@"\" + command + argument);
                        if (currentStack.Text.Length == 0) { //if first command is unknown, ignore the whole group
                            currentStack.IsVisible = false;
                            //Debug.WriteLine("Hidden RichText command " + command);
                        } else {
                            //Debug.WriteLine("Other RichText command " + command);
                        }
                        break;

                }
            }
        }

        private static void ProcessRichTextGroupPop(Stack<RichTextParseResult> stack) {
            var child = stack.Pop();
            var parent = stack.Peek();
            if (child.IsVisible && parent.IsVisible) {
                var childText = child.Text.ToString();
                if (!string.IsNullOrEmpty(childText)) { parent.Text.Append("{" + childText + "}"); }
            }
        }

        #endregion RichText

    }
}
