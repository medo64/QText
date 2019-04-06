/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2008-03-29: Added static constructor in DEBUG configuration.
//2007-12-31: New version.


namespace Medo.Resources {

	/// <summary>
	/// Retrieval of common data types from resources.
	/// This class is thread-safe.
	/// </summary>
	public static class ManifestResources {

		private static readonly object _syncRoot = new object();


#if DEBUG
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification="This is only in DEBUG compilations.")]
        static ManifestResources() {
			System.Diagnostics.Debug.WriteLine("I: Resource names (");
			string[] names = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceNames();
			for (int i = 0; i < names.Length; ++i) {
				System.Diagnostics.Debug.WriteLine("    " + names[i]);
			}
            System.Diagnostics.Debug.WriteLine(").   {{Medo.Resources.ManifestResources}}");
		}
#endif


		/// <summary>
		/// Reads bitmap from resource stream.
		/// </summary>
		/// <param name="name">Name in form "project.resourceName".</param>
		/// <returns>Resource bitmap.</returns>
		public static System.Drawing.Bitmap GetBitmap(string name) {
			lock (_syncRoot) {
				return new System.Drawing.Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream(name));
			}
		}

		/// <summary>
		/// Reads icon from resource stream.
		/// </summary>
		/// <param name="name">Name in form "project.resourceName".</param>
		/// <returns>First icon in icon resource.</returns>
		public static System.Drawing.Icon GetIcon(string name) {
			lock (_syncRoot) {
				return new System.Drawing.Icon(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream(name));
			}
		}

		/// <summary>
		/// Reads icon from resource stream and resizes it if needed.
		/// </summary>
		/// <param name="name">Name in form "project.resourceName".</param>
		/// <param name="height">Desired icon height.</param>
		/// <param name="width">Desired icon width.</param>
		/// <returns>Icon nearest to width and height from icon resource, resized if neccessary.</returns>
		public static System.Drawing.Icon GetIcon(string name, int width, int height) {
			lock (_syncRoot) {
				return new System.Drawing.Icon(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream(name), width, height);
			}
		}

	}

}
