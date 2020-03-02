using System;

using Foundation;
using AppKit;

namespace IPList
{
    public partial class DataViewerWindowController : NSWindowController
    {
        private string portData;
        private string ipAddress;
        private string ipPort;

        public DataViewerWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public DataViewerWindowController(NSCoder coder) : base(coder)
        {
        }

        public DataViewerWindowController(AddressEntry host, PortEntry row) : base("DataViewerWindow")
        {
            portData = row.Data;
            ipAddress = host.Address;
            ipPort = row.Port;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Window.Title = ipAddress + ":" + ipPort + " Data";
            txtData.Value = portData;
        }

        public new DataViewerWindow Window
        {
            get { return (DataViewerWindow)base.Window; }
        }
    }
}
