using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CoreLib
{
    public sealed class AdsSet
    {
        private const int TargetSamplesPerAd = 8000; //ToDo: calculate // 8000 - 2%; 1000 -10% of erorrs; 800 - 15%; 400 - 20%; 
        private readonly List<string> _sortedRefsList;

        readonly Lazy<Dictionary<string, long>> _refCliksStats;
        private long _finishedSessionsInBGroup;
        private static readonly Random Rnd = new Random();
        private readonly long _targetSamplesForBGroup;

        public AdsSet(List<string> baseRefsList)
        {
            BaseRefsList = baseRefsList;

            _sortedRefsList = new List<string>(baseRefsList);
            _sortedRefsList.Sort();
            _refCliksStats = new Lazy<Dictionary<string, long>>(() => { return _sortedRefsList.ToDictionary(x => x, x => 0L); });
            _targetSamplesForBGroup = TargetSamplesPerAd * _sortedRefsList.Count;
        }

        #region EqualityOpers
        bool Equals(AdsSet other)
        {
            return ReferenceEquals(SortedRefs, other.SortedRefs) || this.SortedRefs.SequenceEqual(other.SortedRefs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AdsSet)obj);
        }

        public override int GetHashCode()
        {
            return (_sortedRefsList != null ? _sortedRefsList.GetHashCode() : 0);
        }

        public static bool operator ==(AdsSet left, AdsSet right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AdsSet left, AdsSet right)
        {
            return !Equals(left, right);
        }
        #endregion

        public List<string> SortedRefs
        {
            get { return _sortedRefsList; }
        }

        public List<string> BaseRefsList { get; set; }

        public IEnumerable<string> NextRandomShuffle()
        {
            IEnumerable<string> result = _sortedRefsList.OrderBy(x => Rnd.Next());
            return result;
        }

        // Returns whether the competition is completeв
        public bool ClickOnRef(string clickedRef)
        {
            if (!_refCliksStats.Value.ContainsKey(clickedRef))
            {
                Trace.TraceWarning("Clicked ref {0} does not exists in the AdsSet refs dictinary.", clickedRef);
                return false;
            }

            _refCliksStats.Value[clickedRef]++;
            _finishedSessionsInBGroup++;

            if (IsRefCompetitionFinished())
            {
                FinishCompetition();
                return true;
            }

            return false;
        }

        private bool IsRefCompetitionFinished()
        {
            return _finishedSessionsInBGroup > _targetSamplesForBGroup;
        }

        private void FinishCompetition()
        {
            List<string> optimizedRefList = _refCliksStats.Value.OrderByDescending(de => de.Value).Select(de => de.Key).ToList();
            BaseRefsList = optimizedRefList;
            _finishedSessionsInBGroup = 0;
            foreach (var key in _refCliksStats.Value.Keys.ToList())
            {
                _refCliksStats.Value[key] = 0;
            }
        }
    }
}