using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QText {
    public partial class OptionsAdvancedForm : Form {
        public OptionsAdvancedForm() {
            InitializeComponent();
        }

        private void OptionsAdvancedForm_Load(object sender, EventArgs e) {
            prgSettings.SelectedObject = Settings.Current;
        }
    }
}
