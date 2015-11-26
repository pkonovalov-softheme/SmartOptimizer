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

        //readonly long[] _clikedPositions;
        //private Dictionary<string, long[]> _showStat = new Dictionary<string, long[]>();
        //private readonly long[] _clicksCount;

        Dictionary<string, long> _refCliksStats = new Dictionary<string, long>();
        public long FinishedSessionsInBGroup { get; private set; } = 0;
        private long _totalPermutationsCount = 0;
        private Random _rnd = new Random();

        public AdsSet(List<string> baseRefsList)
        {
            BaseRefsList = baseRefsList;

            _sortedRefsList = new List<string>(baseRefsList);
            _sortedRefsList.Sort();
            //_clikedPositions = new long[baseRefsList.Count];
            //_showStat = new Dictionary<string, long[]>();
            //List<string> sortedBaseRefsList = new List<string>(baseRefsList);
            //sortedBaseRefsList.Sort();

            //_refCliksStats = new Dictionary<string, long>(sortedBaseRefsList.Count);
            //foreach (string curRef in sortedBaseRefsList)
            //{
            //    _refCliksStats.Add(curRef, 0);
            //}

            //_clicksCount = new long[baseRefsList.Count];
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
            //_sortedRefsList.Shuffle();
            IEnumerable<string> result = _sortedRefsList.OrderBy(x => _rnd.Next());
            _totalPermutationsCount++;
            return result;
        }

        public void ClickOnRef(string clickedRef, List<string> showedRefs)
        {
            if (!_refCliksStats.ContainsKey(clickedRef))
            {
                Trace.TraceWarning("Clicked ref {0} does not exists in the AdsSet refs dictinary.", clickedRef);
                return;
            }

            _refCliksStats[clickedRef]++;
            FinishedSessionsInBGroup++;
            //_clikedPositions[showedRefs.IndexOf(clickedRef)]++;
        }

        public List<string> GetOptimizedRefList()
        {
            return _refCliksStats.OrderBy(de => de.Value).Select(de => de.Key).ToList();
        } 

        public long GetTargetSamplesForBGroup()
        {
            return TargetSamplesPerAd * _sortedRefsList.Count;
        }
    }
}