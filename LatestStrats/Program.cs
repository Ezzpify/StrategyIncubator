using System;
using System.Reflection;

namespace StrategyIncubator
{
    class Program
    {
        private static Session _session;

        static void Main(string[] args)
        {
            var settings = Settings.GetConfig();
            if (settings != null)
            {
                settings.appversion = $"{Assembly.GetExecutingAssembly().GetName().Version}";
                Console.Title = $"Strategy Incubator v{settings.appversion}";
                _session = new Session(settings);
            }

            Console.WriteLine("\n\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
