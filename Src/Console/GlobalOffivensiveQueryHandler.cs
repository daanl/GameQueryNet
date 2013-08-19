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
                
                var receiveResult = await udpClient.ReceiveAsync();
                var receivedBytes = receiveResult.Buffer;

                var headerBytes = GetHeader(receivedBytes);
                var responseBodyBytes = GetBody(receivedBytes);

                /* Deal with simple response */
                if (IsSimpleResponse(headerBytes))
                {
                    response = HandleSimpleResponse(responseBodyBytes);
                }
                /* Deal with multi packet response */
                else if (IsMultiResponse(headerBytes))
                {
                    response = HandleMultiResponse(udpClient, responseBodyBytes);
                }
            }

            return response;
        }

        private GlobalOffensiveStatsQueryResponse HandleSimpleResponse(byte[] receivedBytes)
        {
            var response = new GlobalOffensiveStatsQueryResponse();

            response.Protocol = ConvertToString(receivedBytes.Take(1).ToArray());
            
            var result2 = Encoding.ASCII.GetString(receivedBytes);
            Console.WriteLine(string.Join(Environment.NewLine, result2.Split(new[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)));

            return response;
        }

        private GlobalOffensiveStatsQueryResponse HandleMultiResponse(UdpClient updClient, byte[] initialPacketBody)
        {
            var response = new GlobalOffensiveStatsQueryResponse();



            return response;
        }

        private string ConvertToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] GetHeader(byte[] receivedBytes)
        {
            return receivedBytes.Take(4).ToArray();
        }

        public byte[] GetBody(byte[] receivedBytes)
        {
            return receivedBytes.Skip(4).Take(receivedBytes.Length - 5).ToArray();
        }

        private bool IsSimpleResponse(byte[] receivedBytes)
        {
            var simpleResponsePrefix = new byte[] { 255, 255, 255, 255 };

            return receivedBytes.Take(4).ToArray().SequenceEqual(simpleResponsePrefix);
        }

        private bool IsMultiResponse(byte[] receivedBytes)
        {
            var multiResponsePrefix = new byte[] { 255, 255, 255, 254 };

            return receivedBytes.Take(4).ToArray().SequenceEqual(multiResponsePrefix);
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