using System;

using Foundation;
using AppKit;

namespace IPList
{
    public partial class AboutWindow : NSWindow
    {
        public AboutWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public AboutWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
