using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;

namespace QTextTest {

    [TestClass()]
    public class HelperTests {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void EncodeTitle_Colon() {
            var result = Helper.EncodeTitle("1:2");
            Assert.AreEqual("1~3a~2", result);
        }


        [TestMethod()]
        public void EncodeTitle_AllPrintable() {
            var result = Helper.EncodeTitle(@"A""<>|:*?\/Z");
            Assert.AreEqual("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z", result);
        }

        [TestMethod()]
        public void DecodeTitle_AllPrintable() {
            var result = Helper.DecodeTitle(@"A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z");
            Assert.AreEqual(@"A""<>|:*?\/Z", result);
        }


        [TestMethod()]
        public void EncodeTitle_Tab() {
            var result = Helper.EncodeTitle("A\tZ");
            Assert.AreEqual("A~09~Z", result);
        }

        [TestMethod()]
        public void DecodeTitle_Tab() {
            var result = Helper.DecodeTitle(@"A~09~Z");
            Assert.AreEqual("A\tZ", result);
        }


        [TestMethod()]
        public void DecodeTitle_Accidental1() {
            var result = Helper.DecodeTitle("~");
            Assert.AreEqual("~", result);
        }

        [TestMethod()]
        public void DecodeTitle_Accidental2() {
            var result = Helper.DecodeTitle("~1");
            Assert.AreEqual("~1", result);
        }

        [TestMethod()]
        public void DecodeTitle_Accidental3() {
            var result = Helper.DecodeTitle("~1~7c~~");
            Assert.AreEqual("~1|~", result);
        }

        [TestMethod()]
        public void DecodeTitle_Accidental4() {
            var result = Helper.DecodeTitle("A~7c1~~");
            Assert.AreEqual("A~7c1~~", result);
        }

        [TestMethod()]
        public void DecodeTitle_Accidental5() {
            var result = Helper.DecodeTitle("~77~");
            Assert.AreEqual("~77~", result);
        }

    }
}
