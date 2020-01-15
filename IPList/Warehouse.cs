﻿using AppKit;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace IPList
{
    public class Warehouse
    {
        public static Dictionary<string, string> Services = new Dictionary<string, string>();

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
            NSPasteboard clipboard = NSPasteboard.GeneralPasteboard;
            string[] types = { "NSStringPboardType" };

            clipboard.DeclareTypes(types, null);
            clipboard.SetStringForType(text, types[0]);
        }

        public static string GetPortDescription(int port)
        {
            string value;
            if (Services.TryGetValue(port.ToString(), out value)) return value;
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
                        if (port.Length > 1) Services[port[0]] = entry[0];
                    }
                }
            }
            file.Close();

            return;
        }
    }
}
