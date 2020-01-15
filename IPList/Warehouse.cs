using AppKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace IPList
{
    public class Warehouse
    {
        public static IDictionary<string, string> Services = new Dictionary<string, string>();

        public Warehouse() { }

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

        public static void CopyString(string text)
        {
            if (text != null) {
                NSPasteboard clipboard = NSPasteboard.GeneralPasteboard;
                string[] types = { "NSStringPboardType" };

                clipboard.DeclareTypes(types, null);
                clipboard.SetStringForType(text, types[0]);
            }
        }

        public static string GetServiceName(string port)
        {
            if (Services.TryGetValue(port, out string value)) return value;
            if (value == null) value = "";

            return value;
        }

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
