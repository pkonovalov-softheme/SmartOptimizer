using System;
using System.Collections.Generic;
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
            ServiceHost host = new ServiceHost(typeof(BlocksOptimizationServices));

            WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None);

            ServiceEndpoint endPoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(BlocksOptimizationServices)),
            binding, new EndpointAddress(@"http://localhost:8080/BlocksOptimizationServices"));
            WebHttpBehavior webBehavior = new WebHttpBehavior();
            endPoint.Behaviors.Add(webBehavior);
            host.AddServiceEndpoint(endPoint);
            Guid one = Guid.NewGuid();
            Guid two = Guid.NewGuid();


            host.Open();
            Console.WriteLine("BlocksOptimizationServices Service Started.\n Press enter to stop the service");
            Console.ReadLine();
            host.Close();
            //"http://localhost:8080/Calculator/Add?b=10&a=16");
        }
    }
}
