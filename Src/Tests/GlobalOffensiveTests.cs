using System.Threading.Tasks;
using GameQueryNet;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GlobalOffensiveTests
    {
        [Test]
        public async Task CanDoSomething()
        {
            var handler = new GlobalOffivensiveQueryHandler();

            var request = new GlobalOffensiveStatsQueryRequest("78.143.30.16", 27015);
            var result = await handler.Query(request);
        }
    }
}
