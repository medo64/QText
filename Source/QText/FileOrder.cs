using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Medo.Xml;
using System.Text;

namespace QText {
    public class FileOrder {

        private static readonly Encoding Encoding = new UTF8Encoding(false);


        public FileOrder() {
            Reload();
        }


        public void Reload() {
            var orderedFileNames = new List<string>();

            try {
                string fileName = Path.Combine(Settings.FilesLocation, "QText.xml");
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

                                        string currSelectedTitle = CToNNString(xr.GetAttribute("selectedTitle"));
                                        string currSelectedFileName = CToNNString(xr.GetAttribute("selectedFileName"));
                                        if ((!string.IsNullOrEmpty(currSelectedFileName))) {
                                            this.SelectedFileName = currSelectedFileName; //fall-back to titles
                                        } else {
                                            this.SelectedFileName = currSelectedTitle + ".txt";
                                        }

                                        break;

                                    case "File":
                                        if ((walk.Peek() != "FileOrder")) { throw new InvalidOperationException(); }
                                        if ((!xr.IsEmptyElement)) { walk.Push(xr.Name); }

                                        string currTitle = CToNNString(xr.GetAttribute("title"));
                                        string currFileName = CToNNString(xr.GetAttribute("fileName"));
                                        if ((!string.IsNullOrEmpty(currFileName))) {
                                            orderedFileNames.Add(currFileName); //fall-back to titles
                                        } else {
                                            orderedFileNames.Add(currTitle + ".txt");
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
            } catch (Exception) { }


            var files = new List<string>();

            //First files in order
            for (int i = 0; i <= orderedFileNames.Count - 1; i++) {
                var fileName = Path.Combine(Settings.FilesLocation, orderedFileNames[i]);
                if (files.Contains(fileName) == false) { files.Add(fileName); }
            }

            //Then everything else
            foreach (var file in GetSortedFileNames()) {
                var fileName = Path.Combine(Settings.FilesLocation, file);
                if (files.Contains(fileName) == false) { files.Add(fileName); }
            }

            this.Files = files.ToArray();
        }

        public void Save(TabControlDnD tabControl, bool dontThrowExceptions) {
            try {
                using (var xw = new XmlTagWriter(Path.Combine(QText.Settings.FilesLocation, "QText.xml"), Encoding)) {
                    xw.XmlTextWriter.WriteStartDocument();

                    xw.StartTag("QText"); //<QText>
                    if (tabControl.SelectedTab != null) {
                        var selectedTab = (TabFile)(tabControl.SelectedTab);
                        xw.StartTag("FileOrder", "selectedTitle", selectedTab.Title, "selectedFileName", selectedTab.FileName); //<FileOrder>
                    } else {
                        xw.StartTag("FileOrder"); //<FileOrder>
                    }



                    for (int i = 0; i < tabControl.TabPages.Count; i++) {
                        var tf = (TabFile)(tabControl.TabPages[i]);
                        xw.WriteTag("File", "title", tf.Title, "fileName", tf.FileName);
                    }

                    xw.EndTag(); //</FileOrder>
                    xw.EndTag(); //</QText>
                }
            } catch {
                if (dontThrowExceptions == false) { throw; }
            }
        }


        public string[] Files { get; private set; }

        public string SelectedFileName { get; private set; }

        public string SelectedTitle {
            get {
                if ((string.IsNullOrEmpty(this.SelectedFileName))) { return string.Empty; }
                int iEnd = this.SelectedFileName.LastIndexOf('.');
                if ((iEnd >= 0)) {
                    return SelectedFileName.Remove(iEnd);
                } else {
                    return SelectedFileName;
                }
            }
        }



        private static string CToNNString(string value) {
            if ((value != null)) {
                return value;
            } else {
                return "";
            }
        }

        private static IList<string> GetSortedFileNames() {
            var filesAll = new List<string>();
            filesAll.AddRange(Directory.GetFiles(Settings.FilesLocation, "*.txt"));
            filesAll.AddRange(Directory.GetFiles(Settings.FilesLocation, "*.rtf"));

            var files = new List<string>();
            foreach (var fileNameA in filesAll) {
                var fi = new FileInfo(fileNameA);
                if ((fi.Attributes & FileAttributes.System) == FileAttributes.System) { //System
                } else if ((fi.Attributes & FileAttributes.Temporary) == FileAttributes.Temporary) { //Temporary
                } else {
                    files.Add(fi.Name);
                }
            }

            files.Sort();
            return files.AsReadOnly();
        }

    }
}
