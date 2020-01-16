using AppKit;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace IPList
{
    public class W
    {
        public static IDictionary<string, string> Services = new Dictionary<string, string>();

        public W() { }

        // basic validation for IP addresses and CIDR networks
        public static bool IsValidIP(string ip)
        {
            if (string.IsNullOrEmpty(ip) || string.IsNullOrWhiteSpace(ip)) return false;

            string[] octets = ip.Split(".");
            if (octets.Length != 4) return false;

            if (octets[3].Contains("/"))
            {
                string[] values = octets[3].Split("/");
                octets[3] = values[0];
            }

            foreach (string octet in octets) if (int.Parse(octet) < 0 || int.Parse(octet) > 255) return false;
            return true;
        }

        // send a single ping
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
                // continue
            }
            finally
            {
                // cleanup
                if (pinger != null) pinger.Dispose();
            }

            return reply;
        }

        // copy a string to the pasteboard
        public static void CopyString(string text)
        {
            if (text != null) {
                NSPasteboard clipboard = NSPasteboard.GeneralPasteboard;
                string[] types = { "NSStringPboardType" };

                clipboard.DeclareTypes(types, null);
                clipboard.SetStringForType(text, types[0]);
            }
        }

        // get a service name from the Services dictionary
        public static string GetServiceName(string port)
        {
            if (Services.TryGetValue(port, out string value)) return value;
            if (value == null) value = "";

            return value;
        }

        // load /etc/services into a dictionary
        // skips lines starting with # and only extracts TCP ports
        public static void LoadServices()
        {
            string line;

            StreamReader file = new StreamReader(@"/etc/services");
            while ((line = file.ReadLine()) != null)
            {
                if (!line.StartsWith("#"))
                {
                    string[] entry = new Regex("  +").Split(line);
                    if (entry.Length > 1)
                    {
                        string[] port = entry[1].Split("/");
                        if (port.Length > 1 && !port[0].StartsWith("#") && port[1] == "tcp")
                        {
                            Services[port[0]] = entry[0];
                        }
                    }
                }
            }
            file.Close();

            return;
        }
    }
}
