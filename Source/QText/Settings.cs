using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace QText {
    internal class Settings {

        private static readonly Medo.Configuration.RunOnStartup StartupConfig = new Medo.Configuration.RunOnStartup(Medo.Configuration.RunOnStartup.Current.Title, Medo.Configuration.RunOnStartup.Current.ExecutablePath, "/hide");

        public static Settings Current = new Settings();


        [Category("Internal")]
        [DisplayName("No registry writes")]
        [Description("If true, registry writes will be suppressed. Do notice that all settings are to be DELETED upon changing this value.")]
        [DefaultValue(false)]
        public bool NoRegistryWrites {
            get {
                try {
                    using (var key = Registry.CurrentUser.OpenSubKey(Medo.Configuration.Settings.SubkeyPath)) {
                        return (key == null);
                    }
                } catch (SecurityException) {
                    return true;
                }
            }
            set {
                try {
                    if (value) { //remove subkey
                        try {
                            Registry.CurrentUser.DeleteSubKeyTree(Medo.Configuration.Settings.SubkeyPath);
                        } catch (ArgumentException) { }
                    } else {
                        Registry.CurrentUser.CreateSubKey(Medo.Configuration.Settings.SubkeyPath);
                    }
                    Medo.Configuration.Settings.NoRegistryWrites = value;
                    Medo.Windows.Forms.State.NoRegistryWrites = value;
                    Medo.Diagnostics.ErrorReport.DisableAutomaticSaveToTemp = value;
                } catch (IOException) {
                } catch (SecurityException) {
                } catch (UnauthorizedAccessException) { }
            }
        }


        [Category("Behavior")]
        [DisplayName("Hotkey")]
        [Description("Hotkey used for the program activation")]
        [DefaultValue(Keys.Control | Keys.Shift | Keys.Q)]
        public Keys ActivationHotkey {
            get { return (Keys)Medo.Configuration.Settings.Read("ActivationHotkey", Convert.ToInt32(Keys.Control | Keys.Shift | Keys.Q)); }
            set { Medo.Configuration.Settings.Write("ActivationHotkey", (int)value); }
        }

        [Category("Carbon copy")]
        [DisplayName("Directory")]
        [Description("Directory used for storage of file carbon copies.")]
        [DefaultValue("")]
        public string CarbonCopyDirectory {
            get { return Medo.Configuration.Settings.Read("CarbonCopyFolder", ""); }
            set {
                if (FilesLocation == null) { return; }
                if ((string.Compare(value.Trim(), FilesLocation.Trim(), true) == 0)) { return; }
                Medo.Configuration.Settings.Write("CarbonCopyFolder", value);
                if (string.IsNullOrEmpty(value)) { CarbonCopyUse = false; }
            }
        }

        [Category("Carbon copy")]
        [DisplayName("Ignore errors")]
        [Description("If true, errors occuring during carbon copy are silently ignored.")]
        [DefaultValue(true)]
        public bool CarbonCopyIgnoreErrors {
            get { return Medo.Configuration.Settings.Read("CarbonCopyIgnoreErrors", true); }
            set { Medo.Configuration.Settings.Write("CarbonCopyIgnoreErrors", value); }
        }

        [Category("Carbon copy")]
        [DisplayName("Active")]
        [Description("If true, carbon copy functionality is activated. Note that directory has to be set.")]
        [DefaultValue(false)]
        public bool CarbonCopyUse {
            get { return Medo.Configuration.Settings.Read("CarbonCopyUse", false); }
            set {
                if (string.IsNullOrEmpty(this.CarbonCopyDirectory)) { return; }
                Medo.Configuration.Settings.Write("CarbonCopyUse", value);
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
            get { return Medo.Configuration.Settings.Read("DisplayAlwaysOnTop", false); }
            set { Medo.Configuration.Settings.Write("DisplayAlwaysOnTop", value); }
        }

        [Category("Display")]
        [DisplayName("Background color")]
        [Description("Background color for main window.")]
        [DefaultValue(typeof(Color), "Window")]
        public Color DisplayBackgroundColor {
            get {
                try {
                    return Color.FromArgb(Medo.Configuration.Settings.Read("DisplayBackgroundColor", SystemColors.Window.ToArgb()));
                } catch {
                    return SystemColors.Window;
                }
            }
            set { Medo.Configuration.Settings.Write("DisplayBackgroundColor", value.ToArgb()); }
        }

        [Category("Text")]
        [DisplayName("Font")]
        [Description("Default font.")]
        [DefaultValue(typeof(Font), "Vertical")]
        public Font DisplayFont {
            get {
                string tmpFamilyName = Medo.Configuration.Settings.Read("DisplayFont_FamilyName", SystemFonts.MessageBoxFont.Name);
                double tmpSize = Medo.Configuration.Settings.Read("DisplayFont_Size", SystemFonts.MessageBoxFont.Size);
                int tmpStyle = Medo.Configuration.Settings.Read("DisplayFont_Style", (Int32)SystemFonts.MessageBoxFont.Style);
                try {
                    return new Font(tmpFamilyName, Convert.ToSingle(tmpSize), (FontStyle)tmpStyle);
                } catch (Exception) {
                    return SystemFonts.MessageBoxFont;
                }
            }
            set {
                Medo.Configuration.Settings.Write("DisplayFont_FamilyName", value.Name);
                Medo.Configuration.Settings.Write("DisplayFont_Size", value.Size);
                Medo.Configuration.Settings.Write("DisplayFont_Style", (Int32)value.Style);
            }
        }

        [Category("Text")]
        [DisplayName("Foreground color")]
        [Description("Default foreground color for text.")]
        [DefaultValue(typeof(Color), "WindowText")]
        public Color DisplayForegroundColor {
            get {
                try {
                    return Color.FromArgb(Medo.Configuration.Settings.Read("DisplayForegroundColor", SystemColors.WindowText.ToArgb()));
                } catch (Exception) {
                    return System.Drawing.SystemColors.WindowText;
                }
            }
            set { Medo.Configuration.Settings.Write("DisplayForegroundColor", value.ToArgb()); }
        }

        [Category("Display")]
        [DisplayName("Minimize/Maximize")]
        [Description("If true, minimize and maximize window buttons are to be shown.")]
        [DefaultValue(true)]
        public bool DisplayMinimizeMaximizeButtons {
            get { return Medo.Configuration.Settings.Read("DisplayMinimizeMaximizeButtons", true); }
            set { Medo.Configuration.Settings.Write("DisplayMinimizeMaximizeButtons", value); }
        }

        [Category("Display")]
        [DisplayName("Multiline tabs")]
        [Description("If true, tabs are shown in multiple lines.")]
        [DefaultValue(false)]
        public bool MultilineTabs {
            get { return Medo.Configuration.Settings.Read("MultilineTabs", false); }
            set { Medo.Configuration.Settings.Write("MultilineTabs", value); }
        }

        [Category("Display")]
        [DisplayName("Scrollbars")]
        [Description("Controls which scrollbars are shown.")]
        [DefaultValue(typeof(ScrollBars), "Vertical")]
        public ScrollBars ScrollBars {
            get {
                try {
                    return (ScrollBars)Medo.Configuration.Settings.Read("DisplayScrollbars", Convert.ToInt32(ScrollBars.Vertical));
                } catch (Exception) {
                    return ScrollBars.Vertical;
                }
            }
            set { Medo.Configuration.Settings.Write("DisplayScrollbars", Convert.ToInt32(value)); }
        }

        [Category("Display")]
        [DisplayName("Show in taskbar")]
        [Description("If true, program will be displayed in taskbar.")]
        [DefaultValue(false)]
        public bool DisplayShowInTaskbar {
            get { return Medo.Configuration.Settings.Read("DisplayShowInTaskbar", AreWindowsInTabletMode()); }
            set { Medo.Configuration.Settings.Write("DisplayShowInTaskbar", value); }
        }

        [Category("Display")]
        [DisplayName("Show toolbar")]
        [Description("If true, toolbar will be shown.")]
        [DefaultValue(true)]
        public bool ShowToolbar {
            get { return Medo.Configuration.Settings.Read("ShowToolbar", true); }
            set { Medo.Configuration.Settings.Write("ShowToolbar", value); }
        }

        [Category("Text")]
        [DisplayName("Tab width")]
        [Description("Width of tab character.")]
        [DefaultValue(4)]
        public int DisplayTabWidth {
            get { return Medo.Configuration.Settings.Read("DisplayTabWidth", 4); }
            set {
                if ((value >= 1) && (value <= 16)) {
                    Medo.Configuration.Settings.Write("DisplayTabWidth", value);
                }
            }
        }

        [Category("Text")]
        [DisplayName("Display URLs")]
        [Description("If true, URLs are going to be automatically underlined.")]
        [DefaultValue(true)]
        public bool DetectUrls {
            get { return Medo.Configuration.Settings.Read("DisplayUnderlineURLs", true); }
            set { Medo.Configuration.Settings.Write("DisplayUnderlineURLs", value); }
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
            get { return Medo.Configuration.Settings.Read("FilesDeleteToRecycleBin", true); }
            set { Medo.Configuration.Settings.Write("FilesDeleteToRecycleBin", value); }
        }

        [Browsable(false)]
        public string FilesLocation {
            get {
                string defaultPath;
                if (Settings.Current.NoRegistryWrites) {
                    defaultPath = Path.Combine(Application.StartupPath, "Data");
                } else {
                    defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Medo.Reflection.EntryAssembly.Company + "\\" + Medo.Reflection.EntryAssembly.Name);
                }
                return Medo.Configuration.Settings.Read("DataPath", defaultPath);
            }
            set { Medo.Configuration.Settings.Write("DataPath", value); }
        }

        [Browsable(false)]
        public bool FilesPreload {
            get { return Medo.Configuration.Settings.Read("FilesPreload", false); }
            set { Medo.Configuration.Settings.Write("FilesPreload", value); }
        }

        [Category("Text")]
        [DisplayName("Follow URLs")]
        [Description("If true, URLs are clickable.")]
        [DefaultValue(true)]
        public bool FollowURLs {
            get { return Medo.Configuration.Settings.Read("FollowURLs", true); }
            set { Medo.Configuration.Settings.Write("FollowURLs", value); }
        }

        [Category("Text")]
        [DisplayName("Force plain text copy/paste")]
        [Description("If true, text is always pasted as plain.")]
        [DefaultValue(false)]
        public bool ForceTextCopyPaste {
            get { return Medo.Configuration.Settings.Read("ForceTextCopyPaste", false); }
            set { Medo.Configuration.Settings.Write("ForceTextCopyPaste", value); }
        }

        [Category("Text")]
        [DisplayName("Default to RichText")]
        [Description("If true, Rich text is default selection for new files.")]
        [DefaultValue(false)]
        public bool IsRichTextFileDefault {
            get { return Medo.Configuration.Settings.Read("IsRichTextFileDefault", false); }
            set { Medo.Configuration.Settings.Write("IsRichTextFileDefault", value); }
        }

        [Browsable(false)]
        public bool LegacySettingsCopied {
            get { return Medo.Configuration.Settings.Read("LegacySettingsCopied", false); }
            set { Medo.Configuration.Settings.Write("LegacySettingsCopied", value); }
        }

        [Category("Internal")]
        [DisplayName("Log errors to temp")]
        [Description("If true, all errors are logged to temporary directory.")]
        [DefaultValue(true)]
        public bool LogUnhandledErrorsToTemp {
            get { return Medo.Configuration.Settings.Read("LogUnhandledErrorsToTemp", true); }
            set { Medo.Configuration.Settings.Write("LogUnhandledErrorsToTemp", value); }
        }


        [Category("Printing")]
        [DisplayName("Paper name")]
        [Description("Default paper name (i.e. Letter, A4).")]
        [DefaultValue("")]
        public string PrintPaperName {
            get { return Medo.Configuration.Settings.Read("PrintPaperName", ""); }
            set { Medo.Configuration.Settings.Write("PrintPaperName", value); }
        }

        [Category("Printing")]
        [DisplayName("Paper source")]
        [Description("Default paper source.")]
        [DefaultValue("")]
        public string PrintPaperSource {
            get { return Medo.Configuration.Settings.Read("PrintPaperSource", ""); }
            set { Medo.Configuration.Settings.Write("PrintPaperSource", value); }
        }

        [Category("Printing")]
        [DisplayName("Landscape orientation")]
        [Description("If true, printing is done in landscape.")]
        [DefaultValue(false)]
        public bool PrintIsPaperLandscape {
            get { return Medo.Configuration.Settings.Read("PrintIsPaperLandscape", false); }
            set { Medo.Configuration.Settings.Write("PrintIsPaperLandscape", value); }
        }

        [Category("Printing")]
        [DisplayName("Margins")]
        [Description("Printing margins in hundreds of inch (i.e. 50 is 0.5\").")]
        [DefaultValue(typeof(Margins), "50, 50, 50, 50")]
        public Margins PrintMargins {
            get {
                var left = Medo.Configuration.Settings.Read("PrintMarginLeft", 50);
                var right = Medo.Configuration.Settings.Read("PrintMarginRight", 50);
                var top = Medo.Configuration.Settings.Read("PrintMarginTop", 50);
                var bottom = Medo.Configuration.Settings.Read("PrintMarginBottom", 50);
                return new Margins(left, right, top, bottom);
            }
            set {
                Medo.Configuration.Settings.Write("PrintMarginLeft", value.Left);
                Medo.Configuration.Settings.Write("PrintMarginRight", value.Right);
                Medo.Configuration.Settings.Write("PrintMarginTop", value.Top);
                Medo.Configuration.Settings.Write("PrintMarginBottom", value.Bottom);
            }
        }

        [Category("Behavior")]
        [DisplayName("Quick-save interval")]
        [Description("Interval in milliseconds for automatic save.")]
        [DefaultValue(2500)]
        public int QuickSaveInterval {
            get {
                var value = Medo.Configuration.Settings.Read("QuickSaveInterval", 2500);
                if (value < 500) { return 500; }
                if (value > 10000) { return 10000; }
                return value;
            }
            set {
                if (value < 500) { value = 500; }
                if (value > 10000) { value = 10000; }
                Medo.Configuration.Settings.Write("QuickSaveInterval", value);
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
            get { return Medo.Configuration.Settings.Read("TrayOnMinimize", false); }
            set { Medo.Configuration.Settings.Write("TrayOnMinimize", value); }
        }

        [Category("Tray")]
        [DisplayName("One-click")]
        [Description("If true, program is activated by a single click, instead of double.")]
        [DefaultValue(true)]
        public bool TrayOneClickActivation {
            get { return Medo.Configuration.Settings.Read("TrayOneClickActivation", true); }
            set { Medo.Configuration.Settings.Write("TrayOneClickActivation", value); }
        }

        [Browsable(false)]
        public DocumentFolder LastFolder {
            get {
                var lastFolderName = Medo.Configuration.Settings.Read("LastFolder", "");
                return App.Document.GetFolder(lastFolderName) ?? App.Document.RootFolder;
            }
            set { Medo.Configuration.Settings.Write("LastFolder", value.Name); }
        }

        [Category("Behavior")]
        [DisplayName("Selection delimiters")]
        [Description("Delimiters used for end-of-word detection.")]
        [DefaultValue(@""".,:;!?/|\()[]{}<>")]
        public string SelectionDelimiters {
            get { return Medo.Configuration.Settings.Read("SelectionDelimiters", @""".,:;!?/|\()[]{}<>"); }
            set { Medo.Configuration.Settings.Write("SelectionDelimiters", value); }
        }

        [Category("Storage")]
        [DisplayName("Unix line ending")]
        [Description("If true, <LF> is used instead of usual <CR><LF> for the line ending.")]
        [DefaultValue(false)]
        public bool PlainLineEndsWithLf {
            get { return Medo.Configuration.Settings.Read("PlainLineEndsWithLf", false); }
            set { Medo.Configuration.Settings.Write("PlainLineEndsWithLf", value); }
        }

        [Category("Tray")]
        [DisplayName("Show balloon")]
        [Description("If true, information balloon is shown upon next movement to tray.")]
        [DefaultValue(false)]
        public Boolean ShowBalloonOnNextMinimize {
            get { return Medo.Configuration.Settings.Read("ShowBalloonOnNextMinimize", true) && !Settings.Current.NoRegistryWrites; }
            set { Medo.Configuration.Settings.Write("ShowBalloonOnNextMinimize", value); }
        }


        [Browsable(false)]
        public SearchScope SearchScope {
            get {
                switch (Medo.Configuration.Settings.Read("SearchScope", (int)SearchScope.File)) {
                    case 0: return SearchScope.File;
                    case 1: return SearchScope.Folder;
                    case 2: return SearchScope.Folders;
                    default: return SearchScope.File;
                }
            }
            set { Medo.Configuration.Settings.Write("SearchScope", (int)value); }
        }

        [Browsable(false)]
        public Boolean SearchCaseSensitive {
            get { return Medo.Configuration.Settings.Read("SearchCaseSensitive", false); }
            set { Medo.Configuration.Settings.Write("SearchCaseSensitive", value); }
        }


        [Category("Display")]
        [DisplayName("Scale boost")]
        [Description("Amount of boost to apply for toolbar size. Changes apply on the next startup.")]
        [DefaultValue(0.00)]
        public double ScaleBoost {
            get { return Medo.Configuration.Settings.Read("ScaleBoost", 0.00); }
            set {
                if ((value < -1) || (value > 4)) { return; }
                Medo.Configuration.Settings.Write("ScaleBoost", value);
            }
        }

        [Category("Goto")]
        [DisplayName("Sort results")]
        [Description("If true, Goto window results are sorted.")]
        [DefaultValue(true)]
        public bool GotoSortResults {
            get { return Medo.Configuration.Settings.Read("GotoSortResults", true); }
            set { Medo.Configuration.Settings.Write("GotoSortResults", value); }
        }

        [Category("Goto")]
        [DisplayName("Prefer folders")]
        [Description("If true, Goto window results are sorted with folders first.")]
        [DefaultValue(true)]
        public bool GotoSortPreferFolders {
            get { return Medo.Configuration.Settings.Read("GotoSortPreferFolders", true); }
            set { Medo.Configuration.Settings.Write("GotoSortPreferFolders", value); }
        }

        [Category("Goto")]
        [DisplayName("Prefer prefix")]
        [Description("If true, Goto window results are sorted with prefix matches first.")]
        [DefaultValue(true)]
        public bool GotoSortPreferPrefix {
            get { return Medo.Configuration.Settings.Read("GotoSortPreferPrefix", true); }
            set { Medo.Configuration.Settings.Write("GotoSortPreferPrefix", value); }
        }

        [Category("Text")]
        [DisplayName("Unrestricted clipboard")]
        [Description("If true, RichText clipboard is not restricted to text.")]
        [DefaultValue(false)]
        public bool FullRichTextClipboard {
            get { return Medo.Configuration.Settings.Read("FullRichTextClipboard", false); }
            set { Medo.Configuration.Settings.Write("FullRichTextClipboard", value); }
        }

        [Category("Text")]
        [DisplayName("Use RichText 5.0")]
        [Description("If true, RichText uses control version 5.0.\nChange requires restart.")]
        [DefaultValue(true)]
        public bool UseRichText50 {
            get { return Medo.Configuration.Settings.Read("UseRichText50", true); }
            set { Medo.Configuration.Settings.Write("UseRichText50", value); }
        }

        [Category("Text")]
        [DisplayName("Check spelling")]
        [Description("If true, text will be spell checked automatically.\nIt can only be used when RichText 5.0 control is enabled and if Windows 8 or higher is used.")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool UseSpellCheck {
            get {
                if ((Environment.OSVersion.Version.Major < 6) || ((Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor < 2))) { return false; } //not supported below Windows 8
                return Medo.Configuration.Settings.Read("UseSpellCheck", false) && this.UseRichText50;
            }
            set {
                if (value) { this.UseRichText50 = true; }
                Medo.Configuration.Settings.Write("UseSpellCheck", value);
            }
        }


        private static bool AreWindowsInTabletMode() {
            var tabletModeValue = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ImmersiveShell", "TabletMode", 0);
            if (tabletModeValue is int) { return ((int)tabletModeValue != 0); }
            return false;
        }

    }
}
