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
	[Register ("PortScannerController")]
	partial class PortScannerController
	{
		[Outlet]
		AppKit.NSButton btnCopy { get; set; }

		[Outlet]
		AppKit.NSButton btnStart { get; set; }

		[Outlet]
		AppKit.NSButton btnStop { get; set; }

		[Outlet]
		AppKit.NSPopUpButton cmbDelim { get; set; }

		[Outlet]
		AppKit.NSTextField lblIP { get; set; }

		[Outlet]
		AppKit.NSTextField lblLatency { get; set; }

		[Outlet]
		AppKit.NSTextField lblProtocol { get; set; }

		[Outlet]
		AppKit.NSTextField lblStat { get; set; }

		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		AppKit.NSTextField lblTTL { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator prgStatus { get; set; }

		[Outlet]
		AppKit.NSTableColumn tblPort_DataCol { get; set; }

		[Outlet]
		AppKit.NSTableColumn tblPort_PortCol { get; set; }

		[Outlet]
		AppKit.NSTableColumn tblPort_ServiceCol { get; set; }

		[Outlet]
		AppKit.NSTableView tblPorts { get; set; }

		[Action ("btnCopy_Click:")]
		partial void btnCopy_Click (Foundation.NSObject sender);

		[Action ("btnStart_Click:")]
		partial void btnStart_Click (Foundation.NSObject sender);

		[Action ("btnStop_Click:")]
		partial void btnStop_Click (Foundation.NSObject sender);

		[Action ("chkAddPorts_Click:")]
		partial void chkAddPorts_Click (Foundation.NSObject sender);

		[Action ("cmbDelim_Click:")]
		partial void cmbDelim_Click (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCopy != null) {
				btnCopy.Dispose ();
				btnCopy = null;
			}

			if (btnStart != null) {
				btnStart.Dispose ();
				btnStart = null;
			}

			if (btnStop != null) {
				btnStop.Dispose ();
				btnStop = null;
			}

			if (cmbDelim != null) {
				cmbDelim.Dispose ();
				cmbDelim = null;
			}

			if (lblIP != null) {
				lblIP.Dispose ();
				lblIP = null;
			}

			if (lblLatency != null) {
				lblLatency.Dispose ();
				lblLatency = null;
			}

			if (lblProtocol != null) {
				lblProtocol.Dispose ();
				lblProtocol = null;
			}

			if (lblStat != null) {
				lblStat.Dispose ();
				lblStat = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (lblTTL != null) {
				lblTTL.Dispose ();
				lblTTL = null;
			}

			if (prgStatus != null) {
				prgStatus.Dispose ();
				prgStatus = null;
			}

			if (tblPort_DataCol != null) {
				tblPort_DataCol.Dispose ();
				tblPort_DataCol = null;
			}

			if (tblPort_PortCol != null) {
				tblPort_PortCol.Dispose ();
				tblPort_PortCol = null;
			}

			if (tblPort_ServiceCol != null) {
				tblPort_ServiceCol.Dispose ();
				tblPort_ServiceCol = null;
			}

			if (tblPorts != null) {
				tblPorts.Dispose ();
				tblPorts = null;
			}
		}
	}
}
