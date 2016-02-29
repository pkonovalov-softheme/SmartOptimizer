using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OptimizationWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDataProcessor" in both code and config file together.
    [ServiceContract]
    public interface IDataProcessor
    {
        [WebInvoke(Method = "POST", UriTemplate = "dataPositions", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]

        List<string> GetDataPositions(int blockId, Guid userId, Guid sessionId);

        [OperationContract (IsOneWay = true)]
        // An operation does not return a reply message, notifications style communication
        [WebInvoke(Method = "PUT", UriTemplate = "sessionResult", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]

        void SetSessionResult(string sessionId, string clickedLink, int value);

        [WebInvoke(Method = "PUT", UriTemplate = "block/update", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]

        void AddOrUpdateBlock(int blockId, List<string> refsList);

        [WebGet(UriTemplate = "greet", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        string Greeting();
    }
}
