using System;

namespace IPList
{
    public class AddressEntry
    {
        #region Computed Properties
        public string Address { get; set; } = "";
        public string Status { get; set; } = "";
        #endregion

        #region Constructors
        public AddressEntry()
        {
        }
        #endregion

        #region Public Methods
        public AddressEntry(string address, string status)
        {
            this.Address = address;
            this.Status = status;
        }
        #endregion
    }
}
