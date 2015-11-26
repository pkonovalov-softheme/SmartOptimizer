using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOptimizer
{
    public class UserPreference
    {
        public string RefClassId { get; private set; }
        public long VisitCount { get; set; }

        public UserPreference(string refClassId, long visitCount)
        {
            RefClassId = refClassId;
            VisitCount = visitCount;
        }
    }
}