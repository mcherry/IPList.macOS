using System;
using AppKit;
using Foundation;
using LukeSkywalker.IPNetwork;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using IPAddressCollection = LukeSkywalker.IPNetwork.IPAddressCollection;

namespace IPList
{
    public partial class ViewController : NSViewController
    {
        private bool StopPings = false;
        private int runningTasks = 0;
        private object locker = new object();

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            // default state of GUI
            btnCopy.Enabled = false;
            cmbDelimiter.Enabled = false;

            Warehouse.LoadServices();
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
            //List<string> IPList
            object[] array = state as object[];
            List<string> ipList = (List<string>)array[0];

            bool pingHosts = true;
            bool listPingable = true;

            // manipulate items in GUI main thread
            InvokeOnMainThread(() =>
            {
                pingHosts &= chkPIng.IntValue == 1;
                listPingable &= chkList.IntValue == 1;
            });

            foreach (string ip in ipList)
            {
                PingReply pinger = Warehouse.Ping(ip);
                if (pinger.Status == IPStatus.Success)
                {
                    // successful ping, add IP and status to tableview datasource and IP list
                    AddressEntryDelegate.DataSource.AddressEntries.Add(new AddressEntry(ip, "UP", pinger.RoundtripTime.ToString() + "ms", pinger.Options.Ttl.ToString()));
                }
                else
                {
                    if (listPingable != true)
                    {
                        // listing unpingable hosts, add to tableview datasource and IP list
                        AddressEntryDelegate.DataSource.AddressEntries.Add(new AddressEntry(ip, "DOWN"));
                    }
                }

                // update GUI
                InvokeOnMainThread(() => { tblList.ReloadData(); });

                if (this.StopPings == true) break;
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
            ThreadPool.SetMinThreads(3, 0);
            ThreadPool.SetMaxThreads(3, 0);

            foreach (List<string> sublist in ipList)
            {
                lock(locker) runningTasks++;
                ThreadPool.QueueUserWorkItem(new WaitCallback(PingThread), new object[] { sublist });
            }

            lock (locker)
            {
                while (runningTasks > 0) Monitor.Wait(locker);
            }

            AddressEntryDelegate.DataSource.Sort("IP", true);

            InvokeOnMainThread(() =>
            {
                tblList.ReloadData();
                lblStatus.StringValue = AddressEntryDelegate.DataSource.AddressEntries.Count.ToString() + " IP addresses found";
                ToggleGUI(true);

                prgSpinner.StopAnimation(this);
            });

            return;
        }

        partial void btnStop(NSObject sender)
        {
            this.StopPings = true;
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

            Warehouse.CopyString(clip_val.TrimEnd());
        }

        private void ToggleGUI(bool enabled)
        {
            if (enabled == true)
            {
                btnListOutlet.Hidden = false;
                btnStopOutlet.Hidden = true;
                btnStopOutlet.Enabled = false;
                btnCopy.Enabled = true;
                cmbDelimiter.Enabled = true;
                txtNetwork.Enabled = true;
                chkList.Enabled = true;
                chkPIng.Enabled = true;
                tblList.Enabled = true;
                tblList.Menu = popupMenu;
            } else
            {
                btnListOutlet.Hidden = true;
                btnStopOutlet.Hidden = false;
                btnStopOutlet.Enabled = true;
                btnCopy.Enabled = false;
                cmbDelimiter.Enabled = false;
                txtNetwork.Enabled = false;
                chkList.Enabled = false;
                chkPIng.Enabled = false;
                tblList.Enabled = false;
                tblList.Menu = null;
            }
        }

        partial void btnList(NSObject sender)
        {
            string network = txtNetwork.StringValue;
            if (network != null)
            {
                IPAddressCollection subnet = null;
                List<List<string>> ipList = null;
                bool invalid = false;

                try
                {
                    // get list of IPs
                    IPNetwork ipnetwork = IPNetwork.Parse(network);
                    subnet = IPNetwork.ListIPAddress(ipnetwork);
                } catch (System.ArgumentException)
                {
                    invalid = true;
                }

                if (invalid == false)
                {
                    prgSpinner.StartAnimation(sender);

                    AddressEntryDelegate.DataSource = new AddressEntryDataSource();
                    tblList.Delegate = new AddressEntryDelegate(AddressEntryDelegate.DataSource);
                    tblList.DataSource = AddressEntryDelegate.DataSource;
                    tblList.ReloadData();

                    if (chkPIng.IntValue == 1)
                    {
                        this.StopPings = false;

                        ipList = Lists.Split<string>(subnet);
                        runningTasks = 0;
                        Thread monitor = new Thread(() => { MonitorThread(ipList); });
                        monitor.Start();

                        ToggleGUI(false);
                        lblStatus.StringValue = "Pinging " + subnet.Count + " IP addresses";
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

                        ToggleGUI(true);
                        lblStatus.StringValue = tblList.RowCount + " IP addresses found";
                        prgSpinner.StopAnimation(this);
                    }
                } else
                {
                    // invalid input, show an alert
                    var alert = new NSAlert()
                    {
                        AlertStyle = NSAlertStyle.Critical,
                        InformativeText = "Please use CIDR notation when specifying a network to list. Eg: 192.168.1.0/24",
                        MessageText = "Invalid Network"
                    };

                    alert.RunModal();
                    alert.Dispose();
                }
            }
        }

        partial void CopyMenuAction(NSObject sender)
        {
            Warehouse.CopyString(Globals.CurrentIP);
        }

        partial void mnuPingAction(NSObject sender)
        {
            PingWindowController pingWindow = new PingWindowController(Globals.CurrentIP);
            pingWindow.ShowWindow(this);
        }

        partial void mnuPortScan_Click(NSObject sender)
        {
            PortScannerController portScanner = new PortScannerController(Globals.CurrentIP);
            portScanner.ShowWindow(this);
        }
    }
}
