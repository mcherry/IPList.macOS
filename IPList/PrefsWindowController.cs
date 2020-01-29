using System;
using System.Collections.Generic;
using Foundation;
using AppKit;

namespace IPList
{
    public partial class PrefsWindowController : NSWindowController
    {
        public PrefsWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public PrefsWindowController(NSCoder coder) : base(coder)
        {
        }

        public PrefsWindowController() : base("PrefsWindow")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            txtPingTimeout.StringValue = Settings.PingTimeout.ToString();
            txtPortscanTimeout.StringValue = Settings.PortscanTimeout.ToString();

            string portString = "";

            foreach (int port in Settings.PortList)
            {
                portString += port.ToString() + ",";
            }
            
            txtPorts.Value = portString;
        }

        public new PrefsWindow Window
        {
            get { return (PrefsWindow)base.Window; }
        }

        partial void btnSave_Click(NSObject sender)
        {
            if (txtPingTimeout.StringValue == "" || txtPortscanTimeout.StringValue == "" || txtPorts.Value == "")
            {
                W.Error("You must provide values for timeouts and ports to scan.");
            } else
            {
                List<int> newPortList = new List<int>();
                foreach (string port in txtPorts.Value.Split(","))
                {
                    if (port != "") newPortList.Add(int.Parse(port));
                }

                Settings.PingTimeout = int.Parse(txtPingTimeout.StringValue);
                Settings.PortscanTimeout = int.Parse(txtPortscanTimeout.StringValue);
                Settings.PortList = newPortList;

                Window.Close();
            }
        }
    }
}
