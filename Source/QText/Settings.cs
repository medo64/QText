using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using Medo.Configuration;
using Microsoft.Win32;
using QText.Plugins.Reminder;

namespace QText {
    internal class Settings {

        private static readonly RunOnStartup StartupConfig = new RunOnStartup(Medo.Configuration.RunOnStartup.Current.Title, Medo.Configuration.RunOnStartup.Current.ExecutablePath, "/hide");

        public static Settings Current = new Settings();


        [Category("Internal")]
        [DisplayName("No registry writes")]
        [Description("If true, registry writes will be suppressed. Do notice that all settings are to be DELETED upon changing this value.")]
        [DefaultValue(false)]
        public bool NoRegistryWrites {
            get { return !Config.IsAssumedInstalled; }
        }

        [Category("Behavior")]
        [DisplayName("Hotkey")]
        [Description("Hotkey used for the program activation")]
        [DefaultValue(Keys.Control | Keys.Shift | Keys.Q)]
        public Keys ActivationHotkey {
            get { return (Keys)Config.Read("ActivationHotkey", Medo.Configuration.Settings.Read("ActivationHotkey", Convert.ToInt32(Keys.Control | Keys.Shift | Keys.Q))); }
            set { Config.Write("ActivationHotkey", (int)value); }
        }

        [Category("Behavior")]
        [DisplayName("Hotkey toggles visibility")]
        [Description("If trues, hotkey will toggle visibility of window instead of just showing it.")]
        [DefaultValue(false)]
        public bool HotkeyTogglesVisibility {
            get { return Config.Read("HotkeyTogglesVisibility", false); }
            set { Config.Write("HotkeyTogglesVisibility", value); }
        }

        [Category("Carbon copy")]
        [DisplayName("Directory")]
        [Description("Directory used for storage of file carbon copies.")]
        [DefaultValue("")]
        public string CarbonCopyDirectory {
            get { return Config.Read("CarbonCopy.Directory", Medo.Configuration.Settings.Read("CarbonCopyFolder", "")); }
            set {
                if (FilesLocation == null) { return; }
                if ((string.Compare(value.Trim(), FilesLocation.Trim(), true) == 0)) { return; }
                Config.Write("CarbonCopy.Directory", value);
                if (string.IsNullOrEmpty(value)) { CarbonCopyUse = false; }
            }
        }

        [Category("Carbon copy")]
        [DisplayName("Ignore errors")]
        [Description("If true, errors occuring during carbon copy are silently ignored.")]
        [DefaultValue(true)]
        public bool CarbonCopyIgnoreErrors {
            get { return Config.Read("CarbonCopy.IgnoreErrors", Medo.Configuration.Settings.Read("CarbonCopyIgnoreErrors", true)); }
            set { Config.Write("CarbonCopy.IgnoreErrors", value); }
        }

        [Category("Carbon copy")]
        [DisplayName("Active")]
        [Description("If true, carbon copy functionality is activated. Note that directory has to be set.")]
        [DefaultValue(false)]
        public bool CarbonCopyUse {
            get { return Config.Read("CarbonCopy.Active", Medo.Configuration.Settings.Read("CarbonCopyUse", false)); }
            set {
                if (string.IsNullOrEmpty(CarbonCopyDirectory)) { return; }
                Config.Write("CarbonCopy.Active", value);
            }
        }

        [Browsable(false)]
        public string DefaultFilesLocation {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Medo.Reflection.EntryAssembly.Company + @"\" + Medo.Reflection.EntryAssembly.Title); }
        }

        [Category("Display")]
        [DisplayName("Always on top")]
        [Description("If true, program is going to displayed above all other windows.")]
        [DefaultValue(false)]
        public bool DisplayAlwaysOnTop {
            get { return Config.Read("AlwaysOnTop", Medo.Configuration.Settings.Read("DisplayAlwaysOnTop", false)); }
            set { Config.Write("AlwaysOnTop", value); }
        }

