using Discord.Bots.Core.Models;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Discord.Bots.Core.Services;

public class ConfigService
{
    private readonly string _configPath;

    private Bot _config;

    public ConfigService(string configPath = "../config/config.json")
    {
        _configPath = configPath;
    }

    public async Task<Bot> GetConfig(bool refresh = false)
    {
        if (_config == null || refresh)
        {
            if (File.Exists(_configPath))
            {
                _config = JsonConvert.DeserializeObject<Bot>(await File.ReadAllTextAsync(_configPath));
            }
            else
            {
                Console.WriteLine("No config found. Please configure the bot.");

                _config = new Bot();
                _config.Token = GetProperty<string>("token");
                _config.OwnerID = GetProperty<ulong>("owner ID");

                await AddOrUpdateConfig(_config);
            }
        }
        return _config;
    }

    private T GetProperty<T>(string name)
    {
        T result;
        Console.Write($"{name.First().ToString().ToUpper()}{name.Substring(1)}: ");
        var property = Console.ReadLine();

        while (string.IsNullOrEmpty(property) || !ConvertProperty<T>(property, out result))
        {
            Console.WriteLine($"Please fill in a valid {name}");
            Console.Write($"{name.First().ToString().ToUpper()}{name.Substring(1)}: ");
            property = Console.ReadLine();
        }

        return result;
    }

    private bool ConvertProperty<T>(string property, out T convertedProperty)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        bool valid = converter != null && converter.IsValid(property);
        convertedProperty = valid ? (T)converter.ConvertFromString(property) : default(T);
        return valid;
    }

    public async Task<Server> GetServerConfig(ulong serverId)
    {
        var config = await GetConfig();
        return config.Servers[serverId];
    }

    public async Task AddOrUpdateConfig(Bot config)
    {
        _config = config;
        string configJSON = JsonConvert.SerializeObject(config);
        string configDirectory = Path.GetDirectoryName(_configPath);

        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }

        await File.WriteAllTextAsync(_configPath, configJSON);
    }

    public async Task AddOrUpdateServerConfig(ulong serverId, Server serverConfig)
    {
        var config = await GetConfig();

        if (config.Servers.TryGetValue(serverId, out _))
        {
            config.Servers[serverId] = serverConfig;
        }
        else config.Servers.Add(serverId, serverConfig);

        await AddOrUpdateConfig(config);
    }
}
