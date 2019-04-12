using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace QText.Legacy {

    internal static class OrderedFiles {

        public static void Upgrade() {
            var orderedTitles = new List<string>();
            var selectedTitle = default(string);
            try {
                var fileName = Path.Combine(QText.Settings.Current.FilesLocation, "QText.xml");
                if ((File.Exists(fileName))) {
                    using (var xr = new XmlTextReader(fileName)) {
                        var walk = new Stack<string>();

                        while (xr.Read()) {

                            if ((xr.NodeType == XmlNodeType.Element)) {
                                switch (xr.Name) {

                                    case "QText":
                                        if ((walk.Count > 0)) { throw new InvalidOperationException(); }
                                        if ((!xr.IsEmptyElement)) { walk.Push(xr.Name); }
                                        break;

                                    case "FileOrder":
                                        if ((walk.Peek() != "QText")) { throw new InvalidOperationException(); }
                                        if ((!xr.IsEmptyElement)) { walk.Push(xr.Name); }

                                        selectedTitle = xr.GetAttribute("selectedTitle");
                                        break;

                                    case "File":
                                        if ((walk.Peek() != "FileOrder")) { throw new InvalidOperationException(); }
                                        if ((!xr.IsEmptyElement)) { walk.Push(xr.Name); }

                                        var currTitle = xr.GetAttribute("title");
                                        if (string.IsNullOrEmpty(currTitle) == false) {
                                            orderedTitles.Add(currTitle);
                                        }
                                        break;
                                }

                            } else if ((xr.NodeType == System.Xml.XmlNodeType.EndElement)) {
                                switch (xr.Name) {

                                    case "QText":
                                    case "FileOrder":
                                    case "File":
                                        walk.Pop();
                                        break;
                                }
                            }

                        }
                    }

                }
                File.Delete(fileName);
            } catch (Exception) { }

            if (orderedTitles.Count > 0) {
                //write to new format
                FileStream fs = null;
                try {
                    fs = new FileStream(Path.Combine(QText.Settings.Current.FilesLocation, ".qtext"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    try { fs.SetLength(0); } catch (IOException) { } //try to delete content
                    fs.Position = fs.Length;
                    using (var sw = new StreamWriter(fs)) {
                        fs = null;
                        sw.WriteLine("/["); //always start block with /[
                        foreach (var title in orderedTitles) {
                            if (title == selectedTitle) { //selected file is written with //selected
                                sw.WriteLine(title + "//selected");
                            } else {
                                sw.WriteLine(title);
                            }
                        }
                        sw.WriteLine("]/"); //always end block with ]/
                    }
                } catch (IOException) {
                } finally {
                    if (fs != null) { fs.Dispose(); }
                }
            }
        }

    }
}