        [Category("Display")]
        [DisplayName("Background color")]
        [Description("Background color for main window.")]
        [DefaultValue(typeof(Color), "Window")]
        public Color DisplayBackgroundColor {
            get {
                try {
                    return Color.FromArgb(Config.Read("Color.Background", Medo.Configuration.Settings.Read("DisplayBackgroundColor", SystemColors.Window.ToArgb())));
                } catch {
                    return SystemColors.Window;
                }
            }
            set { Config.Write("Color.Background", value.ToArgb()); }
        }

        [Category("Text")]
        [DisplayName("Font")]
        [Description("Default font.")]
        [DefaultValue(typeof(Font), "Vertical")]
        public Font DisplayFont {
            get {
                var tmpFamilyName = Config.Read("Font.Name", Medo.Configuration.Settings.Read("DisplayFont_FamilyName", SystemFonts.MessageBoxFont.Name));
                var tmpSize = Config.Read("Font.Size", Medo.Configuration.Settings.Read("DisplayFont_Size", SystemFonts.MessageBoxFont.Size));
                var tmpStyle = Config.Read("Font.Style", Medo.Configuration.Settings.Read("DisplayFont_Style", (int)SystemFonts.MessageBoxFont.Style));
                try {
                    return new Font(tmpFamilyName, Convert.ToSingle(tmpSize), (FontStyle)tmpStyle);
                } catch (Exception) {
                    return SystemFonts.MessageBoxFont;
                }
            }
            set {
                Config.Write("Font.Name", value.Name);
                Config.Write("Font.Size", value.Size);
                Config.Write("Font.Style", (int)value.Style);
            }
        }

        [Category("Text")]
        [DisplayName("Foreground color")]
        [Description("Default foreground color for text.")]
        [DefaultValue(typeof(Color), "WindowText")]
        public Color DisplayForegroundColor {
            get {
                try {
                    return Color.FromArgb(Config.Read("Color.Foreground", Medo.Configuration.Settings.Read("DisplayForegroundColor", SystemColors.WindowText.ToArgb())));
                } catch (Exception) {
                    return System.Drawing.SystemColors.WindowText;
                }
            }
            set { Config.Write("Color.Foreground", value.ToArgb()); }
        }

        [Category("Display")]
        [DisplayName("Minimize/Maximize")]
        [Description("If true, minimize and maximize window buttons are to be shown.")]
        [DefaultValue(true)]
        public bool DisplayMinimizeMaximizeButtons {
            get { return Config.Read("ShowMinMaxButtons", Medo.Configuration.Settings.Read("DisplayMinimizeMaximizeButtons", true)); }
            set { Config.Write("ShowMinMaxButtons", value); }
        }

        [Category("Display")]
        [DisplayName("Multiline tabs")]
        [Description("If true, tabs are shown in multiple lines.")]
        [DefaultValue(false)]
        public bool MultilineTabs {
            get { return Config.Read("MultilineTabs", Medo.Configuration.Settings.Read("MultilineTabs", false)); }
            set { Config.Write("MultilineTabs", value); }
        }

        [Category("Display")]
        [DisplayName("Scrollbars")]
        [Description("Controls which scrollbars are shown.")]
        [DefaultValue(typeof(ScrollBars), "Vertical")]
        public ScrollBars ScrollBars {
            get {
                try {
                    return (ScrollBars)Config.Read("ScrollBars", Medo.Configuration.Settings.Read("DisplayScrollbars", Convert.ToInt32(ScrollBars.Vertical)));
                } catch (Exception) {
                    return ScrollBars.Vertical;
                }
            }
            set { Config.Write("ScrollBars", Convert.ToInt32(value)); }
        }

