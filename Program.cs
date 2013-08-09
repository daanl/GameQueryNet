using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

namespace GameQueryNet
{
    class Program
    {
        static void Main(string[] args)
        {

            var request = CreateByteRequest("TSource Engine Query");

            string hex = BitConverter.ToString(request);
            hex = hex.Replace("-", " ");

            Console.WriteLine(hex);

            using (var udpClient = new UdpClient())
            {
                udpClient.Connect("78.143.30.16", 27015);

                udpClient.Send(request, request.Length);

                var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var receiveBytes = udpClient.Receive(ref remoteIpEndPoint);

                //var result = Encoding.UTF8.GetString(receiveBytes);

                // see if bytes are in one packet or if in multiple
                byte[] responseHeader = receiveBytes.Take(4).ToArray();

                var simplePacketResponse = new byte[] {255, 255, 255, 255};
                var multiPacketResponse = new byte[] {255, 255, 255, 254};

                // handling of simple packet response
                if (responseHeader.SequenceEqual(simplePacketResponse))
                {
                    var responsePayload = receiveBytes.Skip(4).Take(receiveBytes.Length).ToArray();
                    var result = Encoding.UTF8.GetString(responsePayload);
                }
                    // multi packet response
                else if (responseHeader.SequenceEqual(multiPacketResponse))
                {
                    var responsePacketIDBytes = receiveBytes.Skip(4).Take(4).ToArray();
                    var responsePacketID = BitConverter.ToInt32(responsePacketIDBytes, 0);
                    var totalNumberOfPackets = BitConverter.receiveBytes.Skip(8).Take(1).ToArray();
                    var packetsReceived = 1;
                    var packets = new List<byte[]>();

                    // add first packet to the packet list
                    packets.Add(receiveBytes);

                    // receive all packets
                    while (packetsReceived < totalNumberOfPackets)
                    {
                        packets.Add(udpClient.Receive(ref remoteIpEndPoint));
                        packetsReceived++;
                    }

                    // order packets
                    var sortedPackets = from p in packets orderby p select p; 

                    // 
                    using (var ms = new MemoryStream(sortedPackets.ToArray()))
                    {
                        using (var reader = new BinaryReader(stream))
                        {

                        }
                    }
                    
                    // if msb is set on the response packet ID then it is message is compressed
                    if (GetBit(responsePacketIDBytes[0], 1))
                    {
                        var responsePayload = receiveBytes.Skip(4).Take(receiveBytes.Length).ToArray();

                        using (var unzip = new BZip2InputStream(ms)) //Exception occurs here
                        {
                            buffer = new byte[unzip.Length];
                            unzip.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
                // unexcepted response
                else
                {

                }
            }
        }

        public static byte[] CreateByteRequest(string request)
        {
            var requestBytes = new List<byte>();
            var prefix = new byte[] { 255, 255, 255, 255 };
            var suffix = new byte[] { 0 };

            requestBytes.AddRange(prefix);

            var toBytes = Encoding.UTF8.GetBytes(request);

            requestBytes.AddRange(toBytes);
            requestBytes.AddRange(suffix);

            return requestBytes.ToArray();
        }

        public static string ReadSteamString(BinaryReader reader)
        {
            // To hold the list of bytes making up the string
            var str = new List<byte>();
            var nextByte = reader.ReadByte();
            
            // Read up to and including the null terminator...
            while (nextByte != 0)
            {
                // ...but don't include it in the string
                str.Add(nextByte);
                nextByte = reader.ReadByte();
            }

            // Interpret the result as a UTF-8 sequence      
            return Encoding.UTF8.GetString(str.ToArray());
        }

        public static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

    }
}
