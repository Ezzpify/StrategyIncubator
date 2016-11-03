using System;

namespace StrategyIncubator
{
    class Program
    {
        private static Session _session;

        static void Main(string[] args)
        {
            Console.Title = "Strategy Incubator";
            _session = new Session(Settings.GetSettings());
        }
    }
}
