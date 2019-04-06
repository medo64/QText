/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (NormalizeStringsToUppercase).
//2008-01-22: Returns DialogResult for ShowDialog methods.
//2008-01-22: Changed "Print preview..." to "Print preview"
//2008-01-07: Fixed bug that caused all keys to close form instead of just Escape.
//2008-01-03: New version.


using System.Windows.Forms;

namespace Medo.Windows.Forms {

	/// <summary>
	/// Represents a dialog box form that contains a System.Windows.Forms.PrintPreviewControl.
	/// </summary>
	public class PrintPreviewDialog : System.Windows.Forms.PrintPreviewDialog {

		/// <summary>
		/// Initializes new instance.
		/// </summary>
		public PrintPreviewDialog(System.Drawing.Printing.PrintDocument document)
			: this() {
			base.Document = document;
		}

		/// <summary>
		/// Initializes new instance.
		/// </summary>
		public PrintPreviewDialog()
			: base() {
            if (this is System.Windows.Forms.Form ppdForm) {
                ppdForm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ppdForm_KeyPress);
                ppdForm.Load += new System.EventHandler(ppdForm_Load);
                ppdForm.Icon = Resources.TransparentIcon;
                ppdForm.ShowIcon = true;
                ppdForm.Text = Resources.Caption;
                ppdForm.KeyPreview = true;
                for (var i = 0; i < ppdForm.Controls.Count; i++) {
                    var iControl = ppdForm.Controls[i];
                    if (iControl is System.Windows.Forms.ToolStrip iToolstrip) {
                        for (var j = 0; j < iToolstrip.Items.Count; j++) {
                            var jItem = iToolstrip.Items[j];
                            switch (jItem.Name) {
                                case "printToolStripButton":
                                    jItem.Text = Resources.Print;
                                    break;
                                case "zoomToolStripSplitButton":
                                    jItem.Text = Resources.Zoom;
                                    break;
                                case "onepageToolStripButton":
                                    jItem.Text = Resources.OnePage;
                                    break;
                                case "twopagesToolStripButton":
                                    jItem.Text = Resources.TwoPages;
                                    break;
                                case "threepagesToolStripButton":
                                    jItem.Text = Resources.ThreePages;
                                    break;
                                case "fourpagesToolStripButton":
                                    jItem.Text = Resources.FourPages;
                                    break;
                                case "sixpagesToolStripButton":
                                    jItem.Text = Resources.SixPages;
                                    break;
                                case "closeToolStripButton":
                                    jItem.Text = Resources.Close;
                                    break;
                                case "pageToolStripLabel":
                                    jItem.Text = Resources.Page;
                                    break;
                                default:
                                    break;
                            }//switch
                        }//for(j)
                    }//if
                }//for(i)
            }//if
        }


		/// <summary>
		/// Displays the control to the user.
		/// </summary>
		public new void Show() {
			Show(null);
		}

		/// <summary>
		/// Shows the form with the specified owner to the user.
		/// </summary>
		/// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window and represents the top-level window that will own this form.</param>
		/// <exception cref="System.ArgumentException">The form specified in the owner parameter is the same as the form being shown.</exception>
		public new void Show(System.Windows.Forms.IWin32Window owner) {
			var ppdForm = this as System.Windows.Forms.Form;
			if (owner != null) {
				if (ppdForm != null) {
					ppdForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
				}
				base.Show(owner);
			} else {
				if (ppdForm != null) {
					ppdForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
				}
				base.Show();
			}
		}

		/// <summary>
		/// Shows the form as a modal dialog box with the currently active window set as its owner.
		/// </summary>
		/// <return>One of the System.Windows.Forms.DialogResult values.</return>
		/// <exception cref="System.InvalidOperationException">The form being shown is already visible.-or- The form being shown is disabled.-or- The form being shown is not a top-level window.-or- The form being shown as a dialog box is already a modal form.</exception>
		/// <exception cref="System.InvalidOperationException">The current process is not running in user interactive mode. For more information, see System.Windows.Forms.SystemInformation.UserInteractive.</exception>
		/// <exception cref="System.ArgumentException">The form specified in the owner parameter is the same as the form being shown.</exception>
		public new DialogResult ShowDialog() {
			return ShowDialog(null);
		}

