using AppKit;
using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace IPList
{
    public struct ScanIP
    {
        public bool Open;
        public string Service;
        public string Data;

        public ScanIP(bool p1, string p2, string p3)
        {
            Open = p1;
            Service = p2;
            Data = p3;
        }
    }

    public class W
    {
        private static IDictionary<string, string> tcpServices = new Dictionary<string, string>();
        private static IDictionary<string, string> udpServices = new Dictionary<string, string>();

        private static readonly string VersionURL = "https://raw.githubusercontent.com/mcherry/IPList.macOS/master/Binary/VERSION";
        private static readonly string DownloadURL = "https://github.com/mcherry/IPList.macOS/raw/master/Binary/IPList.app.tgz";
        public static readonly string ProjectURL = "https://github.com/mcherry/IPList.macOS/";

        public static readonly string appVersion = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        public static readonly string appBuild = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();

        public W() { }

        public static void UpdateCheck(bool confirm = false)
        {
            bool hasUpdate = false;

            string versionDetails = new WebClient().DownloadString(VersionURL);
            if (versionDetails == "")
            {
                hasUpdate = false;
            }
            else
            {
                string[] versionNumbers = versionDetails.Split(",");
                if (float.Parse(versionNumbers[0].Trim()) > float.Parse(appVersion)) hasUpdate = true;
                if (int.Parse(versionNumbers[1].Trim()) > int.Parse(appBuild)) hasUpdate = true;
            }

            if (hasUpdate == true)
            {
                NSAlert alert = new NSAlert()
                {
                    AlertStyle = NSAlertStyle.Informational,
                    InformativeText = "An updated version of IPList is available! Would you like to download it now?",
                    MessageText = "Software Update"
                };

                alert.AddButton("Yes");
                alert.AddButton("No");
                alert.ShowsSuppressionButton = true;

                nint result = alert.RunModal();
                if (alert.SuppressionButton.State == NSCellStateValue.On)
                {
                    Settings.UpdateCheck = 0;
                    Alert("Software Update", "Automatic update check has been disabled. You can re-enable it in Preferences.", NSAlertStyle.Informational);
                }

                if (result == 1000) Process.Start(DownloadURL);
                alert.Dispose();
            }
            
            if (confirm) Alert("Software Update", "No new updates were found.", NSAlertStyle.Informational);
        }

        public static void Alert(string title, string message, NSAlertStyle style)
        {
            NSAlert alert = new NSAlert()
            {
                AlertStyle = style,
                InformativeText = message,
                MessageText = title
            };

            alert.RunModal();
            alert.Dispose();
        }

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

        public static bool ipInRange(string ip)
        {
            string[] octets = ip.Split(".");
            if (int.Parse(octets[0]) > 0 && int.Parse(octets[0]) < 255 && int.Parse(octets[3]) > 0 && int.Parse(octets[3]) < 255) return true;
            return false;
        }

        // send a single ping
        public static PingReply Ping(string host)
        {
            Ping pinger = null;
            PingReply reply = null;

            try
            {
                // ping host
                pinger = new Ping();
                reply = pinger.Send(host, Settings.PingTimeout);
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
            while (lists > 30)
            {
                list_size += 5;
                lists = list_count / list_size;
            }

            List<T> NewList = new List<T>();

            foreach (T item in list)
            {
                if (W.IsValidIP(item.ToString()))
                {
                    if (ipInRange(item.ToString()))
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

        public static ScanIP portCheck(string ip, int port)
        {
            ScanIP host = new ScanIP();

            host.Open = false;
            for (int a = 0; a <= 1; a++)
            {
                TcpClient Scan = new TcpClient();
                IAsyncResult result = Scan.BeginConnect(ip, port, null, null);
                _ = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(Settings.PortscanTimeout), false);

                if (Scan.Connected)
                {
                    Scan.EndConnect(result);
                    result.AsyncWaitHandle.Close();
                    result.AsyncWaitHandle.Dispose();

                    host.Open = true;
                    host.Data = tcpReadPort(Scan, ip, port);
                    host.Service = GetServiceName(port);

                    break;
                }

                Scan.Close();
                Scan.Dispose();
            }

            return host;
        }

        public static string tcpReadPort(TcpClient client, string host, int port)
        {
            string returnData = string.Empty;
            string uri = "http";
            string payload = "\r\n";

            switch (port)
            {
                case 80:
                case 591:
                case 8008:
                case 8080:
                    payload = "";
                    break;
                case 443:
                case 1311:
                case 8243:
                case 8333:
                case 12443:
                case 60443:
                    uri += "s";
                    payload = "";
                    break;
            }

            if (payload == "")
            {
                WebResponse response = null;
                StreamReader reader = null;
                
                try
                {
                    WebRequest request = WebRequest.Create(uri + "://" + host);
                    response = request.GetResponse();
                } catch (WebException we)
                {
                    if (we.Status == WebExceptionStatus.ProtocolError)
                    {
                        int code = (int)((HttpWebResponse)we.Response).StatusCode;
                        returnData = code.ToString() + " " + ((HttpWebResponse)we.Response).StatusDescription;
                    } else
                    {
                        returnData = "";
                    }
                }

                if (response != null)
                {
                    try
                    {
                        reader = new StreamReader(response.GetResponseStream());
                        returnData = reader.ReadToEnd();
                    } catch
                    {
                        returnData = "";
                    } finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                            reader.Dispose();
                        }
                    }
                }

                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
            } else
            {
                NetworkStream stream = client.GetStream();
                Byte[] data = Encoding.ASCII.GetBytes(payload);

                stream.Write(data, 0, data.Length);
                data = new Byte[1024];

                try
                {
                    IAsyncResult result = stream.BeginRead(data, 0, data.Length, null, null);
                    bool wait = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(Settings.PortscanTimeout), false);
                    returnData = Encoding.ASCII.GetString(data, 0, data.Length);
                }
                catch
                {
                    returnData = "";
                }

                stream.Close();
                
            }

            return returnData;
            
        }
    }
}
