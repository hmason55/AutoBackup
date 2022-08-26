using AutoBackup.Utilities;
using Newtonsoft.Json;

namespace AutoBackup.Models;

public class Config
{
    public static readonly JsonSerializerSettings Serializer = new()
    {
        NullValueHandling = NullValueHandling.Include,
        Formatting = Formatting.Indented,

    };

    public static string CONFIG_PATH = @"Resources\config.json";

    public int MaximumHistory { get; set; } = 5;
    public float UpdateIntervalInMinutes { get; set; } = 0.1f;
    public const string DATE_FORMAT = "yyyyMMdd-HHmmss";
    public DateTime LastUpdate { get; set; }
    public List<string> TrackedFiles { get; set; }

    public static Config Load()
    {
        Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(CONFIG_PATH));

        Console.WriteLine("Configuration settings:");
        Console.WriteLine($"{nameof(config.MaximumHistory)}: {config?.MaximumHistory}");
        Console.WriteLine($"{nameof(config.UpdateIntervalInMinutes)}: {config?.UpdateIntervalInMinutes}");
        Console.WriteLine($"{nameof(config.LastUpdate)}: {config?.LastUpdate}");

        if (config.TrackedFiles != null && config.TrackedFiles.Any())
        {
            Console.WriteLine("The following files will be backed up:");
            for (int i = 0; i < config.TrackedFiles.Count; i++)
            {
                config.TrackedFiles[i] = config.TrackedFiles[i].ReplaceSeparators();
                Console.WriteLine(config.TrackedFiles[i]);
            }
        }

        return config;
    }

    public static void Save(Config config)
    {
        string json = JsonConvert.SerializeObject(config, Serializer);

        if (File.Exists(CONFIG_PATH))
        {
            File.Delete(CONFIG_PATH);
        }

        if (!File.Exists(CONFIG_PATH))
        {
            File.WriteAllText(CONFIG_PATH, json);
        }
    }
}
