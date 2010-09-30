//Josip Medved <jmedved@jmedved.com> http://www.jmedved.com

//2007-09-19: New version.
//2007-12-24: Added WriteStartDocument method.



namespace Medo.Xml {

	/// <summary>
	/// Some shortcut methods to use when creating XML file.
	/// </summary>
	public class XmlTagWriter : System.IDisposable {

		/// <summary>
		/// Creates an instance of the class using the specified file.
		/// </summary>
		/// <param name="fileName">The filename to write to. If the file exists, it truncates it and overwrites it with the new content.</param>
		/// <param name="encoding">The encoding to generate. If encoding is null it writes the file out as UTF-8, and omits the encoding attribute from the ProcessingInstruction.</param>
		public XmlTagWriter(string fileName, System.Text.Encoding encoding) {
			this.XmlTextWriter = new System.Xml.XmlTextWriter(fileName, encoding);
			this.XmlTextWriter.Formatting = System.Xml.Formatting.Indented;
		}

		/// <summary>
		/// Creates an instance of the class using the specified stream and encoding.
		/// </summary>
		/// <param name="stream">The stream to which you want to write.</param>
		/// <param name="encoding">The encoding to generate. If encoding is null it writes the file out as UTF-8, and omits the encoding attribute from the ProcessingInstruction.</param>
		public XmlTagWriter(System.IO.Stream stream, System.Text.Encoding encoding) {
			this.XmlTextWriter = new System.Xml.XmlTextWriter(stream, encoding);
			this.XmlTextWriter.Formatting = System.Xml.Formatting.Indented;
		}

		/// <summary>
		/// Creates an instance of the class using the specified System.IO.TextWriter.
		/// </summary>
		/// <param name="textWriter">The TextWriter to write to. It is assumed that the TextWriter is already set to the correct encoding.</param>
		public XmlTagWriter(System.IO.TextWriter textWriter) {
			this.XmlTextWriter = new System.Xml.XmlTextWriter(textWriter);
			this.XmlTextWriter.Formatting = System.Xml.Formatting.Indented;
		}


		/// <summary>
		/// Writes the XML declaration with the version "1.0".
		/// </summary>
		public void WriteStartDocument() {
			this.XmlTextWriter.WriteStartDocument();
		}

		/// <summary>
		/// Writes raw markup manually from a character buffer.
		/// </summary>
		/// <param name="buffer">Character array containing the text to write.</param>
		/// <param name="index">The position within the buffer indicating the start of the text to write.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Index or count is less than zero.-or-The buffer length minus index is less than count.</exception>
		/// <exception cref="System.ArgumentNullException">Buffer is null.</exception>
		public void WriteRawLine(char[] buffer, int index, int count) {
			this.XmlTextWriter.WriteRaw(buffer, index, count);
			this.XmlTextWriter.WriteRaw("\n");
		}

		/// <summary>
		/// Writes raw markup manually from a string and appends it with NewLine characters.
		/// </summary>
		/// <param name="data">String containing the text to write.</param>
		public void WriteRawLine(string data) {
			this.XmlTextWriter.WriteRaw(data);
			this.XmlTextWriter.WriteRaw("\n");
		}


		/// <summary>
		/// Writes out a start tag with the specified local name.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		public void StartTag(string localName) {
			this.XmlTextWriter.WriteStartElement(localName);
		}

		/// <summary>
		/// Writes out a start tag with the specified local name and appends it with attributes.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="attributesAndValues">Attributes and their values. In case of uneven number of elements, string is appended.</param>
		public void StartTag(string localName, params string[] attributesAndValues) {
			if (localName == null) { return; }
			if (attributesAndValues == null) { return; }
			this.XmlTextWriter.WriteStartElement(localName);
			for (int i = 0; i < attributesAndValues.Length - 1; i += 2) {
				this.XmlTextWriter.WriteAttributeString(attributesAndValues[i], attributesAndValues[i + 1]);
			}
			if (attributesAndValues.Length % 2 != 0) {
				if (!string.IsNullOrEmpty(attributesAndValues[attributesAndValues.Length - 1])) {
					this.XmlTextWriter.WriteString(attributesAndValues[attributesAndValues.Length - 1]);
				}
			}
		}

		/// <summary>
		/// Closes one element and pops the corresponding namespace scope.
		/// </summary>
		public void EndTag() {
			this.XmlTextWriter.WriteEndElement();
		}

		/// <summary>
		/// Writes out both start and end tag with the specified local name.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		public void WriteTag(string localName) {
			this.StartTag(localName);
			this.EndTag();
		}

		/// <summary>
		/// Writes out both start and end tag with the specified local name and appends it with attributes.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="attributesAndValues">Attributes and their values. In case of uneven number of elements, string is appended.</param>
		public void WriteTag(string localName, params string[] attributesAndValues) {
			this.StartTag(localName, attributesAndValues);
			this.EndTag();
		}



		private System.Xml.XmlTextWriter _xmlTextWriter;
		/// <summary>
		/// Gets underlyings XmlTextWriter.
		/// </summary>
		public System.Xml.XmlTextWriter XmlTextWriter {
			get { return this._xmlTextWriter; }
			private set { this._xmlTextWriter = value; }
		}


		/// <summary>
		/// Closes the stream.
		/// </summary>
		public void Close() {
			this.XmlTextWriter.Close();
		}



		#region IDisposable Members

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
			System.GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
		protected virtual void Dispose(bool disposing) {
			this.Close();
		}

		#endregion
	}

}
