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
            Assert.AreEqual("Jail (DE) Knastrevolution | #1 | by FatalGamerZ.de", result.Name);
            Assert.AreEqual("csgo", result.Folder);
            Assert.AreEqual("Counter-Strike: Global Offensive", result.Game);
            Assert.AreEqual(730, result.Id);
        }
    }
}
