using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private List<UserPreferences> _userPreferenceses;
        private long _userSessionCount = 0;
        private readonly Stopwatch _startWatch = Stopwatch.StartNew();
        private readonly object _stupidLock = new object();
        private readonly Random _rnd = new Random();
        private readonly Dictionary<Guid, UserSession> _userSessions = new Dictionary<Guid, UserSession>();
        private const double GroupBRate = 0.8; //0.2
        private AdsSet _currentAdsSet;


        public StageOptimizer()
        {
            _currentStage = Stage.Base;
            _userPreferenceses = new List<UserPreferences>();
            _currentAdsSet = new AdsSet(new List<string>());
        }

        /*
         *  Fake methods to replace with real cals
         *  ===================================================================
         */
        public List<string> GetTextClasses()
        {
            return new List<string>() {"1", "2", "3", "4", "5"};
        }

        public string GetTextClass(string href)
        {
            return "1";
        }

        /*
         * ===================================================================
        *  Fake methods end
        */


        public List<string> GetDataPositions(Guid userId,
            Dictionary<string, string> userData,
            List<string> refsList,
            Guid sessionId)
        {
            lock (_stupidLock)
            {
                _userSessionCount++;

                switch (_currentStage)
                {
                    case Stage.Base:

                        //if (_baseStageABtest == null)
                        //{
                        //    _baseStageABtest = new ABtest(refsList, TargetExampesCount);
                        //}



                        return GetDataFromBaseStage(userId, userData, refsList, sessionId);

                    //case Stage.Prioritets:



                    //    return GetDataFromPrioritetStage(userId, userData, refsList, sessionId);
                }
            }

            return refsList;
        }

        public void SetSessionResult(Guid sessionId, string clickedLink)
        {
            lock (_stupidLock)
            {
                if (!_userSessions.ContainsKey(sessionId))
                {
                    Trace.TraceWarning("Session with id: {0} not found in the list of started sessions.", sessionId);
                    return;
                }

                UserSession session = _userSessions[sessionId];

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
                    _currentAdsSet.ClickOnRef(clickedLink);
                }
            }
        }

        public List<string> GetBaseBaseRefsList(List<string> refsList)
        {
            return _currentAdsSet.BaseRefsList;
        }

        private List<string> GetDataFromBaseStage(Guid userId, Dictionary<string, string> userData, List<string> refsList, Guid sessionId)
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
                 userData,
                 isInBgroup,
                 _currentAdsSet);

            _userSessions.Add(curSession.Id, curSession);

            return refsList;
        }

        //private List<string> GetDataFromPrioritetStage(Guid userId, Dictionary<string, string> userData,
        //    List<string> refsList, Guid sessionId)
        //{
        //    UserSession curSession;
        //    string sex = userData["sex"];
        //    int curIndex = 0;

        //    if (sex == "F")
        //    {
        //        curIndex = 1;
        //    }

        //    if (ShouldAddUserToBGroup())
        //    {
        //        // B group
        //        string firstRef = _prioritetStageABtests[curIndex].GetNextRandomHrefToTest();
        //        curSession = new UserSession(sessionId, Stage.Base, userId, userData, firstRef, _currentPriorityIndex);
        //        refsList.Remove(firstRef);
        //        refsList.Insert(0, firstRef);
        //    }
        //    else
        //    {
        //        curSession = new UserSession(sessionId, Stage.Base, userId, userData);
        //    }

        //    _userSessions.Add(curSession);

        //    if (_prioritetStageABtests[curIndex].IsComplited)
        //    {
        //        return _prioritetStageABtests[curIndex].GetOrderedHrefs();
        //    }

        //    return refsList;
        //}

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

        //private bool DoesWeKnowTheUser(Guid userId)
        //{
        //    const int knownLimit = 100;

        //    return (_userPreferenceses.Single(userPreferences => userPreferences.UserId == userId).Preferences.Count >
        //            knownLimit);
        //}
    }
}
