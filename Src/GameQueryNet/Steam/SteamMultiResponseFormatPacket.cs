﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GameQueryNet.Steam
{
    /// <summary>
    /// For now this only covers 'Source Server' not 'Goldsource Server' 
    /// </summary>
    public class SteamMultiResponseFormatPacket : SteamPacket
    {
        private SteamPacket _firstPacket;
        
        public SteamMultiResponseFormatPacket(byte[] rawPacket) : base(rawPacket)
        {
            if (Header != SteamPacketType.Multi)
            {
                throw new Exception("Not multi package response");
            }

            var _raw = rawPacket.ToList<byte>() as IList<byte>;
            var IsCompressed = GetMSB(_raw);
            ID = ExtractLong<int>(ref _raw);
            Total = ExtractByte<int>(ref _raw);
            Number = ExtractByte<int>(ref _raw);
            Size = ExtractShort<short>(ref _raw);
        }

        public SteamMultiResponseFormatPacket(SteamPacket firstPacket) : base(firstPacket)
        {

        }

        /// <summary>
        /// If the most significant bit of ID is 1, then the response was compressed with bzip2 before being cut and sent
        /// </summary>
        public bool IsCompressed { get; private set; }

        /// <summary>
        /// Unique number assigned by server per answer.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The total number of packets in the response.
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// The number of the packet. Starts at 0.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Maximum size of packet before packet switching occurs. The default value is 1248 bytes (0x04E0), but the server administrator can decrease this. For older engine versions: the maximum and minimum size of the packet was unchangeable.
        /// </summary>
        public short Size { get; private set; }
    }
}