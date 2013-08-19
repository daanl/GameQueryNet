using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameQueryNet;

namespace GameQueryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CanDoSomething();
            Console.Read();
        }

        private static async Task CanDoSomething()
        {
            var handler = new GlobalOffivensiveQueryHandler();

            var request = new GlobalOffensiveStatsQueryRequest("78.143.30.16", 27015);
            var result = await handler.Query(request);

            Console.WriteLine(result.Protocol);
        }
    }
}
