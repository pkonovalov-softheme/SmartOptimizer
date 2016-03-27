using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreLib;

namespace SmUI
{
    public static class WebServiceWrapper
    {
        public static void SetSettings(bool stealthMode)
        {
            //string jsonInput = string.Format("{\"stealthMode\":\"{0}\"}", stealthMode);
            string jsonInput = "{\"stealthMode\":\"" + stealthMode.ToString().ToLowerInvariant() + "\"}";

           var client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            client.UploadString("http://localhost:8080/BlocksOptimizationServices/SetSettings", "POST", jsonInput);
        }

        public static OptimizationServiceSettings GetSettings()
        {
            const string jsonInput = "{}";
            var client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            var response = client.UploadString("http://localhost:8080/BlocksOptimizationServices/GetSettings", "POST", jsonInput);

            if (string.IsNullOrEmpty(response))
            {
               throw new InvalidOperationException("Empty GetSettings() WCF response");
            }

            OptimizationServiceSettings set = Utils.DeserializeJSON<OptimizationServiceSettings>(response);
            return set;
        }
    }
}
