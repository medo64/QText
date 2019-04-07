using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace QTextTest {

    [TestClass()]
    public class FolderTests {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void DocumentFolder_Rename() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.GetFolder("Alex").Rename("V*alex");


                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Steve", folders[1].Name);
                Assert.AreEqual("V~2a~alex", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Steve", folders[1].Title);
                Assert.AreEqual("V*alex", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("A", files[0].Name);
                    Assert.AreEqual("A", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFolder_ExternalRename() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                Directory.Move(Path.Combine(test.Directory.FullName, "Alex"), Path.Combine(test.Directory.FullName, "V~2a~alex"));
                Thread.Sleep(100);

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Steve", folders[1].Name);
                Assert.AreEqual("V~2a~alex", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Steve", folders[1].Title);
                Assert.AreEqual("V*alex", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("A", files[0].Name);
                    Assert.AreEqual("A", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }


        [TestMethod()]
        public void DocumentFolder_Delete() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.GetFolder("Alex").Delete();

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(2, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Steve", folders[1].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Steve", folders[1].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

        [TestMethod()]
        public void DocumentFolder_ExternalDelete() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                Directory.Delete(Path.Combine(test.Directory.FullName, "Alex"), true);
                Thread.Sleep(100);

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(2, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Steve", folders[1].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Steve", folders[1].Title);

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("B", files[0].Name);
                    Assert.AreEqual("B", files[0].Title);
                }

                doc.DisableWatcher();
            }
        }

    }
}
