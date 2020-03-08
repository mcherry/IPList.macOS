using AppKit;
using Foundation;
using System;
using System.Threading;

namespace IPList
{
    public partial class WhoisWindowController : NSWindowController
    {
        private string whoisHost;

        public WhoisWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public WhoisWindowController(NSCoder coder) : base(coder)
        {
        }

        public WhoisWindowController() : base("WhoisWindow")
        {
        }

        public WhoisWindowController(string host) : base("WhoisWindow")
        {
            whoisHost = host;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            if (whoisHost != null) txtHost.StringValue = whoisHost;
        }

        public new WhoisWindow Window
        {
            get { return (WhoisWindow)base.Window; }
        }

        partial void txtHost_Enter(NSObject sender)
        {
            btnLookup_Click(sender);
        }

        partial void btnLookup_Click(NSObject sender)
        {
            string host = txtHost.StringValue;
            Thread launcher = new Thread(() => { StartWhois(host); });
            launcher.Start();
        }

        private void StartWhois(string host)
        {
            if (host != null && host != "")
            {
                ToggleGUI(false);
                InvokeOnMainThread(() => { txtWhois.Value = W.Shell("whois", host); });
                ToggleGUI(true);
            }
        }

        private void ToggleGUI(bool enabled)
        {
            InvokeOnMainThread(() =>
            {
                if (enabled)
                {
                    txtHost.Enabled = true;
                    btnLookup.Enabled = true;
                }
                else
                {
                    txtHost.Enabled = false;
                    btnLookup.Enabled = false;
                }
            });
        }
    }
}
