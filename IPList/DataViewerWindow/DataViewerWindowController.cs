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

        public DataViewerWindowController(nint portIndex, string ip) : base("DataViewerWindow")
        {
            portData = PortEntryDelegate.GetSelectedData(portIndex);
            ipAddress = ip;
            ipPort = PortEntryDelegate.GetSelectedPort(portIndex);
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
