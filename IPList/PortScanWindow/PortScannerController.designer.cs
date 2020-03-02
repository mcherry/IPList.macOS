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
		AppKit.NSTextField lblBroadcast { get; set; }

		[Outlet]
		AppKit.NSTextField lblDNS { get; set; }

		[Outlet]
		AppKit.NSTextField lblIP { get; set; }

		[Outlet]
		AppKit.NSTextField lblLatency { get; set; }

		[Outlet]
		AppKit.NSTextField lblMAC { get; set; }

		[Outlet]
		AppKit.NSTextField lblNetmask { get; set; }

		[Outlet]
		AppKit.NSTextField lblProtocol { get; set; }

		[Outlet]
		AppKit.NSTextField lblStat { get; set; }

		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		AppKit.NSTextField lblTTL { get; set; }

		[Outlet]
		AppKit.NSTextField lblVendor { get; set; }

		[Outlet]
		AppKit.NSMenu portScanContextMenu { get; set; }

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

		[Action ("mnuCopyData_Click:")]
		partial void mnuCopyData_Click (Foundation.NSObject sender);

		[Action ("mnuCopyPort_Click:")]
		partial void mnuCopyPort_Click (Foundation.NSObject sender);

		[Action ("mnuDNS_Click:")]
		partial void mnuDNS_Click (Foundation.NSObject sender);

		[Action ("mnuMAC_Click:")]
		partial void mnuMAC_Click (Foundation.NSObject sender);

		[Action ("mnuService_Click:")]
		partial void mnuService_Click (Foundation.NSObject sender);

		[Action ("mnuViewData_Click:")]
		partial void mnuViewData_Click (Foundation.NSObject sender);

		[Action ("mnyRow_Click:")]
		partial void mnyRow_Click (Foundation.NSObject sender);

		[Action ("tblPorts_DoubleCliekc:")]
		partial void tblPorts_DoubleCliekc (Foundation.NSObject sender);
		
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

			if (lblBroadcast != null) {
				lblBroadcast.Dispose ();
				lblBroadcast = null;
			}

			if (lblDNS != null) {
				lblDNS.Dispose ();
				lblDNS = null;
			}

			if (lblIP != null) {
				lblIP.Dispose ();
				lblIP = null;
			}

			if (lblLatency != null) {
				lblLatency.Dispose ();
				lblLatency = null;
			}

			if (lblMAC != null) {
				lblMAC.Dispose ();
				lblMAC = null;
			}

			if (lblNetmask != null) {
				lblNetmask.Dispose ();
				lblNetmask = null;
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

			if (lblVendor != null) {
				lblVendor.Dispose ();
				lblVendor = null;
			}

			if (portScanContextMenu != null) {
				portScanContextMenu.Dispose ();
				portScanContextMenu = null;
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
