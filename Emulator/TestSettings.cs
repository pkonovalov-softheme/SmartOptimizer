using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulator
{
    internal static class TestSettings
    {
        /// <summary>
        /// How many time we are retising results (rnd, real and max) [not used for basic tester]
        /// </summary>
        public const int ProfitRetestCount = 300000;
        //-------------->>>>>>>>>>>>>>>

        /// <summary>
        /// Possiblity that user will see next add if he will not click/buy previous
        /// </summary>
        public const double NextPosMoveProbability = 0.6;

        /// <summary>
        /// Starting ads count
        /// </summary>
        public const int StartAdsCount = 200;

        /// <summary>
        /// Min ads effectivity
        /// </summary>
        public const double MinAddEf = 0.0;

        /// <summary>
        /// Max ads effectivity
        /// </summary>
        public const double MaxAddEf = 0.3;

        /// <summary>
        /// Total tests count
        /// </summary>
        public const long TotalTestsCount = 200000;

        /// <summary>
        /// Count of tests between changes
        /// </summary>
        public const long TestsBetweenChanges = TotalTestsCount / 5;
    }
}
