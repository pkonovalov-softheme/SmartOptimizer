using System;
using CoreLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreTests
{
    [TestClass]
    public class WebSampleSizeCalculatorTests
    {
        [TestMethod]
        public void SampleSize_Test_1()
        {
            WebSampleSizeCalculator calc = new WebSampleSizeCalculator(0.118, 0.075, 0.95, 0.8, 5, 10000, false);
            calc.CalculateSamples();
            Assert.AreEqual(calc.SamplesPerTest, 21529);
        }

        [TestMethod]
        public void SampleSize_Test_2()
        {
            WebSampleSizeCalculator calc = new WebSampleSizeCalculator(0.182, 0.063, 0.90, 0.83, 5, 10000, false);
            calc.CalculateSamples();
            Assert.AreEqual(calc.SamplesPerTest, 15666);
        }

        [TestMethod]
        public void SampleSize_Test_BonferroniCorrection()
        {
            WebSampleSizeCalculator calc = new WebSampleSizeCalculator(0.182, 0.063, 0.90, 0.83, 5, 10000, true);
            calc.CalculateSamples();
            Assert.AreEqual(calc.SamplesPerTest, 23683);
        }

        [TestMethod]
        public void SampleSize_Test_AbsoluteDifference()
        {
            WebSampleSizeCalculator calc = new WebSampleSizeCalculator(0.182, 0.063, 0.90, 0.83, 5, 10000, false);
            calc.CalculateSamples();
            double delta = calc.ConvertionRateAbsoluteDifference - 0.0115;
            Assert.IsTrue(delta < 0.001);
        }

        [TestMethod]
        public void SampleSize_Test_ExpectedConvertionRate()
        {
            WebSampleSizeCalculator calc = new WebSampleSizeCalculator(0.182, 0.063, 0.90, 0.83, 5, 10000, false);
            calc.CalculateSamples();
            double delta = calc.ExpectedConvertionRate - 0.1935;
            Assert.IsTrue(delta < 0.001);
        }
    }
}
