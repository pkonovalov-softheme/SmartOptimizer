using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (WebHost.StartHost())
                {
                    Console.WriteLine("Press any key to stop host");
                    Console.ReadLine();
                }
                
            }
            catch (Exception ex)
            {
                Trace.Write("Fatar error starting web host: {0} ", ex.Message);
                throw;
            }
        }
    }
}
