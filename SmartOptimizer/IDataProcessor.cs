using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SmartOptimizer
{
    [ServiceContract]
    public interface IDataProcessor
    {

        [OperationContract]
        List<string> GetDataPositions(Guid userId, List<string> refsList, Guid sessionId);

        [OperationContract]
        void SetSessionResult(string sessionId, string clickedLink);
    }
}
