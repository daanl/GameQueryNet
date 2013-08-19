using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GameQueryNet.Tests
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

            Console.WriteLine(result.Name);

            Assert.AreEqual("I", result.Protocol);
            Assert.AreEqual("Knastrevolution -> 85.131.174.61:27015", result.Name);
            Assert.AreEqual("csgo", result.Folder);
            Assert.AreEqual("Counter-Strike: Global Offensive", result.Game);
            Assert.AreEqual(-9726, result.Id);
        }
    }
}
