using System;

namespace GameQueryNet
{
    public class GlobalOffensiveStatsQueryRequest
    {
        public GlobalOffensiveStatsQueryRequest(string ipAddress, int port)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new Exception("IpAddress is required");
            }
        
            IpAddress = ipAddress;
            Port = port;
        }

        public string IpAddress { get; private set; }
        public int Port { get; private set; }
    }
}