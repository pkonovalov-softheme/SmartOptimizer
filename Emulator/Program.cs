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
        static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            const long testCount = 8000000;
            const double nextPosMoveProbability = 0.7;
            long failedCount = 0;
            long correctCount = 0;

            Random rnd = new Random();

            List<Ad> ads = new List<Ad>()
            {
                new Ad("1.ref", 0.10), new Ad("2.ref", 0.11), new Ad("3.ref", 0.12), new Ad("4.ref", 0.13), new Ad("5.ref", 0.14), new Ad("6.ref", 0.15), 
                new Ad("7.ref", 0.16), new Ad("8.ref", 0.17), new Ad("9.ref", 0.18), new Ad("10.ref", 0.19)
            };

            List<Ad> correctAdsList = ads.OrderByDescending(x => x.ClickProbability).ToList();

            StageOptimizer stageOptimizer = new StageOptimizer();
            for (int i = 0; i < testCount; i++)
            {
                Guid userId = Guid.NewGuid();
                Guid sessionId = Guid.NewGuid();
                List<string> refsList = stageOptimizer.GetDataPositions(userId,
                    new Dictionary<string, string>(),
                    ads.Select(ad => ad.CurrentRef).ToList(),
                    sessionId);

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
                    if (stageOptimizer.SetSessionResult(sessionId, clickedRef))
                    {
                        List<string> finalRefsList = stageOptimizer.GetBaseBaseRefsList(ads.Select(ad => ad.CurrentRef).ToList());
                        for (int y = 0; y < finalRefsList.Count; y++)
                        {
                            if (finalRefsList[y] != correctAdsList[y].CurrentRef)
                            {
                                failedCount++;
                                if (Debugger.IsAttached)
                                {
                                    //Debugger.Break();
                                }
                            }
                            else
                            {
                                correctCount++;
                            }
                        }
                    }
                }
            }

            double perc = (double)failedCount / (failedCount + correctCount);
            Console.WriteLine("The result is not incorrect. Failed: {0}%.", perc * 100);
            Console.WriteLine("Done in {0} ms.", stopwatch.ElapsedMilliseconds);
        }
    }
}
