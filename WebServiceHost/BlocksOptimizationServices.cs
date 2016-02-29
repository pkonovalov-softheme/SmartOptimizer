using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreLib;

namespace WebServiceHost
{
    [ServiceContract()]
    class BlocksOptimizationServices 
    {
        private readonly StageOptimizer _stageOptimizer = new StageOptimizer();

        //curl  -H "Content-Type:application/json" -H "Accept:application/json" -X POST -d"{\"blockId\":\"1\",\"userId\":\"{7c596b41-0dc3-45df-9b4c-08840f1da780}\",\"sessionId\":\"{46cd1b39-5d0a-440a-9650-ae4297b7e2e9}\"}" http://localhost:8080/BlocksOptimizationServices/dataPositions
        [WebInvoke(Method = "POST", UriTemplate = "dataPositions", BodyStyle = WebMessageBodyStyle.WrappedRequest, 
                   ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public List<string> GetDataPositions(int blockId, string userId, string sessionId)
        {
            try
            {
                return _stageOptimizer.GetDataPositions(blockId, userId, sessionId);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in GetDataPositions", ex.ToString());
                throw;
            }
        }

        //curl  -H "Content-Type:application/json" -H "Accept:application/json" -X PUT -d"{\"sessionId\":\"{46cd1b39-5d0a-440a-9650-ae4297b7e2e9}\",\"clickedLink\":\"1.ref\",\"value\":\"1\"}" http://localhost:8080/BlocksOptimizationServices/sessionResult
        // An operation does not return a reply message, notifications style communication
        [OperationContract(IsOneWay = true)]
        [WebInvoke(Method = "PUT", UriTemplate = "sessionResult", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public void SetSessionResult(string sessionId, string clickedLink, int value)
        {
            try
            {
                Thread.Sleep(5000);

                _stageOptimizer.SetSessionResult(sessionId, clickedLink, value);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in SetSessionResult", ex.ToString());
                throw;
            }
        }

        //curl  -H "Content-Type:application/json" -H "Accept:application/json" -X PUT -d"{\"blockId\":\"1\",\"refsList\":[\"1.ref\",\"2.ref\"]}" http://localhost:8080/BlocksOptimizationServices/block/update
        [WebInvoke(Method = "PUT", UriTemplate = "block/update", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public void AddOrUpdateBlock(int blockId, List<string> refsList)
        {
            try
            {
                _stageOptimizer.AddOrUpdateBlock(blockId, refsList);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in AddOrUpdateBlock", ex.ToString());
                throw;
            }
        }

        [WebGet(UriTemplate = "greet", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public string Greeting()
        {
            return "Hello World";
        }

    }
}
