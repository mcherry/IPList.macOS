using AppKit;
using Foundation;
using LukeSkywalker.IPNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace IPList
{
    public partial class PortScannerController : NSWindowController
    {
        private bool hostUp = false;
        private bool stopScan = false;
        private string ipAddress;
        private string protocol;

        private int runningTasks;
        private object locker = new object();

        public PortScannerController(IntPtr handle) : base(handle) { }

        [Export("initWithCoder:")]
        public PortScannerController(NSCoder coder) : base(coder) { }

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

        public PortScannerController(nint ipIndex, string scanProtocol = "tcp") : base("PortScanner")
        {
            ipAddress = AddressEntryDelegate.GetSelectedIP(ipIndex);
            protocol = scanProtocol;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            cmbDelim.SelectItem(Settings.PortDelimiter);

            PingReply reply = W.Ping(ipAddress);
            if (reply.Status == IPStatus.Success)
            {
                hostUp = true;
                setPingStatus("UP", reply.RoundtripTime, reply.Options.Ttl);
            }
            else
            {
                hostUp = false;
                setPingStatus("DOWN");
            }

            IPNetwork ipnetwork = IPNetwork.Parse(ipAddress);
            lblNetmask.StringValue = ipnetwork.Netmask.ToString();
            lblBroadcast.StringValue = ipnetwork.Broadcast.ToString();
            if (Settings.DNSCheck == 1) lblDNS.StringValue = W.DnsLookup(ipAddress);
            lblMAC.StringValue = W.GetMAC(ipAddress);

            StartScan();
        }

        private void ScannerThread(object state)
        {
            object[] arguments = state as object[];

            if (hostUp)
            {
                setStatus("Scanning " + string.Format("{0:n0}", Settings.PortList.Count) + " ports");

                foreach (int port in (List<int>)arguments[0])
                {
                    switch (protocol)
                    {
                        case "tcp":
                            PortInfo scanner = W.PortCheck(ipAddress, port);

                            if (scanner.Open)
                            {
                                PortEntryDelegate.DataSource.Ports.Add(new PortEntry(port, scanner.Name, scanner.Data));
                                ReloadTable();
                            }

                            break;
                        case "udp":
                            // not implemented
                            break;
                    }
                    
                    if (stopScan == true) break;
                }
            }

            lock (locker)
            {
                runningTasks--;
                Monitor.Pulse(locker);
            }

            return;
        }

        private void ReloadTable(bool sort = false)
        {
            if (sort) PortEntryDelegate.DataSource.Sort("Port", true);
            InvokeOnMainThread(() => { tblPorts.ReloadData(); });
        }

        private void MonitorThread(List<List<int>> sublist)
        {
            runningTasks = 0;

            foreach (List<int> pList in sublist)
            { 
                lock(locker) runningTasks++;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ScannerThread), new object[] { pList });
            }

            lock(locker) while (runningTasks > 0) Monitor.Wait(locker);

            ReloadTable(true);

            setStatus("Found " + PortEntryDelegate.DataSource.GetRowCount(tblPorts).ToString() + " open ports");
            ToggleGUI(true);

            return;
        }

        private void setPingStatus(string status, long time = 0, int ttl = 0)
        {
            InvokeOnMainThread(() =>
            {
                Window.Title = protocol.ToUpper() + " Scanning " + ipAddress + " (" + status + ")";
                lblStat.StringValue = status;
                if (status != "DOWN")
                {
                    lblLatency.StringValue = time.ToString() + "ms";
                    lblTTL.StringValue = ttl.ToString();
                }
            });
        }

        private void setStatus(string status)
        {
            InvokeOnMainThread(() => { lblStatus.StringValue = status; });
        }

        private void ToggleGUI(bool enabled)
        {
            InvokeOnMainThread(() =>
            {
                if (enabled == true)
                {
                    prgStatus.StopAnimation(this);
                    Window.Title = protocol.ToUpper() + " Scanned " + ipAddress;
                    btnStop.Enabled = false;
                    btnStop.Hidden = true;
                    btnStart.Hidden = false;
                    btnStart.Enabled = true;
                    cmbDelim.Enabled = true;
                    btnCopy.Enabled = true;
                    tblPorts.Enabled = true;
                    tblPorts.Menu = portScanContextMenu;
                }
                else
                {
                    prgStatus.StartAnimation(this);
                    Window.Title = protocol.ToUpper() + " Scanning " + ipAddress;
                    btnStart.Enabled = false;
                    btnStart.Hidden = true;
                    btnStop.Enabled = true;
                    btnStop.Hidden = false;
                    cmbDelim.Enabled = false;
                    btnCopy.Enabled = false;
                    tblPorts.Enabled = false;
                    tblPorts.Menu = null;
                }
            });
        }

        private void StartScan()
        {
            ToggleGUI(false);

            stopScan = false;
            lblIP.StringValue = ipAddress;

            PortEntryDelegate.DataSource = new PortEntryDataSource();
            tblPorts.Delegate = new PortEntryDelegate(PortEntryDelegate.DataSource);
            tblPorts.DataSource = PortEntryDelegate.DataSource;
            tblPorts.ReloadData();

            Thread monitor = new Thread(() => { MonitorThread(W.Split<int>(Settings.PortList)); });
            monitor.Start();
        }

        public new PortScanner Window
        {
            get { return (PortScanner)base.Window; }
        }

        partial void btnStart_Click(NSObject sender)
        {
            StartScan();
        }

        partial void btnStop_Click(NSObject sender)
        {
            stopScan = true;
            ToggleGUI(true);
        }

        partial void btnCopy_Click(NSObject sender)
        {
            string delim = "";
            StringBuilder value = new StringBuilder();
            
            switch (cmbDelim.SelectedItem.Title)
            {
                case "Newline": delim = Environment.NewLine; break;
                case "Comma":   delim = ",";  break;
                case "Tab":     delim = "\t"; break;
                case "Space":   delim = " ";  break;
            }
            
            foreach (PortEntry port in PortEntryDelegate.DataSource.Ports)
            {
                value.Append(port.Port + delim);
            }

            W.CopyString(value.ToString().TrimEnd());
        }

        partial void cmbDelim_Click(NSObject sender)
        {
            Settings.PortDelimiter = cmbDelim.SelectedItem.Title;
        }

        partial void mnuCopyPort_Click(NSObject sender)
        {
            if (tblPorts.SelectedRow != null && tblPorts.SelectedRow != -1)
            {
                W.CopyString(PortEntryDelegate.GetSelectedPort(tblPorts.SelectedRow));
            }
        }

        partial void mnuCopyData_Click(NSObject sender)
        {
            if (tblPorts.SelectedRow != null && tblPorts.SelectedRow != -1)
            {
                W.CopyString(PortEntryDelegate.GetSelectedData(tblPorts.SelectedRow));
            }
        }

        partial void mnuViewData_Click(NSObject sender)
        {
            if (tblPorts.SelectedRow != null && tblPorts.SelectedRow != -1)
            {
                DataViewerWindowController dataView = new DataViewerWindowController(tblPorts.SelectedRow, ipAddress);
                dataView.ShowWindow(this);
            }
        }

        partial void tblPorts_DoubleCliekc(NSObject sender)
        {
            mnuViewData_Click(sender);
        }
    }
}
 