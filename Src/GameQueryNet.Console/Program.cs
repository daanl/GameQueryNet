﻿using System.Threading.Tasks;

namespace GameQueryNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            while (!CanDoSomething().IsCompleted)
            { ; }
            System.Console.Read();
        }

        private static async Task CanDoSomething()
        {
            var handler = new GlobalOffivensiveQueryHandler();

            var request = new GlobalOffensiveStatsQueryRequest("78.143.30.16", 27015);
            var result = await handler.Query(request);

            System.Console.WriteLine(result.Protocol);
        }
    }
}
