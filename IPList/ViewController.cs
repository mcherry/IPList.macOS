using AppKit;
using Foundation;
using LukeSkywalker.IPNetwork;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

using IPAddressCollection = LukeSkywalker.IPNetwork.IPAddressCollection;

namespace IPList
{
    public partial class ViewController : NSViewController
    {
        private bool stopPings = false;
        private int runningTasks = 0;
        private object locker = new object();

        public ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            btnCopy.Enabled = false;
            cmbDelimiter.Enabled = false;

            chkPIng.IntValue = Settings.PingCheck;
            if (Settings.PingCheck == 0)
            {
                chkList.IntValue = 0;
                chkList.Enabled = false;
            }
            else
            {
                chkList.IntValue = Settings.ListCheck;
            }

            chkDNS.IntValue = Settings.DNSCheck;
            cmbDelimiter.SelectItem(Settings.PingDelimiter);

            W.LoadServices();
        }
        
        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        [Action("txtNetworkAction:")]
        public void PressedEnter(NSObject sender)
        {
            btnList(this);
        }

        [Action("PrefsWindow:")]
        public void PrefsWindow(NSObject sender)
        {
            PrefsWindowController prefsWindow = new PrefsWindowController();
            prefsWindow.ShowWindow(this);
        }

        private void PingThread(object state)
        {
            object[] arguments = state as object[];

            List<string> ipList = (List<string>)arguments[0];
            int listPingable = (int)arguments[1];
            int checkDNS = (int)arguments[2];

            foreach (string ip in ipList)
            {
                PingReply pinger = W.Ping(ip);
                if (pinger.Status == IPStatus.Success)
                {
                    string hostname = "";
                    if (checkDNS == 1) hostname = W.dnsLookup(ip);

                    // successful ping, add details to tableview datasource
                    AddressEntryDelegate.DataSource.AddressEntries.Add(new AddressEntry(
                        ip,
                        "UP",
                        pinger.RoundtripTime,
                        pinger.Options.Ttl,
                        hostname));
                }
                else
                {
                    if (listPingable != 1)
                    {
                        // listing unpingable hosts, add to tableview datasource
                        AddressEntryDelegate.DataSource.AddressEntries.Add(new AddressEntry(ip, "DOWN"));
                    }
                }
                
                ReloadTable();

                if (stopPings == true) break;
            }

            lock(locker)
            {
                runningTasks--;
                Monitor.Pulse(locker);
            }

            return;
        }

        private void MonitorThread(List<List<string>> ipList)
        {
            int ip_count = 0;

            ToggleGUI(false);

            // add all the lists of IP addresses to the threadpool
            foreach (List<string> sublist in ipList)
            {
                int checkList = 0;
                int checkDNS = 0;

                InvokeOnMainThread(() =>
                {
                    checkList = chkList.IntValue;
                    checkDNS = chkDNS.IntValue;
                });

                ip_count += sublist.Count;
                lock (locker) runningTasks++;
                
                ThreadPool.QueueUserWorkItem(new WaitCallback(PingThread), new object[] { sublist, checkList, checkDNS });
            }

            setStatus("Pinging " + ip_count + " IPs...");

            lock (locker) while (runningTasks > 0) Monitor.Wait(locker);

            ReloadTable(true);
            setStatus(AddressEntryDelegate.DataSource.AddressEntries.Count.ToString() + " IPs found");
            ToggleGUI(true);

            return;
        }

        private void setStatus(string status)
        {
            InvokeOnMainThread(() => { lblStatus.StringValue = status; });
        }

        private void ReloadTable(bool sort = false)
        {
            if (sort) AddressEntryDelegate.DataSource.Sort("IP", true);
            InvokeOnMainThread(() => { tblList.ReloadData(); });
        }

        partial void btnStop(NSObject sender)
        {
            stopPings = true;
        }

        partial void cmdDelimiter_Click(NSObject sender)
        {
            Settings.PingDelimiter = cmbDelimiter.SelectedItem.Title;
        }

        partial void chkList_Click(NSObject sender)
        {
            Settings.ListCheck = chkList.IntValue;
        }

        partial void chkDNS_Click(NSObject sender)
        {
            Settings.DNSCheck = chkDNS.IntValue;
        }

        partial void chkPingAction(NSObject sender)
        {
            Settings.PingCheck = chkPIng.IntValue;

            if (chkPIng.IntValue == 0)
            {
                chkList.IntValue = 0;
                chkList.Enabled = false;
            } else
            {
                chkList.Enabled = true;
                chkList.IntValue = 1;
            }
        }

