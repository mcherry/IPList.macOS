using AppKit;
using Foundation;
using LukeSkywalker.IPNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private AddressEntryDelegate Delegate;
        private AddressEntryDataSource DataSource;

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

            if (Settings.UpdateCheck == 1) W.UpdateCheck();

            // required to ignore invalid ssl certs
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
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

        [Action("UpdateCheck:")]
        public void UpdateCheck(NSObject sender)
        {
            W.UpdateCheck(true);
        }

        [Action("showHelp:")]
        public void OpenProjectPage(NSObject sender)
        {
            Process.Start(W.projectURL);
        }

        [Action("showAbout:")]
        public void ShowAbout(NSObject sender)
        {
            AboutWindowController aboutWin = new AboutWindowController();
            aboutWin.ShowWindow(this);
        }

        [Action("mnuWhois:")]
        public void mnuWhois_Click(NSObject sender)
        {
            WhoisWindowController whoisWin = new WhoisWindowController();
            whoisWin.ShowWindow(this);
        }

        private void PingThread(object state)
        {
            object[] arguments = state as object[];

            List<string> ipList = (List<string>)arguments[0];
            int listPingable = (int)arguments[1];
            int checkDNS = (int)arguments[2];

            foreach (string ip in ipList)
            {
                string hostname = "";
                if (checkDNS == 1) hostname = W.DnsLookup(ip);

                PingReply pinger = W.Ping(ip);
                if (pinger.Status == IPStatus.Success)
                {
                    // successful ping, add details to tableview datasource
                    DataSource.AddressEntries.Add(new AddressEntry(
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
                        DataSource.AddressEntries.Add(new AddressEntry(ip, "DOWN", 0, 0, hostname));
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
            setStatus(DataSource.AddressEntries.Count.ToString() + " IPs found");

            RefreshARP();

            ReloadTable();
            ToggleGUI(true);

            return;
        }

        private void RefreshARP()
        {
            int index = 0;
            W.LoadARPTable();
            foreach (AddressEntry item in DataSource.AddressEntries)
            {
                DataSource.AddressEntries[index].MAC = W.GetMAC(item.Address);
                index++;
            }
        }

        private void IPListThread(List<string> network, int dnsCheck)
        {
            foreach (string ip in network)
            {
                if (W.IpInRange(ip.ToString()))
                {
                    string hostname = "";
                    if (dnsCheck == 1) hostname = W.DnsLookup(ip.ToString());

                    DataSource.AddressEntries.Add(new AddressEntry(ip.ToString(), "", 0, 0, hostname));
                    ReloadTable();
                }
            }

            setStatus(network.Count + " IPs found");
            RefreshARP();
            ToggleGUI(true);
        }

        private void setStatus(string status)
        {
            InvokeOnMainThread(() => { lblStatus.StringValue = status; });
        }

        private void ReloadTable(bool sort = false)
        {
            if (sort) DataSource.Sort("IP", sort);
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
            string delim = GetDelimiter();
            StringBuilder value = new StringBuilder();

            foreach (AddressEntry ip in DataSource.AddressEntries)
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
                    if (chkPIng.IntValue == 1) chkList.Enabled = true;
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
                    if (chkPIng.IntValue == 1) chkList.Enabled = false;
                    chkPIng.Enabled = false;
                    chkDNS.Enabled = false;
                    tblList.Enabled = false;
                    tblList.Menu = null;
                }
            });
        }

        partial void tblList_DoubleClick(NSObject sender)
        {
            mnuPortScan_Click(sender);
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

                    DataSource = new AddressEntryDataSource();
                    Delegate = new AddressEntryDelegate(DataSource);
                    tblList.Delegate = Delegate;
                    tblList.DataSource = DataSource;
                    ReloadTable();

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
                        int checkDNS = chkDNS.IntValue;
                        Thread list = new Thread(() => { IPListThread(subnetwork, checkDNS); });
                        list.Start();
                    }
                } else
                {
                    // invalid input, show an alert
                    W.Alert("Error", errormsg, NSAlertStyle.Critical);
                }
            }
        }

        partial void CopyMenuAction(NSObject sender)
        {
            AddressEntry row = GetSelectedRow();
            if (row != null)
            {
                W.CopyString(row.Address);
            }
        }

        partial void mnyCopyDNS_Click(NSObject sender)
        {
            AddressEntry row = GetSelectedRow();
            if (row != null)
            {
                W.CopyString(row.DNS);
            }
        }

        partial void mnuPingAction(NSObject sender)
        {
            if (tblList.SelectedRow != null && tblList.SelectedRow != -1)
            {
                PingWindowController pingWindow = new PingWindowController(GetSelectedRow().Address);
                pingWindow.ShowWindow(this);
            }
        }

        partial void mnuPortScan_Click(NSObject sender)
        {
            AddressEntry row = GetSelectedRow();
            if (row != null)
            {
                PortScannerController portScanner = new PortScannerController(row);
                portScanner.ShowWindow(this);
            }
        }

        partial void mnuUDPScan(NSObject sender)
        {
            AddressEntry row = GetSelectedRow();
            if (row != null)
            {
                PortScannerController portScanner = new PortScannerController(row, "udp");
                portScanner.ShowWindow(this);
            }
        }

        private AddressEntry GetSelectedRow()
        {
            if (tblList.SelectedRow != null && tblList.SelectedRow != -1)
            {
                return Delegate.GetRow(tblList.SelectedRow);
            }

            return null;
        }

        partial void mnuRow_Click(NSObject sender)
        {
            AddressEntry row = GetSelectedRow();
            if (row != null)
            {
                string delim = GetDelimiter();
                W.CopyString(
                    row.Address + delim +
                    row.Status + delim +
                    row.Latency + delim +
                    row.TTL + delim +
                    row.DNS + delim +
                    row.MAC);
            }
        }

        partial void mnuMAC_Cick(NSObject sender)
        {
            AddressEntry row = GetSelectedRow();
            if (row != null)
            {
                W.CopyString(row.MAC);
            }
        }

        private string GetDelimiter()
        {
            string delim = "";
            switch (cmbDelimiter.SelectedItem.Title)
            {
                case "Newline": delim = Environment.NewLine; break;
                case "Comma": delim = ","; break;
                case "Tab": delim = "\t"; break;
                case "Space": delim = " "; break;
            }

            return delim;
        }
    }
}
