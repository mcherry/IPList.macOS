using System;
namespace IPList
{
    public class PortEntry
    {
        public string Port { get; set; } = "";
        public string Service { get; set; } = "";
        public string Data { get; set; } = "";

        public PortEntry()
        {
        }

        public PortEntry(string port, string service = "", string data = "")
        {
            this.Port = port;
            this.Service = service;
            this.Data = data;
        }
    }
}
