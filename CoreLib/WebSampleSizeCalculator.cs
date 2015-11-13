using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Univariate;

namespace CoreLib
{
    public sealed class WebSampleSizeCalculator
    {
        private readonly double _baseConvertionRate;
        private readonly double _targetConvertionRateLift;
        private readonly double _confidenceLevel;
        private readonly double _statisticalPower;
        private readonly int _numberOfTests;
        private readonly long _visitorsPerDay;
        private readonly bool _bonferroniCorrectionEnabled;
        private long _samplesPerTest;


        public WebSampleSizeCalculator(
            double baseConvertionRate,
            double targetConvertionRateLift,
            double confidenceLevel,
            double statisticalPower,
            int numberOfTests,
            long visitorsPerDay,
            bool bonferroniCorrectionEnabled
            )
        {
            _baseConvertionRate = baseConvertionRate;
            _targetConvertionRateLift = targetConvertionRateLift;
            _confidenceLevel = confidenceLevel;
            _statisticalPower = statisticalPower;
            _numberOfTests = numberOfTests;
            _visitorsPerDay = visitorsPerDay;
            _bonferroniCorrectionEnabled = bonferroniCorrectionEnabled;
        }

        /// <summary>
        /// 1 - Alpha
        /// </summary>
        public double ConfidenceLevel
        {
            get
            {
                if (_bonferroniCorrectionEnabled)
                {
                    double correctedConflevel = 1 - ((1 - _confidenceLevel) / (_numberOfTests - 1));
                    return correctedConflevel;
                }

                return _confidenceLevel;
            }
        }

        public double ConfidenceThreshold
        {
            get
            {
                NormalDistribution dist = new NormalDistribution(0, 1);
                double result = dist.InverseDistributionFunction(1 - (1 - ConfidenceLevel) / 2);
                return result;
            }
        }
        

        /// <summary>
        /// 1 - Beta
        /// </summary>
        public double StatisticalPower
        {
            get { return _statisticalPower; }
        }

        public double StatisticalPowerThreshold
        {
            get
            {
                NormalDistribution dist = new NormalDistribution(0, 1);
                double result = dist.InverseDistributionFunction(1 - (1 - StatisticalPower));
                return result;
            }
        }

        public double BaseConvertionRate
        {
            get { return _baseConvertionRate; }
        }

        public double ConvertionRateAbsoluteDifference
        {
            get
            {
                return BaseConvertionRate * TargetConvertionRateLift;
            }
        }

        public double ExpectedConvertionRate
        {
            get
            {
                return BaseConvertionRate *  (1 + TargetConvertionRateLift);
            }
        }

        public double TargetConvertionRateLift
        {
            get { return _targetConvertionRateLift; }
        }

        public long SamplesPerTest
        {
            get { return _samplesPerTest; }
            set { _samplesPerTest = value; }
        }

        public void CalculateSamples()
        {
            double result =
                          BaseConvertionRate *
                          (
                                (1 - BaseConvertionRate) + 
                                (1 + TargetConvertionRateLift) * (1 - (1 + TargetConvertionRateLift) * BaseConvertionRate)
                          ) *
                          Math.Pow(ConfidenceThreshold + StatisticalPowerThreshold, 2) /
                          Math.Pow(BaseConvertionRate - BaseConvertionRate*(1 + TargetConvertionRateLift), 2);

            _samplesPerTest = Convert.ToInt64(Math.Floor(result));
        }
    }
}
