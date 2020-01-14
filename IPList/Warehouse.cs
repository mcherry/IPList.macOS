using AppKit;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace IPList
{
    public class Warehouse
    {
        public static IDictionary<string, string> Services = new Dictionary<string, string>();

        public Warehouse()
        {
        }

        public static PingReply Ping(string host, int timeout = 750)
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

        public static void LoadServices()
        {
            string line;

            StreamReader file = new StreamReader(@"/etc/services");
            while ((line = file.ReadLine()) != null)
            {
                Regex regex = new Regex("  +");
                if (!line.StartsWith("#"))
                {
                    string[] entry = regex.Split(line);
                    if (entry.Length > 1)
                    {
                        string[] port = entry[1].Split("/");
                        if (port.Length > 1)
                        {
                            Services[port[0]] = entry[0];
                        }
                    }
                }
            }
            file.Close();
        }
    }
}
