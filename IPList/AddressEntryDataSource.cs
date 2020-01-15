using System;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace IPList
{
    public class AddressEntryDataSource : NSTableViewDataSource
    {
        public List<AddressEntry> AddressEntries = new List<AddressEntry>();

        public AddressEntryDataSource() { }

        public void Sort(string key, bool ascending)
        {
            switch (key)
            {
                case "IP":
                    if (ascending)
                    {
                        AddressEntries.Sort((x, y) => ExtensionMethods.CompareTo(x.Address, y.Address));
                    }
                    else
                    {
                        AddressEntries.Sort((x, y) => -1 * ExtensionMethods.CompareTo(x.Address, y.Address));
                    }
                    break;
                case "Stat":
                    if (ascending)
                    {
                        AddressEntries.Sort((x, y) => string.Compare(x.Status, y.Status, StringComparison.Ordinal));
                    } else
                    {
                        AddressEntries.Sort((x, y) => -1 * string.Compare(x.Status, y.Status, StringComparison.Ordinal));
                    }
                    break;
                case "Latency":
                    if (ascending)
                    {
                        AddressEntries.Sort((x, y) => string.Compare(x.Latency, y.Latency, StringComparison.Ordinal));
                    }
                    else
                    {
                        AddressEntries.Sort((x, y) => -1 * string.Compare(x.Latency, y.Latency, StringComparison.Ordinal));
                    }
                    break;
                case "TTL":
                    if (ascending)
                    {
                        AddressEntries.Sort((x, y) => string.Compare(x.TTL, y.TTL, StringComparison.Ordinal));
                    }
                    else
                    {
                        AddressEntries.Sort((x, y) => -1 * string.Compare(x.TTL, y.TTL, StringComparison.Ordinal));
                    }
                    break;
            }
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return AddressEntries.Count;
        }

        public override void SortDescriptorsChanged(NSTableView tableView, NSSortDescriptor[] oldDescriptors)
        {
            if (oldDescriptors.Length > 0)
            {
                Sort(oldDescriptors[0].Key, oldDescriptors[0].Ascending);
            } else
            {
                NSSortDescriptor[] tbSort = tableView.SortDescriptors;
                Sort(tbSort[0].Key, tbSort[0].Ascending);
            }
            tableView.ReloadData();
        }
    }
}