        partial void btnCopyAction(NSObject sender)
        {
            string delim = "";
            StringBuilder value = new StringBuilder();
            

            switch (cmbDelimiter.SelectedItem.Title)
            {
                case "Newline": delim = Environment.NewLine; break;
                case "Comma":   delim = ",";  break;
                case "Tab":     delim = "\t"; break;
                case "Space":   delim = " ";  break;
            }

            foreach (AddressEntry ip in AddressEntryDelegate.DataSource.AddressEntries)
            {
                value.Append(ip.Address + delim);
            }

            W.CopyString(value.ToString().TrimEnd());
        }

        private void ToggleGUI(bool enabled)
        {
            InvokeOnMainThread(() =>
            {
                if (enabled == true)
                {
                    prgSpinner.StopAnimation(this);
                    btnListOutlet.Hidden = false;
                    btnStopOutlet.Hidden = true;
                    btnStopOutlet.Enabled = false;
                    btnCopy.Enabled = true;
                    cmbDelimiter.Enabled = true;
                    txtNetwork.Enabled = true;
                    chkList.Enabled = true;
                    chkPIng.Enabled = true;
                    chkDNS.Enabled = true;
                    tblList.Enabled = true;
                    tblList.Menu = popupMenu;
                }
                else
                {
                    prgSpinner.StartAnimation(this);
                    btnListOutlet.Hidden = true;
                    btnStopOutlet.Hidden = false;
                    btnStopOutlet.Enabled = true;
                    btnCopy.Enabled = false;
                    cmbDelimiter.Enabled = false;
                    txtNetwork.Enabled = false;
                    chkList.Enabled = false;
                    chkPIng.Enabled = false;
                    chkDNS.Enabled = false;
                    tblList.Enabled = false;
                    tblList.Menu = null;
                }
            });
        }

        partial void btnList(NSObject sender)
        {
            string errormsg = "";
            string network = txtNetwork.StringValue;

            if (network != null)
            {
                IPNetwork ipnetwork = null;
                IPAddressCollection subnet = null;
                bool invalid = false;

                if (!W.IsValidIP(network))
                {
                    invalid = true;
                    errormsg = "Invalid IP or network address. When specifying a network address you must use CIDR notation. Example: 192.168.1.0/24";
                } else
                {
                    try
                    {
                        // get list of IPs
                        ipnetwork = IPNetwork.Parse(network);
                        subnet = IPNetwork.ListIPAddress(ipnetwork);
                    }
                    catch (ArgumentException)
                    {
                        invalid = true;
                        errormsg = "Failed to retrieve IP addresses for the network.";
                    }
                }

                if (invalid == false)
                {
                    ToggleGUI(false);

                    AddressEntryDelegate.DataSource = new AddressEntryDataSource();
                    tblList.Delegate = new AddressEntryDelegate(AddressEntryDelegate.DataSource);
                    tblList.DataSource = AddressEntryDelegate.DataSource;
                    tblList.ReloadData();

                    List<string> subnetwork = new List<string>();
                    foreach (IPAddress ip in subnet) subnetwork.Add(ip.ToString());

                    if (chkPIng.IntValue == 1)
                    {
                        runningTasks = 0;
                        stopPings = false;

                        // launch monitoring thread that fills threadpool
                        Thread monitor = new Thread(() => { MonitorThread(W.Split<string>(subnetwork)); });
                        monitor.Start();
                    } else
                    {
                        foreach (string ip in subnetwork)
                        {
                            string[] split_ip = ip.ToString().Split(".");
                            if (split_ip[3] != "0" && split_ip[3] != "255")
                            {
                                string hostname = "";
                                if (chkDNS.IntValue == 1) hostname = W.dnsLookup(ip.ToString());

                                AddressEntryDelegate.DataSource.AddressEntries.Add(new AddressEntry(ip.ToString(), "", 0, 0, hostname));
                            }
                        }

                        AddressEntryDelegate.DataSource.Sort("IP", true);
                        tblList.ReloadData();

                        setStatus(tblList.RowCount + " IPs found");
                        ToggleGUI(true);
                    }
                } else
                {
                    // invalid input, show an alert
                    W.Error(errormsg);
                }
            }
        }

        partial void CopyMenuAction(NSObject sender)
        {
            W.CopyString(W.CurrentIP);
        }

        partial void mnuPingAction(NSObject sender)
        {
            PingWindowController pingWindow = new PingWindowController(W.CurrentIP);
            pingWindow.ShowWindow(this);
        }

        partial void mnuPortScan_Click(NSObject sender)
        {
            PortScannerController portScanner = new PortScannerController(W.CurrentIP);
            portScanner.ShowWindow(this);
        }

        partial void mnuUDPScan(NSObject sender)
        {
            PortScannerController portScanner = new PortScannerController(W.CurrentIP, "udp");
            portScanner.ShowWindow(this);
        }
    }
}
