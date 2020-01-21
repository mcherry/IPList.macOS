using System;
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
        private int pingCount = 0;

        public PingWindowController(IntPtr handle) : base(handle) { }

        [Export("initWithCoder:")]
        public PingWindowController(NSCoder coder) : base(coder) { }

        public PingWindowController(string ip_address) : base("PingWindow")
        {
            ipAddress = ip_address;
        }

        private void PingThread()
        {
            while (true)
            {
                pingCount++;

                PingReply reply = W.Ping(ipAddress);
                if (reply.Status == IPStatus.Success)
                {
                    setPingStatus("UP", reply.RoundtripTime.ToString() + "ms", reply.Options.Ttl.ToString());
                } else
                {
                    setPingStatus("DOWN");
                }

                Thread.Sleep(1000);
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            lblIP.StringValue = ipAddress;
            prgStatus.StartAnimation(this);

            Pinger = new Thread(() => { PingThread(); });
            Pinger.Start();
        }

        public new PingWindow Window
        {
            get { return (PingWindow)base.Window; }
        }

        private void setPingStatus(string status, string time = "", string ttl = "")
        {
            InvokeOnMainThread(() =>
            {
                lblStatus.StringValue = status;
                lblLatency.StringValue = time;
                lblTTL.StringValue = ttl;
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
