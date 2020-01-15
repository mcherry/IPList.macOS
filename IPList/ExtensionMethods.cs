using System;
using System.Net;

namespace IPList
{
    public static class ExtensionMethods
    {
        public static int CompareTo(this string ip1, string ip2)
        {
            IPAddress x = IPAddress.Parse(ip1);
            IPAddress y = IPAddress.Parse(ip2);

            var result = x.AddressFamily.CompareTo(y.AddressFamily);
            if (result != 0) return result;

            var xBytes = x.GetAddressBytes();
            var yBytes = y.GetAddressBytes();

            var octets = Math.Min(xBytes.Length, yBytes.Length);
            for (var i = 0; i < octets; i++)
            {
                var octetResult = xBytes[i].CompareTo(yBytes[i]);
                if (octetResult != 0) return octetResult;
            }

            return 0;
        }
    }
}