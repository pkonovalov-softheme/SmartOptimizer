using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Statistics
{
    public sealed class PositionsStats
    {
        public PositionsStats()
        {
            PositionsConvertion = new Dictionary<int, ConvertionObject>();
        }

        /// <summary>
        /// ConvertionObject(value) for specific position(key)
        /// </summary>
        public Dictionary<int, ConvertionObject> PositionsConvertion { get; private set; }
    }
}
