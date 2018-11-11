using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QText.Plugins.Reminder {
    internal class ReminderPlugin : IPlugin {

        private TrayContext TrayContext;
        private BackgroundWorker Worker;


        public void Initialize(TrayContext trayContext) {
            TrayContext = trayContext;

            Worker = new BackgroundWorker() {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            Worker.RunWorkerAsync();

            Worker.DoWork += delegate {
                var minuteDigit = DateTime.Now.Minute;
                while (!Worker.CancellationPending) {
                    var time = DateTime.Now;
                    if (minuteDigit != time.Minute) {
                        minuteDigit = time.Minute;

                        var anyChanges = false;
                        var reminders = new List<ReminderData>(Settings.Current.Reminders);
                        var text = new StringBuilder();
                        foreach (var reminder in reminders) {
                            if (reminder.Enabled && (reminder.Time.Hours == time.Hour) && (reminder.Time.Minutes == time.Minute)) {
                                reminder.Enabled = false;
                                anyChanges = true;
                                if (!string.IsNullOrEmpty(reminder.Description)) {
                                    if (text.Length > 0) { text.Append(Environment.NewLine); }
                                    text.Append(reminder.Description);
                                }
                            }
                        }
                        if (anyChanges) {
                            if (text.Length == 0) { text.Append("Reminder"); }
                            Settings.Current.Reminders = reminders;
                            var state = new string[] {
                                time.TimeOfDay.ToString (ReminderData.TimeSpanFormat,CultureInfo.CurrentCulture),
                                text.ToString()
                            };
                            Worker.ReportProgress(0, state);
                        }
                    }
                    Thread.Sleep((60 - DateTime.Now.Second) * 1000); //wait until next minute
                }
            };

            Worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e) {
                var state = e.UserState as string[];
                var title = state[0];
                var text = state[1];
                TrayContext.ShowBalloon(title, text);
                SystemSounds.Asterisk.Play();
            };
        }

        public void Terminate() {
            Worker.CancelAsync();
        }


        public IEnumerable<ToolStripItem> GetToolStripItems() {
            var button = new ToolStripButton() {
                Name = "mnuReminder",
                Text = "Reminders",
                Image = QText.Properties.Resources.mnuReminder_16,
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                ToolTipText = "Show and set reminders."
            };
            button.Click += delegate {
                using (var frm = new RemindersForm()) {
                    frm.ShowDialog(TrayContext.Form);
                }
            };

            yield return button;
        }

    }
}
