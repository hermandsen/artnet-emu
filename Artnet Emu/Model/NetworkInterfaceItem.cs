using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model
{
    public class NetworkInterfaceItem
    {
        public readonly string Name;
        public readonly IPAddress Address;

        public NetworkInterfaceItem(NetworkInterface ni)
        {
            Name = ni.Name;
            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Address = ip.Address;
                }
            }
        }
        public NetworkInterfaceItem(string name, IPAddress address)
        {
            Name = name;
            Address = address;
        }
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Address.ToString());
        }
    }
}
