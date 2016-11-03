namespace LatestStrats
{
    class Program
    {
        private static Session _session;

        static void Main(string[] args)
        {
            _session = new Session(Settings.GetSettings());
        }
    }
}
