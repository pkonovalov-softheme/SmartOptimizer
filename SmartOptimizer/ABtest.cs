using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace SmartOptimizer
{
    public class ABtest
    {
        Dictionary<string, long> _refsClicksCountInB = new Dictionary<string, long>();
        Dictionary<string, long> _refsSessionCountInB = new Dictionary<string, long>();

        private readonly long _refTestCountInB;

        private readonly Random _rnd = new Random();

        public bool IsComplited
        {
            get
            {
                bool complited = _refsClicksCountInB.Values.All(sessionsCount => sessionsCount > _refTestCountInB);
                return complited;
            }
        }


        public Dictionary<string, long> RefsClicksCountInB
        {
            get { return _refsClicksCountInB; }
            set { _refsClicksCountInB = value; }
        }

        public Dictionary<string, long> RefsSessionCountInB
        {
            get { return _refsSessionCountInB; }
            set { _refsSessionCountInB = value; }
        }

        public ABtest(List<string> hrefs, long refTestCountInB)
        {
            this._refTestCountInB = refTestCountInB;

            foreach (string href in hrefs)
            {
                _refsClicksCountInB.Add(href, 0);
                _refsSessionCountInB.Add(href, 0);
            }
        }

        public string GetNextRandomHrefToTest()
        {
            int targetIndex = _rnd.Next(_refsClicksCountInB.Count);
            string ratgetHref =  _refsSessionCountInB.Keys.ToList()[targetIndex];
            return ratgetHref;
        }

        public List<string> GetOrderedHrefs()
        {
            Dictionary <string, double> clickRate = new Dictionary<string, double>();
            foreach (var kvp in _refsSessionCountInB)
            {
                string curRef = kvp.Key;
                long sessionCount = kvp.Value;
                long clickCount = _refsClicksCountInB[curRef];
                double rate = (double)sessionCount/clickCount;

                clickRate.Add(curRef, rate);
            }

            //Dictionary<string, double> clickRate2 = clickRate.OrderBy(kvp => kvp.Value).ToDictionary();

            var sortedDict = from entry in clickRate orderby entry.Value ascending select entry;
            List<string> oderedList = sortedDict.Select(ss => ss.Key).ToList();
            return oderedList;
        }
    }
}