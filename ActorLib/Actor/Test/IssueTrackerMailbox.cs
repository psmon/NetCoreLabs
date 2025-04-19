using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;

namespace ActorLib.Actor.Test;

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

public class Issue : IJsonSerializable
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
