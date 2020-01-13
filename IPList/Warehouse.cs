using AppKit;
using System.Net.NetworkInformation;

namespace IPList
{
    public class Warehouse
    {
        public Warehouse()
        {
        }

        public static PingReply Ping(string host, int timeout = 500)
        {
            Ping pinger = null;
            PingReply reply = null;

            try
            {
                // ping host
                pinger = new Ping();
                reply = pinger.Send(host, timeout);
            }
            catch (PingException)
            {
                // nothing to see here, ignoring exceptions
            }
            finally
            {
                // cleanup
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return reply;
        }

        public static void CopyString(string text)
        {
            NSPasteboard clipboard = NSPasteboard.GeneralPasteboard;
            string[] types = { "NSStringPboardType" };

            clipboard.DeclareTypes(types, null);
            clipboard.SetStringForType(text, types[0]);
        }
    }
}
