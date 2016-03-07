using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            AddBlock();
            TestGetDataPositions();
        }

        static void AddBlock()
        {
            const int refsToGen = 5000;
            //string jsonInput = "{\"blockId\":\"1\",\"refsList\":[\"1.ref\",\"2.ref\"]}";
            string jsonInput = "{\"blockId\":\"1\",\"refsList\":[";

            using (var client = new WebClient())
            {
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;

                for (int i = 0; i < refsToGen; i++)
                {
                    if (i > 0)
                    {
                        jsonInput += ",";
                    }

                    jsonInput += "\"" + i + ".ref\"" ;
                }

                jsonInput += "]}";
                var response = client.UploadString("http://localhost:8080/BlocksOptimizationServices/block/update", "PUT", jsonInput);
            }
        }

        //static void TestGetDataPositions()
        //{
        //    const long testCount = 2;
        //    const string jsonInput = "{\"blockId\":\"1\",\"userId\":\"{7c596b41-0dc3-45df-9b4c-08840f1da780}\",\"sessionId\":\"{46cd1b39-5d0a-440a-9650-ae4297b7e2e9}\"}";

        //    Stopwatch watch = Stopwatch.StartNew();

        //    using (var client = new WebClient())
        //    {
        //        client.Headers["Content-type"] = "application/json";
        //        client.Encoding = Encoding.UTF8;

        //        for (int i = 0; i < testCount; i++)
        //        {
        //            var response = client.UploadString("http://localhost:8080/BlocksOptimizationServices/dataPositions", "POST", jsonInput);
        //        }

        //        watch.Stop();
        //        double speed = watch.ElapsedMilliseconds / (double)testCount; 
        //        Console.WriteLine("Avg speed: {0} request/ms", testCount);
        //    }
        //}

        static void TestGetDataPositions()
        {
            const long testCount = 1;
            const string jsonInput = "{\"blockId\":\"1\",\"userId\":\"{7c596b41-0dc3-45df-9b4c-08840f1da780}\",\"sessionId\":\"{46cd1b39-5d0a-440a-9650-ae4297b7e2e9}\"}";
            List<WebClient> clients = new List<WebClient>();
            for (int i = 0; i < testCount; i++)
            {
                var client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                clients.Add(client);
            }

            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < testCount; i++)
            {
                WebClient client = clients[i];
                var response = client.UploadString("http://localhost:8080/BlocksOptimizationServices/dataPositions", "POST", jsonInput);
            }

            watch.Stop();
            double speed = (double)testCount / watch.ElapsedMilliseconds;
            Console.WriteLine("Avg speed: {0} requests per ms", speed);
        }
    }
}
