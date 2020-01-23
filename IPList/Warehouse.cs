using AppKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

using IPAddressCollection = LukeSkywalker.IPNetwork.IPAddressCollection;

namespace IPList
{
    public class W
    {
        public static string CurrentIP;
        private static IDictionary<string, string> tcpServices = new Dictionary<string, string>();
        private static IDictionary<string, string> udpServices = new Dictionary<string, string>();

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

        public static void Error(string message)
        {
            NSAlert alert = new NSAlert()
            {
                AlertStyle = NSAlertStyle.Critical,
                InformativeText = message,
                MessageText = "Error"
            };

            alert.RunModal();
            alert.Dispose();
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
        public static string GetServiceName(int port, string protocol = "tcp")
        {
            string value = "";
            switch (protocol)
            {
                case "tcp":
                    if (tcpServices.TryGetValue(port.ToString(), out value)) return value;
                    break;
                case "udp":
                    if (udpServices.TryGetValue(port.ToString(), out value)) return value;
                    break;
            }
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
                        if (port.Length > 1 && !port[0].StartsWith("#")) {
                            switch (port[1])
                            {
                                case "tcp":
                                    tcpServices[port[0]] = entry[0];
                                    break;
                                case "udp":
                                    udpServices[port[0]] = entry[0];
                                    break;
                            }
                        }
                    }
                }
            }
            file.Close();

            return;
        }

        // split collection of ints (ports) into sublists
        public static List<List<T>> Split<T>(List<T> list, int list_size = 10)
        {
            // calculate chunk size based on number of IPs and a max of 30 threads
            double list_count = list.Count;
            double lists = list_count / list_size;
            while (lists > 20)
            {
                list_size *= 2;
                lists = list_count / list_size;
            }

            List<T> NewList = new List<T>();

            foreach (T item in list)
            {
                if (W.IsValidIP(item.ToString()))
                {
                    string[] octets = item.ToString().Split(".");
                    if (octets[3] != "0" && octets[3] != "255")
                    {
                        NewList.Add((T)Convert.ChangeType(item.ToString(), typeof(T)));
                    }
                }
                else
                {
                    NewList.Add((T)Convert.ChangeType(item.ToString(), typeof(T)));
                }
            }

            List<List<T>> chunks = new List<List<T>>();
            List<T> temp = new List<T>();
            int count = 0;

            foreach (T element in NewList)
            {
                if (count++ == list_size)
                {
                    chunks.Add(temp);
                    temp = new List<T>();
                    count = 1;
                }
                temp.Add(element);
            }

            chunks.Add(temp);
            return chunks;
        }

        public static string dnsLookup(string ip)
        {
            string hostname = "";

            try
            {
                hostname = Dns.GetHostEntry(ip).HostName;
            }
            catch
            {
                hostname = "";
            }
            if (hostname == ip) hostname = "";

            return hostname;
        }
    }
}
