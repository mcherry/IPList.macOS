﻿using AppKit;
using System.Net.NetworkInformation;

namespace IPList
{
    public class Warehouse
    {
        public Warehouse()
        {
        }

        public static bool Ping(string host, int timeout = 500)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                // ping host
                pinger = new Ping();
                PingReply reply = pinger.Send(host, timeout);
                pingable = reply.Status == IPStatus.Success;
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

            return pingable;
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
