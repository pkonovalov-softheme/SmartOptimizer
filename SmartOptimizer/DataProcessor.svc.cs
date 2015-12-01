using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using CoreLib;

namespace SmartOptimizer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataProcessor" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DataProcessor.svc or DataProcessor.svc.cs at the Solution Explorer and start debugging.
    public class DataProcessor : IDataProcessor
    {
        private readonly StageOptimizer _stageOptimizer = new StageOptimizer();

        public List<string> GetDataPositions(Guid userId,
            List<string> refsList,
            Guid sessionId)
        {
            try
            {
                return _stageOptimizer.GetDataPositions(userId, refsList, sessionId.ToString());
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0}", ex.ToString());
                return refsList;
            }
        }

        public void SetSessionResult(string sessionId, string clickedLink)
        {
            _stageOptimizer.SetSessionResult(sessionId, clickedLink);
        }
    }
}
