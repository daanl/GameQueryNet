using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameQueryNet.Steam
{
    /// <summary>
    /// SteamPacket class, mainly created for multiple packet response
    /// </summary>
    public class SteamPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawPacket"></param>
        public SteamPacket(byte[] rawPacket)
        {
            RawPacket = rawPacket;
            Header = DetermineResponse(rawPacket);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="steamPacket"></param>
        protected SteamPacket(SteamPacket steamPacket)
        {
            RawPacket = steamPacket.RawPacket;
            Header = steamPacket.Header;
            Payload = steamPacket.Payload;
        }

        public byte[] RawPacket { get; private set; }
        public SteamPacketType Header { get; private set; }
        public IList<byte> Payload { get; protected set; }

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
    }
}