        [Category("Display")]
        [DisplayName("Show in taskbar")]
        [Description("If true, program will be displayed in taskbar.")]
        [DefaultValue(false)]
        public bool DisplayShowInTaskbar {
            get { return Config.Read("ShowInTaskbar", Medo.Configuration.Settings.Read("DisplayShowInTaskbar", AreWindowsInTabletMode())); }
            set { Config.Write("ShowInTaskbar", value); }
        }

        [Category("Display")]
        [DisplayName("Show toolbar")]
        [Description("If true, toolbar will be shown.")]
        [DefaultValue(true)]
        public bool ShowToolbar {
            get { return Config.Read("ShowToolbar", Medo.Configuration.Settings.Read("ShowToolbar", true)); }
            set { Config.Write("ShowToolbar", value); }
        }

        [Category("Text")]
        [DisplayName("Tab width")]
        [Description("Width of tab character.")]
        [DefaultValue(4)]
        public int DisplayTabWidth {
            get { return Config.Read("TabWidth", Medo.Configuration.Settings.Read("DisplayTabWidth", 4)); }
            set {
                if ((value >= 1) && (value <= 16)) {
                    Config.Write("TabWidth", value);
                }
            }
        }

        [Category("Text")]
        [DisplayName("Display URLs")]
        [Description("If true, URLs are going to be automatically underlined.")]
        [DefaultValue(true)]
        public bool DetectUrls {
            get { return Config.Read("DetectURLs", Medo.Configuration.Settings.Read("DisplayUnderlineURLs", true)); }
            set { Config.Write("DetectURLs", value); }
        }

        [Browsable(false)]
        public bool DisplayWordWrap {
            get { return (ScrollBars == ScrollBars.Vertical); }
        }

        [Category("Storage")]
        [DisplayName("Delete to recycle bin")]
        [Description("If true, all deleted files are moved to recycle bin.")]
        [DefaultValue(true)]
        public bool FilesDeleteToRecycleBin {
            get { return Config.Read("DeleteToRecycleBin", Medo.Configuration.Settings.Read("FilesDeleteToRecycleBin", true)); }
            set { Config.Write("DeleteToRecycleBin", value); }
        }

        [Browsable(false)]
        public string FilesLocation {
            get {
                string defaultPath;
                if (Config.IsAssumedInstalled == false) {
                    defaultPath = Path.Combine(Application.StartupPath, "Data");
                } else {
                    defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Medo.Reflection.EntryAssembly.Company + "\\" + Medo.Reflection.EntryAssembly.Name);
                }
                return Config.Read("FilesLocation", Medo.Configuration.Settings.Read("DataPath", defaultPath));
            }
            set { Config.Write("FilesLocation", value); }
        }

        [Browsable(false)]
        public bool FilesPreload {
            get { return Config.Read("PreloadFiles", Medo.Configuration.Settings.Read("FilesPreload", false)); }
            set { Config.Write("PreloadFiles", value); }
        }

        [Category("Text")]
        [DisplayName("Follow URLs")]
        [Description("If true, URLs are clickable.")]
        [DefaultValue(true)]
        public bool FollowURLs {
            get { return Config.Read("FollowURLs", Medo.Configuration.Settings.Read("FollowURLs", true)); }
            set { Config.Write("FollowURLs", value); }
        }

        [Category("Text")]
        [DisplayName("Force plain text copy/paste")]
        [Description("If true, text is always pasted as plain.")]
        [DefaultValue(false)]
        public bool ForceTextCopyPaste {
            get { return Config.Read("ForcePlainCopyPaste", Medo.Configuration.Settings.Read("ForceTextCopyPaste", false)); }
            set { Config.Write("ForcePlainCopyPaste", value); }
        }

        [Category("Text")]
        [DisplayName("Default to RichText")]
        [Description("If true, Rich text is default selection for new files.")]
        [DefaultValue(false)]
        public bool IsRichTextFileDefault {
            get { return Config.Read("DefaultToRichText", Medo.Configuration.Settings.Read("IsRichTextFileDefault", false)); }
            set { Config.Write("DefaultToRichText", value); }
        }

