using System.Diagnostics;

namespace CoreLib.Statistics
{
    public class AdStats
    {
        public AdStats()
        {
            ConvObject = new ConvertionObject();
        }

        public long Views
        {
            get
            {
                return ConvObject.Views;
            }

            set
            {
                ConvObject.Views = value;
            }
        }

        public long Value
        {
            get
            {
                return ConvObject.Value;
            }

            set
            {
                ConvObject.Value = value;
            }
        }

        public long Clicks
        {
            get
            {
                return ConvObject.Clicks;
            }

            set
            {
                ConvObject.Clicks = value;
            }
        }

        public ConvertionObject ConvObject { get; private set; }
    }
}
