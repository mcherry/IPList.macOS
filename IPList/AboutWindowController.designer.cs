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
	[Register ("AboutWindowController")]
	partial class AboutWindowController
	{
		[Outlet]
		AppKit.NSImageView imgLogo { get; set; }

		[Outlet]
		AppKit.NSTextView txtLicense { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imgLogo != null) {
				imgLogo.Dispose ();
				imgLogo = null;
			}

			if (txtLicense != null) {
				txtLicense.Dispose ();
				txtLicense = null;
			}
		}
	}
}
