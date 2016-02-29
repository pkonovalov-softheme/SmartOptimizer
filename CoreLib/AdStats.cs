using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class AdStats
    {
        public AdStats()
        {
            ClicksCount = 0;
            TotalValue = 0;
            //Views = 0;
        }

       // public long Views { get; set; }

        public long TotalValue { get; set; }

        public long ClicksCount { get; set; }
    }
}
