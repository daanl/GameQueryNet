namespace GameQueryNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var handler = new GlobalOffivensiveQueryHandler();

            var request = new GlobalOffensiveStatsQueryRequest("78.143.30.16", 27015);
            var result = handler.Query(request);
        }
    }
}
