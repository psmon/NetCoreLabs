using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;

namespace ActorLib.Actors.Test
{
    public class IssueTrackerMailbox : UnboundedPriorityMailbox
    {
        public IssueTrackerMailbox(Settings settings, Config config) : base(settings, config)
        {            
        }

        protected override int PriorityGenerator(object message)
        {
            var issue = message as Issue;

            if (issue != null)
            {
                if (issue.IsSecurityFlaw)
                    return 0;

                if (issue.IsBug)
                    return 1;
            }

            return 2;
        }
    }

    public class Issue
    {
        public bool IsSecurityFlaw { get; set; }

        public bool IsBug { get; set; }
    }

    public class ExpectIssue
    {
    }

    public class NoIssue
    {
    }

}
