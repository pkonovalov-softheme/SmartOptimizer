using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulator
{
    sealed class TestingResult
    {
        public long MaxProfit;
        public long RndProfit;
        public long RealProfit;

        public TestingResult(long maxProfit, long rndProfit, long realProfit)
        {
            MaxProfit = maxProfit;
            RndProfit = rndProfit;
            RealProfit = realProfit;
        }
    }
}
