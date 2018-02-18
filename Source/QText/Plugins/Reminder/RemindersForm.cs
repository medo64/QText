using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QText.Plugins.Reminder {
    internal partial class RemindersForm : Form {
        public RemindersForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            lsvReminders.Font = SystemFonts.MessageBoxFont;

            Medo.Windows.Forms.State.SetupOnLoadAndClose(this, lsvReminders);
        }

        protected override bool ProcessDialogKey(Keys keyData) {
            switch (keyData) {
                case Keys.Insert:
                    btnAdd.PerformClick();
                    return true;
                case Keys.F2:
                    btnEdit.PerformClick();
                    return true;
                case Keys.Delete:
                    btnRemove.PerformClick();
                    return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void Form_Load(object sender, System.EventArgs e) {
            foreach (var reminder in Settings.Current.Reminders) {
                var lvi = new ListViewItem(new string[] { reminder.Time.ToString(ReminderData.TimeSpanFormat, CultureInfo.CurrentCulture), reminder.Description }) {
                    Checked = reminder.Enabled,
                    Tag = reminder
                };
                lsvReminders.Items.Add(lvi);
            }
            if (lsvReminders.Items.Count > 0) {
                lsvReminders.Items[0].Selected = true;
            }
        }


        private void lsvReminders_SelectedIndexChanged(object sender, System.EventArgs e) {
            btnEdit.Enabled = (lsvReminders.SelectedItems.Count > 0);
            btnRemove.Enabled = (lsvReminders.SelectedItems.Count > 0);
        }


        private void btnAdd_Click(object sender, System.EventArgs e) {
            using (var frm = new ReminderForm(null)) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    if (frm.Data != null) {
                        var lvi = new ListViewItem(new string[] { frm.Data.Time.ToString(ReminderData.TimeSpanFormat, CultureInfo.CurrentCulture), frm.Data.Description }) {
                            Checked = frm.Data.Enabled,
                            Tag = frm.Data
                        };
                        lsvReminders.Items.Add(lvi);
                        lvi.Selected = true;
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, System.EventArgs e) {
            if (lsvReminders.SelectedItems.Count > 0) {
                var lvi = lsvReminders.SelectedItems[0];
                var data = lsvReminders.SelectedItems[0].Tag as ReminderData;
                using (var frm = new ReminderForm(new ReminderData(lvi.Checked, data.Time, data.Description))) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        if (frm.Data != null) {
                            lvi.Text = frm.Data.Time.ToString(ReminderData.TimeSpanFormat, CultureInfo.CurrentCulture);
                            lvi.SubItems[1].Text = frm.Data.Description;
                            lvi.Checked = frm.Data.Enabled;
                            lvi.Tag = frm.Data;
                        }
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e) {
            if (lsvReminders.SelectedItems.Count > 0) {
                var index = lsvReminders.Items.IndexOf(lsvReminders.SelectedItems[0]);
                lsvReminders.Items.RemoveAt(index);
                if (lsvReminders.Items.Count > 0) {
                    index = Math.Min(Math.Max(index, 0), lsvReminders.Items.Count - 1);
                    lsvReminders.Items[index].Selected = true;
                }
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e) {
            var list = new List<ReminderData>();
            foreach (ListViewItem item in lsvReminders.Items) {
                var data = item.Tag as ReminderData;
                list.Add(new ReminderData(item.Checked, data.Time, data.Description));
            }
            Settings.Current.Reminders = list;
        }
    }
}
