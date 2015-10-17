using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOptimizer
{
    public enum Stage
    {
        Base,               // Without clusters, general optimization
        Prioritets,         // Optimizing prioritet clusters
        GeneralCluster,     // General cluster optimiztion
    };
}