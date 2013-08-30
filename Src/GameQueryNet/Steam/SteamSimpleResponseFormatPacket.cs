namespace GameQueryNet.Steam
{
    public class SteamSimpleResponseFormatPacket : SteamPacket
    {
        public SteamSimpleResponseFormatPacket(byte[] rawPacket) : base(rawPacket)
        {
            if (Header == SteamPacketType.Simple)
            {
                Payload = ReceivedBytesWithOutPrefixAndSuffix(rawPacket);
            }
        }

        public SteamSimpleResponseFormatPacket(SteamPacket steamPacket)
            : base(steamPacket)
        {
            if (Header == SteamPacketType.Simple)
            {
                Payload = ReceivedBytesWithOutPrefixAndSuffix(steamPacket.RawPacket);
            }
        }
    }
}