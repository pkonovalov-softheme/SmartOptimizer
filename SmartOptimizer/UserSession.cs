using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOptimizer
{
    public class UserSession
    {
        public Guid Id { get; private set; }

        public Dictionary<string, string> UserData { get; private set; }

        public Guid UserId { get; private set; }

        /// <summary>
        /// Is this user session inside group where we are randomly testing efficiency of ads permutations
        /// </summary>
        public bool InBGroup { get; private set; }

        public AdsSet SessionAdsSet { get; private set; }

        public List<string> ShowedRefs
        {
            get { return _showedRefs; }
        }

        //private string _clickedRef;
        private List<string> _showedRefs; 


        public UserSession(Guid id, Guid userId, 
            Dictionary<string, string> userData, 
            bool inBGroup,
            AdsSet sessionAdsSet,
            List<string> showedRefs)
        {
            UserId = userId;
            UserData = userData;
            Id = id;
            InBGroup = inBGroup;
            SessionAdsSet = sessionAdsSet;
        }

        /// <summary>
        /// Sets session result - sets the ref that was clicked
        /// </summary>
        /// <param name="clickedRef"></param>
        public void SetSessionResult(string clickedRef)
        {
            _clickedRef = clickedRef;
        }
    }
}