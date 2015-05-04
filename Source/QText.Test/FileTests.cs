using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace QTextTest {

    [TestClass()]
    public class FileTests {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void DocumentFile_RenamePlain() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.GetFolder("Alex").GetFileByTitle("A").Rename("A*");


                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("A~2a~", files[0].Name);
                    Assert.AreEqual("A*", files[0].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFile_RenameRich() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.rtf");
                test.CreateRawFile("Steve\\B.rtf");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.GetFolder("Alex").GetFileByTitle("A").Rename("A*");


                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("A~2a~", files[0].Name);
                    Assert.AreEqual("A*", files[0].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFile_RenameEncryptedRich() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.rtf.aes256cbc");
                test.CreateRawFile("Steve\\B.rtf");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.GetFolder("Alex").GetFileByTitle("A").Rename("A*");


                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("A~2a~", files[0].Name);
                    Assert.AreEqual("A*", files[0].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }


        [TestMethod()]
        public void DocumentFile_ExternalRenamePlain() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                Directory.Move(Path.Combine(test.Directory.FullName, "Alex", "A.txt"), Path.Combine(test.Directory.FullName, "Alex", "A~2a~.txt"));
                Thread.Sleep(100);

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("A~2a~", files[0].Name);
                    Assert.AreEqual("A*", files[0].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFile_ExternalRenamePlainToRich() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                Directory.Move(Path.Combine(test.Directory.FullName, "Alex", "A.txt"), Path.Combine(test.Directory.FullName, "Alex", "A~2a~.rtf"));
                Thread.Sleep(100);

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("A~2a~", files[0].Name);
                    Assert.AreEqual(".rtf", files[0].Extension);
                    Assert.AreEqual("A*", files[0].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFile_ExternalRenamePlainToUnsupported() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                Directory.Move(Path.Combine(test.Directory.FullName, "Alex", "A.txt"), Path.Combine(test.Directory.FullName, "Alex", "A~2a~.txt2"));
                Thread.Sleep(100);

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(0, files.Count);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFile_ExternalRenameUnsupportedToPlain() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt2");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                Directory.Move(Path.Combine(test.Directory.FullName, "Alex", "A.txt2"), Path.Combine(test.Directory.FullName, "Alex", "A~2a~.txt"));
                Thread.Sleep(100);

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("A~2a~", files[0].Name);
                    Assert.AreEqual("A*", files[0].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }


        [TestMethod()]
        public void DocumentFile_MovePlain() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.GetFolder("Alex").GetFileByTitle("A").Move(doc.GetFolder("Steve"));

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(0, files.Count);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(2, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                    Assert.AreEqual("A", files[1].Name);
                    Assert.AreEqual("A", files[1].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFile_ExternalMovePlain() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                Directory.Move(Path.Combine(test.Directory.FullName, "Alex", "A.txt"), Path.Combine(test.Directory.FullName, "Steve", "A~2a~.txt"));
                Thread.Sleep(100);

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(0, files.Count);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(2, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                    Assert.AreEqual("A~2a~", files[1].Name);
                    Assert.AreEqual("A*", files[1].Title);
                }

                doc.DisableWatcher();
            }
        }


        [TestMethod()]
        public void DocumentFile_Delete() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.GetFolder("Alex").GetFileByTitle("A").Delete();

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(0, files.Count);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFile_ExternalDelete() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                File.Delete(Path.Combine(test.Directory.FullName, "Alex", "A.txt"));
                Thread.Sleep(100);

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(0, files.Count);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

    }
}
