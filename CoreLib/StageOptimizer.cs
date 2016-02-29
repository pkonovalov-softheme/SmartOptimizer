using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Caching.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SmartOptimizer;

namespace CoreLib
{
    public class StageOptimizer
    {
        private readonly object _stupidLock = new object();
        private readonly Random _rnd = new Random();
        private const double GroupBRate = 0.8; //0.2
       // private AdsSet _currentAdsSet = new AdsSet(new List<string>());
        readonly MemoryCache _userSessionsCache = new MemoryCache("CachingProvider");
        readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy() {SlidingExpiration = TimeSpan.FromMinutes(10)};
        readonly Dictionary<int, AdsBlock> _adsBlocks = new Dictionary<int, AdsBlock>();

        public void AddOrUpdateBlock(int blockId, List<string> refsList)
        {
            if (_adsBlocks.ContainsKey(blockId))
            {
                _adsBlocks[blockId] = new AdsBlock(blockId, refsList);
            }
            else
            {
                _adsBlocks.Add(blockId, new AdsBlock(blockId, refsList));
            }
        }

        public AdsBlock GeAdsBlock(int blockId)
        {
            return _adsBlocks[blockId];
        }

        public List<string> GetDataPositions(int blockId, string userId, string sessionId)
        {
            lock(_stupidLock)
            {
                AdsBlock adsBlock;
                try
                {
                    adsBlock = _adsBlocks[blockId];
                }
                catch (KeyNotFoundException)
                {
                    throw new InvalidOperationException("The is no added block with id: " + blockId);
                }

                bool isInBgroup = false;
                List<string> refsList;
                if (ShouldAddUserToBGroup())
                {
                    refsList = adsBlock.NextRandomShuffle().ToList();
                    isInBgroup = true;
                }
                else
                {
                    refsList = adsBlock.BaseRefsList;
                }

                var curSession = new UserSession(sessionId,
                     userId,
                     null,
                     isInBgroup,
                     adsBlock);

                _userSessionsCache.Add(curSession.Id, curSession, _cachePolicy);

                return refsList;
            }
        }

        public void SetSessionResult(string sessionId, string clickedLink, int value)
        {
            lock (_stupidLock)
            {
                if (!_userSessionsCache.Contains(sessionId))
                {
                    Trace.TraceWarning("Session with id: {0} not found in the list of started sessions.", sessionId);
                    return;
                }

                UserSession session = (UserSession)_userSessionsCache[sessionId];
                _userSessionsCache.Remove(sessionId);

                if (clickedLink == null)
                {
                    Trace.TraceWarning("Session with id: {0} ended with null clickedLink.", sessionId);
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                    return;
                }

                if (session.InBGroup)
                {
                    session.Block.ClickOnRef(clickedLink, value);
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
