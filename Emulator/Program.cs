using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreLib;

namespace Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //string connectionString = "data source=localhost;initial catalog = BlockOptimizationStats; persist security info = True;Integrated Security = SSPI";
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    SqlCommand cmd = new SqlCommand("INSERT INTO [BlockStats] (Position, BlockId, Views, Clicks, Value) VALUES (@Position, @BlockId, @Views, @Clicks, @Value)");
            //    cmd.CommandType = CommandType.Text;
            //    cmd.Connection = connection;
            //    cmd.Parameters.AddWithValue("@Position", 1);
            //    cmd.Parameters.AddWithValue("@BlockId", 2);
            //    cmd.Parameters.AddWithValue("@Views", 0);
            //    cmd.Parameters.AddWithValue("@Clicks", 0);
            //    cmd.Parameters.AddWithValue("@Value", 0);
            //    connection.Open();
            //    cmd.ExecuteNonQuery();
            //}


            //  BasicTester tester = new BasicTester();
            // tester.TestProfit();

            TestIt();
        }

        static void TestIt()
        {
            FullChaineTester tester1 = new FullChaineTester();
            TestingResult result1 = tester1.TestFullChaine();

            int testsCount = Environment.ProcessorCount;

            var results = new ConcurrentBag<TestingResult>();

            Parallel.For(0, testsCount, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount},
                   index => {
                       FullChaineTester tester = new FullChaineTester();
                       TestingResult result = tester.TestFullChaine();
                       results.Add(result);
                   });

            long realProfit = results.Sum(res => res.RealProfit);
            long rndProfit = results.Sum(res => res.RndProfit);
            long maxProfit = results.Sum(res => res.MaxProfit);

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            Console.WriteLine("Real: {0}.  Rnd: {1}. Max {2}.",
                realProfit.ToString("#,#.00", nfi),
                rndProfit.ToString("#,#.00", nfi),
                maxProfit.ToString("#,#.00", nfi));

            double proc = (maxProfit - realProfit) / (double)maxProfit;
            Console.WriteLine("Real is smaller then max on {0}. ", proc.ToString("0.##%"));

            double rndProc = (maxProfit - rndProfit) / (double)maxProfit;
            Console.WriteLine("Rnd is smaller then max on {0}. ", rndProc.ToString("0.##%"));
        }
    }
}
