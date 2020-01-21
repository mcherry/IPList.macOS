using System;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace IPList
{
    public class AddressEntry
    {
        public string Address { get; set; } = "";
        public string Status { get; set; } = "";
        public string Latency { get; set; } = "";
        public string TTL { get; set; } = "";
        public string DNS { get; set; } = "";

        public AddressEntry() { }

        public AddressEntry(string address, string status = "", string latency = "", string ttl = "", string dns = "")
        {
            this.Address = address;
            this.Status = status;
            this.Latency = latency;
            this.TTL = ttl;
            this.DNS = dns;
        }   
    }

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
                    }
                    else
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
                case "DNS":
                    if (ascending)
                    {
                        AddressEntries.Sort((x, y) => string.Compare(x.DNS, y.DNS, StringComparison.Ordinal));
                    }
                    else
                    {
                        AddressEntries.Sort((x, y) => -1 * string.Compare(x.DNS, y.DNS, StringComparison.Ordinal));
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
            }
            else
            {
                NSSortDescriptor[] tbSort = tableView.SortDescriptors;
                Sort(tbSort[0].Key, tbSort[0].Ascending);
            }
            tableView.ReloadData();
        }
    }

    public class AddressEntryDelegate : NSTableViewDelegate
    {
        private const string CellIdentifier = "AddressCell";

        public static AddressEntryDataSource DataSource;

        public AddressEntryDelegate(AddressEntryDataSource datasource)
        {
            DataSource = datasource;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSTextField view = (NSTextField)tableView.MakeView(CellIdentifier, this);
            if (view == null)
            {
                view = new NSTextField();
                view.Identifier = CellIdentifier;
                view.BackgroundColor = NSColor.Clear;
                view.Bordered = false;
                view.Selectable = false;
                view.Editable = false;
            }

            switch (tableColumn.Title)
            {
                case "Address":
                    view.StringValue = DataSource.AddressEntries[(int)row].Address;
                    break;
                case "Status":
                    view.StringValue = DataSource.AddressEntries[(int)row].Status;
                    break;
                case "Latency":
                    view.StringValue = DataSource.AddressEntries[(int)row].Latency;
                    break;
                case "TTL":
                    view.StringValue = DataSource.AddressEntries[(int)row].TTL;
                    break;
                case "DNS":
                    view.StringValue = DataSource.AddressEntries[(int)row].DNS;
                    break;
            }

            return view;
        }

        public override bool ShouldSelectRow(NSTableView tableView, nint row)
        {
            W.CurrentIP = DataSource.AddressEntries[(int)row].Address;
            return true;
        }
    }
}
