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
            const long testCount = 50000;
            const double nextPosMoveProbability = 0.9;

            Random rnd = new Random();

            List<Ad> ads = new List<Ad>()
            {
                new Ad("1.ref", 0.2), new Ad("2.ref", 0.11), new Ad("3.ref", 0.32), new Ad("4.ref", 0.21), new Ad("5.ref", 0.43), new Ad("6.ref", 0.06), 
                new Ad("7.ref", 0.25), new Ad("8.ref", 0.27), new Ad("9.ref", 0.19), new Ad("10.ref", 0.35)
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
                    stageOptimizer.SetSessionResult(sessionId, clickedRef);
                }
            }

            bool failed = false;
            int failedCount = 0;
            List<string> finalRefsList = stageOptimizer.GetBaseBaseRefsList(ads.Select(ad => ad.CurrentRef).ToList());
            for (int i = 0; i < finalRefsList.Count; i++)
            {
                if (finalRefsList[i] != correctAdsList[i].CurrentRef)
                {
                    failed = true;
                    failedCount++;
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                }
            }

            if (!failed)
            {
                Console.WriteLine("Correct result!");
            }
            else
            {
                Console.WriteLine("The result is not incorrect. Failed {0} from {1} times.", failedCount, finalRefsList.Count);
            }
        }
    }
}
