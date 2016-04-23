using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class ThreadSafeRandom
    {
        private static readonly Random _global = new Random();
        [ThreadStatic]
        private static Random _local;

        public ThreadSafeRandom()
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }
        }
        public int Next()
        {
            return _local.Next();
        }

        public int Next(int minValue, int maxValue)
        {
            return _local.Next(minValue, maxValue);
        }

        public double NextDouble()
        {
            return _local.NextDouble();
        }

        public double NextDouble(double minimum, double maximum)
        {
            return _local.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
