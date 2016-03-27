using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CoreService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Trace.TraceInformation("Starting Block optimisation service...");

            var servicesToRun = new ServiceBase[]
            {
                new CoreService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
