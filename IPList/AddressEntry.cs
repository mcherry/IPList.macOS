namespace IPList
{
    public class AddressEntry
    {
        public string Address { get; set; } = "";
        public string Status { get; set; } = "";
        public string Latency { get; set; } = "";
        public string TTL { get; set; } = "";

        public AddressEntry()
        {
        }

        public AddressEntry(string address, string status, string latency, string ttl)
        {
            this.Address = address;
            this.Status = status;
            this.Latency = latency;
            this.TTL = ttl;
        }   
    }
}
