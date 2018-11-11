using System;
using System.IO;

namespace QTextTest {
    internal class TestDirectory : IDisposable {

        public TestDirectory(bool dontCreate = false) {
            Directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "QText.Test " + Guid.NewGuid().ToString()));
            if (!dontCreate) {
                Directory.Create();
            }
        }

        ~TestDirectory() {
            ((IDisposable)this).Dispose();
        }


        public DirectoryInfo Directory { get; private set; }


        public void CreateRawFile(string fileName) {
            var file = new FileInfo(Path.Combine(Directory.FullName, fileName));
            if (!file.Directory.Exists) { file.Directory.Create(); }
            file.Create().Close();
        }


        void IDisposable.Dispose() {
            if (Directory.Exists) { Directory.Delete(true); }
            GC.SuppressFinalize(this);
        }

    }
}
