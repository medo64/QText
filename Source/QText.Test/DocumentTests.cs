using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace QTextTest {

    [TestClass()]
    public class DocumentTests {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void Document_OpenPreExisting() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("A.txt");
                test.CreateRawFile("B.txt");
                test.CreateRawFile("C.txt");
                test.CreateRawFile("Alex\\D.txt");
                test.CreateRawFile("Alex\\E.txt");
                test.CreateRawFile("Steve\\F.txt");
                test.CreateRawFile("Steve\\A.txt");

                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();


                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(3, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Alex", folders[1].Name);
                Assert.AreEqual("Steve", folders[2].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("Alex", folders[1].Title);
                Assert.AreEqual("Steve", folders[2].Title);

                {
                    var files = new List<DocumentFile>(folders[0].GetFiles());
                    Assert.AreEqual(3, files.Count);
                    Assert.AreEqual("A", files[0].Name);
                    Assert.AreEqual("B", files[1].Name);
                    Assert.AreEqual("C", files[2].Name);
                    Assert.AreEqual("A", files[0].Title);
                    Assert.AreEqual("B", files[1].Title);
                    Assert.AreEqual("C", files[2].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(2, files.Count);
                    Assert.AreEqual("D", files[0].Name);
                    Assert.AreEqual("E", files[1].Name);
                    Assert.AreEqual("D", files[0].Title);
                    Assert.AreEqual("E", files[1].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[2].GetFiles());
                    Assert.AreEqual(2, files.Count);
                    Assert.AreEqual("A", files[0].Name);
                    Assert.AreEqual("F", files[1].Name);
                    Assert.AreEqual("A", files[0].Title);
                    Assert.AreEqual("F", files[1].Title);
                }

                doc.DisableWatcher();
            }
        }


        [TestMethod()]
        public void Document_CreateFolder() {
            using (var test = new TestDirectory()) {
                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.CreateFolder();

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(2, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("New folder", folders[1].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("New folder", folders[1].Title);

                Assert.AreEqual("New folder", test.Directory.GetDirectories()[0].Name);
            }
        }

        [TestMethod()]
        public void Document_CreateFolderWithName() {
            using (var test = new TestDirectory()) {
                var doc = new Document(test.Directory.FullName);
                doc.EnableWatcher();

                doc.CreateFolder("/");

                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(2, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("~2f~", folders[1].Name);
                Assert.AreEqual("(Default)", folders[0].Title);
                Assert.AreEqual("/", folders[1].Title);

                Assert.AreEqual("~2f~", test.Directory.GetDirectories()[0].Name);
            }
        }

    }
}
