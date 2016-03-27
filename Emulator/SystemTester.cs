using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib;

namespace Emulator
{
    public static class RandomExtensions
    {
        public static double NextDouble(
            this Random random,
            double minValue,
            double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }

    class SystemTester
    {
        private List<Ad> _correctAdsList;
        private long _failedCount = 0;
        private long _correctCount = 0;
        private Random _rnd = new Random();
        private bool _shouldExit = false;
        const double NextPosMoveProbability = 0.6;
        const int ProfitRetestCount = 300000;
        private long _maxAbsProfit = 0;
        private long _realProfit = 0;
        private long _rndProfit = 0;

        private void CompetitionFinished(object sender, EventArgs e)
        {
            lock (_correctAdsList)
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

                _maxAbsProfit = CalculateAbsMaxProfit();

                List<Ad> blocks = ConvertToListOfAds(finalRefsList);

                _realProfit = CalculateAdsBlockMaxProfit(blocks);
                List<Ad> rndAds = ConvertToListOfAds(block.NextRandomShuffle(false));
                _rndProfit = CalculateAdsBlockMaxProfit(rndAds);
                _shouldExit = true;
            }
        }

        private List<Ad> ConvertToListOfAds(IEnumerable<string> refsList)
        {
            List<Ad> blocks = new List<Ad>();
            foreach (string curRef in refsList)
            {
                Ad curAd = _correctAdsList.Single(ad => ad.CurrentRef == curRef);
                blocks.Add(curAd);
            }
            return blocks;
        }

        private long CalculateAbsMaxProfit()
        {
            return CalculateAdsBlockMaxProfit(_correctAdsList);
        }

        private long CalculateAdsBlockMaxProfit(List<Ad> adsList)
        {
            long profit = 0;
            for (long i = 0; i < ProfitRetestCount; i++)
            {
                foreach (Ad add in adsList)
                {
                    double curRnd = _rnd.NextDouble();
                    if (curRnd < add.ClickProbability)
                    {
                        profit += 1;
                        break;
                    }

                    curRnd = _rnd.NextDouble();
                    if (curRnd > NextPosMoveProbability)
                    {
                        break;
                    }
                }
            }

            return profit;
        }

        public void TestGeneralPerfomance()
        {
            ResetState();
            List<Ad> ads = new List<Ad>()
            {
                new Ad("1.ref", 0.10), new Ad("2.ref", 0.11), new Ad("3.ref", 0.12), new Ad("4.ref", 0.13), new Ad("5.ref", 0.14),
                new Ad("6.ref", 0.15), new Ad("7.ref", 0.16), new Ad("8.ref", 0.17), new Ad("9.ref", 0.18), new Ad("10.ref", 0.19)
            };

            TestPerfomance(ads);
        }
        public void TestlPerfomanceWithBigAdsCount()
        {
            const int GenerationCount = 10000;
            //long totalRndProfit = 0;
            //long totalMaxProfit = 0;
            //long totalRealProfit = 0;
            //totalMaxProfit += _maxAbsProfit;
            //_maxAbsProfit = 0;

            //totalRealProfit += _realProfit;
            //_realProfit = 0;

            //totalRndProfit += _rndProfit;
            //_rndProfit = 0;


            ResetState();
            for (int a = 0; a < GenerationCount; a++)
            {
                List<Ad> ads = GenerateAds(200, 0, 0.3);
                TestPerfomance(ads);
                _rnd = new Random();
            }


            //double perc = (double)_failedCount / (_failedCount + _correctCount);
            //Console.WriteLine("Failed: {0}%.", perc * 100);
            // Console.WriteLine("Done in {0} ms.", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Max profit: {0}. Real profit: {1}. Rnd profit: {2}", _maxAbsProfit, _realProfit, _rndProfit);
            Console.WriteLine("Real profit is smaller then max on: {0}%. Rnd profit: {1}%", (-100d * (_maxAbsProfit - _realProfit) / _maxAbsProfit), (-100d * (_maxAbsProfit - _rndProfit) / _maxAbsProfit));
        }

        private void TestPerfomance(List<Ad> ads)
        {
            CalculatePerfomanceInternal(ads);

           // double perc = (double)_failedCount / (_failedCount + _correctCount);
           // Console.WriteLine("Failed: {0}%.", perc * 100);
           //// Console.WriteLine("Done in {0} ms.", stopwatch.ElapsedMilliseconds);
           // Console.WriteLine("Max profit: {0}. Real profit: {1}. Rnd profit: {2}", _maxAbsProfit , _realProfit, _rndProfit);
           // Console.WriteLine("Real profit is smaller then max on: {0}%. Rnd profit: {1}%", (-100d * (_maxAbsProfit - _realProfit) / _maxAbsProfit), (-100d*(_maxAbsProfit - _rndProfit) / _maxAbsProfit));
        }

        private void CalculatePerfomanceInternal(List<Ad> ads)
        {
            int testPassed;
            const int adsBlockId = 0;
            const long testCount = 10000000;
            Stopwatch stopwatch = Stopwatch.StartNew();

            _correctAdsList = ads.OrderByDescending(x => x.ClickProbability).ToList();

            StageOptimizer stageOptimizer = new StageOptimizer();
            List<string> refs = ads.Select(ad => ad.CurrentRef).ToList();
            stageOptimizer.AddOrUpdateBlock(adsBlockId, refs);

            AdsBlock block = stageOptimizer.GeAdsBlock(adsBlockId);
            block.CompetitionFinished += CompetitionFinished;

            for (testPassed = 0; testPassed < testCount; testPassed++)
            {
                Guid userId = Guid.NewGuid();
                string sessionId = Guid.NewGuid().ToString();
                List<string> refsList = stageOptimizer.GetDataPositions(adsBlockId, userId.ToString(), sessionId);

                lock (_correctAdsList)
                {
                    if (_shouldExit)
                    {
                     //   break;
                    }
                }

                string clickedRef = null;

                foreach (string curRef in refsList)
                {
                    Ad curAd = ads.Single(ad => ad.CurrentRef == curRef);
                    double curRnd = _rnd.NextDouble();
                    if (curRnd < curAd.ClickProbability)
                    {
                        clickedRef = curAd.CurrentRef;
                        break;
                    }

                    curRnd = _rnd.NextDouble();
                    if (curRnd > NextPosMoveProbability)
                    {
                        break;
                    }
                }


                if (clickedRef != null)
                {
                    stageOptimizer.SetSessionResult(sessionId, clickedRef, 1);
                }
            }
        }

        public void TestProfit()
        {
            ResetState();
            List<Ad> ads = GenerateAds(100, 0, 0.3);
            TestPerfomance(ads);
        }

        private List<Ad> GenerateAds(int count, double minValue, double maxValue)
        {
            List <Ad> result = new List<Ad>();

            for (int i = 0; i < count; i++)
            {
                double eff = _rnd.NextDouble(minValue, maxValue);
                string name = i + ".ref";
                result.Add(new Ad(name, eff));
            }

            return result;
        } 

        private void ResetState()
        {
           _failedCount = 0;
           _correctCount = 0;
            _correctAdsList?.Clear();
        }
    }
}
