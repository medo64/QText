using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace QTextTest {

    [TestClass()]
    public class DocumentCopierTests {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void DocumentCopier_New() {
            using (var src = new TestDirectory())
            using (var dst = new TestDirectory(dontCreate: true)) {
                src.CreateRawFile("A.txt");
                src.CreateRawFile("B.txt");
                src.CreateRawFile("C.txt");
                src.CreateRawFile("Alex\\D.txt");
                src.CreateRawFile("Alex\\E.txt");
                src.CreateRawFile("Steve\\F.txt");
                src.CreateRawFile("Steve\\Steve.Inner\\G.txt");

                var doc = new Document(src.Directory.FullName);
                doc.RootFolder.GetFileByName("A").OrderAfter(doc.RootFolder.GetFileByName("C"));
                doc.WriteOrder();

                var copier = new DocumentCopier(doc, dst.Directory.FullName);
                Assert.IsFalse(copier.DestinationRootAlreadyExisted);

                copier.CopyAll();

                Assert.AreEqual("Alex", dst.Directory.GetDirectories()[0].Name);
                Assert.AreEqual("Steve", dst.Directory.GetDirectories()[1].Name);
                Assert.AreEqual("Steve.Inner", dst.Directory.GetDirectories()[1].GetDirectories()[0].Name);

                Assert.AreEqual(".qtext", dst.Directory.GetFiles()[0].Name);
                Assert.AreEqual("A.txt", dst.Directory.GetFiles()[1].Name);
                Assert.AreEqual("B.txt", dst.Directory.GetFiles()[2].Name);
                Assert.AreEqual("C.txt", dst.Directory.GetFiles()[3].Name);
                Assert.AreEqual("D.txt", dst.Directory.GetDirectories()[0].GetFiles()[0].Name);
                Assert.AreEqual("E.txt", dst.Directory.GetDirectories()[0].GetFiles()[1].Name);
                Assert.AreEqual("F.txt", dst.Directory.GetDirectories()[1].GetFiles()[0].Name);
                Assert.AreEqual("G.txt", dst.Directory.GetDirectories()[1].GetDirectories()[0].GetFiles()[0].Name);

                doc = copier.GetDestinationDocument();
                var files = new List<DocumentFile>(doc.RootFolder.GetFiles());
                Assert.AreEqual(3, files.Count);
                Assert.AreEqual("B", files[0].Name);
                Assert.AreEqual("C", files[1].Name);
                Assert.AreEqual("A", files[2].Name);
            }
        }

        [TestMethod()]
        public void DocumentCopier_DestinationPresent() {
            using (var src = new TestDirectory())
            using (var dst = new TestDirectory()) {
                src.CreateRawFile("A.txt");
                src.CreateRawFile("B.txt");
                src.CreateRawFile("C.txt");
                src.CreateRawFile("Alex\\D.txt");
                src.CreateRawFile("Alex\\E.txt");
                src.CreateRawFile("Steve\\F.txt");
                src.CreateRawFile("Steve\\Steve.Inner\\G.txt");

                var doc = new Document(src.Directory.FullName);
                doc.RootFolder.GetFileByName("A").OrderAfter(doc.RootFolder.GetFileByName("C"));
                doc.WriteOrder();

                var copier = new DocumentCopier(doc, dst.Directory.FullName);
                Assert.IsTrue(copier.DestinationRootAlreadyExisted);

                copier.CopyAll();

                Assert.AreEqual("Alex", dst.Directory.GetDirectories()[0].Name);
                Assert.AreEqual("Steve", dst.Directory.GetDirectories()[1].Name);
                Assert.AreEqual("Steve.Inner", dst.Directory.GetDirectories()[1].GetDirectories()[0].Name);

                Assert.AreEqual(".qtext", dst.Directory.GetFiles()[0].Name);
                Assert.AreEqual("A.txt", dst.Directory.GetFiles()[1].Name);
                Assert.AreEqual("B.txt", dst.Directory.GetFiles()[2].Name);
                Assert.AreEqual("C.txt", dst.Directory.GetFiles()[3].Name);
                Assert.AreEqual("D.txt", dst.Directory.GetDirectories()[0].GetFiles()[0].Name);
                Assert.AreEqual("E.txt", dst.Directory.GetDirectories()[0].GetFiles()[1].Name);
                Assert.AreEqual("F.txt", dst.Directory.GetDirectories()[1].GetFiles()[0].Name);
                Assert.AreEqual("G.txt", dst.Directory.GetDirectories()[1].GetDirectories()[0].GetFiles()[0].Name);

                doc = copier.GetDestinationDocument();
                var files = new List<DocumentFile>(doc.RootFolder.GetFiles());
                Assert.AreEqual(3, files.Count);
                Assert.AreEqual("B", files[0].Name);
                Assert.AreEqual("C", files[1].Name);
                Assert.AreEqual("A", files[2].Name);
            }
        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DocumentCopier_NotInOwn() {
            using (var src = new TestDirectory()) {
                src.CreateRawFile("A.txt");
                src.CreateRawFile("B.txt");
                src.CreateRawFile("C.txt");
                src.CreateRawFile("Alex\\D.txt");
                src.CreateRawFile("Alex\\E.txt");
                src.CreateRawFile("Steve\\F.txt");
                src.CreateRawFile("Steve\\Steve.Inner\\G.txt");

                var doc = new Document(src.Directory.FullName);

                var copier = new DocumentCopier(doc, doc.RootPath);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DocumentCopier_NotInOwnTree() {
            using (var src = new TestDirectory()) {
                src.CreateRawFile("A.txt");
                src.CreateRawFile("B.txt");
                src.CreateRawFile("C.txt");
                src.CreateRawFile("Alex\\D.txt");
                src.CreateRawFile("Alex\\E.txt");
                src.CreateRawFile("Steve\\F.txt");
                src.CreateRawFile("Steve\\Steve.Inner\\G.txt");

                var doc = new Document(src.Directory.FullName);

                var copier = new DocumentCopier(doc, Path.Combine(doc.RootPath, "XXX"));
            }
        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DocumentCopier_NotInCarbonCopy() {
            using (var src = new TestDirectory())
            using (var cc = new TestDirectory()) {
                src.CreateRawFile("A.txt");
                src.CreateRawFile("B.txt");
                src.CreateRawFile("C.txt");
                src.CreateRawFile("Alex\\D.txt");
                src.CreateRawFile("Alex\\E.txt");
                src.CreateRawFile("Steve\\F.txt");
                src.CreateRawFile("Steve\\Steve.Inner\\G.txt");

                var doc = new Document(src.Directory.FullName) {
                    CarbonCopyRootPath = cc.Directory.FullName
                };

                var copier = new DocumentCopier(doc, doc.CarbonCopyRootPath);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DocumentCopier_NotInCarbonCopyTree() {
            using (var src = new TestDirectory())
            using (var cc = new TestDirectory()) {
                src.CreateRawFile("A.txt");
                src.CreateRawFile("B.txt");
                src.CreateRawFile("C.txt");
                src.CreateRawFile("Alex\\D.txt");
                src.CreateRawFile("Alex\\E.txt");
                src.CreateRawFile("Steve\\F.txt");
                src.CreateRawFile("Steve\\Steve.Inner\\G.txt");

                var doc = new Document(src.Directory.FullName) {
                    CarbonCopyRootPath = cc.Directory.FullName
                };

                var copier = new DocumentCopier(doc, Path.Combine(doc.CarbonCopyRootPath, "XXX"));
            }
        }

    }
}