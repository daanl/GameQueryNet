using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameQueryNet
{
    public class GlobalOffivensiveQueryHandler
    {
        public async Task<GlobalOffensiveStatsQueryResponse> Query(GlobalOffensiveStatsQueryRequest request)
        {
            var byteRequest = CreateByteRequest("TSource Engine Query");
            var response = new GlobalOffensiveStatsQueryResponse();

            using (var udpClient = new UdpClient())
            {
                udpClient.Connect(request.IpAddress, request.Port);
                udpClient.Send(byteRequest, byteRequest.Length);
                
                var recieveResult = await udpClient.ReceiveAsync();
                var recievedBytes = recieveResult.Buffer;

                var prefixBytes = RecievedBytesPrefix(recievedBytes);
                var responseBodyBytes = RecievedBytesWithOutPrefixAndSuffix(recievedBytes);

                if (IsSimpleResponse(prefixBytes))
                {
                    response = HandleSimpleResponse(responseBodyBytes);
                }
            }

            return response;
        }

        private GlobalOffensiveStatsQueryResponse HandleSimpleResponse(byte[] recievedBytes)
        {
            var result2 = Encoding.ASCII.GetString(recievedBytes);
            Console.WriteLine(string.Join(Environment.NewLine, result2.Split(new[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)));

            return new GlobalOffensiveStatsQueryResponse();
        }

        public byte[] RecievedBytesPrefix(byte[] recievedBytes)
        {
            return recievedBytes.Take(4).ToArray();
        }

        public byte[] RecievedBytesWithOutPrefixAndSuffix(byte[] recievedBytes)
        {
            return recievedBytes.Skip(4).Take(recievedBytes.Length - 5).ToArray();
        }

        private bool IsSimpleResponse(byte[] recievedBytes)
        {
            var simpleResponsePrefix = new byte[] { 255, 255, 255, 255 };

            return recievedBytes.Take(4).ToArray().SequenceEqual(simpleResponsePrefix);
        }

        private bool IsMutliResponse(byte[] recievedBytes)
        {
            var multiResponsePrefix = new byte[] { 255, 255, 255, 254 };

            return recievedBytes.Take(4).ToArray().SequenceEqual(multiResponsePrefix);
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

        public static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }
    }
}