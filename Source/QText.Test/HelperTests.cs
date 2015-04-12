using Microsoft.VisualStudio.TestTools.UnitTesting;
using QText;

namespace QTextTest {

    [TestClass()]
    public class HelperTests {

        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void EncodeFileName_TestColon() {
            var result = Helper.EncodeFileName("1:2");
            Assert.AreEqual("1~3a~2", result);
        }

        [TestMethod()]
        public void EncodeFileName_TestAllPrintable() {
            var result = Helper.EncodeFileName(@"A""<>|:*?\/Z");
            Assert.AreEqual("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z", result);
        }

        [TestMethod()]
        public void DecodeFileName_TestAllPrintable() {
            var result = Helper.DecodeFileName(@"A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z");
            Assert.AreEqual(@"A""<>|:*?\/Z", result);
        }

        [TestMethod()]
        public void DecodeFileName_TestAccidental1() {
            var result = Helper.DecodeFileName("~");
            Assert.AreEqual("~", result);
        }

        [TestMethod()]
        public void DecodeFileName_TestAccidental2() {
            var result = Helper.DecodeFileName("~1");
            Assert.AreEqual("~1", result);
        }

        [TestMethod()]
        public void DecodeFileName_TestAccidental3() {
            var result = Helper.DecodeFileName("~1~7c~~");
            Assert.AreEqual("~1|~", result);
        }

        [TestMethod()]
        public void DecodeFileName_TestAccidental4() {
            var result = Helper.DecodeFileName("A~7c1~~");
            Assert.AreEqual("A~7c1~~", result);
        }

        [TestMethod()]
        public void DecodeFileName_TestAccidental5() {
            var result = Helper.DecodeFileName("~77~");
            Assert.AreEqual("~77~", result);
        }

    }
}
