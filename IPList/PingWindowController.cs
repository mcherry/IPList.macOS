using System;
using System.Threading;
using System.Net.NetworkInformation;
using Foundation;
using AppKit;

namespace IPList
{
    public partial class PingWindowController : NSWindowController
    {
        private string IPAddress;
        private bool StopPing = false;
        private Thread Pinger = null;

        public PingWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public PingWindowController(NSCoder coder) : base(coder)
        {
        }

        public PingWindowController(string ip_address) : base("PingWindow")
        {
            this.IPAddress = ip_address;
        }

        private void PingThread()
        {
            while (true)
            {
                PingReply reply = Warehouse.Ping(this.IPAddress);
                if (reply.Status == IPStatus.Success)
                {
                    InvokeOnMainThread(() =>
                    {
                        lblStatus.StringValue = "UP";
                        lblLatency.StringValue = reply.RoundtripTime.ToString() + "ms";
                        lblTTL.StringValue = reply.Options.Ttl.ToString();
                    });
                } else
                {
                    InvokeOnMainThread(() =>
                    {
                        lblStatus.StringValue = "DOWN";
                        lblLatency.StringValue = "";
                        lblTTL.StringValue = "";
                    });
                }

                if (this.StopPing == true)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            lblIP.StringValue = this.IPAddress;
            prgStatus.StartAnimation(this);

            Pinger = new Thread(() => {
                PingThread();
            });
            Pinger.Start();
        }

        public new PingWindow Window
        {
            get { return (PingWindow)base.Window; }
        }

        partial void btnStop_Click(NSObject sender)
        {
            Pinger.Abort();
            Window.Close();
        }
    }
}
