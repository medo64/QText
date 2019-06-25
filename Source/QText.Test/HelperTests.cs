using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;

namespace QTextTest {

    [TestClass()]
    public class HelperTests {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void EncodeTitle_Colon() {
            var actualFile = Helper.EncodeFileTitle("1:2");
            Assert.AreEqual("1~3a~2", actualFile);

            var actualFolder = Helper.EncodeFolderTitle("1:2");
            Assert.AreEqual("1~3a~2", actualFolder);
        }


        [TestMethod()]
        public void EncodeTitle_AllPrintable() {
            var actualFile = Helper.EncodeFileTitle(@"A""<>|:*?\/Z");
            Assert.AreEqual("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z", actualFile);

            var actualFolder = Helper.EncodeFolderTitle(@"A""<>|:*?\/Z");
            Assert.AreEqual("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_AllPrintable() {
            var actualFile = Helper.DecodeFileTitle(@"A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z");
            Assert.AreEqual(@"A""<>|:*?\/Z", actualFile);

            var actualFolder = Helper.DecodeFolderTitle(@"A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z");
            Assert.AreEqual(@"A""<>|:*?\/Z", actualFolder);
        }


        [TestMethod()]
        public void EncodeTitle_Tab() {
            var actualFile = Helper.EncodeFileTitle("A\tZ");
            Assert.AreEqual("A~09~Z", actualFile);

            var actualFolder = Helper.EncodeFolderTitle("A\tZ");
            Assert.AreEqual("A~09~Z", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_Tab() {
            var actualFile = Helper.DecodeFileTitle(@"A~09~Z");
            Assert.AreEqual("A\tZ", actualFile);

            var actualFolder = Helper.DecodeFolderTitle(@"A~09~Z");
            Assert.AreEqual("A\tZ", actualFolder);
        }


        [TestMethod()]
        public void EncodeTitle_DotInMiddle() {
            var actualFile = Helper.EncodeFileTitle("A.Z");
            Assert.AreEqual("A.Z", actualFile);

            var actualFolder = Helper.EncodeFolderTitle("A.Z");
            Assert.AreEqual("A.Z", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_DotInMiddle() {
            var actualFile = Helper.DecodeFileTitle(@"A.Z");
            Assert.AreEqual("A.Z", actualFile);

            var actualFolder = Helper.DecodeFolderTitle(@"A.Z");
            Assert.AreEqual("A.Z", actualFolder);
        }

        [TestMethod()]
        public void EncodeTitle_DotAtEnd() {
            var actualFile = Helper.EncodeFileTitle(@"AZ.");
            Assert.AreEqual("AZ.", actualFile);

            var actualFolder = Helper.EncodeFolderTitle("AZ.");
            Assert.AreEqual("AZ~2e~", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_DotAtEnd() {
            var actualFile = Helper.DecodeFileTitle(@"AZ.");
            Assert.AreEqual("AZ.", actualFile);

            var actualFolder = Helper.DecodeFolderTitle(@"AZ~2e~");
            Assert.AreEqual("AZ.", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_AccidentalDotAtEnd() {
            var actualFile = Helper.DecodeFileTitle(@"AZ~2e~");
            Assert.AreEqual("AZ.", actualFile);

            var actualFolder = Helper.DecodeFolderTitle(@"AZ.");
            Assert.AreEqual("AZ.", actualFolder);
        }


        [TestMethod()]
        public void DecodeTitle_Accidental1() {
            var actualFile = Helper.DecodeFileTitle("~");
            Assert.AreEqual("~", actualFile);

            var actualFolder = Helper.DecodeFolderTitle("~");
            Assert.AreEqual("~", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_Accidental2() {
            var actualFile = Helper.DecodeFileTitle("~1");
            Assert.AreEqual("~1", actualFile);

            var actualFolder = Helper.DecodeFolderTitle("~1");
            Assert.AreEqual("~1", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_Accidental3() {
            var actualFile = Helper.DecodeFileTitle("~1~7c~~");
            Assert.AreEqual("~1|~", actualFile);

            var actualFolder = Helper.DecodeFolderTitle("~1~7c~~");
            Assert.AreEqual("~1|~", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_Accidental4() {
            var actualFile = Helper.DecodeFileTitle("A~7c1~~");
            Assert.AreEqual("A~7c1~~", actualFile);

            var actualFolder = Helper.DecodeFolderTitle("A~7c1~~");
            Assert.AreEqual("A~7c1~~", actualFolder);
        }

        [TestMethod()]
        public void DecodeTitle_Accidental5() {
            var actualFile = Helper.DecodeFileTitle("~77~");
            Assert.AreEqual("~77~", actualFile);

            var actualFolder = Helper.DecodeFolderTitle("~77~");
            Assert.AreEqual("~77~", actualFolder);
        }

    }
}
