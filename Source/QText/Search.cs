using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QText {
    internal static class Search {

        public static bool FindNext(IWin32Window owner, TabFiles currentTabs, TabFile selectedTab) {
            TabFile resultTab = null;
            switch (SearchStatus.Scope) {
                case SearchScope.File: resultTab = Search.FindInFile(selectedTab); break;
                case SearchScope.Folder: resultTab = Search.FindInFolder(currentTabs, selectedTab); break;
                case SearchScope.Folders: resultTab = Search.FindInFolders(currentTabs, selectedTab); break;
            }

            if (resultTab != null) {
                if (resultTab != null) {
                    if (currentTabs.TabPages.Contains(resultTab) == false) {
                        //reload
                        System.Diagnostics.Debug.WriteLine("");
                    }
                }
                currentTabs.SelectedTab = resultTab;
                return true;
            } else {
                Medo.MessageBox.ShowInformation(owner, "Text \"" + SearchStatus.Text + "\" cannot be found.");
                return false;
            }
        }


        public static TabFile FindInFile(TabFile selectedTab) {
            if ((selectedTab != null) && (!string.IsNullOrEmpty(SearchStatus.Text))) {
                if (selectedTab.Find(SearchStatus.Text, SearchStatus.CaseSensitive)) {
                    return selectedTab;
                }
            }
            return null;
        }

        public static TabFile FindInFolder(TabFiles tabs, TabFile selectedTab) {
            if (string.IsNullOrEmpty(SearchStatus.Text)) { return null; }
            if (tabs.TabPages.Count == 0) { return null; }

            var all = new List<TabFile>();
            foreach (TabFile tab in tabs.TabPages) {
                all.Add(tab);
            }
            for (int i = 0; i < all.Count; i++) {
                if (all[0].Equals(selectedTab)) {
                    break;
                }
                all.Add(all[0]);
                all.RemoveAt(0);
            }

            for (int i = 0; i < all.Count; i++) {
                var tab = all[i];
                if (tab.FindForward(SearchStatus.Text, SearchStatus.CaseSensitive, (i == 0) ? selectedTab.TextBox.SelectionStart + selectedTab.TextBox.SelectionLength : 0)) {
                    return tab;
                }
            }

            return null;
        }

        public static TabFile FindInFolders(TabFiles tabs, TabFile selectedTab) {
            if (string.IsNullOrEmpty(SearchStatus.Text)) { return null; }

            //search to end of current tabs
            var currTabs = new List<TabFile>();
            var afterTabs = new List<TabFile>();
            foreach (TabFile tab in tabs.TabPages) {
                currTabs.Add(tab);
            }
            var count = currTabs.Count;
            for (int i = 0; i < count; i++) {
                if (currTabs[0].Equals(selectedTab)) {
                    break;
                }
                afterTabs.Add(currTabs[0]); //save for later
                currTabs.RemoveAt(0);
            }
            for (int i = 0; i < currTabs.Count; i++) {
                var tab = currTabs[i];
                if (tab.FindForward(SearchStatus.Text, SearchStatus.CaseSensitive, (i == 0) ? selectedTab.TextBox.SelectionStart + selectedTab.TextBox.SelectionLength : 0)) {
                    return tab;
                }
            }

            //search in other folders
            var folders = new List<string>(DocumentFolder.GetFolders());
            for (int i = 0; i < folders.Count; i++) {
                if (folders[0].Equals(tabs.CurrentFolder)) {
                    folders.RemoveAt(0);
                    break;
                }
                folders.Add(folders[0]);
                folders.RemoveAt(0);
            }
            foreach (var folder in folders) {
                var folderTabs = DocumentFolder.GetTabs(DocumentFolder.GetFilePaths(folder), null);
                foreach (var folderTab in folderTabs) {
                    if (folderTab.FindForward(SearchStatus.Text, SearchStatus.CaseSensitive, 0)) {
                        if (string.Equals(folder, tabs.CurrentFolder, StringComparison.OrdinalIgnoreCase) == false) {
                            tabs.FolderOpen(folder);
                        }
                        foreach (TabFile tab in tabs.TabPages) {
                            if (string.Equals(tab.CurrentFile.FullName, folderTab.CurrentFile.FullName, StringComparison.OrdinalIgnoreCase)) {
                                tab.Open();
                                tab.TextBox.SelectionStart = folderTab.TextBox.SelectionStart;
                                tab.TextBox.SelectionLength = folderTab.TextBox.SelectionLength;
                                return tab;
                            }
                        }
                        return null;
                    }
                }
            }

            //search from begining of current folder to selected tab
            foreach (var tab in afterTabs) {
                if (tab.FindForward(SearchStatus.Text, SearchStatus.CaseSensitive, 0)) {
                    return tab;
                }
            }

            return null;
        }

    }
}
