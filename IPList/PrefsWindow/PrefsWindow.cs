using System;

using Foundation;
using AppKit;

namespace IPList
{
    public partial class PrefsWindow : NSWindow
    {
        public PrefsWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public PrefsWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
