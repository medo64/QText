//Josip Medved <jmedved@jmedved.com>   www.medo64.com

//2012-05-15: Initial version.


using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Medo.Security.Cryptography {

    /// <summary>
    /// Defines an Open-SSL compatible AES stream.
    /// </summary>
    public class OpenSslAesStream : Stream {

        /// <summary>
        /// Creates new instance using 256 bit AES in CBC mode.
        /// </summary>
        /// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
        /// <param name="password">Password used for key and IV derivation. UTF-8 encoding is assumed.</param>
        /// <param name="streamMode">The cryptographic transformation that is to be performed on the stream.</param>
        /// <exception cref="System.ArgumentNullException">Stream cannot be null. -or- Password cannot be null.</exception>
        /// <exception cref="System.ArgumentException">Stream mode must be either Read or Write. -or- Stream is not readable. -or- Stream is not writable.</exception>
        /// <exception cref="System.IO.InvalidDataException">Unexpected end of stream. -or- Salted stream expected.</exception>
        public OpenSslAesStream(Stream stream, string password, CryptoStreamMode streamMode)
            : this(stream, UTF8Encoding.UTF8.GetBytes(password), streamMode, 256, CipherMode.CBC) {
        }

        /// <summary>
        /// Creates new instance using 256 bit AES in CBC mode.
        /// </summary>
        /// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
        /// <param name="password">Password used for key and IV derivation.</param>
        /// <param name="streamMode">The cryptographic transformation that is to be performed on the stream.</param>
        /// <exception cref="System.ArgumentNullException">Stream cannot be null. -or- Password cannot be null.</exception>
        /// <exception cref="System.ArgumentException">Stream mode must be either Read or Write. -or- Stream is not readable. -or- Stream is not writable.</exception>
        /// <exception cref="System.IO.InvalidDataException">Unexpected end of stream. -or- Salted stream expected.</exception>
        public OpenSslAesStream(Stream stream, byte[] password, CryptoStreamMode streamMode)
            : this(stream, password, streamMode, 256, CipherMode.CBC) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
        /// <param name="password">Password used for key and IV derivation. UTF-8 encoding is assumed.</param>
        /// <param name="streamMode">The cryptographic transformation that is to be performed on the stream.</param>
        /// <param name="keySize">Size of key.</param>
        /// <param name="cipherMode">The mode for operation of the symmetric algorithm.</param>
        /// <exception cref="System.ArgumentNullException">Stream cannot be null. -or- Password cannot be null.</exception>
        /// <exception cref="System.ArgumentException">Stream mode must be either Read or Write. -or- Stream is not readable. -or- Stream is not writable. -or- Key size mode must be either 128, 192 or 256 bits. -or- Cipher mode must be either CBC or OFB.</exception>
        /// <exception cref="System.IO.InvalidDataException">Unexpected end of stream. -or- Salted stream expected.</exception>
        public OpenSslAesStream(Stream stream, string password, CryptoStreamMode streamMode, int keySize, CipherMode cipherMode)
            : this(stream, UTF8Encoding.UTF8.GetBytes(password), streamMode, keySize, cipherMode) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
        /// <param name="password">Password used for key and IV derivation.</param>
        /// <param name="streamMode">The cryptographic transformation that is to be performed on the stream.</param>
        /// <param name="keySize">Size of key.</param>
        /// <param name="cipherMode">The mode for operation of the symmetric algorithm.</param>
        /// <exception cref="System.ArgumentNullException">Stream cannot be null. -or- Password cannot be null.</exception>
        /// <exception cref="System.ArgumentException">Stream mode must be either Read or Write. -or- Stream is not readable. -or- Stream is not writable. -or- Key size mode must be either 128, 192 or 256 bits. -or- Cipher mode must be either CBC or OFB.</exception>
        /// <exception cref="System.IO.InvalidDataException">Unexpected end of stream. -or- Salted stream expected.</exception>
        public OpenSslAesStream(Stream stream, byte[] password, CryptoStreamMode streamMode, int keySize, CipherMode cipherMode) {
            if (stream == null) { throw new ArgumentNullException("stream", "Stream cannot be null."); }
            if (password == null) { throw new ArgumentNullException("password", "Password cannot be null."); }
            if ((streamMode != CryptoStreamMode.Read) && (streamMode != CryptoStreamMode.Write)) { throw new ArgumentException("Stream mode must be either Read or Write.", "streamMode"); }
            if ((streamMode == CryptoStreamMode.Read) && (stream.CanRead == false)) { throw new ArgumentException("Stream is not readable.", "stream"); }
            if ((streamMode == CryptoStreamMode.Write) && (stream.CanWrite == false)) { throw new ArgumentException("Stream is not writable.", "stream"); }
            if ((keySize != 128) && (keySize != 192) && (keySize != 256)) { throw new ArgumentException("Key size mode must be either 128, 192 or 256 bits.", "keySize"); }
            if ((cipherMode != CipherMode.CBC) && (cipherMode != CipherMode.ECB)) { throw new ArgumentException("Cipher mode must be either CBC or OFB.", "cipherMode"); }

            if (streamMode == CryptoStreamMode.Read) {
                var buffer = new byte[16];
                var len = stream.Read(buffer, 0, 16);
                if (len < 16) { throw new InvalidDataException("Unexpected end of stream."); }
                for (int i = 0; i < 8; i++) {
                    if (buffer[i] != SaltedTextCache[i]) { throw new InvalidDataException("Salted stream expected."); }
                }
                var salt = new byte[8];
                Buffer.BlockCopy(buffer, 8, salt, 0, 8);

                GenerateKeyAndIV(password, salt, keySize, 128, out var key, out var iv);
                using (var aes = new RijndaelManaged()) {
                    aes.BlockSize = 128;
                    aes.KeySize = keySize;
                    aes.Mode = cipherMode;
                    aes.Padding = PaddingMode.PKCS7;
                    Transform = aes.CreateDecryptor(key, iv);
                    Stream = new CryptoStream(stream, Transform, CryptoStreamMode.Read);
                }
            } else {
                var salt = new byte[8];
                Rnd.GetBytes(salt);
                stream.Write(SaltedTextCache, 0, 8);
                stream.Write(salt, 0, 8);

                GenerateKeyAndIV(password, salt, keySize, 128, out var key, out var iv);
                using (var aes = new RijndaelManaged()) {
                    aes.BlockSize = 128;
                    aes.KeySize = keySize;
                    aes.Mode = cipherMode;
                    aes.Padding = PaddingMode.PKCS7;
                    Transform = aes.CreateEncryptor(key, iv);
                    Stream = new CryptoStream(stream, Transform, CryptoStreamMode.Write);
                }
            }
        }


        private readonly CryptoStream Stream;
        private readonly ICryptoTransform Transform;

        private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();
        private static byte[] SaltedTextCache = new byte[] { 0x53, 0x61, 0x6c, 0x74, 0x65, 0x64, 0x5f, 0x5f }; //Salted__


        /// <summary>
        /// Gets a value indicating whether the current System.Security.Cryptography.CryptoStream is readable.
        /// </summary>
        /// <value>True if the current stream is readable; otherwise, false.</value>
        public override bool CanRead {
            get { return Stream.CanRead; }
        }

        /// <summary>
        /// Gets a value indicating whether you can seek within the current System.Security.Cryptography.CryptoStream.
        /// </summary>
        /// <value>Always false.</value>
        public override bool CanSeek {
            get { return Stream.CanSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current System.Security.Cryptography.CryptoStream is writable.
        /// </summary>
        /// <value>True if the current stream is writable; otherwise, false.</value>
        public override bool CanWrite {
            get { return Stream.CanWrite; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush() {
            Stream.FlushFinalBlock();
            Stream.Flush();
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value>This property is not supported.</value>
        /// <exception cref="System.NotSupportedException">This property is not supported.</exception>
        public override long Length {
            get { return Stream.Length; }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <value>This property is not supported.</value>
        /// <exception cref="System.NotSupportedException">This property is not supported.</exception>
        public override long Position {
            get { return Stream.Position; }
            set { Stream.Position = value; }
        }

        /// <summary>
        /// Reads a sequence of bytes and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. A maximum of count bytes are read from the current stream and stored in buffer.</param>
        /// <param name="offset">The byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero if the end of the stream has been reached.</returns>
        /// <exception cref="System.NotSupportedException">The System.Security.Cryptography.CryptoStreamMode associated with current System.Security.Cryptography.CryptoStream object does not match the underlying stream. For example, this exception is thrown when using System.Security.Cryptography.CryptoStreamMode.Read with an underlying stream that is write only.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The offset parameter is less than zero.-or- The count parameter is less than zero.</exception>
        /// <exception cref="System.ArgumentException">The sum of the count and offset parameters is longer than the length of the buffer.</exception>
        public override int Read(byte[] buffer, int offset, int count) {
            return Stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A System.IO.SeekOrigin object indicating the reference point used to obtain the new position.</param>
        /// <returns>This method is not supported.</returns>
        /// <exception cref="System.NotSupportedException">This method is not supported.</exception>
        public override long Seek(long offset, SeekOrigin origin) {
            return Stream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="System.NotSupportedException">This property exists only to support inheritance from System.IO.Stream, and cannot be used.</exception>
        public override void SetLength(long value) {
            Stream.SetLength(value);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current System.Security.Cryptography.CryptoStream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="System.NotSupportedException">The System.Security.Cryptography.CryptoStreamMode associated with current System.Security.Cryptography.CryptoStream object does not match the underlying stream. For example, this exception is thrown when using System.Security.Cryptography.CryptoStreamMode.Write with an underlying stream that is read only.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The offset parameter is less than zero.-or- The count parameter is less than zero.</exception>
        /// <exception cref="System.ArgumentException">The sum of the count and offset parameters is longer than the length of the buffer.</exception>
        public override void Write(byte[] buffer, int offset, int count) {
            Stream.Write(buffer, offset, count);
        }


        #region Disposing

        /// <summary>
        /// Releases the unmanaged resources used and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                Stream.Dispose();
                Transform.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion


        #region Helper

        private static void GenerateKeyAndIV(byte[] password, byte[] salt, int keySize, int blockSize, out byte[] key, out byte[] iv) {
            using (var md5 = MD5.Create()) {
                using (var ms = new MemoryStream((keySize + blockSize) / 8)) {
                    var hash = new byte[] { };
                    for (int i = 0; i < (keySize + blockSize); i += md5.HashSize) {
                        byte[] step = new byte[hash.Length + password.Length + salt.Length];
                        Buffer.BlockCopy(hash, 0, step, 0, hash.Length);
                        Buffer.BlockCopy(password, 0, step, hash.Length, password.Length);
                        Buffer.BlockCopy(salt, 0, step, hash.Length + password.Length, salt.Length);
                        hash = md5.ComputeHash(step);
                        ms.Write(hash, 0, hash.Length);
                    }

                    ms.Position = 0;

                    key = new byte[keySize / 8];
                    ms.Read(key, 0, key.Length);

                    iv = new byte[blockSize / 8];
                    ms.Read(iv, 0, iv.Length);
                }
            }
        }

        #endregion

    }
}
