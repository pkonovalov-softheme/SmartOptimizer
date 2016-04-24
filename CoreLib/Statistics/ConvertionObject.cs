using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Statistics
{
    public sealed class ConvertionObject
    {
        public long Views { get; set; }

        public long Clicks { get; set; }

        public long Value { get; set; } 

        public double CurrentClickPerView { get; set; } 

        public double CurrentValuePerView { get; set; } 

        public void NextStage()
        {
            double stageClickPerView = Clicks / (double)Views;
            if (CurrentClickPerView == 0)
            {
                CurrentClickPerView = stageClickPerView;
            }
            else
            {
                CurrentClickPerView = LowPass(stageClickPerView, CurrentClickPerView);
            }

            double stageValuePerView = Value / (double)Views;
            if (CurrentValuePerView == 0)
            {
                CurrentValuePerView = stageValuePerView;
            }
            else
            {
                CurrentValuePerView = LowPass(stageValuePerView, CurrentValuePerView);
            }

            Views = 0;
            Clicks = 0;
            Value = 0;
        }

        // Simple low-pass filter
        private double LowPass(double current, double last)
        {
            return last * (1.0d - GeneralSettings.SmoothingParameter) + current *GeneralSettings.SmoothingParameter;
        }
    }
}
