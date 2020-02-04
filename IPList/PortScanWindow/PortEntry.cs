using System;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace IPList
{
    public class PortEntry
    {
        public string Port { get; set; } = "";
        public string Service { get; set; } = "";
        public string Data { get; set; } = "";

        public PortEntry() { }

        public PortEntry(int port, string service = "", string data = "")
        {
            Port = port.ToString();
            Service = service;
            Data = data;
        }
    }

    public class PortEntryDataSource : NSTableViewDataSource
    {
        public List<PortEntry> Ports = new List<PortEntry>();

        public PortEntryDataSource() { }

        public override nint GetRowCount(NSTableView tableView)
        {
            return Ports.Count;
        }

        public void Sort(string key, bool ascending)
        {
            switch (key)
            {
                case "Port":
                    if (ascending)
                    {
                        Ports.Sort((x, y) => x.Port.CompareTo(y.Port));
                    }
                    else
                    {
                        Ports.Sort((x, y) => -1 * x.Port.CompareTo(y.Port));
                    }
                    break;
                case "Service":
                    if (ascending)
                    {
                        Ports.Sort((x, y) => x.Service.CompareTo(y.Service));
                    }
                    else
                    {
                        Ports.Sort((x, y) => -1 * x.Service.CompareTo(y.Service));
                    }
                    break;
                case "Data":
                    if (ascending)
                    {
                        Ports.Sort((x, y) => x.Data.CompareTo(y.Data));
                    }
                    else
                    {
                        Ports.Sort((x, y) => -1 * x.Data.CompareTo(y.Data));
                    }
                    break;
            }
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

    public class PortEntryDelegate : NSTableViewDelegate
    {
        private const string CellIdentifier = "PortCell";

        public static PortEntryDataSource DataSource;

        public PortEntryDelegate(PortEntryDataSource datasource)
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
                case "Port":
                    view.StringValue = DataSource.Ports[(int)row].Port;
                    break;
                case "Service":
                    view.StringValue = DataSource.Ports[(int)row].Service;
                    break;
                case "Data":
                    view.StringValue = DataSource.Ports[(int)row].Data;
                    break;
            }

            return view;
        }

        public static string GetSelectedPort(nint index)
        {
            return DataSource.Ports[(int)index].Port;
        }

        public static string GetSelectedData(nint index)
        {
            return DataSource.Ports[(int)index].Data.Trim();
        }

        public override bool ShouldSelectRow(NSTableView tableView, nint row)
        {
            return true;
        }
    }
}
