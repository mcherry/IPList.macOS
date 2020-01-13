using System;

using Foundation;
using AppKit;

namespace IPList
{
    public partial class PingWindow : NSWindow
    {
        public PingWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public PingWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