        [Category("Internal")]
        [DisplayName("Log errors to temp")]
        [Description("If true, all errors are logged to temporary directory.")]
        [DefaultValue(true)]
        public bool LogUnhandledErrorsToTemp {
            get { return Config.Read("LogUnhandledErrorsToTemp", Medo.Configuration.Settings.Read("LogUnhandledErrorsToTemp", true)); }
            set { Config.Write("LogUnhandledErrorsToTemp", value); }
        }


        [Category("Printing")]
        [DisplayName("Paper name")]
        [Description("Default paper name (i.e. Letter, A4).")]
        [DefaultValue("")]
        public string PrintPaperName {
            get { return Config.Read("Paper.Name", Medo.Configuration.Settings.Read("PrintPaperName", "")); }
            set { Config.Write("Paper.Name", value); }
        }

        [Category("Printing")]
        [DisplayName("Paper source")]
        [Description("Default paper source.")]
        [DefaultValue("")]
        public string PrintPaperSource {
            get { return Config.Read("Paper.Source", Medo.Configuration.Settings.Read("PrintPaperSource", "")); }
            set { Config.Write("Paper.Source", value); }
        }

        [Category("Printing")]
        [DisplayName("Landscape orientation")]
        [Description("If true, printing is done in landscape.")]
        [DefaultValue(false)]
        public bool PrintIsPaperLandscape {
            get { return Config.Read("Paper.Landscape", Medo.Configuration.Settings.Read("PrintIsPaperLandscape", false)); }
            set { Config.Write("Paper.Landscape", value); }
        }

        [Category("Printing")]
        [DisplayName("Margins")]
        [Description("Printing margins in hundreds of inch (i.e. 50 is 0.5\").")]
        [DefaultValue(typeof(Margins), "50, 50, 50, 50")]
        public Margins PrintMargins {
            get {
                var left = Config.Read("Paper.Margin.Left", Medo.Configuration.Settings.Read("PrintMarginLeft", 50));
                var right = Config.Read("Paper.Margin.Right", Medo.Configuration.Settings.Read("PrintMarginRight", 50));
                var top = Config.Read("Paper.Margin.Top", Medo.Configuration.Settings.Read("PrintMarginTop", 50));
                var bottom = Config.Read("Paper.Margin.Bottom", Medo.Configuration.Settings.Read("PrintMarginBottom", 50));
                return new Margins(left, right, top, bottom);
            }
            set {
                Config.Write("Paper.Margin.Left", value.Left);
                Config.Write("Paper.Margin.Right", value.Right);
                Config.Write("Paper.Margin.Top", value.Top);
                Config.Write("Paper.Margin.Bottom", value.Bottom);
            }
        }

        [Category("Behavior")]
        [DisplayName("Quick-save interval")]
        [Description("Interval in milliseconds for automatic save.")]
        [DefaultValue(2500)]
        public int QuickSaveInterval {
            get {
                var value = Config.Read("QuickSaveInterval", Medo.Configuration.Settings.Read("QuickSaveInterval", 2500));
                if (value < 500) { return 500; }
                if (value > 10000) { return 10000; }
                return value;
            }
            set {
                if (value < 500) { value = 500; }
                if (value > 10000) { value = 10000; }
                Config.Write("QuickSaveInterval", value);
            }
        }

        [Category("Behavior")]
        [DisplayName("Run at startup")]
        [Description("If true, program is started automatically with Windows.")]
        [DefaultValue(true)]
        public bool StartupRun {
            get {
                return StartupConfig.RunForCurrentUser;
            }
            set {
                try {
                    StartupConfig.RunForCurrentUser = value;
                } catch (Exception) { }
            }
        }

        [Category("Tray")]
        [DisplayName("Minimize to tray")]
        [Description("If true, program is moved to tray upon minimize.")]
        [DefaultValue(false)]
        public bool TrayOnMinimize {
            get { return Config.Read("MinimizeToTray", Medo.Configuration.Settings.Read("TrayOnMinimize", false)); }
            set { Config.Write("MinimizeToTray", value); }
        }

