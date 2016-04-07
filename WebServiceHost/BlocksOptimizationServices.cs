using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using CoreLib;

namespace WebServiceHost
{
    [ServiceContract()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class BlocksOptimizationServices 
    {
        private readonly StageOptimizer _stageOptimizer = new StageOptimizer();
        private OptimizationServiceSettings _settings = new OptimizationServiceSettings();

        //curl  -H "Content-Type:application/json" -H "Accept:application/json" -X POST -d"{\"blockId\":\"1\",\"userId\":\"{7c596b41-0dc3-45df-9b4c-08840f1da780}\",\"sessionId\":\"{46cd1b39-5d0a-440a-9650-ae4297b7e2e9}\"}" http://localhost:8080/BlocksOptimizationServices/dataPositions
        [WebInvoke(Method = "POST", UriTemplate = "dataPositions", BodyStyle = WebMessageBodyStyle.WrappedRequest, 
                   ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public List<string> GetDataPositions(int blockId, string userId, string sessionId)
        {
            try
            {
                //Trace.TraceInformation($"GetDataPositions called. blockId:{blockId}, sessionId:{sessionId}.");
                return _stageOptimizer.GetDataPositions(blockId, userId, sessionId);
            }
            catch (InvalidOperationException ex)
            {
                if (string.Equals(ex.Message, StageOptimizer.BlockNotFoundErrorCode.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new WebFaultException<string>(ex.Message, HttpStatusCode.NotAcceptable);
                }

                throw;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in GetDataPositions", ex.ToString());
                if (ex.GetType() == typeof(WebFaultException))
                {
                    throw;
                }

                throw new WebFaultException<string>(ex.Message, HttpStatusCode.SeeOther);
            }
        }

        //curl  -H "Content-Type:application/json" -H "Accept:application/json" -X PUT -d"{\"sessionId\":\"{46cd1b39-5d0a-440a-9650-ae4297b7e2e9}\",\"finalLink\":\"1.ref\",\"value\":\"1\"}" http://localhost:8080/BlocksOptimizationServices/sessionResult
        // An operation does not return a reply message, notifications style communication
        [OperationContract(IsOneWay = true)]
        [WebInvoke(Method = "PUT", UriTemplate = "sessionResult", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public void SetSessionResult(string sessionId, string finalLink, int value)
        {
            try
            {
                //Trace.TraceInformation($"SetSessionResult called. sessionId:{sessionId}, clicked link:{finalLink}.");
                _stageOptimizer.SetSessionResult(sessionId, finalLink, value);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in SetSessionResult", ex.ToString());
                if (ex.GetType() == typeof(WebFaultException))
                {
                    throw;
                }

                throw new WebFaultException<string>(ex.Message, HttpStatusCode.SeeOther);
            }
        }

        //curl  -H "Content-Type:application/json" -H "Accept:application/json" -X PUT -d"{\"blockId\":\"1\",\"refsList\":[\"1.ref\",\"2.ref\"]}" http://localhost:8080/BlocksOptimizationServices/block/update
        [WebInvoke(Method = "PUT", UriTemplate = "block/update", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public void AddOrUpdateBlock(int blockId, List<string> refsList)
        {
            try
            {
                Trace.TraceInformation($"AddOrUpdateBlock called. Block id:{blockId} has been changed.");
                _stageOptimizer.AddOrUpdateBlock(blockId, refsList);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in AddOrUpdateBlock", ex.ToString());
                if (ex.GetType() == typeof(WebFaultException))
                {
                    throw;
                }

                throw new WebFaultException<string>(ex.Message, HttpStatusCode.SeeOther);
            }
        }

        [WebGet(UriTemplate = "greet", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public string Greeting()
        {
            return "Hello World";
        }


        [WebInvoke(Method = "POST", UriTemplate = "GetSettings", BodyStyle = WebMessageBodyStyle.WrappedRequest,
           ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        public OptimizationServiceSettings GetSettings()
        {
            return _stageOptimizer.ServiceSettings;
        }

        //curl  -H "Content-Type:application/json" -H "Accept:application/json" -X PUT -d"{\"stealthMode\":\"true\"}" http://localhost:8080/BlocksOptimizationServices/SetSettings
        [WebInvoke(Method = "POST", UriTemplate = "SetSettings", BodyStyle = WebMessageBodyStyle.WrappedRequest, 
           ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        public void SetSettings(bool stealthMode)
        {
            Trace.TraceInformation("Changing StealthMode from {0} to {1}.", _stageOptimizer.ServiceSettings.StealthMode, stealthMode);
            _stageOptimizer.ServiceSettings.StealthMode = stealthMode;
        }
    }
}
