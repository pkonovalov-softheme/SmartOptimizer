using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading;
using CoreLib;

namespace TestClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AddBlock();
            TestGetDataPositions();
            //GetSettings();
           // SetSettings();
           // GetSettings();
        }

        private static void AddBlock()
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

                    jsonInput += "\"" + i + ".ref\"";
                }

                jsonInput += "]}";
                var response = client.UploadString("http://localhost:8080/BlocksOptimizationServices/block/update",
                    "PUT", jsonInput);
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

        private static void TestGetDataPositions()
        {
            const long testCount = 3000;
            const string jsonInput =
                "{\"blockId\":\"1\",\"userId\":\"{7c596b41-0dc3-45df-9b4c-08840f1da780}\",\"sessionId\":\"{46cd1b39-5d0a-440a-9650-ae4297b7e2e9}\"}";
            List<WebClient> clients = new List<WebClient>();
            for (int i = 0; i < testCount; i++)
            {
                var client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                clients.Add(client);
            }

            Stopwatch watch = Stopwatch.StartNew();

            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1;
            //0.39
            Parallel.For(0, testCount, po, index =>
            {
                WebClient client = clients[(int) index];
                var response = client.UploadString("http://localhost:8080/BlocksOptimizationServices/dataPositions",
                    "POST", jsonInput);
            });

            watch.Stop();
            double speed = (double) testCount/watch.ElapsedMilliseconds;
            Console.WriteLine("Avg speed: {0} requests per ms", speed);
        }

        private static void SetSettings()
        {
            OptimizationServiceSettings op = new OptimizationServiceSettings();
            op.StealthMode = true;

            string jsonInput = Utils.ToJson(op);

           // string jsonInput = "{\"stealthMode\":\"true\"}";
            var client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            var response = client.UploadString("http://localhost:8080/BlocksOptimizationServices/SetSettings", "POST", jsonInput);
        }

        private static void GetSettings()
        {
            const string jsonInput = "{}";
            var client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            var response = client.UploadString("http://localhost:8080/BlocksOptimizationServices/GetSettings", "POST", jsonInput);

            if (!string.IsNullOrEmpty(response))
            {
                OptimizationServiceSettings set = Utils.DeserializeJSON<OptimizationServiceSettings>(response);
            }

        }
    }
}
