using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib;

namespace Emulator
{
    class Program
    {
        private static List<Ad> _correctAdsList;
        private static long _failedCount = 0;
        private static long _correctCount = 0;

        private static void CompetitionFinished(object sender, EventArgs e)
        {
            CompetitionFinishedEventArgs args = (CompetitionFinishedEventArgs)e;
            AdsBlock block = args.AdsBlock;

            List<string> finalRefsList = block.BaseRefsList;

            for (int y = 0; y < finalRefsList.Count; y++)
            {
                if (finalRefsList[y] != _correctAdsList[y].CurrentRef)
                {
                    _failedCount++;
                    if (Debugger.IsAttached)
                    {
                        //Debugger.Break();
                    }
                }
                else
                {
                    _correctCount++;
                }
            }
        }

        static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            const long testCount = 100000;
            const double nextPosMoveProbability = 0.9;

            Random rnd = new Random();

            List<Ad> ads = new List<Ad>()
            {
                new Ad("1.ref", 0.10), new Ad("2.ref", 0.11), new Ad("3.ref", 0.12), new Ad("4.ref", 0.13), new Ad("5.ref", 0.14),
                new Ad("6.ref", 0.15), new Ad("7.ref", 0.16), new Ad("8.ref", 0.17), new Ad("9.ref", 0.18), new Ad("10.ref", 0.19)
            };

            const int adsBlockId = 0;

            _correctAdsList = ads.OrderByDescending(x => x.ClickProbability).ToList();

            StageOptimizer stageOptimizer = new StageOptimizer();
            List<string> refs = ads.Select(ad => ad.CurrentRef).ToList();
            stageOptimizer.AddOrUpdateBlock(adsBlockId, refs);

            AdsBlock block = stageOptimizer.GeAdsBlock(adsBlockId);
            block.CompetitionFinished += CompetitionFinished;

            for (int i = 0; i < testCount; i++)
            {
                Guid userId = Guid.NewGuid();
                string sessionId = Guid.NewGuid().ToString();
                List<string> refsList = stageOptimizer.GetDataPositions(adsBlockId, userId.ToString(), sessionId);

                string clickedRef = null;

                foreach (string curRef in refsList)
                {
                    Ad curAd = ads.Single(ad => ad.CurrentRef == curRef);
                    double curRnd = rnd.NextDouble();
                    if (curRnd < curAd.ClickProbability)
                    {
                        clickedRef = curAd.CurrentRef;
                        break;
                    }

                    curRnd = rnd.NextDouble();
                    if (curRnd > nextPosMoveProbability)
                    {
                        break;
                    }
                }


                if (clickedRef != null)
                {
                    stageOptimizer.SetSessionResult(sessionId, clickedRef, 1);
                }
            }

            double perc = (double)_failedCount / (_failedCount + _correctCount);
            Console.WriteLine("The result is not incorrect. Failed: {0}%.", perc * 100);
            Console.WriteLine("Done in {0} ms.", stopwatch.ElapsedMilliseconds);
        }
    }
}
