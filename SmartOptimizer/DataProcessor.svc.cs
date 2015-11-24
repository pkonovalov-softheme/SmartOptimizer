﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace SmartOptimizer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataProcessor" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DataProcessor.svc or DataProcessor.svc.cs at the Solution Explorer and start debugging.
    public class DataProcessor : IDataProcessor
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
        private readonly List<UserSession> _userSessions = new List<UserSession>(); 
        private int _currentPriorityIndex = 0;
        private const double GroupBRate = 0.2;
        private AdsSet _currentAdsSet;


        public DataProcessor()
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

                    case Stage.Prioritets:



                        return GetDataFromPrioritetStage(userId, userData, refsList, sessionId);
                }
            }

            return refsList;
        }

        public void SetSessionResult(Guid sessionId, string clickedLink)
        {
            lock (_stupidLock)
            {
                UserSession session = _userSessions.SingleOrDefault(userSession => userSession.Id == sessionId);

                if (session == null)
                {
                    Trace.TraceWarning("Session with id: {0} not found in the list of started sessions.", sessionId);
                    return;
                }

                    if (session.InBGroup)
                {
                    _currentAdsSet.ClickOnRef(clickedLink, session.ShowedRefs);
                    if (_currentAdsSet.FinishedSessionsInBGroup > _currentAdsSet.GetTargetSamplesForBGroup())
                    {
                        _currentAdsSet.BaseRefsList = _currentAdsSet.GetOptimizedRefList();
                    }
                }
            }
        }

        private void BaseStageObservationFinished()
        {
            
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
                 _currentAdsSet,
                 refsList);

            _userSessions.Add(curSession);

            return refsList;
        }

        private List<string> GetDataFromPrioritetStage(Guid userId, Dictionary<string, string> userData,
            List<string> refsList, Guid sessionId)
        {
            UserSession curSession;
            string sex = userData["sex"];
            int curIndex = 0;

            if (sex == "F")
            {
                curIndex = 1;
            }

            if (ShouldAddUserToBGroup())
            {
                // B group
                string firstRef = _prioritetStageABtests[curIndex].GetNextRandomHrefToTest();
                curSession = new UserSession(sessionId, Stage.Base, userId, userData, firstRef, _currentPriorityIndex);
                refsList.Remove(firstRef);
                refsList.Insert(0, firstRef);
            }
            else
            {
                curSession = new UserSession(sessionId, Stage.Base, userId, userData);
            }

            _userSessions.Add(curSession);

            if (_prioritetStageABtests[curIndex].IsComplited)
            {
                return _prioritetStageABtests[curIndex].GetOrderedHrefs();
            }

            return refsList;
        }

        /// <summary>
        /// Should we add to group with random permutations where we teach
        /// </summary>
        /// <returns></returns>
        private bool ShouldAddUserToBGroup()
        {
            double curVal = _rnd.NextDouble();
            return curVal > GroupBRate;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        private bool DoesWeKnowTheUser(Guid userId)
        {
            const int knownLimit = 100;

            return (_userPreferenceses.Single(userPreferences => userPreferences.UserId == userId).Preferences.Count >
                    knownLimit);
        }
    }
}
