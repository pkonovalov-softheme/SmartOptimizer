using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib;

namespace Emulator
{
    class BasicTester
    {
        private List<Ad> _correctAdsList;
        private long _failedCount = 0;
        private long _correctCount = 0;
        private bool _shouldExit = false;
        private ThreadSafeRandom _rnd = new ThreadSafeRandom();
        private long _maxAbsProfit = 0;
        private long _realProfit = 0;
        private long _rndProfit = 0;

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
                _rnd = new ThreadSafeRandom();
            }


            //double perc = (double)_failedCount / (_failedCount + _correctCount);
            //Console.WriteLine("Failed: {0}%.", perc * 100);
            // Console.WriteLine("Done in {0} ms.", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Max profit: {0}. Real profit: {1}. Rnd profit: {2}", _maxAbsProfit, _realProfit, _rndProfit);
            Console.WriteLine("Real profit is smaller then max on: {0}%. Rnd profit: {1}%", (-100d * (_maxAbsProfit - _realProfit) / _maxAbsProfit), (-100d * (_maxAbsProfit - _rndProfit) / _maxAbsProfit));
        }

        public void TestProfit()
        {
            ResetState();
            List<Ad> ads = GenerateAds(100, 0, 0.3);
            TestPerfomance(ads);
        }

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
            for (long i = 0; i < TestSettings.ProfitRetestCount; i++)
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
                    if (curRnd > TestSettings.NextPosMoveProbability)
                    {
                        break;
                    }
                }
            }

            return profit;
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
            const long testCount = 1000000;
            Stopwatch stopwatch = Stopwatch.StartNew();

            _correctAdsList = ads.OrderByDescending(x => x.ClickProbability).ToList();

            var stageOptimizer = new StageOptimizer();
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
                    if (curRnd > TestSettings.NextPosMoveProbability)
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

    class FullChaineTester
    {



        private double _breakProbability = 1d /TestSettings.TestsBetweenChanges;

        private readonly ThreadSafeRandom _rnd = new ThreadSafeRandom();
        private readonly List<Ad> _ads;
        private List<Ad> _correctAdsList;
        private long _maxProfit = 0;
        private long _realProfit = 0;
        private long _rndProfit = 0;

        private List<string> _maxRefsList;
        private readonly StageOptimizer _stageOptimizer = new StageOptimizer();
        const int AdsBlockId = 0;

        private List<Ad> GenerateAds(int count, double minValue, double maxValue)
        {
            List<Ad> result = new List<Ad>();

            for (int i = 0; i < count; i++)
            {
                double eff = _rnd.NextDouble(minValue, maxValue);
                string name = i + ".ref";
                result.Add(new Ad(name, eff));
            }

            return result;
        }

        public FullChaineTester()
        {
            _ads = GenerateAds(TestSettings.StartAdsCount, TestSettings.MinAddEf, TestSettings.MaxAddEf);
            _stageOptimizer.AddOrUpdateBlock(AdsBlockId, _ads.Select(ad => ad.CurrentRef).ToList());
            UpdateCorrectAdsList();
        }

        public TestingResult TestFullChaine()
        {
            for (int i = 0; i < TestSettings.TotalTestsCount; i++)
            {
                double val = _rnd.NextDouble();
                if (val < _breakProbability)
                {
                    int indexToChange = _rnd.Next(0, _ads.Count);
                    double eff = _rnd.NextDouble(TestSettings.MinAddEf, TestSettings.MaxAddEf);
                    string name = Guid.NewGuid() + ".ref";
                    _ads[indexToChange] = new Ad(name, eff);
                    _stageOptimizer.AddOrUpdateBlock(AdsBlockId, _ads.Select(ad => ad.CurrentRef).ToList());
                    UpdateCorrectAdsList();
                }

                Guid userId = Guid.NewGuid();
                Guid sessionId = Guid.NewGuid();
                List<string> refsList = _stageOptimizer.GetDataPositions(AdsBlockId, userId.ToString(), sessionId.ToString());

                string clickedLink = GetClickedLink(refsList, sessionId);
                if (clickedLink != null)
                {
                    _realProfit += 1;
                    _stageOptimizer.SetSessionResult(sessionId.ToString(), clickedLink, 1);
                }

                string maxClickedLink = GetClickedLink(_maxRefsList, sessionId);
                if (maxClickedLink != null)
                {
                    _maxProfit += 1;
                }

                string rndClickedLink = GetClickedLink(Utils.Shuffle(_maxRefsList), sessionId);
                if (rndClickedLink != null)
                {
                    _rndProfit += 1;
                }
            }

            //Console.WriteLine("Real: {0}.  Rnd: {1}. Max {2}.", _realProfit, _rndProfit, _maxProfit);

            //double proc = (_maxProfit - _realProfit)/(double)_maxProfit;
            //Console.WriteLine("Real is smaller then max on {0}. ", proc.ToString("0.##%"));

            //double rndProc = (_maxProfit - _rndProfit) / (double)_maxProfit;
            //Console.WriteLine("Rnd is smaller then max on {0}. ", rndProc.ToString("0.##%"));

            return new TestingResult(_maxProfit, _rndProfit, _realProfit);
        }

        private long CalCulateProfitFast(IEnumerable<string> adsList, long runCount)
        {
            double prob = 0;
            foreach (string adStr in adsList)
            {
                Ad ad = _ads.Single(curAd => curAd.CurrentRef == adStr);

                if (prob == 0)
                {
                    prob = ad.ClickProbability;
                }
                else
                {
                    prob += (ad.ClickProbability * TestSettings.NextPosMoveProbability);
                }
            }

            long formulaProfit = (long)(prob * runCount);

            return formulaProfit;
        }

        private string GetClickedLink(IEnumerable<string> refsList, Guid sessionId)
        {
            string clickedRef = null;
            foreach (string curRef in refsList)
            {
                Ad curAd = _ads.Single(ad => ad.CurrentRef == curRef);
                double curRnd = _rnd.NextDouble();
                if (curRnd < curAd.ClickProbability)
                {
                    clickedRef = curAd.CurrentRef;
                    break;
                }

                curRnd = _rnd.NextDouble();
                if (curRnd > TestSettings.NextPosMoveProbability)
                {
                    break;
                }
            }

            return clickedRef;
        }

        private void UpdateCorrectAdsList()
        {
            _correctAdsList = _ads.OrderByDescending(x => x.ClickProbability).ToList();
            _maxRefsList = _correctAdsList.Select(ca => ca.CurrentRef).ToList();
        }
    }
}
