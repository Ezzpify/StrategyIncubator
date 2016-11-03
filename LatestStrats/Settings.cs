using System;
using Newtonsoft.Json;
using System.IO;

namespace LatestStrats
{
    class Settings
    {
        public static Config GetSettings()
        {
            if (!File.Exists(Endpoint.SETTINGS_FILE))
            {
                File.WriteAllText(Endpoint.SETTINGS_FILE, JsonConvert.SerializeObject(new Config(), Formatting.Indented));
                Console.WriteLine("Settings file created");
                return null;
            }

            string json = File.ReadAllText(Endpoint.SETTINGS_FILE);
            
            try
            {
                return JsonConvert.DeserializeObject<Config>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Settings threw an exception: {ex.Message}");
                return null;
            }
        }
    }
}
