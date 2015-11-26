using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulator
{
    class Ad
    {
        private readonly string _currentRef;
        private readonly double _clickProbability;

        public Ad(string currentRef, double clickProbability)
        {
            _currentRef = currentRef;
            _clickProbability = clickProbability;
        }

        public string CurrentRef
        {
            get { return _currentRef; }
        }

        public double ClickProbability
        {
            get { return _clickProbability; }
        }
    }
}
