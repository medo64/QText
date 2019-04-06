/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2014-12-12: Bug fixing.
//2014-01-12: Initial version.


using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;

namespace Medo.Configuration {

    /// <summary>
    /// Enables loading and saving of generic historical data.
    /// Each saved item is checked for uniqueness.
    /// It is written in History key at HKEY_CURRENT_USER branch withing defined SubKeyPath.
    /// </summary>
    public class History {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public History()
            : this(16, null) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="maximumCount">Maximum number of items to load or save.</param>
        public History(int maximumCount)
            : this(maximumCount, null) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="groupName">Name of a group.</param>
        public History(string groupName)
            : this(16, groupName) {
        }


        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="maximumCount">Maximum number of items to load or save.</param>
        /// <param name="groupName">Name of a group.</param>
        public History(int maximumCount, string groupName) {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) { assembly = Assembly.GetCallingAssembly(); } //i.e. when running unit tests

            string company = null;
            var companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if ((companyAttributes != null) && (companyAttributes.Length >= 1)) {
                company = ((AssemblyCompanyAttribute)companyAttributes[companyAttributes.Length - 1]).Company;
            }

            string product;
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                product = ((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
            } else {
                var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                    product = ((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                } else {
                    product = assembly.GetName().Name;
                }
            }

            var basePath = "Software";
            if (!string.IsNullOrEmpty(company)) { basePath += "\\" + company; }
            if (!string.IsNullOrEmpty(product)) { basePath += "\\" + product; }

            Subkey = basePath + "\\History";

            MaximumCount = maximumCount;
            if (string.IsNullOrEmpty(groupName)) {
                GroupName = null;
            } else {
                GroupName = groupName;
            }
        }


        /// <summary>
        /// Gets registry sub-key where data is written.
        /// </summary>
        internal string Subkey { get; private set; }

        /// <summary>
        /// Group name.
        /// </summary>
        public string GroupName { get; private set; }


        /// <summary>
        /// Gets/sets whether settings should be written to registry.
        /// </summary>
        public static bool NoRegistryWrites { get; set; }


        /// <summary>
        /// Gets maximum number of items to be saved.
        /// </summary>
        public int MaximumCount { get; private set; }

        private StringComparer _comparer = StringComparer.Ordinal;
        /// <summary>
        /// Gets/sets the comparer that is used to determine equality of keys for the dictionary.
        /// </summary>
        public StringComparer Comparer {
            get { return _comparer; }
            set {
                _comparer = value ?? throw new ArgumentNullException("value", "Value cannot be null.");
            }
        }


        /// <summary>
        /// Gets all items as an enumeration.
        /// </summary>
        public IEnumerable<string> Items {
            get {
                foreach (var item in Load()) {
                    yield return item;
                }
            }
        }


        /// <summary>
        /// Removes all items.
        /// </summary>
        public void Clear() {
            Save(new List<string>());
        }

        /// <summary>
        /// Adds item to the top of the list.
        /// Duplicate items are merged together.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Prepend(string item) {
            var items = Load(item);
            items.Insert(0, item);
            if (items.Count > MaximumCount) { items.RemoveAt(MaximumCount); }
            Save(items);
        }

        /// <summary>
        /// Adds item to the bottom of the list.
        /// Duplicate items are merged together.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Append(string item) {
            var items = Load(item);
            items.Add(item);
            if (items.Count > MaximumCount) { items.RemoveAt(0); }
            Save(items);
        }

        /// <summary>
        /// Removes all occurrances of a given item.
        /// All changes are immediately saved.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Remove(string item) {
            var items = Load(item);
            Save(items);
        }


        #region Helper

        private IList<string> Load(string itemToRemove = null) {
            try {
                using (var rk = Registry.CurrentUser.OpenSubKey(Subkey, false)) {
                    if (rk != null) {
                        var valueCU = rk.GetValue(GroupName, null);
                        if (valueCU != null) {
                            var valueKind = RegistryValueKind.MultiString;
                            if (!History.IsRunningOnMono) { valueKind = rk.GetValueKind(GroupName); }
                            if (valueKind == RegistryValueKind.MultiString) {
                                if (valueCU is string[] array) {
                                    var dict = new Dictionary<string, object>(Comparer);
                                    var items = new List<string>();
                                    foreach (var item in array) {
                                        if (!dict.ContainsKey(item)) {
                                            if ((itemToRemove != null) && Comparer.Equals(item, itemToRemove)) { continue; }
                                            items.Add(item);
                                            if (items.Count == MaximumCount) { break; }
                                        }
                                    }
                                    if (itemToRemove != null) {
                                        for (var i = items.Count - 1; i >= 0; i--) {
                                            if (Comparer.Equals(items[i], itemToRemove)) {
                                                items.RemoveAt(i);
                                            }
                                        }
                                    }
                                    if (items.Count > MaximumCount) {
                                        items.RemoveRange(MaximumCount, items.Count - MaximumCount);
                                    }
                                    return items;
                                }
                            }
                        }
                    }
                }
            } catch (SecurityException) { }

            return new List<string>();
        }

        /// <summary>
        /// Saves current list to registry.
        /// This is automaticaly done on each insert.
        /// </summary>
        private void Save(IList<string> items) {
            var array = new string[items.Count];
            items.CopyTo(array, 0);
            if (History.NoRegistryWrites == false) {
                using (var rk = Registry.CurrentUser.CreateSubKey(Subkey)) {
                    rk.SetValue(GroupName, array, RegistryValueKind.MultiString);
                }
            }
        }


        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }

        #endregion

    }
}
