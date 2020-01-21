using AppKit;
using Foundation;
using LukeSkywalker.IPNetwork;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

            // Do any additional setup after loading the view.
            // default state of GUI
            btnCopy.Enabled = false;
            cmbDelimiter.Enabled = false;

            W.LoadServices();

            txtNetwork.EditingEnded += (object sender, EventArgs e) =>
            {
               btnList(this);
            };
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

        private void PingThread(object state)
        {
            object[] arguments = state as object[];

            bool pingHosts = true;
            bool listPingable = true;
            bool checkDNS = true;

            // manipulate items in GUI main thread
            InvokeOnMainThread(() =>
            {
                pingHosts &= chkPIng.IntValue == 1;
                listPingable &= chkList.IntValue == 1;
                checkDNS &= chkDNS.IntValue == 1;
            });

            foreach (string ip in (List<string>)arguments[0])
            {
                PingReply pinger = W.Ping(ip);
                if (pinger.Status == IPStatus.Success)
                {
                    string hostname = "";

                    if (checkDNS == true)
                    {
                        try
                        {
                            hostname = Dns.GetHostEntry(ip).HostName;
                        }
                        catch (SocketException)
                        {
                            hostname = "";
                        }
                        if (hostname == ip) hostname = "";
                    }

                    // successful ping, add details to tableview datasource
                    AddressEntryDelegate.DataSource.AddressEntries.Add(new AddressEntry(
                        ip,
                        "UP",
                        pinger.RoundtripTime.ToString() + "ms",
                        pinger.Options.Ttl.ToString(),
                        hostname));
                }
                else
                {
                    if (listPingable != true)
                    {
                        // listing unpingable hosts, add to tableview datasource
                        AddressEntryDelegate.DataSource.AddressEntries.Add(new AddressEntry(ip, "DOWN"));
                    }
                }

                // update GUI
                InvokeOnMainThread(() => { tblList.ReloadData(); });

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
                ip_count += sublist.Count;

                lock (locker) runningTasks++;
                ThreadPool.QueueUserWorkItem(new WaitCallback(PingThread), new object[] { sublist });
            }

            setStatus("Pinging " + ip_count + " IPs...");

            lock (locker) while (runningTasks > 0) Monitor.Wait(locker);

            AddressEntryDelegate.DataSource.Sort("IP", true);

            InvokeOnMainThread(() => { tblList.ReloadData(); });
            setStatus(AddressEntryDelegate.DataSource.AddressEntries.Count.ToString() + " IPs found");
            ToggleGUI(true);

            return;
        }

        private void setStatus(string status)
        {
            InvokeOnMainThread(() =>
            {
                lblStatus.StringValue = status;
            });
        }

        partial void btnStop(NSObject sender)
        {
            stopPings = true;
        }

        partial void chkPingAction(NSObject sender)
        {
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
            string clip_val = "";
            

            switch (cmbDelimiter.SelectedItem.Title)
            {
                case "Newline":
                    delim = Environment.NewLine;
                    break;
                case "Comma":
                    delim = ",";
                    break;
                case "Tab":
                    delim = "\t";
                    break;
                case "Space":
                    delim = " ";
                    break;
            }

            foreach (AddressEntry ip in AddressEntryDelegate.DataSource.AddressEntries)
            {
                clip_val += ip.Address + delim;
            }

            W.CopyString(clip_val.TrimEnd());
        }

        private void ToggleGUI(bool enabled)
        {
            InvokeOnMainThread(() =>
            {
                if (enabled == true)
                {
                    prgSpinner.StopAnimation(this);
                    prgSpinner.Hidden = true;
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
                    prgSpinner.Hidden = false;
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
                    errormsg = "Invalid IP address. Please use CIDR notation when specifying a network to list. Eg: 192.168.1.0/24";
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
                    AddressEntryDelegate.DataSource = new AddressEntryDataSource();
                    tblList.Delegate = new AddressEntryDelegate(AddressEntryDelegate.DataSource);
                    tblList.DataSource = AddressEntryDelegate.DataSource;
                    tblList.ReloadData();

                    if (chkPIng.IntValue == 1)
                    {
                        runningTasks = 0;
                        stopPings = false;

                        // launch monitoring thread that fills threadpool
                        Thread monitor = new Thread(() => { MonitorThread(W.Split<string>(subnet)); });
                        monitor.Start();
                    } else
                    {
                        foreach (IPAddress ip in subnet)
                        {
                            string[] split_ip = ip.ToString().Split(".");
                            if (split_ip[3] != "0" && split_ip[3] != "255")
                            {
                                AddressEntryDelegate.DataSource.AddressEntries.Add(new AddressEntry(ip.ToString()));
                            }
                        }

                        AddressEntryDelegate.DataSource.Sort("IP", true);
                        tblList.ReloadData();

                        setStatus(tblList.RowCount + " IPs found");
                    }
                } else
                {
                    // invalid input, show an alert
                    NSAlert alert = new NSAlert()
                    {
                        AlertStyle = NSAlertStyle.Critical,
                        InformativeText = errormsg,
                        MessageText = "Error"
                    };

                    alert.RunModal();
                    alert.Dispose();
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
    }
}
