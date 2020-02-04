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
	[Register ("DataViewerWindowController")]
	partial class DataViewerWindowController
	{
		[Outlet]
		AppKit.NSTextView txtData { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtData != null) {
				txtData.Dispose ();
				txtData = null;
			}
		}
	}
}
