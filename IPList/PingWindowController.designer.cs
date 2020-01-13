// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace IPList
{
	[Register ("PingWindowController")]
	partial class PingWindowController
	{
		[Outlet]
		AppKit.NSTextField lblIP { get; set; }

		[Outlet]
		AppKit.NSTextField lblIPAddress { get; set; }

		[Outlet]
		AppKit.NSTextField lblLatency { get; set; }

		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		AppKit.NSTextField lblTTL { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator prgStatus { get; set; }

		[Action ("btnStop_Click:")]
		partial void btnStop_Click (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblIP != null) {
				lblIP.Dispose ();
				lblIP = null;
			}

			if (lblIPAddress != null) {
				lblIPAddress.Dispose ();
				lblIPAddress = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (lblLatency != null) {
				lblLatency.Dispose ();
				lblLatency = null;
			}

			if (lblTTL != null) {
				lblTTL.Dispose ();
				lblTTL = null;
			}

			if (prgStatus != null) {
				prgStatus.Dispose ();
				prgStatus = null;
			}
		}
	}
}
