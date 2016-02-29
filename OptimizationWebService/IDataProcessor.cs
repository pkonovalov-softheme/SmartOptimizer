using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace OptimizationWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDataProcessor" in both code and config file together.
    [ServiceContract]
    public interface IDataProcessor
    {
        [OperationContract]
        List<string> GetDataPositions(int blockId, Guid userId, Guid sessionId);


        [OperationContract]
        void SetSessionResult(string sessionId, string clickedLink, int value);

    }
}
