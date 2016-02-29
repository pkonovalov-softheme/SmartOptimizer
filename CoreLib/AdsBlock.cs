using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public delegate void CompetitionFinishedEventHandler(object sender, EventArgs e);

    public class CompetitionFinishedEventArgs : EventArgs
    {
        private readonly AdsBlock _adsBlock;

        public CompetitionFinishedEventArgs(AdsBlock adsBlock)
        {
            _adsBlock = adsBlock;
        }

        public AdsBlock AdsBlock
        {
            get { return _adsBlock; }
        }
    }

    public class AdsBlock
    {
        private const int TargetSamplesPerAd = 2000; //ToDo: calculate // 8000 - 2%; 1000 -10% of erorrs; 800 - 15%; 400 - 20%; 

        private readonly int _blockId;
        private List<string> _baseRefsList;
        private List<string> _prevRefsList;
        private readonly long _targetSamplesForBGroup;
        private static readonly Random Rnd = new Random();
        readonly Dictionary<string, AdStats> _refCliksStats;
        private static long _missedSessions = 0;
        private long _startedSessionsInBGroup;

        public event CompetitionFinishedEventHandler CompetitionFinished;

        protected virtual void OnCompetitionFinished(EventArgs e)
        {
           var handler = CompetitionFinished;
            if (handler == null)
            {
                return;
            }

            handler(this, new CompetitionFinishedEventArgs(this));
        }

        public AdsBlock(int blockId, List<string> baseRefsList)
        {
            _blockId = blockId;
            _baseRefsList = baseRefsList;
            _targetSamplesForBGroup = TargetSamplesPerAd * baseRefsList.Count;
            _refCliksStats = _baseRefsList.ToDictionary(x => x, x => new AdStats()); 
        }

        public void UpdateAdsBlock(List<string> refsLis)
        {
            _prevRefsList = _baseRefsList;
            _baseRefsList = refsLis;
        }

        public List<string> BaseRefsList
        {
            get { return _baseRefsList; }
        }

        public int BlockId
        {
            get { return _blockId; }
        }

        public IEnumerable<string> NextRandomShuffle()
        {
            IEnumerable<string> result = _baseRefsList.OrderBy(x => Rnd.Next());

            _startedSessionsInBGroup++;
            if (IsRefCompetitionFinished())
            {
                FinishCompetition();
            }

            return result;
        }


        // Returns whether the competition is completeв
        public void ClickOnRef(string clickedRef, int value)
        {
            if (!_refCliksStats.ContainsKey(clickedRef))
            {
                _missedSessions++;
                Trace.TraceWarning("Clicked ref {0} does not exists in the AdsSet refs dictinary.", clickedRef);
                return;
            }

            _refCliksStats[clickedRef].ClicksCount++;
            _refCliksStats[clickedRef].TotalValue += value;
        }

        private bool IsRefCompetitionFinished()
        {
            return _startedSessionsInBGroup > _targetSamplesForBGroup;
        }

        private void FinishCompetition()
        {
            List<string> optimizedRefList = _refCliksStats.OrderByDescending(de => de.Value.TotalValue).Select(de => de.Key).ToList();
            _baseRefsList = optimizedRefList;
            OnCompetitionFinished(EventArgs.Empty);

            _startedSessionsInBGroup = 0;
            foreach (var key in _refCliksStats.Keys.ToList())
            {
                AdStats stat = _refCliksStats[key];
                stat.ClicksCount = 0;
                stat.TotalValue = 0;
            }
        }
    }
}
