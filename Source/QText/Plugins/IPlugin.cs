using System.Collections.Generic;
using System.Windows.Forms;

namespace QText.Plugins {
    internal interface IPlugin {

        void Initialize(TrayContext trayContext);
        void Terminate();

        IEnumerable<ToolStripItem> GetToolStripItems();

    }
}
