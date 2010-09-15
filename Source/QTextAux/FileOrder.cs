using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace QTextAux {
    public class FileOrder {

        private List<string> _OrderedFileNames = new List<string>();
        private List<string> _FileNames = new List<string>();

        private string _SelectedFileName;
        public FileOrder() {
            try {
                var files = GetAllFileNames(false);

                for (int i = 0; i <= files.Count - 1; i++) {
                    var fi = new FileInfo(files[i]);
                    string title = fi.Name.Remove(fi.Name.Length - fi.Extension.Length, fi.Extension.Length);
                    this._FileNames.Add(fi.Name);
                }

                string fileName = System.IO.Path.Combine(Settings.FilesLocation, "QText.xml");
                if ((System.IO.File.Exists(fileName))) {
                    using (var xr = new XmlTextReader(fileName)) {
                        var walk = new Stack<string>();

                        while (xr.Read()) {

                            if ((xr.NodeType == XmlNodeType.Element)) {
                                switch (xr.Name) {

                                    case "QText":
                                        if ((walk.Count > 0))
                                            return;

                                        if ((!xr.IsEmptyElement))
                                            walk.Push(xr.Name);

                                        break;
                                    case "FileOrder":
                                        if ((walk.Peek() != "QText"))
                                            return;

                                        if ((!xr.IsEmptyElement))
                                            walk.Push(xr.Name);
                                        string currSelectedTitle = CToNNString(xr.GetAttribute("selectedTitle"));
                                        string currSelectedFileName = CToNNString(xr.GetAttribute("selectedFileName"));
                                        if ((!string.IsNullOrEmpty(currSelectedFileName))) {
                                            this._SelectedFileName = currSelectedFileName;
                                            //fall-back to titles
                                        } else {
                                            this._SelectedFileName = currSelectedTitle + ".txt";
                                        }

                                        break;
                                    case "File":
                                        if ((walk.Peek() != "FileOrder"))
                                            return;

                                        if ((!xr.IsEmptyElement))
                                            walk.Push(xr.Name);
                                        string currTitle = CToNNString(xr.GetAttribute("title"));
                                        string currFileName = CToNNString(xr.GetAttribute("fileName"));
                                        if ((!string.IsNullOrEmpty(currFileName))) {
                                            this._OrderedFileNames.Add(currFileName);
                                            //fall-back to titles
                                        } else {
                                            this._OrderedFileNames.Add(currTitle + ".txt");
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
            } catch (Exception) {}
        }

        private static IList<string> GetAllFileNames(bool includeHidden) {
            var filesAll = new List<string>();
            filesAll.AddRange(Directory.GetFiles(Settings.FilesLocation, "*.txt"));
            filesAll.AddRange(Directory.GetFiles(Settings.FilesLocation, "*.rtf"));

            var files = new List<string>();
            foreach (var fileNameA in filesAll) {
                var fi = new FileInfo(fileNameA);
                if ((fi.Attributes & FileAttributes.System) == FileAttributes.System) { //System
                } else if ((fi.Attributes & FileAttributes.Temporary) == FileAttributes.Temporary) { //Temporary
                } else if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) { //Hidden
                    if (includeHidden) { files.Add(fileNameA); }
                } else {
                    files.Add(fileNameA);
                }
            }
            return files;
        }
        List<string> static_GetFiles_retFiles;


        public string[] GetFiles() {

            if ((static_GetFiles_retFiles == null)) {
                static_GetFiles_retFiles = new List<string>();

                //First get from files.
                for (int i = 0; i <= this._OrderedFileNames.Count - 1; i++) {
                    if (this._FileNames.Contains(this._OrderedFileNames[i])) {
                        static_GetFiles_retFiles.Add(Path.Combine(Settings.FilesLocation, this._OrderedFileNames[i]));
                        this._FileNames.Remove(this._OrderedFileNames[i]);
                    }
                }

                //Then everything else
                this._FileNames.Sort();
                for (int i = 0; i <= this._FileNames.Count - 1; i++) {
                    static_GetFiles_retFiles.Add(System.IO.Path.Combine(Settings.FilesLocation, this._FileNames[i]));
                }
            }

            return static_GetFiles_retFiles.ToArray();
        }

        public string SelectedTitle {
            get {
                if ((string.IsNullOrEmpty(this._SelectedFileName)))
                    return string.Empty;
                int iEnd = this._SelectedFileName.LastIndexOf('.');
                if ((iEnd >= 0)) {
                    return _SelectedFileName.Remove(iEnd);
                } else {
                    return _SelectedFileName;
                }
            }
        }

        public void Rename(string oldFileName, string newFileName) {
            FileInfo oldFile = new FileInfo(oldFileName);
            FileInfo newFile = new FileInfo(newFileName);
            for (int i = 0; i <= _FileNames.Count - 1; i++) {
                if ((string.Compare(_FileNames[i], oldFile.Name, System.StringComparison.OrdinalIgnoreCase) == 0)) {
                    _FileNames[i] = newFile.Name;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
        }

        public string SelectedFileName {
            get { return _SelectedFileName; }
        }

        private static string CToNNString(string value) {
            if ((value != null)) {
                return value;
            } else {
                return "";
            }
        }

    }
}