		/// <summary>
		/// Shows the form as a modal dialog with the specified owner.
		/// </summary>
		/// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog.</param>
		/// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
		/// <exception cref="System.InvalidOperationException">The form being shown is already visible.-or- The form being shown is disabled.-or- The form being shown is not a top-level window.-or- The form being shown as a dialog box is already a modal form.</exception>
		/// <exception cref="System.InvalidOperationException">The current process is not running in user interactive mode. For more information, see System.Windows.Forms.SystemInformation.UserInteractive.</exception>
		/// <exception cref="System.ArgumentException">The form specified in the owner parameter is the same as the form being shown.</exception>
		public new DialogResult ShowDialog(System.Windows.Forms.IWin32Window owner) {
			var ppdForm = this as System.Windows.Forms.Form;
			if (owner != null) {
				if (ppdForm != null) {
					ppdForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
				}
				return base.ShowDialog(owner);
			} else {
				if (ppdForm != null) {
					ppdForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
				}
				return base.ShowDialog();
			}
		}


		#region Events

		private void ppdForm_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (e.KeyChar == System.Convert.ToChar(27)) {
                if (sender is System.Windows.Forms.Form form) {
                    form.Close();
                }
            }
		}

        private void ppdForm_Load(object sender, System.EventArgs e) {
            if (sender is System.Windows.Forms.Form) {
                WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
        }

		#endregion


		private static class Resources {

			internal static string Caption {
				get { return GetInCurrentLanguage("Print preview", "Pregled ispisa"); }
			}

			internal static string Print {
				get { return GetInCurrentLanguage("Print", "Ispis"); }
			}

			internal static string Zoom {
				get { return GetInCurrentLanguage("Zoom", "Povećanje"); }
			}

			internal static string OnePage {
				get { return GetInCurrentLanguage("One page", "Jedna stranica"); }
			}

			internal static string TwoPages {
				get { return GetInCurrentLanguage("Two pages", "Dvije stranice"); }
			}

			internal static string ThreePages {
				get { return GetInCurrentLanguage("Three pages", "Tri stranice"); }
			}

			internal static string FourPages {
				get { return GetInCurrentLanguage("Four pages", "Četiri stranice"); }
			}

			internal static string SixPages {
				get { return GetInCurrentLanguage("Six pages", "Šest stranica"); }
			}

			internal static string Close {
				get { return GetInCurrentLanguage("&Close", "&Zatvori"); }
			}

			internal static string Page {
				get { return GetInCurrentLanguage("&Page", "&Stranica"); }
			}

			internal static System.Drawing.Icon TransparentIcon {
				get {
					var iconBuffer = System.Convert.FromBase64String("AAABAAEAEBAQAAAABAAoAQAAFgAAACgAAAAQAAAAIAAAAAEABAAAAAAAgAAAAAAAAAAAAAAAEAAAABAAAAAAAAAAAACAAACAAAAAgIAAgAAAAIAAgACAgAAAgICAAMDAwAAAAP8AAP8AAAD//wD/AAAA/wD/AP//AAD///8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA");
					return new System.Drawing.Icon(new System.IO.MemoryStream(iconBuffer));
				}
			}


			private static string GetInCurrentLanguage(string en_US, string hr_HR) {
				switch (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToUpperInvariant()) {
					case "EN":
					case "EN-US":
					case "EN-GB":
						return en_US;

					case "HR":
					case "HR-HR":
					case "HR-BA":
						return hr_HR;

					default:
						return en_US;
				}
			}

		}

	}

}
