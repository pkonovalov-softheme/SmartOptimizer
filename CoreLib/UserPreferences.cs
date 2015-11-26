using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOptimizer
{
    public class UserPreferences
    {
        public List<UserPreference> Preferences { get; private set; }
        public Guid UserId { get; private set; }

        public UserPreferences(Guid userId)
        {
            Preferences = new List<UserPreference>();
            this.UserId = userId;
        }
    }
}