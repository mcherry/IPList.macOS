using System;

using Foundation;
using AppKit;

namespace IPList
{
    public partial class WhoisWindow : NSWindow
    {
        public WhoisWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public WhoisWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
