using System;

namespace IPList
{
    public class AddressEntry
    {
        public string Address { get; set; } = "";
        public string Status { get; set; } = "";

        public AddressEntry()
        {
        }

        public AddressEntry(string address, string status)
        {
            this.Address = address;
            this.Status = status;
        }
    }
}
