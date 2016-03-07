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
    public sealed  class WebHost : IDisposable
    {
        private const string WebHostAdress = @"http://localhost:8080/BlocksOptimizationServices";

        private readonly ServiceHost _host;

        private  WebHost(ServiceHost serviceHost)
        {
            _host = serviceHost;
        }

        public static WebHost StartHost()
        {
            WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None);
            ServiceEndpoint endPoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(BlocksOptimizationServices)),
            binding, new EndpointAddress(WebHostAdress));
            WebHttpBehavior webBehavior = new WebHttpBehavior();
            endPoint.Behaviors.Add(webBehavior);

            ServiceHost serviceHost = new ServiceHost(typeof(BlocksOptimizationServices));
            serviceHost.AddServiceEndpoint(endPoint);
            serviceHost.Open();
            Trace.WriteLine("Service Host Started.");
            return new WebHost(serviceHost);

        }

        public void Stop()
        {
            Trace.WriteLine("Stoping Service Host...");
            _host.Close();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
