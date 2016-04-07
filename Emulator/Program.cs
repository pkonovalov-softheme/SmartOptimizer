using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            FullChaineTester tester = new FullChaineTester();
            tester.TestFullChaine();
        }
    }
}
