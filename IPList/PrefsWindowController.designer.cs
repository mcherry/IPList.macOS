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
	[Register ("PrefsWindowController")]
	partial class PrefsWindowController
	{
		[Outlet]
		AppKit.NSTextField txtPingTimeout { get; set; }

		[Outlet]
		AppKit.NSTextView txtPorts { get; set; }

		[Outlet]
		AppKit.NSTextField txtPortscanTimeout { get; set; }

		[Action ("btnSave_Click:")]
		partial void btnSave_Click (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (txtPorts != null) {
				txtPorts.Dispose ();
				txtPorts = null;
			}

			if (txtPingTimeout != null) {
				txtPingTimeout.Dispose ();
				txtPingTimeout = null;
			}

			if (txtPortscanTimeout != null) {
				txtPortscanTimeout.Dispose ();
				txtPortscanTimeout = null;
			}
		}
	}
}
