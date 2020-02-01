using System;
using System.Diagnostics;
using System.Threading;
using System.Net.NetworkInformation;
using Foundation;
using AppKit;

namespace IPList
{
    public partial class PingWindowController : NSWindowController
    {
        private string ipAddress;
        private Thread Pinger = null;

        public PingWindowController(IntPtr handle) : base(handle) { }

        [Export("initWithCoder:")]
        public PingWindowController(NSCoder coder) : base(coder) { }

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
            Process.Start(W.ProjectURL);
        }

        [Action("showAbout:")]
        public void ShowAbout(NSObject sender)
        {
            AboutWindowController aboutWin = new AboutWindowController();
            aboutWin.ShowWindow(this);
        }

        public PingWindowController(nint ipIndex) : base("PingWindow")
        {
            ipAddress = AddressEntryDelegate.GetSelectedIP(ipIndex);
            Window.Title = "Pinging " + ipAddress;
        }

        private void PingThread()
        {
            int pingCount = 1;

            while (true)
            {
                PingReply reply = W.Ping(ipAddress);
                if (reply.Status == IPStatus.Success)
                {
                    setPingStatus(pingCount, "UP", reply.RoundtripTime, reply.Options.Ttl);
                } else
                {
                    setPingStatus(pingCount, "DOWN");
                }

                pingCount++;
                Thread.Sleep(1000);
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            prgStatus.StartAnimation(this);
            lblIP.StringValue = ipAddress;

            Pinger = new Thread(() => { PingThread(); });
            Pinger.Start();
        }

        public new PingWindow Window
        {
            get { return (PingWindow)base.Window; }
        }

        private void setPingStatus(int pingCount, string status, long time = 0, int ttl = 0)
        {
            InvokeOnMainThread(() =>
            {
                Window.Title = "Pinging " + ipAddress + " (" + status + ")";
                lblStatus.StringValue = status;
                if (status != "DOWN")
                {
                    lblLatency.StringValue = time.ToString() + "ms";
                    lblTTL.StringValue = ttl.ToString();
                }
                lblCount.StringValue = pingCount.ToString();
            });
        }

        partial void btnStop_Click(NSObject sender)
        {
            Pinger.Abort();
            Window.Close();
        }
    }
}
