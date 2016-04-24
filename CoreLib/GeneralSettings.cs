using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public static class GeneralSettings
    {
        /// <summary>
        /// How much time we need to test each add(in avg) to say that testing is complete
        /// </summary>
        public const int TargetSamplesPerAd = 1; //ToDo: calculate // 8000 - 2%; 1000 -10% of erorrs; 800 - 15%; 400 - 20%; 

        /// <summary>
        /// What fraction of all new user session should be in B group(group where we will make permutations)
        /// </summary>
        public const double GroupBRate = 0.2;

        /// <summary>
        /// Value to make date change faster to get beatefull dashboard info fast
        /// </summary>
        public const bool ChangeTimeSpeed = false;


        /// <summary>
        /// Save stats in DB or not
        /// </summary>
        public const bool SaveStats = false;

        /// <summary>
        /// Smoothing factor for LowPass filter
        /// </summary>
        public const double SmoothingParameter = 0.3;
    }
}
