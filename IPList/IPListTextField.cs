using System;
using AppKit;

namespace IPList
{
	public partial class IPListTextField : NSTextField
	{
        public static int KeyCode = 0;

		public IPListTextField (IntPtr handle) : base (handle)
		{
		}

        public override void KeyUp(NSEvent theEvent)
        {
            base.KeyUp(theEvent);
            KeyCode = theEvent.KeyCode;
        }

        // this doesnt seem to work.
        public override void KeyDown(NSEvent theEvent)
        {
            base.KeyDown(theEvent);
            KeyCode = theEvent.KeyCode;
        }
    }
}
