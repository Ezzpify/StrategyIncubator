using System;
using System.Reflection;

namespace StrategyIncubator
{
    class Program
    {
        private static Session _session;

        static void Main(string[] args)
        {
            Console.Title = $"Strategy Incubator v{Assembly.GetExecutingAssembly().GetName().Version}";

            /*Session will hi-jack this thread until it's complete*/
            _session = new Session(Settings.GetSettings());

            Console.WriteLine("\n\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
