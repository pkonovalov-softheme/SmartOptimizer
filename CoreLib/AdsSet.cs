using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SmartOptimizer
{
    public class AdsSet
    {
        private const int TargetSamplesPerAd = 400; //ToDo: calculate
        private readonly List<string> _sortedRefsList;

        readonly Dictionary<string, long> _refCliksStats = new Dictionary<string, long>();
        private long _finishedSessionsInBGroup;
        private readonly Random _rnd = new Random();
        private readonly long _targetSamplesForBGroup;

        public AdsSet(List<string> baseRefsList)
        {
            BaseRefsList = baseRefsList;

            _sortedRefsList = new List<string>(baseRefsList);
            _sortedRefsList.Sort();

            foreach (string curRef in _sortedRefsList)
            {
                _refCliksStats.Add(curRef, 0);
            }

            _targetSamplesForBGroup = TargetSamplesPerAd * _sortedRefsList.Count;
        }

        #region EqualityOpers
        protected bool Equals(AdsSet other)
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
            IEnumerable<string> result = _sortedRefsList.OrderBy(x => _rnd.Next());
            return result;
        }

        public void ClickOnRef(string clickedRef)
        {
            if (!_refCliksStats.ContainsKey(clickedRef))
            {
                Trace.TraceWarning("Clicked ref {0} does not exists in the AdsSet refs dictinary.", clickedRef);
                return;
            }

            _refCliksStats[clickedRef]++;
            _finishedSessionsInBGroup++;

            if (IsRefCompetitionFinished())
            {
                FinishCompetition();
            }
        }

        private bool IsRefCompetitionFinished()
        {
            return _finishedSessionsInBGroup > _targetSamplesForBGroup;
        }

        private void FinishCompetition()
        {
            List<string> optimizedRefList = _refCliksStats.OrderByDescending(de => de.Value).Select(de => de.Key).ToList();
            BaseRefsList = optimizedRefList;
            _finishedSessionsInBGroup = 0;
            foreach (var key in _refCliksStats.Keys.ToList())
            {
                _refCliksStats[key] = 0;
            }
        }
    }
}