using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameQueryNet.Steam;

namespace GameQueryNet
{
    /// <summary>
    /// https://developer.valvesoftware.com/wiki/Server_Queries
    /// </summary>
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

                var steamPacket = SteamPacket.Create(receivedBytes);

                if (steamPacket is SteamSimpleResponseFormatPacket)
                {
                    response = HandleSimpleResponse(steamPacket as SteamSimpleResponseFormatPacket);
                }
                else if (steamPacket is SteamMultiResponseFormatPacket)
                {
                    response = await HandleMultiPacketResponse(udpClient, steamPacket as SteamMultiResponseFormatPacket);
                }
            }

            return response;
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

        /// <summary>
        /// Handle the simple response (synchronously)
        /// </summary>
        /// <param name="steamPacket">The first and only packet that will come in</param>
        /// <returns>A response in the form of a QueryResponse</returns>
        private GlobalOffensiveStatsQueryResponse HandleSimpleResponse(SteamSimpleResponseFormatPacket steamPacket)
        {
            var response = new GlobalOffensiveStatsQueryResponse();

            response.Raw = steamPacket.RawPacket;
            var reader = steamPacket.Reader;

            response.Header = reader.ExtractLong<int>();
            response.Protocol = reader.ExtractByte<string>();
            response.Name = reader.ExtractString<string>();
            response.Map = reader.ExtractString<string>();
            response.Folder = reader.ExtractString<string>();
            response.Game = reader.ExtractString<string>();
            response.Id = reader.ExtractShort<int>();
            response.Players = reader.ExtractByte<int>();

            return response;
        }

        /// <summary>
        /// Handle the multi packet response
        /// </summary>
        /// <param name="updClient">the udpClient that will receive the results</param>
        /// <param name="initialPacket">the first packet that came</param>
        /// <returns>Task with Response as type parameter</returns>
        private async Task<GlobalOffensiveStatsQueryResponse> HandleMultiPacketResponse(UdpClient updClient, SteamMultiResponseFormatPacket initialPacket)
        {
            var response = new GlobalOffensiveStatsQueryResponse();
            
            int numberOfReceivedPackets = 1;
            var receivedPackets = new List<byte[]>();
            receivedPackets.Add(initialPacket.RawPacket); // add initial packet to 

            /* read all the packets or if some packets were dropped then timeout should occur
            use CancellationTokenSource for this */
            while (numberOfReceivedPackets <= initialPacket.Total)
            {
                var udpReceiveResult = await updClient.ReceiveAsync();
                receivedPackets.Add(udpReceiveResult.Buffer);
            }

            /* TODO: order the packets */

            /* TODO: decompress if necessary */

            /* TODO: read out the data */

            return response;
        }
    }
}