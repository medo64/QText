//Josip Medved <jmedved@jmedved.com> http://www.jmedved.com

//2008-06-24: First version.


using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Medo.Windows.Forms
{

    /// <summary>
    /// Intercepting of windows hot key.
    /// </summary>
    public class Hotkey : IDisposable
    {

        private HotkeyWindow _window;
        static internal int _commonID;
        private int _id;
        private readonly object _syncRoot = new object();


        /// <summary>
        /// Creates new instance.
        /// </summary>
        public Hotkey()
        {
            lock (_syncRoot)
            {
                _id = _commonID; //An application must specify an unique id value in the range 0x0000 through 0xBFFF
                _commonID += 1;
            }
            this.Key = Keys.None;
            this.IsRegistered = false;
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
        public void Register(Keys key)
        {
            if (this.IsRegistered) { throw new System.InvalidOperationException("Already registered."); }


            Keys keyAlt = (key & Keys.Alt);
            Keys keyControl = (key & Keys.Control);
            Keys keyShift = (key & Keys.Shift);

            uint modValue = 0;
            if ((keyAlt == Keys.Alt))
                modValue += NativeMethods.MOD_ALT;
            if ((keyControl == Keys.Control))
                modValue += NativeMethods.MOD_CONTROL;
            if ((keyShift == Keys.Shift))
                modValue += NativeMethods.MOD_SHIFT;
            uint keyValue = (uint)key - (uint)keyAlt - (uint)keyControl - (uint)keyShift;

            this._window = new HotkeyWindow();
            this._window.CreateHandle(new CreateParams());
            this._window.HotkeyMessage += Window_HotkeyMessage;

            this.IsRegistered = !(NativeMethods.RegisterHotKey(this._window.Handle, _id, modValue, keyValue) == 0);
            if (!this.IsRegistered)
            {
                this._window.DestroyHandle();
                this._window = null;
                throw new System.InvalidOperationException("Registration failed.");
            }
            this.Key = key;
        }


        /// <summary>
        /// Removes hotkey registration.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">No registered hotkey.</exception>
        public void Unregister()
        {
            if (!this.IsRegistered) { throw new System.InvalidOperationException("No registered hotkey."); }

            this.IsRegistered = (NativeMethods.UnregisterHotKey(this._window.Handle, _id) == 0);
            if (this.IsRegistered == false)
            {
                this._window.DestroyHandle();
                this._window = null;
                this.Key = Keys.None;
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
        protected void OnHotkeyActivated(EventArgs e)
        {
            if (HotkeyActivated != null)
            {
                HotkeyActivated(this, e);
            }
        }

        private void Window_HotkeyMessage(object sender, System.EventArgs e)
        {
            this.OnHotkeyActivated(new EventArgs());
        }


        #region "IDisposable Support"

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsRegistered )
            {
                this.Unregister();
            }
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion


        private class HotkeyWindow : NativeWindow
        {

            internal event EventHandler<System.EventArgs> HotkeyMessage;


            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case NativeMethods.WM_HOTKEY:
                        if (HotkeyMessage != null)
                        {
                            HotkeyMessage(this, new System.EventArgs());
                        }
                        break;
                }
                base.WndProc(ref m);
            }

        }


        private static class NativeMethods
        {

            internal const uint MOD_ALT = 1;
            internal const uint MOD_CONTROL = 2;
            internal const uint MOD_SHIFT = 4;
            internal const uint MOD_WIN = 8;
            internal const int WM_HOTKEY = 786;

            [DllImport("user32.dll")]
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            internal static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [DllImport("user32.dll")]
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            internal static extern int UnregisterHotKey(IntPtr hWnd, int id);

        }

    }
}