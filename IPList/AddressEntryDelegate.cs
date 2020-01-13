using System;
using AppKit;

namespace IPList
{
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
            }

            return view;
        }

        public override bool ShouldSelectRow(NSTableView tableView, nint row)
        {
            Globals.CurrentIP = DataSource.AddressEntries[(int)row].Address;
            return true;
        }
    }
}
