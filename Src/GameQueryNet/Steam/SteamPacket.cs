using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameQueryNet.Steam
{
    public abstract class SteamPacket
    {
        protected SteamPacket(byte[] rawPacket)
        {
            RawPacket = rawPacket;
            Header = DetermineResponse(rawPacket);
        }

        /// <summary>
        /// Mostly used for multiple response
        /// </summary>
        protected SteamPacket(SteamPacket steamPacket)
        {
            RawPacket = steamPacket.RawPacket;
            Header = steamPacket.Header;
            Payload = steamPacket.Payload;
        }

        public byte[] RawPacket { get; private set; }
        public SteamPacketType Header { get; private set; }
        public IList<byte> Payload { get; protected set; }

        /// <summary>
        /// Steam doesn't have a length for string we have to determin that our self 
        /// </summary>
        /// <returns>Index of the string delimiter</returns>
        public int GetIndexOfString(IList<byte> bytes)
        {
            return bytes.ToList().IndexOf(0x00);
        }

        public T ExtractString<T>(ref IList<byte> bytes, bool removedUsed = true)
        {
            var index = GetIndexOfString(bytes);
            var result = bytes.Take(index).ToArray();

            if (removedUsed)
            {
                bytes = bytes.Skip(index + 1).ToArray();
            }

            return ConvertToType<T>(result);
        }

        public T ExtractShort<T>(ref IList<byte> bytes)
        {
            var result = bytes.Take(2).ToArray();
            bytes = bytes.Skip(2).ToArray();

            return ConvertToType<T>(result);
        }

        /// <summary>
        /// TODO: describe what MSB is or set a clearer name
        /// </summary>
        public bool GetMSB(IList<byte> bytes)
        {
            return GetBit(bytes[1], 1);
        }

        public T ExtractLong<T>(ref IList<byte> bytes)
        {
            var result = bytes.Take(4).ToArray();
            bytes = bytes.Skip(4).ToArray();

            return ConvertToType<T>(result);
        }

        public T ExtractByte<T>(ref IList<byte> bytes)
        {
            var result = bytes.First();
            bytes = bytes.Skip(1).ToArray();

            return ConvertToType<T>(new[] { result });
        }

        public T ConvertToType<T>(byte[] value)
        {
            var type = typeof(T);

            if (type == typeof(string))
            {
                object result = Encoding.UTF8.GetString(value);
                return (T)result;
            }

            if (type == typeof(int))
            {
                object result = null;

                if (value.Length == 1)
                {
                    result = (int)value[0];
                }
                else if (value.Length == 2)
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

        public int ConvertToInt(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt16(bytes, 0);
        }

        public IList<byte> ReceivedBytesPrefix(byte[] recievedBytes)
        {
            return recievedBytes.Take(4).ToList();
        }

        public IList<byte> ReceivedBytesWithOutPrefixAndSuffix(byte[] recievedBytes)
        {
            return recievedBytes.Skip(4).Take(recievedBytes.Length - 5).ToList();
        }

        public SteamPacketType DetermineResponse(IList<byte> recievedBytes)
        {
            var simpleResponsePrefix = new byte[] { 255, 255, 255, 255 };
            var multiResponsePrefix = new byte[] { 255, 255, 255, 254 };

            if (recievedBytes.Take(4).ToArray().SequenceEqual(simpleResponsePrefix))
            {
                return SteamPacketType.Simple;
            }

            if (recievedBytes.Take(4).ToArray().SequenceEqual(multiResponsePrefix))
            {
                return SteamPacketType.Multi;
            }

            return SteamPacketType.Unknown;
        }

        /// <summary>
        /// TODO: Explain what this does
        /// http://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte 
        /// </summary>
        public static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }
    }
}