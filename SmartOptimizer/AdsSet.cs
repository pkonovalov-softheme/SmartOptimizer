using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOptimizer
{
    public class AdsSet
    {
        private readonly List<string> _baseRefsList;
        private readonly List<string> _sortedList;
        private readonly long[] _clicksCount;
        private long _totalCliksCount = 0;
        private long _totalPermutationsCount = 0;

        public AdsSet(List<string> baseRefsList)
        {
            _baseRefsList = baseRefsList;
            _sortedList = new List<string>(_baseRefsList);
            _clicksCount = new long[baseRefsList.Count];
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

        public List<string> Sorted
        {
            get { return _sortedList; }
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
    }
}