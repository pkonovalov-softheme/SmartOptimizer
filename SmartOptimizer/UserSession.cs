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

        public IList<string> ShowedStartRefs { get; set; }


        public UserSession(Guid id, Guid userId, Dictionary<string, string> userData, bool inBGroup, IList<string> showedStartRefs)
        {
            UserId = userId;
            UserData = userData;
            Id = id;
            InBGroup = inBGroup;
            ShowedStartRefs = showedStartRefs;
        }
    }
}