        [Category("Tray")]
        [DisplayName("One-click")]
        [Description("If true, program is activated by a single click, instead of double.")]
        [DefaultValue(true)]
        public bool TrayOneClickActivation {
            get { return Config.Read("OneClickTrayActivation", Medo.Configuration.Settings.Read("TrayOneClickActivation", true)); }
            set { Config.Write("OneClickTrayActivation", value); }
        }

        [Browsable(false)]
        public DocumentFolder LastFolder {
            get {
                var lastFolderName = Config.Read("LastFolder", Medo.Configuration.Settings.Read("LastFolder", ""));
                return App.Document.GetFolder(lastFolderName) ?? App.Document.RootFolder;
            }
            set { Config.Write("LastFolder", value.Name); }
        }

        [Category("Behavior")]
        [DisplayName("Selection delimiters")]
        [Description("Delimiters used for end-of-word detection.")]
        [DefaultValue(@""".,:;!?/|\()[]{}<>")]
        public string SelectionDelimiters {
            get { return Config.Read("SelectionDelimiters", Medo.Configuration.Settings.Read("SelectionDelimiters", @""".,:;!?/|\()[]{}<>")); }
            set { Config.Write("SelectionDelimiters", value); }
        }

        [Category("Storage")]
        [DisplayName("Unix line ending")]
        [Description("If true, <LF> is used instead of usual <CR><LF> for the line ending.")]
        [DefaultValue(false)]
        public bool PlainLineEndsWithLf {
            get { return Config.Read("PlainLineEndsWithLf", Medo.Configuration.Settings.Read("PlainLineEndsWithLf", false)); }
            set { Config.Write("PlainLineEndsWithLf", value); }
        }

        [Category("Tray")]
        [DisplayName("Show balloon")]
        [Description("If true, information balloon is shown upon next movement to tray.")]
        [DefaultValue(false)]
        public bool ShowBalloonOnNextMinimize {
            get { return Config.Read("ShowBalloonOnNextTray", Medo.Configuration.Settings.Read("ShowBalloonOnNextMinimize", true)) && !Settings.Current.NoRegistryWrites; }
            set { Config.Write("ShowBalloonOnNextTray", value); }
        }


        [Browsable(false)]
        public SearchScope SearchScope {
            get {
                switch (Config.Read("Search.Scope", Medo.Configuration.Settings.Read("SearchScope", (int)SearchScope.File))) {
                    case 0:
                        return SearchScope.File;
                    case 1:
                        return SearchScope.Folder;
                    case 2:
                        return SearchScope.Folders;
                    default:
                        return SearchScope.File;
                }
            }
            set { Config.Write("Search.Scope", (int)value); }
        }

        [Browsable(false)]
        public bool SearchCaseSensitive {
            get { return Config.Read("Search.CaseSensitive", Medo.Configuration.Settings.Read("SearchCaseSensitive", false)); }
            set { Config.Write("Search.CaseSensitive", value); }
        }


        [Category("Display")]
        [DisplayName("Scale boost")]
        [Description("Amount of boost to apply for toolbar size. Changes apply on the next startup.")]
        [DefaultValue(0.00)]
        public double ScaleBoost {
            get { return Config.Read("ScaleBoost", Medo.Configuration.Settings.Read("ScaleBoost", 0.00)); }
            set {
                if ((value < -1) || (value > 4)) { return; }
                Config.Write("ScaleBoost", value);
            }
        }

        [Category("Goto")]
        [DisplayName("Sort results")]
        [Description("If true, Goto window results are sorted.")]
        [DefaultValue(true)]
        public bool GotoSortResults {
            get { return Config.Read("Goto.Sort", Medo.Configuration.Settings.Read("GotoSortResults", true)); }
            set { Config.Write("Goto.Sort", value); }
        }

        [Category("Goto")]
        [DisplayName("Prefer folders")]
        [Description("If true, Goto window results are sorted with folders first.")]
        [DefaultValue(true)]
        public bool GotoSortPreferFolders {
            get { return Config.Read("Goto.Sort.PreferFolders", Medo.Configuration.Settings.Read("GotoSortPreferFolders", true)); }
            set { Config.Write("Goto.Sort.PreferFolders", value); }
        }

        [Category("Goto")]
        [DisplayName("Prefer prefix")]
        [Description("If true, Goto window results are sorted with prefix matches first.")]
        [DefaultValue(true)]
        public bool GotoSortPreferPrefix {
            get { return Config.Read("Goto.Sort.PreferPrefix", Medo.Configuration.Settings.Read("GotoSortPreferPrefix", true)); }
            set { Config.Write("Goto.Sort.PreferPrefix", value); }
        }

        [Category("Text")]
        [DisplayName("Unrestricted clipboard")]
        [Description("If true, RichText clipboard is not restricted to text.")]
        [DefaultValue(false)]
        public bool UnrestrictedRichTextClipboard {
            get { return Config.Read("UnrestrictedRichTextClipboard", Medo.Configuration.Settings.Read("FullRichTextClipboard", false)); }
            set { Config.Write("UnrestrictedRichTextClipboard", value); }
        }

        [Category("Text")]
        [DisplayName("Use RichText 5.0")]
        [Description("If true, RichText uses control version 5.0.\nChange requires restart.")]
        [DefaultValue(true)]
        public bool UseRichText50 {
            get { return Config.Read("UseRichText50", Medo.Configuration.Settings.Read("UseRichText50", true)); }
            set { Config.Write("UseRichText50", value); }
        }

        [Category("Text")]
        [DisplayName("Check spelling")]
        [Description("If true, text will be spell checked automatically.\nIt can only be used when RichText 5.0 control is enabled and if Windows 8 or higher is used.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
        public bool UseSpellCheck {
            get {
                if ((Environment.OSVersion.Version.Major < 6) || ((Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor < 2))) { return false; } //not supported below Windows 8
                return Config.Read("UseSpellCheck", Medo.Configuration.Settings.Read("UseSpellCheck", true)) && UseRichText50;
            }
            set {
                if (value) { UseRichText50 = true; }
                Config.Write("UseSpellCheck", value);
            }
        }

        [Category("Text")]
        [DisplayName("Date/time format")]
        [Description("Format of default date/time string.")]
        [DefaultValue("g")]
        public string DateTimeFormat {
            get { return Config.Read("DateTimeFormat", "g"); }
            set { Config.Write("DateTimeFormat", value); }
        }

        [Category("Text")]
        [DisplayName("Date/time separator")]
        [Description("Separator to be used after date/time format has been inserted.")]
        [DefaultValue("\t")]
        public string DateTimeSeparator {
            get { return Config.Read("DateTimeSeparator", "\t"); }
            set { Config.Write("DateTimeSeparator", value); }
        }


        [Category("Reminders")]
        [DisplayName("Reminders")]
        [Description("List of reminders.")]
        [Browsable(false)]
        public IEnumerable<ReminderData> Reminders {
            get {
                var list = new List<ReminderData>();
                foreach (var data in Config.Read("Reminder")) {
                    var reminder = ReminderData.Parse(data);
                    if (reminder != null) { list.Add(reminder); }
                }
                return list.AsReadOnly();
            }
            set {
                var list = new List<string>();
                foreach (var reminder in value) {
                    list.Add(reminder.ToString());
                }
                Config.Write("Reminder", list);
            }
        }


        private static bool AreWindowsInTabletMode() {
            var tabletModeValue = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ImmersiveShell", "TabletMode", 0);
            if (tabletModeValue is int) { return ((int)tabletModeValue != 0); }
            return false;
        }

    }
}
