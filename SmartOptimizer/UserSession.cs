using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOptimizer
{
    public class UserSession
    {
        public int CurrentTestedIndex { get; private set; }
        public Stage Stage { get; private set; }
        public Guid Id { get; private set; }
        public string FirstHref { get; private set; }

        public Dictionary<string, string> UserData { get; private set; }

        public Guid UserId { get; private set; }

        public bool IsDefault
        {
            get
            {
                return FirstHref == null;
            }
        }
        

        public UserSession(Guid id, Stage stage, Guid userId, Dictionary<string, string> userData, string firstHref = null, int currentTestedIndex = 0)
        {
            CurrentTestedIndex = currentTestedIndex;
            UserId = userId;
            UserData = userData;
            Stage = stage;
            this.Id = id;
            this.FirstHref = firstHref;
        }
    }
}