namespace KGB.Config
{
    public class BotConfig
    {
        public BotConfig()
        {
            Servers = new();
        }

        public string Token { get; set; }
        public ulong OwnerID { get; set; }
        public string Prefix { get; set; }
        public Dictionary<ulong, ServerConfig> Servers { get; set; }
    }
}
