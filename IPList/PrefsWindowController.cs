using System;
using System.Collections.Generic;
using System.Threading;
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
            cmbPortList.SelectItem(Settings.PortListName);

            Thread loader = new Thread(() => { LoadList(Settings.PortList); });
            loader.Start();
            
        }

        public new PrefsWindow Window
        {
            get { return (PrefsWindow)base.Window; }
        }

        partial void btnSave_Click(NSObject sender)
        {
            bool error = false;
            string error_msg = "The following errors were encountered while trying to save the port list:\n\n";

            if (txtPingTimeout.StringValue == "" || txtPingTimeout.StringValue == "0")
            {
                error = true;
                error_msg += "Ping timeout must be greater than 0.\n";
            }
                

            if (txtPortscanTimeout.StringValue == "" || txtPortscanTimeout.StringValue == "0")
            {
                error = true;
                error_msg += "Port scan timeout must be greater than 0.\n";
            }
                

            if (txtPorts.Value == "" || txtPorts.Value == "0")
            {
                error = true;
                error_msg += "At least one port is required for scanning.\n";
            }
            
            if (error == true)
            {
                W.Error(error_msg);
            } else
            {
                List<int> newPortList = new List<int>();
                foreach (string port in txtPorts.Value.Split(","))
                {
                    if (port != "") newPortList.Add(int.Parse(port));
                }

                Settings.PingTimeout = int.Parse(txtPingTimeout.StringValue);
                Settings.PortscanTimeout = int.Parse(txtPortscanTimeout.StringValue);
                Settings.PortListName = cmbPortList.SelectedItem.Title;
                Settings.PortList = newPortList;

                Window.Close();
            }
        }

        partial void cmbPortList_Click(NSObject sender)
        {
            Thread loader = null;

            switch (cmbPortList.SelectedItem.Title)
            {
                case "Top 1,000":
                    loader = new Thread(() => { LoadList(Settings.topPorts1000); });
                    break;
                case "Top 100":
                    loader = new Thread(() => { LoadList(Settings.topPorts100); });
                    break;
                case "All 65,535":
                    loader = new Thread(() => { LoadAllPorts(); });
                    break;
                case "Custom":
                    txtPorts.Value = "";
                    setStatus("");
                    break;
            }

            if (cmbPortList.SelectedItem.Title != "Custom") loader.Start();
        }

        private void ToggleGUI(bool enabled)
        {
            InvokeOnMainThread(() => {
                if (enabled == true)
                {
                    prgStatus.StopAnimation(this);
                    cmbPortList.Enabled = true;
                    txtPingTimeout.Enabled = true;
                    txtPortscanTimeout.Enabled = true;
                    btnSave.Enabled = true;
                } else
                {
                    txtPorts.Value = "";
                    cmbPortList.Enabled = false;
                    btnSave.Enabled = false;
                    txtPingTimeout.Enabled = false;
                    txtPortscanTimeout.Enabled = false;
                    prgStatus.StartAnimation(this);
                }
            });
        }

        private void LoadAllPorts()
        {
            string portString = "";

            ToggleGUI(false);
            setStatus("Generating port list...");

            for (int port = 1; port <= 65535; port++)
            {
                portString += port.ToString() + ",";
            }

            InvokeOnMainThread(() => { txtPorts.Value = portString; });
            setStatus("65,535 ports to scan");
            ToggleGUI(true);
        }

        private void LoadList(List<int> list)
        {
            string portString = "";

            ToggleGUI(false);

            if (list.Count > 0)
            {
                foreach (int port in list)
                {
                    portString += port.ToString() + ",";
                }
            }

            InvokeOnMainThread(() => { txtPorts.Value = portString; });
            setStatus(string.Format("{0:n0}", list.Count) + " ports to scan");
            ToggleGUI(true);
        }

        private void setStatus(string status)
        {
            InvokeOnMainThread(() => { lblStatus.StringValue = status; });
        }
    }
}
