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
        private readonly List<string> _userDataPriority = new List<string>()
        {
            "Gender"
        };


        private readonly Stage _currentStage;
        private readonly object _stupidLock = new object();
        private readonly Random _rnd = new Random();
        //private readonly Dictionary<Guid, UserSession> _userSessions = new Dictionary<Guid, UserSession>();

        private const double GroupBRate = 0.8; //0.2
        private AdsSet _currentAdsSet;
        readonly MemoryCache _userSessionsCache = new MemoryCache("CachingProvider");
        readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy() {SlidingExpiration = TimeSpan.FromMinutes(10)};

        public StageOptimizer()
        {
            _currentStage = Stage.Base;
            _currentAdsSet = new AdsSet(new List<string>());
        }

        public List<string> GetDataPositions(Guid userId,
            List<string> refsList,
            string sessionId)
        {
            lock(_stupidLock)
            {
                switch (_currentStage)
                {
                    case Stage.Base:
                        return GetDataFromBaseStage(userId, refsList, sessionId);
                }
            }

            return refsList;
        }

        public bool SetSessionResult(string sessionId, string clickedLink)
        {
            lock (_stupidLock)
            {
                if (!_userSessionsCache.Contains(sessionId))
                {
                    Trace.TraceWarning("Session with id: {0} not found in the list of started sessions.", sessionId);
                    return false;
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

                    return false;
                }

                if (session.InBGroup)
                {
                    return _currentAdsSet.ClickOnRef(clickedLink);
                }

                return true;
            }
        }

        public List<string> GetBaseBaseRefsList(List<string> refsList)
        {
            return _currentAdsSet.BaseRefsList;
        }

        private List<string> GetDataFromBaseStage(Guid userId, List<string> refsList, string sessionId)
        {
            bool isInBgroup = false;
            AdsSet set = new AdsSet(refsList);

            if (ShouldAddUserToBGroup())
            {
                refsList = set.NextRandomShuffle().ToList();
                isInBgroup = true;
            }
            else
            {
                refsList = set.BaseRefsList;
            }

            if (set != _currentAdsSet)
            {
                _currentAdsSet = set;
            }

            var curSession = new UserSession(sessionId,
                 userId,
                 null,
                 isInBgroup,
                 _currentAdsSet);

            _userSessionsCache.Add(curSession.Id, curSession, _cachePolicy);

            return refsList;
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
