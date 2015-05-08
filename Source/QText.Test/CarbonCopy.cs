using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace QTextTest {

    [TestClass()]
    public class CarbonTests {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void CarbonCopy_IgnoreRootSub() {
            using (var test = new TestDirectory()) {
                var doc = new Document(test.Directory.FullName);
                doc.CarbonCopyRootPath = Path.Combine(doc.RootPath, "Sub");

                Assert.IsNull(doc.CarbonCopyRootPath, "Carbon-copy path must be null.");
            }
        }

        [TestMethod()]
        public void CarbonCopy_WriteAll() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");
                using (var testCC = new TestDirectory()) {
                    var doc = new Document(test.Directory.FullName);
                    doc.CarbonCopyRootPath = testCC.Directory.FullName;

                    Assert.IsNotNull(doc.CarbonCopyRootPath, "Carbon-copy path must not be null.");
                    doc.WriteAllCarbonCopies();

                    var directories = new List<DirectoryInfo>(testCC.Directory.GetDirectories());
                    Assert.AreEqual(2, directories.Count);
                    Assert.AreEqual("Alex", directories[0].Name);
                    Assert.AreEqual("Steve", directories[1].Name);

                    {
                        var files = new List<FileInfo>(directories[0].GetFiles());
                        Assert.AreEqual(1, files.Count);
                        Assert.AreEqual("A.txt", files[0].Name);
                    }
                    {
                        var files = new List<FileInfo>(directories[1].GetFiles());
                        Assert.AreEqual(1, files.Count);
                        Assert.AreEqual("B.txt", files[0].Name);
                    }
                }
            }
        }

        [TestMethod()]
        public void CarbonCopy_WriteOne() {
            using (var test = new TestDirectory()) {
                test.CreateRawFile("Alex\\A.txt");
                test.CreateRawFile("Steve\\B.txt");
                using (var testCC = new TestDirectory()) {
                    var doc = new Document(test.Directory.FullName);
                    doc.CarbonCopyRootPath = testCC.Directory.FullName;
                    Assert.IsNotNull(doc.CarbonCopyRootPath, "Carbon-copy path must not be null.");

                    using (var ms = new MemoryStream(new byte[] { 0x41 })) {
                        doc.GetFolder("Alex").GetFileByTitle("A").Write(ms);
                    }

                    var directories = new List<DirectoryInfo>(testCC.Directory.GetDirectories());
                    Assert.AreEqual(1, directories.Count);
                    Assert.AreEqual("Alex", directories[0].Name);

                    {
                        var files = new List<FileInfo>(directories[0].GetFiles());
                        Assert.AreEqual(1, files.Count);
                        Assert.AreEqual("A.txt", files[0].Name);
                        Assert.AreEqual("A", File.ReadAllText(files[0].FullName));
                        Assert.AreEqual(1, files[0].Length);
                    }
                }
            }
        }

    }
}