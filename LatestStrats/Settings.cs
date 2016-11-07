using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace StrategyIncubator
{
    class Settings
    {
        public static Config GetConfig()
        {
            if (!File.Exists(Endpoint.SETTINGS_FILE))
            {
                /*No settings file exists, so we'll print
                a new one with empty values*/
                var newConfig = new Config();
                for (int i = 0; i < 3; i++)
                    newConfig.tasks.Add(new Task());

                File.WriteAllText(Endpoint.SETTINGS_FILE, JsonConvert.SerializeObject(newConfig, Formatting.Indented));
                Console.WriteLine("Settings file created");

            }
            else
            {
                bool error = false;
                Config cfg = null;

                try
                {
                    string json = File.ReadAllText(Endpoint.SETTINGS_FILE);
                    cfg = JsonConvert.DeserializeObject<Config>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Settings threw an exception: {ex.Message}");
                }

                if (cfg.HasMissingProperties())
                {
                    Console.WriteLine($"Settings has missing properties.");
                    error = true;
                }

                foreach (var task in cfg.tasks.Select((value, i) => new { i, value }))
                {
                    if (task.value.HasMissingProperties())
                    {
                        Console.WriteLine($"Task [{task.i}] has missing properties.");
                        error = true;
                    }
                }

                if (!error)
                    return cfg;
            }
            
            return null;
        }
    }
}
