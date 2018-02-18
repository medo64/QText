using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QText.Plugins.Reminder {
    internal partial class ReminderForm : Form {
        public ReminderForm(ReminderData data) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Data = data;
        }

        public ReminderData Data { get; private set; }


        private void Form_Load(object sender, System.EventArgs e) {
            if (this.Data != null) {
                chbTime.Checked = this.Data.Enabled;
                txtTime.Text = this.Data.Time.ToString(ReminderData.TimeSpanFormat, CultureInfo.CurrentCulture);
                txtDescription.Text = this.Data.Description;
            } else {
                var nextTime = DateTime.Now.AddMinutes(15);
                while ((nextTime.Minute % 10) != 0) { //find next "round" time
                    nextTime = nextTime.AddMinutes(1);
                }

                chbTime.Checked = true;
                txtTime.Text = nextTime.TimeOfDay.ToString(ReminderData.TimeSpanFormat, CultureInfo.CurrentCulture);
                txtTime.Select();
            }
        }


        private void txtTime_TextChanged(object sender, EventArgs e) {
            btnOK.Enabled = TimeSpan.TryParseExact(txtTime.Text.Trim(), ReminderData.TimeSpanFormat, CultureInfo.CurrentCulture, out var _);
        }


        private void btnOK_Click(object sender, EventArgs e) {
            if (TimeSpan.TryParseExact(txtTime.Text.Trim(), ReminderData.TimeSpanFormat, CultureInfo.CurrentCulture, out var time)) {
                this.Data = new ReminderData(chbTime.Enabled, time, txtDescription.Text);
            }
        }
    }
}
