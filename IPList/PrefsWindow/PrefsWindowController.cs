using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

            chkUpdates.IntValue = Settings.UpdateCheck;
            txtPingTimeout.StringValue = Settings.PingTimeout.ToString();
            txtPortscanTimeout.StringValue = Settings.PortscanTimeout.ToString();
            cmbPortList.SelectItem(Settings.PortListName);

            ThreadLoader(LoadList, Settings.PortList);
        }

        public new PrefsWindow Window
        {
            get { return (PrefsWindow)base.Window; }
        }

        [Action("UpdateCheck:")]
        public void UpdateCheck(NSObject sender)
        {
            W.UpdateCheck(true);
        }

        [Action("showHelp:")]
        public void OpenProjectPage(NSObject sender)
        {
            Process.Start(W.ProjectURL);
        }

        [Action("showAbout:")]
        public void ShowAbout(NSObject sender)
        {
            AboutWindowController aboutWin = new AboutWindowController();
            aboutWin.ShowWindow(this);
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
                W.Alert("Error", error_msg, NSAlertStyle.Critical);
            } else
            {
                List<int> newPortList = new List<int>();
                foreach (string port in txtPorts.Value.Split(","))
                {
                    if (port != "") newPortList.Add(int.Parse(port));
                }

                Settings.UpdateCheck = chkUpdates.IntValue;
                Settings.PingTimeout = int.Parse(txtPingTimeout.StringValue);
                Settings.PortscanTimeout = int.Parse(txtPortscanTimeout.StringValue);
                Settings.PortListName = cmbPortList.SelectedItem.Title;
                Settings.PortList = newPortList;

                Window.Close();
            }
        }

        partial void cmbPortList_Click(NSObject sender)
        {
            switch (cmbPortList.SelectedItem.Title)
            {
                case "Top 1,000":
                    ThreadLoader(LoadList, Settings.topPorts1000);
                    break;
                case "Top 100":
                    ThreadLoader(LoadList, Settings.topPorts100);
                    break;
                case "All 65,535":
                    ThreadLoader(LoadList, GeneratePortList());
                    break;
                case "Custom":
                    txtPorts.Value = "";
                    setStatus("");
                    break;
            }
        }

        private void ThreadLoader(Action<List<int>> method, List<int> portList)
        {
            Thread loader = new Thread(() => { method(portList); });
            loader.Start();
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

        private List<int> GeneratePortList()
        {
            List<int> returnList = new List<int>();

            for (int port = 1; port <= 65535; port++)
            {
                returnList.Add(port);
            }

            return returnList;
        }

        private void LoadList(List<int> list)
        {
            StringBuilder sb = new StringBuilder();

            ToggleGUI(false);
            setStatus("Generating port list...");

            if (list.Count > 0)
            {
                foreach (int port in list)
                {
                    sb.Append(port.ToString() + ",");
                }
            }
            
            InvokeOnMainThread(() => { txtPorts.Value = sb.ToString(); });
            setStatus(string.Format("{0:n0}", list.Count) + " ports to scan");
            ToggleGUI(true);
        }

        private void setStatus(string status)
        {
            InvokeOnMainThread(() => { lblStatus.StringValue = status; });
        }
    }
}
