using System;
using System.Collections.Generic;
using System.Linq;

namespace GameQueryNet.Steam
{
    /// <summary>
    /// SteamPacket class, mainly created for multiple packet response
    /// </summary>
    public abstract class SteamPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawPacket"></param>
        protected SteamPacket(byte[] rawPacket)
        {
            RawPacket = rawPacket;
            Reader = new SteamByteReader(rawPacket);
        }

        public SteamByteReader Reader { get; private set; }
        public byte[] RawPacket { get; private set; }
        public IList<byte> Payload { get; protected set; }

        public IList<byte> ReceivedBytesPrefix(byte[] recievedBytes)
        {
            return recievedBytes.Take(4).ToList();
        }

        public IList<byte> ReceivedBytesWithOutPrefixAndSuffix(byte[] recievedBytes)
        {
            return recievedBytes.Skip(4).Take(recievedBytes.Length - 5).ToList();
        }

        private static SteamPacketType DetermineResponse(IList<byte> recievedBytes)
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

             throw new Exception("Invalid response");
        }

        public static SteamPacket Create(byte[] receivedBytes)
        {
            var type = DetermineResponse(receivedBytes);

            switch (type)
            {
                case SteamPacketType.Simple:
                    return new SteamSimpleResponseFormatPacket(receivedBytes);
                case SteamPacketType.Multi:
                    return new SteamMultiResponseFormatPacket(receivedBytes);
                default:
                    throw new Exception("Invalid response");
            }
        }
    }
}