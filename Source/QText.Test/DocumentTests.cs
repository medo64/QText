using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;
using System.Collections.Generic;

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
                test.CreateRawFile("Inner\\C.txt");

                var doc = new Document(test.Directory.FullName);
                var folders = new List<DocumentFolder>(doc.GetFolders());

                Assert.AreEqual(2, folders.Count);
                Assert.AreEqual("", folders[0].Name);
                Assert.AreEqual("Inner", folders[1].Name);

                {
                    var files = new List<DocumentFile>(folders[0].GetFiles());
                    Assert.AreEqual(3, files.Count);
                    Assert.AreEqual("A", files[0].Title);
                    Assert.AreEqual("B", files[1].Title);
                    Assert.AreEqual("C", files[2].Title);
                }

                {
                    var files = new List<DocumentFile>(folders[1].GetFiles());
                    Assert.AreEqual(1, files.Count);
                    Assert.AreEqual("C", files[0].Title);
                }
            }
        }





    }
}
