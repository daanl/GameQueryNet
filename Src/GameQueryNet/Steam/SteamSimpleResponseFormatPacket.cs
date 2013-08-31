namespace GameQueryNet.Steam
{
    public class SteamSimpleResponseFormatPacket : SteamPacket
    {
        public SteamSimpleResponseFormatPacket(byte[] rawPacket) : base(rawPacket)
        {
            Payload = ReceivedBytesWithOutPrefixAndSuffix(rawPacket);
        }
    }
}