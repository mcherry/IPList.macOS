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
            if (whoisHost != null)
            {
                txtHost.StringValue = whoisHost;
            }
        }

        public new WhoisWindow Window
        {
            get { return (WhoisWindow)base.Window; }
        }

        partial void txtHost_Enter(NSObject sender)
        {
            StartWhois();
        }

        partial void btnLookup_Click(NSObject sender)
        {
            StartWhois();
        }

        private void StartWhois()
        {
            if (txtHost.StringValue != null && txtHost.StringValue != "")
            {
                ToggleGUI(false);
                txtWhois.Value = W.Shell("whois", txtHost.StringValue);
                ToggleGUI(true);
            }
        }

        private void ToggleGUI(bool enabled)
        {
            if (enabled)
            {
                txtHost.Enabled = true;
                btnLookup.Enabled = true;
            } else
            {
                txtHost.Enabled = false;
                btnLookup.Enabled = false;
            }
        }
    }
}
