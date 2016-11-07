using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.IO;

namespace StrategyIncubator
{
    class Utils
    {
        public static RandomProvider Random = new RandomProvider();

        public class RandomProvider
        {
            private static int seed = Environment.TickCount;

            private ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
                new Random(Interlocked.Increment(ref seed))
            );

            public Random GetThreadRandom()
            {
                return randomWrapper.Value;
            }
        }
    }
}
