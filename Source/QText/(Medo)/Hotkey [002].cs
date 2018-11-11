//Josip Medved <jmedved@jmedved.com>   www.medo64.com

//2012-11-24: Suppressing bogus CA5122 warning (http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical).
//2008-06-24: First version.


using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Medo.Windows.Forms {

    /// <summary>
    /// Intercepting of windows hot key.
    /// </summary>
    public class Hotkey : IDisposable {

        private HotkeyWindow _window;
        static internal int _commonID;
        private readonly int _id;
        private readonly object _syncRoot = new object();


        /// <summary>
        /// Creates new instance.
        /// </summary>
        public Hotkey() {
            lock (_syncRoot) {
                _id = _commonID; //An application must specify an unique id value in the range 0x0000 through 0xBFFF
                _commonID += 1;
            }
            Key = Keys.None;
            IsRegistered = false;
        }


        /// <summary>
        /// Occurs when hotkey is activated.
        /// </summary>
        public event EventHandler<System.EventArgs> HotkeyActivated;

        /// <summary>
        /// Registers hotkey.
        /// </summary>
        /// <param name="key">Key to register as hotkey.</param>
        /// <exception cref="System.InvalidOperationException">Already registered. -or - Registration failed.</exception>
        public void Register(Keys key) {
            if (IsRegistered) { throw new System.InvalidOperationException("Already registered."); }

            Keys keyAlt = (key & Keys.Alt);
            Keys keyControl = (key & Keys.Control);
            Keys keyShift = (key & Keys.Shift);

            uint modValue = 0;
            if ((keyAlt == Keys.Alt)) { modValue += NativeMethods.MOD_ALT; }

            if ((keyControl == Keys.Control)) { modValue += NativeMethods.MOD_CONTROL; }
            if ((keyShift == Keys.Shift)) { modValue += NativeMethods.MOD_SHIFT; }
            uint keyValue = (uint)key - (uint)keyAlt - (uint)keyControl - (uint)keyShift;

            _window = new HotkeyWindow();
            _window.CreateHandle(new CreateParams());
            _window.HotkeyMessage += Window_HotkeyMessage;

            IsRegistered = !(NativeMethods.RegisterHotKey(_window.Handle, _id, modValue, keyValue) == 0);
            if (!IsRegistered) {
                _window.DestroyHandle();
                _window = null;
                throw new System.InvalidOperationException("Registration failed.");
            }
            Key = key;
        }

        /// <summary>
        /// Removes hotkey registration.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">No registered hotkey.</exception>
        public void Unregister() {
            if (!IsRegistered) { throw new System.InvalidOperationException("No registered hotkey."); }

            IsRegistered = (NativeMethods.UnregisterHotKey(_window.Handle, _id) == 0);
            if (IsRegistered == false) {
                _window.DestroyHandle();
                _window = null;
                Key = Keys.None;
            }
        }

        /// <summary>
        /// Gets whether hotkey is registered.
        /// </summary>
        public bool IsRegistered { get; private set; }

        /// <summary>
        /// Gets key defined as hotkey.
        /// </summary>
        public Keys Key { get; private set; }

        /// <summary>
        /// Invoked when a HotkeyActivated routed event occurs.
        /// </summary>
        /// <param name="e">Event data</param>
        [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        protected void OnHotkeyActivated(EventArgs e) {
            HotkeyActivated?.Invoke(this, e);
        }

        private void Window_HotkeyMessage(object sender, System.EventArgs e) {
            OnHotkeyActivated(new EventArgs());
        }

        #region "IDisposable Support"

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        protected virtual void Dispose(bool disposing) {
            if (IsRegistered) {
                Unregister();
            }
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion


        private class HotkeyWindow : NativeWindow {

            internal event EventHandler<System.EventArgs> HotkeyMessage;

            protected override void WndProc(ref Message m) {
                switch (m.Msg) {
                    case NativeMethods.WM_HOTKEY:
                        HotkeyMessage?.Invoke(this, new System.EventArgs());
                        break;
                }
                base.WndProc(ref m);
            }

        }


        private static class NativeMethods {

            internal const uint MOD_ALT = 1;
            internal const uint MOD_CONTROL = 2;
            internal const uint MOD_SHIFT = 4;
            internal const uint MOD_WIN = 8;
            internal const int WM_HOTKEY = 786;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")] //http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical
            [DllImport("user32.dll")]
            internal static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")] //http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical
            [DllImport("user32.dll")]
            internal static extern int UnregisterHotKey(IntPtr hWnd, int id);

        }

    }
}
