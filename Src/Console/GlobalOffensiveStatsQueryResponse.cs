namespace GameQueryNet
{
    public class GlobalOffensiveStatsQueryResponse
    {
        public string Protocol { get; internal set; }
        public string Name { get; internal set; }
        public string Map { get; internal set; }
        public string Folder { get; internal set; }
        public string Game { get; internal set; }
        public int Id { get; internal set; }
        public string Raw { get; internal set; }
        public int Players { get; set; }
    }
}