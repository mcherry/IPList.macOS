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
		AppKit.NSButton btnSave { get; set; }

		[Outlet]
		AppKit.NSButton chkUpdates { get; set; }

		[Outlet]
		AppKit.NSPopUpButton cmbPortList { get; set; }

		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator prgStatus { get; set; }

		[Outlet]
		AppKit.NSTextField txtPingTimeout { get; set; }

		[Outlet]
		AppKit.NSTextView txtPorts { get; set; }

		[Outlet]
		AppKit.NSTextField txtPortscanTimeout { get; set; }

		[Action ("btnSave_Click:")]
		partial void btnSave_Click (Foundation.NSObject sender);

		[Action ("chkUpdates_Click:")]
		partial void chkUpdates_Click (Foundation.NSObject sender);

		[Action ("cmbPortList_Click:")]
		partial void cmbPortList_Click (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (cmbPortList != null) {
				cmbPortList.Dispose ();
				cmbPortList = null;
			}

			if (txtPingTimeout != null) {
				txtPingTimeout.Dispose ();
				txtPingTimeout = null;
			}

			if (txtPorts != null) {
				txtPorts.Dispose ();
				txtPorts = null;
			}

			if (chkUpdates != null) {
				chkUpdates.Dispose ();
				chkUpdates = null;
			}

			if (txtPortscanTimeout != null) {
				txtPortscanTimeout.Dispose ();
				txtPortscanTimeout = null;
			}

			if (prgStatus != null) {
				prgStatus.Dispose ();
				prgStatus = null;
			}

			if (btnSave != null) {
				btnSave.Dispose ();
				btnSave = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}
		}
	}
}
