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
        private readonly Random _rnd = new Random();
        private const double GroupBRate = 0.2;
        // private AdsSet _currentAdsSet = new AdsSet(new List<string>());
        private readonly MemoryCache _userSessionsCache = new MemoryCache("CachingProvider");

        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };

        private readonly Dictionary<int, AdsBlock> _adsBlocks = new Dictionary<int, AdsBlock>();
        public const bool ChangeTimeSpeed = true;
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

        public void AddOrUpdateBlock(int blockId, List<string> refsList)
        {
            if (_adsBlocks.ContainsKey(blockId))
            {
                _adsBlocks[blockId] = new AdsBlock(blockId, refsList);
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

                    adsBlock.BlockStats.GroupBConvertion.Views++;
                    // Trace.TraceInformation("b.Views++");
                }
                else
                {
                    refsList = adsBlock.BaseRefsList;
                    adsBlock.BlockStats.GroupAConvertion.Views++;
                }

                var curSession = new UserSession(sessionId,
                    userId,
                    null,
                    isInBgroup,
                    adsBlock,
                    refsList);

                _userSessionsCache.Add(curSession.Id, curSession, _cachePolicy);

                if (ChangeTimeSpeed)
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
                    return;
                }

                UserSession session = (UserSession) _userSessionsCache[sessionId];
                BlockStats blockStats = session.Block.BlockStats;

                _userSessionsCache.Remove(sessionId);

                if (finalLink == null)
                {
                    Trace.TraceWarning("Session with id: {0} ended with null finalLink.", sessionId);
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                    return;
                }

                if (session.InBGroup)
                {
                    blockStats.GroupBConvertion.Clicks++;
                    blockStats.GroupBConvertion.Value += value;
                }
                else
                {
                    blockStats.GroupAConvertion.Clicks++;
                    blockStats.GroupAConvertion.Value += value;
                    // Trace.TraceInformation("b.Value++");
                }

                int positionId = session.ShowedList.IndexOf(finalLink);
                blockStats.AddClick(positionId, value);

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
            bool result = curVal < GroupBRate;
            return result;
        }
    }
}
