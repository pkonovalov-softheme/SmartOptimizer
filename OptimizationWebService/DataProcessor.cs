using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CoreLib;

namespace OptimizationWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataProcessor" in both code and config file together.
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
                Trace.TraceError("Fatal unhandled exception: {0} in GetDataPositions", ex.ToString());
                throw;
            }
        }

        public void SetSessionResult(string sessionId, string clickedLink)
        {
            try
            {
                _stageOptimizer.SetSessionResult(sessionId, clickedLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in SetSessionResult", ex.ToString());
                throw;
            }
        }
    }
}
