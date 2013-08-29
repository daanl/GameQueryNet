using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameQueryNet
{
    public enum PacketType
    {
        Simple,
        Multi,
        Unknown
    }

    public class SteamPacket
    {
        #region constructor(s)

        public SteamPacket(byte[] rawPacket)
        {
            RawPacket = rawPacket;
            Header = DetermineResponse(rawPacket);

            /* set other properties in derived classes */
        }

        /* only available when already instantiated somewhere else */
        protected SteamPacket(SteamPacket steamPacket)
        {
            this.RawPacket = steamPacket.RawPacket;
            this.Header = steamPacket.Header;
            this.Payload = steamPacket.Payload;
        }

        #endregion constructor(s)

        #region properties

        public byte[] RawPacket { get; private set; }
        public PacketType Header { get; private set; }
        public IList<byte> Payload { get; protected set; }

        #endregion properties

        #region methods

        private int GetIndexOfString(IList<byte> bytes)
        {
            return bytes.ToList().IndexOf(0x00);
        }

        internal T ExtractString<T>(ref IList<byte> bytes, bool removedUsed = true)
        {
            var index = GetIndexOfString(bytes);
            var result = bytes.Take(index).ToArray();

            if (removedUsed)
            {
                bytes = bytes.Skip(index + 1).ToArray();
            }

            return ConvertToType<T>(result);
        }

        internal T ExtractShort<T>(ref IList<byte> bytes)
        {
            var result = bytes.Take(2).ToArray();
            bytes = bytes.Skip(2).ToArray();

            return ConvertToType<T>(result);
        }

        internal bool GetMSB(IList<byte> bytes)
        {
            return GetBit(bytes[1], 1);
        }

        internal T ExtractLong<T>(ref IList<byte> bytes)
        {
            var result = bytes.Take(4).ToArray();
            bytes = bytes.Skip(4).ToArray();

            return ConvertToType<T>(result);
        }

        internal T ExtractByte<T>(ref IList<byte> bytes)
        {
            var result = bytes.First();
            bytes = bytes.Skip(1).ToArray();

            return ConvertToType<T>(new[] { result });
        }

        private T ConvertToType<T>(byte[] value)
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

        private int ConvertToInt(byte[] bytes)
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

        private PacketType DetermineResponse(IEnumerable<byte> recievedBytes)
        {
            var simpleResponsePrefix = new byte[] { 255, 255, 255, 255 };
            var multiResponsePrefix = new byte[] { 255, 255, 255, 254 };

            if (recievedBytes.Take(4).ToArray().SequenceEqual(simpleResponsePrefix))
                return PacketType.Simple;
            else if (recievedBytes.Take(4).ToArray().SequenceEqual(multiResponsePrefix))
                return PacketType.Multi;
            else return PacketType.Unknown;
        }

        /* http://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte */
        private static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        #endregion methods
    }

    internal class SimpleResponseFormatPacket : SteamPacket
    {
        internal SimpleResponseFormatPacket(byte[] rawPacket) : base(rawPacket)
        {
            if (Header == PacketType.Simple)
                Payload = ReceivedBytesWithOutPrefixAndSuffix(rawPacket);
        }

        internal SimpleResponseFormatPacket(SteamPacket steamPacket) : base(steamPacket)
        {
            if (Header == PacketType.Simple)
                Payload = ReceivedBytesWithOutPrefixAndSuffix(steamPacket.RawPacket);
        }

    }

    /* for now this only covers 'Source Server' not 'Goldsource Server' */
    internal class MultiResponseFormatPacket : SteamPacket
    {
        private SteamPacket firstPacket;

        internal MultiResponseFormatPacket(byte[] rawPacket) : base(rawPacket)
        {
            /* if not multi package then exit */
            if (Header != PacketType.Multi)
                return;

            var _raw = rawPacket.ToList<byte>() as IList<byte>;
            var IsCompressed = GetMSB(_raw);
            ID = ExtractLong<int>(ref _raw);
            Total = ExtractByte<int>(ref _raw);
            Number = ExtractByte<int>(ref _raw);
            Size = ExtractShort<short>(ref _raw);
        }

        public MultiResponseFormatPacket(SteamPacket firstPacket) : base(firstPacket)
        {

        }

        /* If the most significant bit of ID is 1, then the response was compressed with bzip2 before being cut and sent */
        public bool IsCompressed { get; private set; }

        /* Unique number assigned by server per answer. */
        public int ID { get; private set; }

        /* The total number of packets in the response. */
        public int Total { get; private set; }

        /* The number of the packet. Starts at 0. */
        public int Number { get; private set; }

        /* Maximum size of packet before packet switching occurs. The default value is 1248 bytes (0x04E0), but the server administrator can decrease this. For older engine versions: the maximum and minimum size of the packet was unchangeable. */
        public short Size { get; private set; }
    }
}