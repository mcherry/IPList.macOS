using System;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace IPList
{
    public class PortEntryDataSource : NSTableViewDataSource
    {
        public List<PortEntry> Ports = new List<PortEntry>();

        public PortEntryDataSource()
        {
        }
        
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
                    } else
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
            } else
            {
                NSSortDescriptor[] tbSort = tableView.SortDescriptors;
                Sort(tbSort[0].Key, tbSort[0].Ascending);
            }

            tableView.ReloadData();
        }
    }
}
