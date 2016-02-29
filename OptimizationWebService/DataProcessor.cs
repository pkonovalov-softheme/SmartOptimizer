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

        public void UpdateBlockGroup(int blockId, List<string> refsList, bool optimize)
        {
            
        }

        public List<string> GetDataPositions(int blockId, Guid userId, Guid sessionId)
        {
            try
            {
                return _stageOptimizer.GetDataPositions(blockId, userId, sessionId.ToString());
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in GetDataPositions", ex.ToString());
                throw;
            }
        }

        public void SetSessionResult(string sessionId, string clickedLink, int value)
        {
            try
            {
                _stageOptimizer.SetSessionResult(sessionId, clickedLink, value);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Fatal unhandled exception: {0} in SetSessionResult", ex.ToString());
                throw;
            }
        }
    }
}
