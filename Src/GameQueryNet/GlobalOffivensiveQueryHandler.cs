using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameQueryNet.Steam;

namespace GameQueryNet
{
    /* https://developer.valvesoftware.com/wiki/Server_Queries */
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

                var firstPacket = new SteamPacket(receivedBytes);

                // Deal with simple response
                if (firstPacket.Header == SteamPacketType.Simple)
                {
                    var simple = new SteamSimpleResponseFormatPacket(firstPacket);
                    response = HandleSimpleResponse(simple);
                }

                // Deal with multi packet response
                else if (firstPacket.Header == SteamPacketType.Multi)
                {
                    var multi = new SteamMultiResponseFormatPacket(firstPacket);
                    var multiPacketResponse = HandleMultiPacketResponse(udpClient, multi);

                    if (multiPacketResponse.IsCompleted)
                    {
                        response = multiPacketResponse.Result;
                    }
                    else
                    {
                        throw new Exception("Unknown packet type");
                    }
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

        private GlobalOffensiveStatsQueryResponse HandleSimpleResponse(SteamSimpleResponseFormatPacket steamPacket)
        {
            var response = new GlobalOffensiveStatsQueryResponse();

            //response.Raw = ExtractString<string>(ref receivedBytes, false);
            response.Raw = steamPacket.RawPacket;
            var payload = steamPacket.Payload;

            response.Protocol = steamPacket.ExtractByte<string>(ref payload);
            response.Name = steamPacket.ExtractString<string>(ref payload);
            response.Map = steamPacket.ExtractString<string>(ref payload);
            response.Folder = steamPacket.ExtractString<string>(ref payload);
            response.Game = steamPacket.ExtractString<string>(ref payload);
            response.Id = steamPacket.ExtractShort<int>(ref payload);
            response.Players = steamPacket.ExtractByte<int>(ref payload);

            return response;
        }

        private async Task<GlobalOffensiveStatsQueryResponse> HandleMultiPacketResponse(UdpClient updClient, SteamMultiResponseFormatPacket initialPacket)
        {
            var response = new GlobalOffensiveStatsQueryResponse();
            
            int numberOfReceivedPackets = 1;
            var receivedPackets = new List<UdpReceiveResult>();

            while (numberOfReceivedPackets <= initialPacket.Total)
            {
                var udpReceiveResult = await updClient.ReceiveAsync();
                receivedPackets.Add(udpReceiveResult);
            }

            /* TODO: order the packets */

            /* TODO: decompress if necessary */

            /* TODO: read out the data */

            return response;
        }
    }
}