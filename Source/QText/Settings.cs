using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace QText {
    internal static class Settings {

        private static readonly Medo.Configuration.RunOnStartup StartupConfig = new Medo.Configuration.RunOnStartup(Medo.Configuration.RunOnStartup.Current.Title, Medo.Configuration.RunOnStartup.Current.ExecutablePath, "/hide");


        public static bool NoRegistryWrites {
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


        public static Keys ActivationHotkey {
            get { return (Keys)Medo.Configuration.Settings.Read("ActivationHotkey", Convert.ToInt32(Keys.Control | Keys.Shift | Keys.Q)); }
            set { Medo.Configuration.Settings.Write("ActivationHotkey", (int)value); }
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

        public static bool MultilineTabs {
            get { return Medo.Configuration.Settings.Read("MultilineTabs", false); }
            set { Medo.Configuration.Settings.Write("MultilineTabs", value); }
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

        public static bool FilesDeleteToRecycleBin {
            get { return Medo.Configuration.Settings.Read("FilesDeleteToRecycleBin", true); }
            set { Medo.Configuration.Settings.Write("FilesDeleteToRecycleBin", value); }
        }

        public static string FilesLocation {
            get {
                string defaultPath;
                if (Settings.NoRegistryWrites) {
                    defaultPath = Path.Combine(Application.StartupPath, "Data");
                } else {
                    defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Medo.Reflection.EntryAssembly.Company + "\\" + Medo.Reflection.EntryAssembly.Name);
                }
                return Medo.Configuration.Settings.Read("DataPath", defaultPath);
            }
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


        public static string PrintPaperName {
            get { return Medo.Configuration.Settings.Read("PrintPaperName", ""); }
            set { Medo.Configuration.Settings.Write("PrintPaperName", value); }
        }

        public static string PrintPaperSource {
            get { return Medo.Configuration.Settings.Read("PrintPaperSource", ""); }
            set { Medo.Configuration.Settings.Write("PrintPaperSource", value); }
        }

        public static bool PrintIsPaperLandscape {
            get { return Medo.Configuration.Settings.Read("PrintIsPaperLandscape", false); }
            set { Medo.Configuration.Settings.Write("PrintIsPaperLandscape", value); }
        }

        public static Margins PrintMargins {
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


        public static int QuickSaveInterval {
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

        public static string LastFolder {
            get { return Medo.Configuration.Settings.Read("LastFolder", ""); }
            set { Medo.Configuration.Settings.Write("LastFolder", value); }
        }

        public static string SelectionDelimiters {
            get { return Medo.Configuration.Settings.Read("SelectionDelimiters", @""".,:;!?/|\()[]{}<>"); }
            set { Medo.Configuration.Settings.Write("SelectionDelimiters", value); }
        }

        public static bool PlainLineEndsWithLf {
            get { return Medo.Configuration.Settings.Read("PlainLineEndsWithLf", false); }
            set { Medo.Configuration.Settings.Write("PlainLineEndsWithLf", value); }
        }

        public static Boolean ShowBalloonOnNextMinimize {
            get { return Medo.Configuration.Settings.Read("ShowBalloonOnNextMinimize", true) && !Settings.NoRegistryWrites; }
            set { Medo.Configuration.Settings.Write("ShowBalloonOnNextMinimize", value); }
        }


        public static SearchScope SearchScope {
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

        public static Boolean SearchCaseSensitive {
            get { return Medo.Configuration.Settings.Read("SearchCaseSensitive", false); }
            set { Medo.Configuration.Settings.Write("SearchCaseSensitive", value); }
        }

        public static double ScaleBoost {
            get { return Medo.Configuration.Settings.Read("ScaleBoost", 0.25); }
            set { Medo.Configuration.Settings.Write("ScaleBoost", value); }
        }

    }
}
