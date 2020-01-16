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
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton btnCopy { get; set; }

		[Outlet]
		AppKit.NSButton btnListOutlet { get; set; }

		[Outlet]
		AppKit.NSButton btnStopOutlet { get; set; }

		[Outlet]
		AppKit.NSButton chkList { get; set; }

		[Outlet]
		AppKit.NSButton chkPIng { get; set; }

		[Outlet]
		AppKit.NSPopUpButton cmbDelimiter { get; set; }

		[Outlet]
		AppKit.NSTableColumn colAddress { get; set; }

		[Outlet]
		AppKit.NSTableColumn colDNS { get; set; }

		[Outlet]
		AppKit.NSTableColumn colStatus { get; set; }

		[Outlet]
		AppKit.NSTextField lblDelim { get; set; }

		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		AppKit.NSMenu popupMenu { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator prgSpinner { get; set; }

		[Outlet]
		AppKit.NSTableView tblList { get; set; }

		[Outlet]
		AppKit.NSTextField txtNetwork { get; set; }

		[Action ("btnCopyAction:")]
		partial void btnCopyAction (Foundation.NSObject sender);

		[Action ("btnList:")]
		partial void btnList (Foundation.NSObject sender);

		[Action ("btnStop:")]
		partial void btnStop (Foundation.NSObject sender);

		[Action ("chkPingAction:")]
		partial void chkPingAction (Foundation.NSObject sender);

		[Action ("CopyMenuAction:")]
		partial void CopyMenuAction (Foundation.NSObject sender);

		[Action ("mnuPingAction:")]
		partial void mnuPingAction (Foundation.NSObject sender);

		[Action ("mnuPortScan_Click:")]
		partial void mnuPortScan_Click (Foundation.NSObject sender);

		[Action ("tblListAction:")]
		partial void tblListAction (Foundation.NSObject sender);

		[Action ("txtNetworkAction:")]
		partial void txtNetworkAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCopy != null) {
				btnCopy.Dispose ();
				btnCopy = null;
			}

			if (btnListOutlet != null) {
				btnListOutlet.Dispose ();
				btnListOutlet = null;
			}

			if (btnStopOutlet != null) {
				btnStopOutlet.Dispose ();
				btnStopOutlet = null;
			}

			if (chkList != null) {
				chkList.Dispose ();
				chkList = null;
			}

			if (chkPIng != null) {
				chkPIng.Dispose ();
				chkPIng = null;
			}

			if (cmbDelimiter != null) {
				cmbDelimiter.Dispose ();
				cmbDelimiter = null;
			}

			if (colAddress != null) {
				colAddress.Dispose ();
				colAddress = null;
			}

			if (colDNS != null) {
				colDNS.Dispose ();
				colDNS = null;
			}

			if (colStatus != null) {
				colStatus.Dispose ();
				colStatus = null;
			}

			if (lblDelim != null) {
				lblDelim.Dispose ();
				lblDelim = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (popupMenu != null) {
				popupMenu.Dispose ();
				popupMenu = null;
			}

			if (prgSpinner != null) {
				prgSpinner.Dispose ();
				prgSpinner = null;
			}

			if (tblList != null) {
				tblList.Dispose ();
				tblList = null;
			}

			if (txtNetwork != null) {
				txtNetwork.Dispose ();
				txtNetwork = null;
			}
		}
	}
}
