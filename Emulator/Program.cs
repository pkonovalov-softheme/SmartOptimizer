using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib;

namespace Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemTester tester = new SystemTester();
            tester.TestlPerfomanceWithBigAdsCount();
        }
    }
}
