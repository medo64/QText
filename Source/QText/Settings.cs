using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QText {
    internal static class Settings {

        private static readonly Medo.Configuration.RunOnStartup StartupConfig = new Medo.Configuration.RunOnStartup(Medo.Configuration.RunOnStartup.Current.Title, Medo.Configuration.RunOnStartup.Current.ExecutablePath, "/hide");

        public static Keys ActivationHotkey {
            get { return (Keys)Medo.Configuration.Settings.Read("ActivationHotkey", Convert.ToInt32(Keys.Control | Keys.Shift | Keys.Q)); }
            set {
                Medo.Configuration.Settings.Write("ActivationHotkey", (int)value);
                if (App.Hotkey.IsRegistered) {
                    App.Hotkey.Unregister();
                }
                if (Settings.ActivationHotkey != Keys.None) {
                    App.Hotkey.Register(Settings.ActivationHotkey);
                }
            }
        }

        public static bool CarbonCopyCreateFolder {
            get { return Medo.Configuration.Settings.Read("CarbonCopyCreateFolder", true); }
            set { Medo.Configuration.Settings.Write("CarbonCopyCreateFolder", value); }
        }

        public static string CarbonCopyFolder {
            get { return Medo.Configuration.Settings.Read("CarbonCopyFolder", ""); }
            set {
                if (FilesLocation == null) { return; }
                if ((string.Compare(value.Trim(), FilesLocation.Trim(), true) == 0)) { return; }
                Medo.Configuration.Settings.Write("CarbonCopyFolder", value);
                if (string.IsNullOrEmpty(value)) { CarbonCopyUse = false; }
            }
        }

        public static bool CarbonCopyIgnoreErrors {
            get { return Medo.Configuration.Settings.Read("CarbonCopyIgnoreErrors", true); }
            set { Medo.Configuration.Settings.Write("CarbonCopyIgnoreErrors", value); }
        }

        public static bool CarbonCopyUse {
            get { return Medo.Configuration.Settings.Read("CarbonCopyUse", false); }
            set { Medo.Configuration.Settings.Write("CarbonCopyUse", value); }
        }

        public static string DefaultFilesLocation {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Medo.Reflection.EntryAssembly.Company + @"\" + Medo.Reflection.EntryAssembly.Name); }
        }

        public static bool DisplayAlwaysOnTop {
            get { return Medo.Configuration.Settings.Read("DisplayAlwaysOnTop", false); }
            set { Medo.Configuration.Settings.Write("DisplayAlwaysOnTop", value); }
        }

        public static Color DisplayBackgroundColor {
            get {
                try {
                    return Color.FromArgb(Medo.Configuration.Settings.Read("DisplayBackgroundColor", SystemColors.Window.ToArgb()));
                } catch {
                    return SystemColors.Window;
                }
            }
            set { Medo.Configuration.Settings.Write("DisplayBackgroundColor", value.ToArgb()); }
        }

        public static Font DisplayFont {
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

        public static Color DisplayForegroundColor {
            get {
                try {
                    return Color.FromArgb(Medo.Configuration.Settings.Read("DisplayForegroundColor", SystemColors.WindowText.ToArgb()));
                } catch (Exception) {
                    return System.Drawing.SystemColors.WindowText;
                }
            }
            set { Medo.Configuration.Settings.Write("DisplayForegroundColor", value.ToArgb()); }
        }

        public static bool DisplayMinimizeMaximizeButtons {
            get { return Medo.Configuration.Settings.Read("DisplayMinimizeMaximizeButtons", true); }
            set { Medo.Configuration.Settings.Write("DisplayMinimizeMaximizeButtons", value); }
        }

        public static bool DisplayMultilineTabHeader {
            get { return Medo.Configuration.Settings.Read("DisplayMultilineTabHeader", false); }
            set { Medo.Configuration.Settings.Write("DisplayMultilineTabHeader", value); }
        }

        public static ScrollBars ScrollBars {
            get {
                try {
                    return (ScrollBars)Medo.Configuration.Settings.Read("DisplayScrollbars", Convert.ToInt32(ScrollBars.Vertical));
                } catch (Exception) {
                    return ScrollBars.Vertical;
                }
            }
            set { Medo.Configuration.Settings.Write("DisplayScrollbars", Convert.ToInt32(value)); }
        }

        public static bool DisplayShowInTaskbar {
            get { return Medo.Configuration.Settings.Read("DisplayShowInTaskbar", false); }
            set { Medo.Configuration.Settings.Write("DisplayShowInTaskbar", value); }
        }

        public static bool ShowToolbar {
            get { return Medo.Configuration.Settings.Read("ShowToolbar", true); }
            set { Medo.Configuration.Settings.Write("ShowToolbar", value); }
        }

        public static int DisplayTabWidth {
            get { return Medo.Configuration.Settings.Read("DisplayTabWidth", 4); }
            set {
                if ((value >= 1) && (value <= 16)) {
                    Medo.Configuration.Settings.Write("DisplayTabWidth", value);
                }
            }
        }

        public static bool DetectUrls {
            get { return Medo.Configuration.Settings.Read("DisplayUnderlineURLs", true); }
            set { Medo.Configuration.Settings.Write("DisplayUnderlineURLs", value); }
        }

        public static bool DisplayWordWrap {
            get { return (ScrollBars == ScrollBars.Vertical); }
        }

        public static bool EnableQuickAutoSave {
            get { return Medo.Configuration.Settings.Read("EnableQuickAutoSave", true); }
            set { Medo.Configuration.Settings.Write("EnableQuickAutoSave", value); }
        }

        public static int FilesAutoSaveInterval {
            get {
                int value = Medo.Configuration.Settings.Read("FilesAutoSaveInterval", 60);
                if (value < 15) { return 15; }
                if (value > 300) { return 300; }
                return value;
            }
            set {
                if (value < 15) { value = 15; }
                if (value > 300) { value = 300; }
                Medo.Configuration.Settings.Write("FilesAutoSaveInterval", value);
            }
        }

        public static bool FilesDeleteToRecycleBin {
            get { return Medo.Configuration.Settings.Read("FilesDeleteToRecycleBin", true); }
            set { Medo.Configuration.Settings.Write("FilesDeleteToRecycleBin", value); }
        }

        public static string FilesLocation {
            get { return Medo.Configuration.Settings.Read("DataPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Medo.Reflection.EntryAssembly.Company + "\\" + Medo.Reflection.EntryAssembly.Name)); }
            set { Medo.Configuration.Settings.Write("DataPath", value); }
        }

        public static bool FilesPreload {
            get { return Medo.Configuration.Settings.Read("FilesPreload", false); }
            set { Medo.Configuration.Settings.Write("FilesPreload", value); }
        }

        public static bool FollowURLs {
            get { return Medo.Configuration.Settings.Read("FollowURLs", true); }
            set { Medo.Configuration.Settings.Write("FollowURLs", value); }
        }

        public static bool ForceTextCopyPaste {
            get { return Medo.Configuration.Settings.Read("ForceTextCopyPaste", false); }
            set { Medo.Configuration.Settings.Write("ForceTextCopyPaste", value); }
        }

        public static bool IsRichTextFileDefault {
            get { return Medo.Configuration.Settings.Read("IsRichTextFileDefault", false); }
            set { Medo.Configuration.Settings.Write("IsRichTextFileDefault", value); }
        }

        public static bool LegacySettingsCopied {
            get { return Medo.Configuration.Settings.Read("LegacySettingsCopied", false); }
            set { Medo.Configuration.Settings.Write("LegacySettingsCopied", value); }
        }

        public static bool LogUnhandledErrorsToTemp {
            get { return Medo.Configuration.Settings.Read("LogUnhandledErrorsToTemp", true); }
            set { Medo.Configuration.Settings.Write("LogUnhandledErrorsToTemp", value); }
        }

        public static bool PrintApplicationName {
            get { return Medo.Configuration.Settings.Read("PrintApplicationName", false); }
            set { Medo.Configuration.Settings.Write("PrintApplicationName", value); }
        }

        public static int QuickAutoSaveSeconds {
            get {
                var value = Medo.Configuration.Settings.Read("QuickAutoSaveSeconds", 3);
                if (value < 1) { return 1; }
                if (value > 15) { return 15; }
                return value;
            }
            set {
                if (value < 1) { value = 1; }
                if (value > 15) { value = 15; }
                Medo.Configuration.Settings.Write("QuickAutoSaveSeconds", value);
            }
        }

        public static bool StartupRememberSelectedFile {
            get { return Medo.Configuration.Settings.Read("StartupRememberSelectedFile", true); }
            set { Medo.Configuration.Settings.Write("StartupRememberSelectedFile", value); }
        }

        public static bool StartupRun {
            get {
                return StartupConfig.RunForCurrentUser;
            }
            set {
                try {
                    StartupConfig.RunForCurrentUser = value;
                } catch (Exception) { }
            }
        }

        public static bool TrayOnMinimize {
            get { return Medo.Configuration.Settings.Read("TrayOnMinimize", false); }
            set { Medo.Configuration.Settings.Write("TrayOnMinimize", value); }
        }

        public static bool TrayOneClickActivation {
            get { return Medo.Configuration.Settings.Read("TrayOneClickActivation", true); }
            set { Medo.Configuration.Settings.Write("TrayOneClickActivation", value); }
        }

        public static bool ZoomToolbarWithDpiChange {
            get { return Medo.Configuration.Settings.Read("ZoomToolbarWithDpiChange", true); }
            set { Medo.Configuration.Settings.Write("ZoomToolbarWithDpiChange", value); }
        }

        public static string LastFolder {
            get { return Medo.Configuration.Settings.Read("LastFolder", ""); }
            set { Medo.Configuration.Settings.Write("LastFolder", (value != null) ? value : ""); }
        }

    }
}
