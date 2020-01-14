using System;

using Foundation;
using AppKit;

namespace IPList
{
    public partial class PortScanner : NSWindow
    {
        public PortScanner(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public PortScanner(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
