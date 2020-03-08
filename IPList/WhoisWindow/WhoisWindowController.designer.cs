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
	[Register ("WhoisWindowController")]
	partial class WhoisWindowController
	{
		[Outlet]
		AppKit.NSButton btnLookup { get; set; }

		[Outlet]
		AppKit.NSTextField txtHost { get; set; }

		[Outlet]
		AppKit.NSTextView txtWhois { get; set; }

		[Action ("btnLookup_Click:")]
		partial void btnLookup_Click (Foundation.NSObject sender);

		[Action ("txtHost_Enter:")]
		partial void txtHost_Enter (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (txtHost != null) {
				txtHost.Dispose ();
				txtHost = null;
			}

			if (btnLookup != null) {
				btnLookup.Dispose ();
				btnLookup = null;
			}

			if (txtWhois != null) {
				txtWhois.Dispose ();
				txtWhois = null;
			}
		}
	}
}
