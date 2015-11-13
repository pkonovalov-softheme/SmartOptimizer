using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SmartOptimizer
{
    public class AdsSet
    {
        private readonly List<string> _baseRefsList;
        //private readonly List<string> _sortedList;
        //private readonly long[] _clicksCount;

        Dictionary<string, long> _refCliksStats = new Dictionary<string, long>();
        private long _totalCliksCount = 0;
        private long _totalPermutationsCount = 0;

        public AdsSet(List<string> baseRefsList)
        {
            _baseRefsList = baseRefsList;

            List<string> sortedBaseRefsList = new List<string>(baseRefsList);
            sortedBaseRefsList.Sort();

            _refCliksStats = new Dictionary<string, long>(sortedBaseRefsList.Count);
            foreach (string curRef in sortedBaseRefsList)
            {
                _refCliksStats.Add(curRef, 0);
            }

            //_clicksCount = new long[baseRefsList.Count];
        }

        #region EqualityOpers
        protected bool Equals(AdsSet other)
        {
            return ReferenceEquals(Sorted, other.Sorted) || this.Sorted.SequenceEqual(other.Sorted);
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
            return (_sortedList != null ? _sortedList.GetHashCode() : 0);
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

        public IEnumerable<string> Sorted
        {
            get { return _refCliksStats.Keys; }
        }

        public List<string> BaseRefsList
        {
            get { return _baseRefsList; }
        }

        public void NextRandomSet()
        {
            _baseRefsList.Shuffle();
            _totalPermutationsCount++;
        }

        public void ClickOnRef(string clickedRef)
        {
            if (!_refCliksStats.ContainsKey(clickedRef))
            {
                Trace.TraceWarning("Clicked ref {0} does not exists in the refs dictinary.", clickedRef);
                return;
            }

            _refCliksStats[clickedRef]++;
        } 
    }
}