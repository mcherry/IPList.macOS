using System;
using AppKit;
using Foundation;
using LukeSkywalker.IPNetwork;
using Plugin.Clipboard;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using IPAddressCollection = LukeSkywalker.IPNetwork.IPAddressCollection;

namespace IPList
{
    public partial class ViewController : NSViewController
    {
        private bool StopPings = false;
        private List<string> ipList = new List<string>();
        private Thread[] ThreadList = null;

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

        private void PingThread(ref AddressEntryDataSource DataSource, List<string> IPList)
        {
            bool pingHosts = true;
            bool listPingable = true;

            // manipulate items in GUI main thread
            InvokeOnMainThread(() =>
            {
                pingHosts &= chkPIng.IntValue == 1;
                listPingable &= chkList.IntValue == 1;
            });

            foreach (string ip in IPList)
            {
               
                if (Warehouse.Ping(ip) == true)
                {
                    // successful ping, add IP and status to tableview datasource and IP list
                    DataSource.AddressEntries.Add(new AddressEntry(ip, "UP"));
                    ipList.Add(ip);
                }
                else
                {
                    if (listPingable != true)
                    {
                        // listing unpingable hosts, add to tableview datasource and IP list
                        DataSource.AddressEntries.Add(new AddressEntry(ip, "DOWN"));
                        ipList.Add(ip);
                    }
                }

                // update GUI
                InvokeOnMainThread(() =>
                {
                    tblList.ReloadData();
                });

                if (this.StopPings == true) break;
            }

            return;
        }

        private void ThreadMonitor(NSObject sender, ref AddressEntryDataSource DataSource)
        {
            int cleanup = 0;

            while (true)
            {
                cleanup = 0;

                foreach (Thread thread in ThreadList)
                {
                    if (thread.IsAlive == true) cleanup++;
                }

                if (cleanup == 0) break;
                Thread.Sleep(500);
            }

            DataSource.Sort("IP", true);
            ipList.Sort(ExtensionMethods.CompareTo);

            InvokeOnMainThread(() =>
            {
                prgSpinner.StopAnimation(sender);
                tblList.ReloadData();

                lblStatus.StringValue = ipList.Count.ToString() + " IP addresses found";
                ToggleGUI(true);
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

            foreach (string ip in ipList)
            {
                clip_val += ip + delim;
            }

            CrossClipboard.Current.SetText(clip_val.TrimEnd());
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
                List<List<string>> IPList = null;
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

                    ipList.Clear();
                    AddressEntryDataSource DataSource = new AddressEntryDataSource();
                    tblList.Delegate = new AddressEntryDelegate(DataSource);
                    tblList.DataSource = DataSource;

                    if (chkPIng.IntValue == 1)
                    {
                        this.StopPings = false;

                        IPList = Lists.Split<string>(subnet);
                        ThreadList = new Thread[IPList.Count];

                        int a = 0;
                        int ip_count = 0;
                        foreach (List<string> sublist in IPList)
                        {
                            ip_count += sublist.Count;

                            ThreadList[a] = new Thread(() =>
                            {
                                PingThread(ref DataSource, sublist);
                            });
                            ThreadList[a].Start();

                            a++;
                        }

                        Thread threadMonitor = new Thread(() =>
                        {
                            ThreadMonitor(sender, ref DataSource);
                        });
                        threadMonitor.Start();

                        ToggleGUI(false);
                        lblStatus.StringValue = "Pinging " + ip_count.ToString() + " IP addresses";
                    } else
                    {
                        foreach (IPAddress ip in subnet)
                        {
                            string[] split_ip = ip.ToString().Split(".");
                            if (split_ip[3] != "0" && split_ip[3] != "255")
                            {
                                DataSource.AddressEntries.Add(new AddressEntry(ip.ToString(), "Unknown"));
                                ipList.Add(ip.ToString());
                            }
                        }

                        DataSource.Sort("IP", true);
                        ipList.Sort(ExtensionMethods.CompareTo);
                        tblList.ReloadData();

                        ToggleGUI(true);
                        lblStatus.StringValue = ipList.Count.ToString() + " IP addresses found";
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

                prgSpinner.StopAnimation(sender);
            }
        }

        partial void CopyMenuAction(NSObject sender)
        {
            CrossClipboard.Current.SetText(Globals.CurrentIP);
        }
    }
}
