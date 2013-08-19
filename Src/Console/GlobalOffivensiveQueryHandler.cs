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

        private GlobalOffensiveStatsQueryResponse HandleSimpleResponse(IList<byte> recievedBytes)
        {
            var response = new GlobalOffensiveStatsQueryResponse();

            response.Raw = ExtractString<string>(ref recievedBytes, false);
            response.Protocol = ExtractByte<string>(ref recievedBytes);
            response.Name = ExtractString<string>(ref recievedBytes);
            response.Map = ExtractString<string>(ref recievedBytes);
            response.Folder = ExtractString<string>(ref recievedBytes);
            response.Game = ExtractString<string>(ref recievedBytes);
            response.Id = ExtractShort<int>(ref recievedBytes);
            response.Players = ExtractByte<byte>(ref recievedBytes);

            return response;
        }

        private int GetIndexOfString(IList<byte> bytes)
        {
            return bytes.ToList().IndexOf(0x00);
        }

        private T ExtractString<T>(ref IList<byte> bytes, bool removedUsed = true)
        {
            var index = GetIndexOfString(bytes);
            var result = bytes.Take(index).ToArray();

            if (removedUsed)
            {
                bytes = bytes.Skip(index + 1).ToArray();
            }

            return ConvertToType<T>(result);
        }

        private T ExtractShort<T>(ref IList<byte> bytes)
        {
            var result = bytes.Take(2).ToArray();
            bytes = bytes.Skip(2).ToArray();

            return ConvertToType<T>(result);
        }

        private T ExtractByte<T>(ref IList<byte> bytes)
        {
            var result = bytes.First();
            bytes = bytes.Skip(1).ToArray();

            return ConvertToType<T>(new[] {result});
        }

        private T ConvertToType<T>(byte[] value)
        {
            var type = typeof (T);

            if (type == typeof(string))
            {
                object result = Encoding.UTF8.GetString(value);
                return (T) result;
            }

            if (type == typeof (int))
            {
                object result = null;

                if (value.Length == 2)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(value);
                    }

                    result = Convert.ToInt32(BitConverter.ToInt16(value, 0));
                }
                else
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(value);
                    }
                 
                    result = Convert.ToInt32(BitConverter.ToInt32(value, 0));
                }
               
                return (T)result;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private int ConvertToInt(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt16(bytes, 0);
        }

        public List<byte> RecievedBytesPrefix(byte[] recievedBytes)
        {
            return recievedBytes.Take(4).ToList();
        }

        public List<byte> RecievedBytesWithOutPrefixAndSuffix(byte[] recievedBytes)
        {
            return recievedBytes.Skip(4).Take(recievedBytes.Length - 5).ToList();
        }

        private bool IsSimpleResponse(IEnumerable<byte> recievedBytes)
        {
            var simpleResponsePrefix = new byte[] { 255, 255, 255, 255 };

            return recievedBytes.Take(4).ToArray().SequenceEqual(simpleResponsePrefix);
        }

        private bool IsMutliResponse(IEnumerable<byte> recievedBytes)
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