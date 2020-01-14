using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace IPList
{
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

        public override bool ShouldSelectRow(NSTableView tableView, nint row)
        {
            return true;
        }
    }
}
