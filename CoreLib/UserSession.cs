using System;
using System.Collections.Generic;
using SmartOptimizer;

namespace CoreLib
{
    public class UserSession
    {
        public string Id { get; private set; }

        public Dictionary<string, string> UserData { get; private set; }

        public Guid UserId { get; private set; }

        /// <summary>
        /// Is this user session inside group where we are randomly testing efficiency of ads permutations
        /// </summary>
        public bool InBGroup { get; private set; }

        public AdsSet SessionAdsSet { get; private set; }

        public DateTime CreationTime { get; private set; }


        public UserSession(string id, Guid userId, 
            Dictionary<string, string> userData, 
            bool inBGroup,
            AdsSet sessionAdsSet)
        {
            UserId = userId;
            UserData = userData;
            Id = id;
            InBGroup = inBGroup;
            SessionAdsSet = sessionAdsSet;
            CreationTime = DateTime.Now;
        }
    }
}