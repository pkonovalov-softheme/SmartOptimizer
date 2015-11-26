using System;
using System.Collections.Generic;
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
            const long testCount = 100000;
            const double nextPosMoveProbability = 0.9;

            Random rnd = new Random();

            List<Ad> ads = new List<Ad>()
            {
                new Ad("1.ref", 0.2), new Ad("2.ref", 0.11), new Ad("3.ref", 0.32), new Ad("4.ref", 0.21), new Ad("5.ref", 0.43), new Ad("6.ref", 0.06), 
                new Ad("7.ref", 0.25), new Ad("8.ref", 0.27), new Ad("9.ref", 0.19), new Ad("10.ref", 0.35)
            };

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
                    double curRnd = rnd.Next();
                    if (curRnd < curAd.ClickProbability)
                    {
                        clickedRef = curAd.CurrentRef;
                        break;
                    }

                    if (rnd.Next() > nextPosMoveProbability)
                    {
                        break;
                    }
                }
            }

            List<string> finalRefsList = stageOptimizer.GetDataPositions(Guid.NewGuid(),
                    new Dictionary<string, string>(),
                    ads.Select(ad => ad.CurrentRef).ToList(),
                    Guid.NewGuid());
        }
    }
}
