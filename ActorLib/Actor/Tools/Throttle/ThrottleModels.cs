using Akka.Actor;

namespace ActorLib.Actor.Tools.Throttle;

public class SetTarget
{
    public SetTarget(IActorRef @ref)
    {
        Ref = @ref;
    }

    public IActorRef Ref { get; }
}

public class EventCmd
{
    public string Message { get; set; }
}

public class Flush { }

public class Todo
{
    public string Id { get; set; }
    public string Title { get; set; }
}

public class TodoQueue
{
    public Todo Todo { get; set; }
}

public class ChangeTPS
{
    public int processCouuntPerSec { get; set; }
}

public class TPSInfoReq
{
}
