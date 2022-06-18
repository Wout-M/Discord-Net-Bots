namespace ExWi.Config
{
    public class ServerConfig
    {
        public ServerConfig()
        {
            SortRoles = new();
        }

        public string Name { get; set; }
        public ulong? ModChannel { get; set; }
        public ulong? LogChannel { get; set; }
        public List<ulong> SortRoles { get; set; }
    }
}
