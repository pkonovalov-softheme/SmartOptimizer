using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Statistics;

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
        public BlockPositionStats BlockPositionStats { get; set; }
        private const int TargetSamplesPerAd = 200; //ToDo: calculate // 8000 - 2%; 1000 -10% of erorrs; 800 - 15%; 400 - 20%; 

        private readonly int _blockId;
        private List<string> _baseRefsList;
        //private List<string> _prevRefsList;
        private readonly long _targetSamplesForBGroup;
        private static readonly Random Rnd = new Random();
        private Dictionary<string, AdStats> _refPerfomanceStats;
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
            BlockPositionStats = new BlockPositionStats(blockId);
            _blockId = blockId;
            _baseRefsList = baseRefsList;
            _targetSamplesForBGroup = TargetSamplesPerAd * baseRefsList.Count;
            _refPerfomanceStats = _baseRefsList.ToDictionary(x => x, x => new AdStats()); 
        }

        public void UpdateBlock(List<string> baseRefsList, Dictionary<string, AdStats> refPerfomanceStats)
        {
            _baseRefsList = baseRefsList;
            _refPerfomanceStats = refPerfomanceStats;
        }

        public List<string> BaseRefsList
        {
            get { return _baseRefsList; }
            set { _baseRefsList = value; }
        }

        public int BlockId
        {
            get { return _blockId; }
        }

        public long TargetSamplesForBGroup
        {
            get { return _targetSamplesForBGroup; }
        }

        public Dictionary<string, AdStats> RefPerfomanceStats
        {
            get { return _refPerfomanceStats; }
        }

        public IEnumerable<string> NextRandomShuffle()
        {
            return NextRandomShuffle(true);
        }

        public IEnumerable<string> NextRandomShuffle(bool checkSessionCount)
        {
            IEnumerable<string> result = _baseRefsList.OrderBy(x => Rnd.Next());

            if (checkSessionCount)
            {
                _startedSessionsInBGroup++;
                if (IsRefCompetitionFinished())
                {
                    FinishCompetition();
                }
            }

            return result;
        }

        // Returns whether the competition is complete
        public void ClickOnRef(string clickedRef, int value)
        {
            if (!_refPerfomanceStats.ContainsKey(clickedRef))
            {
                _missedSessions++;
                Trace.TraceWarning("Clicked ref {0} does not exists in the AdsSet refs dictinary.", clickedRef);
                return;
            }

            _refPerfomanceStats[clickedRef].Clicks++;
            _refPerfomanceStats[clickedRef].Value += value;
        }

        private bool IsRefCompetitionFinished()
        {
            return _startedSessionsInBGroup > _targetSamplesForBGroup;
        }

        private void FinishCompetition()
        {
            OnCompetitionFinished(EventArgs.Empty);

            _startedSessionsInBGroup = 0;
            foreach (var key in _refPerfomanceStats.Keys.ToList())
            {
                AdStats stat = _refPerfomanceStats[key];
                stat.Views = BlockPositionStats.GroupBConvertion.Views;
                stat.ConvObject.NextStage();
            }

            List<string> optimizedRefList = _refPerfomanceStats.OrderByDescending(de => de.Value.ConvObject.CurrentValuePerView).Select(de => de.Key).ToList();
            _baseRefsList = optimizedRefList;

            Trace.TraceInformation("Competition was fineshed. Block id:{0}", BlockId);
        }
    }
}
