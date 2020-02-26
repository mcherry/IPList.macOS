using System;
using System.Diagnostics;
using Foundation;
using AppKit;

namespace IPList
{
    public partial class AboutWindowController : NSWindowController
    {
        public AboutWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public AboutWindowController(NSCoder coder) : base(coder)
        {
        }

        public AboutWindowController() : base("AboutWindow")
        {
        }

        [Action("UpdateCheck:")]
        public void UpdateCheck(NSObject sender)
        {
            W.UpdateCheck(true);
        }

        [Action("showHelp:")]
        public void OpenProjectPage(NSObject sender)
        {
            Process.Start(W.projectURL);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            imgLogo.Image = NSImage.ImageNamed("AppIcon");
            lblVersion.StringValue = "Version " + W.appVersion.ToString() + " Build " + W.appBuild.ToString();
            txtLicense.Value =
                "Redistribution and use in source and binary forms, with or without modification, " +
                "are permitted provided that the following conditions are met:\n\n" +
                "1.Redistributions of source code must retain the above copyright notice, this "+
                "list of conditions and the following disclaimer.\n\n" +
                "2.Redistributions in binary form must reproduce the above copyright notice, " +
                "this list of conditions and the following disclaimer in the documentation and / or " +
                "other materials provided with the distribution.\n\n" +
                "THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" AND " +
                "ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED " +
                "WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. " +
                "IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, " +
                "INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES(INCLUDING, " +
                "BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, " +
                "DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF " +
                "LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE " +
                "OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED " +
                "OF THE POSSIBILITY OF SUCH DAMAGE.\n\n" +
                "This program uses the IPNetwork library which is Copyright 2015 lduchosal and located " +
                "at https://github.com/lduchosal/ipnetwork\n\n" +
                "This program also uses the SharpCifs.Std library which is Copyright 2017 Do-Be's. " +
                "SharpCifs.Std is licensed under the LGPL v2.1 license which may be viewed at " +
                "https://github.com/ume05rw/SharpCifs.Std/blob/master/LICENSE. The project may be " +
                "downloaded at https://github.com/ume05rw/SharpCifs.Std\n\n" +
                "Mac and macOS are trademarks of Apple Inc., registered in the United States and other " +
                "countries.";
        }

        public new AboutWindow Window
        {
            get { return (AboutWindow)base.Window; }
        }
    }
}
