using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public sealed class OptimizationTartget
    {
        private readonly int _unitsCount;

        /// <summary>
        /// unitsCount - count of units for the optimization
        /// </summary>
        /// <param name="unitsCount"></param>
        public OptimizationTartget(int unitsCount)
        {
            _unitsCount = unitsCount;
            CurrectUnit = 0;
        }

        public int UnitsCount
        {
            get { return _unitsCount; }
        }

        public int CurrectUnit { get; set; }
    }
}
