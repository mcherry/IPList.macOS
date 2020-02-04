using System;

using Foundation;
using AppKit;

namespace IPList
{
    public partial class DataViewerWindow : NSWindow
    {
        public DataViewerWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public DataViewerWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
