using System;
using System.Globalization;

namespace QText.Plugins.Reminder {
    internal class ReminderData {

        public ReminderData(bool enabled, TimeSpan time, string description) {
            this.Enabled = enabled;
            this.Time = time;
            this.Description = description;
        }


        public bool Enabled { get; set; }

        private TimeSpan _time;
        public TimeSpan Time {
            get { return this._time; }
            set {
                if ((value.TotalDays < 0) || (value.TotalDays >= 1)) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be within a day."); }
                this._time = new TimeSpan(value.Hours, value.Minutes, 0);
            }
        }

        private string _description;
        public string Description {
            get { return this._description; }
            set { this._description = (value ?? ""); }
        }


        public static ReminderData Parse(string text) {
            var parts = text.Split(new char[] { ' ' }, 2);
            var timePart = parts[0];
            var description = (parts.Length > 1) ? parts[1] : "";

            var enabled = true;
            if (timePart.StartsWith("-")) {
                timePart = timePart.Substring(1);
                enabled = false;
            }
            if (TimeSpan.TryParseExact(timePart, ReminderData.TimeSpanFormat, CultureInfo.InvariantCulture, out var time)) {
                return new ReminderData(enabled, time, description);
            } else {
                return null; //cannot parse, give up
            }
        }

        public override string ToString() {
            return (this.Enabled ? "" : "-") + this.Time.ToString(ReminderData.TimeSpanFormat, CultureInfo.InvariantCulture) + " " + this.Description;
        }


        public static string TimeSpanFormat { get { return "hh':'mm"; } }
    }
}
