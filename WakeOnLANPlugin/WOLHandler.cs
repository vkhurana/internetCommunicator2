using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace WakeOnLANPlugin
{
    class WOLHandler
    {
        private string lastError;

        public string LastError
        {
            get
            {
                return lastError;
            }
        }

        public WOLHandler()
        {
            lastError = "null";
        }

        public bool wakeUpMac(string macAddress)
        {
            bool result = false;
            string address = macAddress;
            try
            {
                address = address.Replace("-", "");
                address = address.Replace(":", "");
                address = address.Replace(".", "");
                address = address.Replace("_", "");
                address = address.Replace(" ", "");
                address = address.Replace(",", "");
                address = address.Replace(";", "");

                if (address.Length != 12)
                {
                    lastError = "Invalid MAC Address\n Usage: wol <MAC Address>";
                    return result;
                }

                //this is used to hold the 12 chars of the mac address
                byte[] mac = new byte[6];
                //put the string in a byte array
                for (int k = 0; k < 6; k++)
                {
                    mac[k] = Byte.Parse(address.Substring(k * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }

                // WOL packet is sent over UDP 255.255.255.0:40000.
                UdpClient client = new UdpClient();
                //client.Connect(IPAddress.Broadcast, 9); //2304?
                //client.Connect(IPAddress.Broadcast, 8); //2304?
                //client.Connect(IPAddress.Broadcast, 7); //2304?
                client.Connect(IPAddress.Broadcast, 40000); //2304?
                //client.Connect(IPAddress.Broadcast, 2304); //2304?

                // WOL packet contains a 6-bytes trailer and 16 times a 6-bytes sequence containing the MAC address.
                byte[] packet = new byte[17 * 6];

                // Trailer of 6 times 0xFF.
                for (int i = 0; i < 6; i++)
                    packet[i] = 0xFF;
                // Body of magic packet contains 16 times the MAC address.
                for (int i = 1; i <= 16; i++)
                    for (int j = 0; j < 6; j++)
                        packet[i * 6 + j] = mac[j];

                client.Send(packet, packet.Length);
                lastError = "Success";
                result = true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                result = false;
            }

            return result;
        }
    }
}
