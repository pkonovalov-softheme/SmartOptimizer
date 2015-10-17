using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private const int TargetExampesCount = 1000; // ToDo: calculate it

        private readonly Stage _currentStage;
        private List<UserPreferences> _userPreferenceses;
        private long _userSessionCount = 0;
        private readonly Stopwatch _startWatch = Stopwatch.StartNew();
        private readonly object _stupidLock = new object();
        private readonly Random _rnd = new Random();
        private readonly List<UserSession> _userSessions = new List<UserSession>(); 
        private ABtest _baseStageABtest = null;
        private List<ABtest> _prioritetStageABtests = null;
        private int _currentPriorityIndex = 0;

        private long _intemsTestedInA = 0;
        private long _intemsClickedInA = 0;

        public DataProcessor()
        {
            _currentStage = Stage.Base;
            _userPreferenceses = new List<UserPreferences>();
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

                if (DoesWeKnowTheUser(userId))
                {
                    string[] refClasses = new string[refsList.Count];

                    List<UserPreference> orderedUserPreferences =
                        _userPreferenceses.Single(userPreferencese => userPreferencese.UserId == userId)
                            .Preferences.OrderBy(pr => pr.VisitCount)
                            .ToList();

                    foreach (var curRef in refsList)
                    {
                        string curClass = GetTextClass(curRef);
                        int targetIndex =
                            orderedUserPreferences.IndexOf(
                                orderedUserPreferences.Single(pref => pref.RefClassId == curClass));
                        refClasses[targetIndex] = curClass;
                    }

                    return refClasses.ToList();
                }
                else
                {
                    switch (_currentStage)
                    {
                        case Stage.Base:

                            if (_baseStageABtest == null)
                            {
                                _baseStageABtest = new ABtest(refsList, TargetExampesCount);
                            }

                            return GetDataFromBaseStage(userId, userData, refsList, sessionId);

                        case Stage.Prioritets:

                            if (_prioritetStageABtests == null)
                            {
                                _prioritetStageABtests = new List<ABtest>();

                                //ToDo: correct
                                _prioritetStageABtests.Add(new ABtest(refsList, TargetExampesCount));
                                _prioritetStageABtests.Add(new ABtest(refsList, TargetExampesCount));

                            }

                            return GetDataFromPrioritetStage(userId, userData, refsList, sessionId);
                    }
                }
            }

            return refsList;
        }

        public void SetSessionResult(Guid sessionId, string finalNews)
        {
            UserSession session = _userSessions.Single(userSession => userSession.Id == sessionId);

            if (session.IsDefault)
            {
                if (finalNews != null)
                {
                    _intemsClickedInA++;
                }

                _intemsTestedInA++;
            }
            else
            {
                if (session.Stage == Stage.Base)
                {
                    if (finalNews != null)
                    {
                        _baseStageABtest.RefsClicksCountInB[session.FirstHref]++;
                    }

                    _baseStageABtest.RefsSessionCountInB[session.FirstHref]++;
                }

                if (session.Stage == Stage.Prioritets)
                {
                    //Dictionary<string, string> userData = 
                    string sex = session.UserData["sex"];
                    int curIndex = 0;

                    if (sex == "F")
                    {
                        curIndex = 1;
                    }
 

                    if (finalNews != null)
                    {
                        _prioritetStageABtests[curIndex].RefsClicksCountInB[session.FirstHref]++;
                    }

                    _prioritetStageABtests[curIndex].RefsSessionCountInB[session.FirstHref]++;
                }

            }

            if (finalNews != null)
            {
                Guid userId = _userSessions.First(us => us.Id == sessionId).UserId;

                string newsClass = GetTextClass(finalNews);
                _userPreferenceses.Single(pref => pref.UserId == userId).Preferences
                    .Single(pref => pref.RefClassId == newsClass)
                    .VisitCount++;
            }
        }

        private List<string> GetDataFromBaseStage(Guid userId, Dictionary<string, string> userData, List<string> refsList, Guid sessionId)
        {
            UserSession curSession;

            if (ShouldAddUserToBGroup())
            {
                // B group
                string firstRef = _baseStageABtest.GetNextRandomHrefToTest();
                curSession = new UserSession(sessionId, Stage.Base, userId, userData, firstRef);
                refsList.Remove(firstRef);
                refsList.Insert(0, firstRef);
            }
            else
            {
                curSession = new UserSession(sessionId, Stage.Base, userId, userData);
            }

            _userSessions.Add(curSession);

            if (_baseStageABtest.IsComplited)
            {
                return _baseStageABtest.GetOrderedHrefs();
            }

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

        private bool ShouldAddUserToBGroup()
        {
            TimeSpan appropriateTestTime = TimeSpan.FromMinutes(5);

            double sessionsRate = (double)_userSessionCount / _startWatch.Elapsed.Milliseconds;

            // appropriateTestTimeMs * sessionsRate * targetProbabili = targetClickCount
            double targetProbability = TargetExampesCount / (TargetExampesCount * appropriateTestTime.TotalSeconds * sessionsRate);

            Debug.Assert(targetProbability > 0 && targetProbability < 1, "targetProbability should be between 0 and 1");

            double nextval = _rnd.Next();

            return nextval < targetProbability;
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

        private void GetUserType(string userId, Dictionary<string, string> userData)
        {
            
        }

        private bool DoesWeKnowTheUser(Guid userId)
        {
            const int knownLimit = 100;

            return (_userPreferenceses.Single(userPreferences => userPreferences.UserId == userId).Preferences.Count >
                    knownLimit);
        }
    }
}
