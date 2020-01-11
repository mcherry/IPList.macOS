using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace IPList
{
    public class AddressEntryDelegate : NSTableViewDelegate
    {
        #region Constants
        private const string CellIdentifier = "AddressCell";
        #endregion

        #region Private Variables
        public static AddressEntryDataSource DataSource;
        #endregion

        #region Constructors
        public AddressEntryDelegate(AddressEntryDataSource datasource)
        {
            DataSource = datasource;
        }
        #endregion

        #region Override Methods
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
            }

            return view;
        }

        public override bool ShouldSelectRow(NSTableView tableView, nint row)
        {
            Globals.CurrentIP = DataSource.AddressEntries[(int)row].Address;
            return true;
        }
        #endregion
    }
}
