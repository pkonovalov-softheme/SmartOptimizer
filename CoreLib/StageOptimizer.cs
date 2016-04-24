using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Runtime.Caching.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CoreLib.Statistics;
using SmartOptimizer;

namespace CoreLib
{
    public class StageOptimizer
    {
        private readonly object _stupidLock = new object();
        private readonly ThreadSafeRandom _rnd = new ThreadSafeRandom();

        // private AdsSet _currentAdsSet = new AdsSet(new List<string>());
        private readonly MemoryCache _userSessionsCache = new MemoryCache("CachingProvider");

        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };

        private readonly Dictionary<int, AdsBlock> _adsBlocks = new Dictionary<int, AdsBlock>();
        private readonly TimeSpan _timeOffset = TimeSpan.FromSeconds(1);
        public static DateTime CurrentDate { get; set; } = DateTime.Now;
        public const int BlockNotFoundErrorCode = 2277;

        private OptimizationServiceSettings _optimizationServiceSettings = new OptimizationServiceSettings();

        private bool StealthMode
        {
            get { return _optimizationServiceSettings.StealthMode; }
        }

        public OptimizationServiceSettings ServiceSettings
        {
            get { return _optimizationServiceSettings; }
            set
            {
                lock (_stupidLock)
                {
                    _optimizationServiceSettings = value;
                }
            }
        }

        public MemoryCache UserSessionsCache
        {
            get { return _userSessionsCache; }
        }

        public bool ChangeTimeSpeed
        {
            get { return GeneralSettings.ChangeTimeSpeed; }
        }

        public void AddOrUpdateBlock(int blockId, List<string> refsList)
        {
            if (_adsBlocks.ContainsKey(blockId))
            {
                AdsBlock block = _adsBlocks[blockId];

                IEnumerable<string> newAds = refsList.Where(item => !block.BaseRefsList.Contains(item));
                Trace.TraceInformation("There are:{0} new ads.", newAds.Count());
                AdsBlock curBlock = _adsBlocks[blockId];

                Dictionary<string, AdStats> newRefStats = new Dictionary<string, AdStats>();

                IEnumerable<double> valuesPerView = curBlock.RefPerfomanceStats.Values.Select(stat => stat.ConvObject.CurrentValuePerView);
                double maxEf = valuesPerView.Max();
                double minEf = valuesPerView.Min();


                foreach (string curRef in refsList)
                {
                    AdStats curStats = curBlock.RefPerfomanceStats.SingleOrDefault(ps => ps.Key == curRef).Value;
                    var newAdStats = new AdStats();

                    //  newAdStats.ConvObject.CurrentValuePerView = GetRandomNumber(minEf, maxEf); - avg random

                    double avg = (maxEf - minEf) / 2; // - shifted for new values up to 50%
                    newAdStats.ConvObject.CurrentValuePerView = _rnd.NextDouble(avg, maxEf);

                    newRefStats.Add(curRef, curStats ?? new AdStats());

                    List<string> optimizedRefList = block.RefPerfomanceStats.OrderByDescending(de => de.Value.ConvObject.CurrentValuePerView).Select(de => de.Key).ToList();
                    block.BaseRefsList = optimizedRefList;
                }

                curBlock.UpdateBlock(refsList, newRefStats);
                Trace.TraceInformation("Block with id:{0} was updated.", blockId);
            }
            else
            {
                _adsBlocks.Add(blockId, new AdsBlock(blockId, refsList));
                Trace.TraceInformation("Block with id:{0} was added.", blockId);
            }
        }

        public AdsBlock GeAdsBlock(int blockId)
        {
            return _adsBlocks[blockId];
        }

        public List<string> GetDataPositions(int blockId, string userId, string sessionId)
        {
            lock (_stupidLock)
            {
                AdsBlock adsBlock;
                try
                {
                    adsBlock = _adsBlocks[blockId];
                }
                catch (KeyNotFoundException)
                {
                    Trace.TraceInformation("Block with id:{0} was not found.", blockId);
                    throw new InvalidOperationException(BlockNotFoundErrorCode.ToString());
                }

                bool isInBgroup = false;
                List<string> refsList;
                if (ShouldAddUserToBGroup() && !StealthMode)
                {
                    refsList = adsBlock.NextRandomShuffle().ToList();
                    isInBgroup = true;

                    adsBlock.BlockPositionStats.GroupBConvertion.Views++;
                }
                else
                {
                    refsList = adsBlock.BaseRefsList;
                    adsBlock.BlockPositionStats.GroupAConvertion.Views++;
                }

                var curSession = new UserSession(sessionId,
                    userId,
                    null,
                    isInBgroup,
                    adsBlock,
                    refsList);

                _userSessionsCache.Add(curSession.Id, curSession, _cachePolicy);

                if (GeneralSettings.ChangeTimeSpeed)
                {
                    CurrentDate += _timeOffset;
                }

                return refsList;
            }
        }

        public void SetSessionResult(string sessionId, string finalLink, int value)
        {
            lock (_stupidLock)
            {
                if (!_userSessionsCache.Contains(sessionId))
                {
                    Trace.TraceWarning("Session with id: {0} not found in the list of started sessions.", sessionId);
                    Utils.DbgBreak();
                    return;
                }

                UserSession session = (UserSession) _userSessionsCache[sessionId];
                BlockPositionStats blockPositionStats = session.Block.BlockPositionStats;

                _userSessionsCache.Remove(sessionId);

                if (finalLink == null)
                {
                    Trace.TraceWarning("Session with id: {0} ended with null finalLink.", sessionId);
                    Utils.DbgBreak();
                    return;
                }

                if (session.InBGroup)
                {
                    blockPositionStats.GroupBConvertion.Clicks++;
                    blockPositionStats.GroupBConvertion.Value += value;
                }
                else
                {
                    blockPositionStats.GroupAConvertion.Clicks++;
                    blockPositionStats.GroupAConvertion.Value += value;
                }

                int positionId = session.ShowedList.IndexOf(finalLink);
                blockPositionStats.AddClick(positionId, value);

                if (session.InBGroup)
                {
                    session.Block.ClickOnRef(finalLink, value);
                }
            }
        }

        /// <summary>
        /// Should we add to group with random permutations where we teach
        /// </summary>
        /// <returns></returns>
        private bool ShouldAddUserToBGroup()
        {
            double curVal = _rnd.NextDouble();
            bool result = curVal < GeneralSettings.GroupBRate;
            return result;
        }
    }
}
