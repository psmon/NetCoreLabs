using Akka.Actor;
using Akka.Event;

namespace ActorLib.Actor.Tools.Throttle;

// Timer Base Throttle
public class ThrottleTimerActor : ReceiveActor
{
    private readonly ILoggingAdapter logger = Context.GetLogger();

    private IActorRef? consumer;

    private Queue<object> eventQueue = new Queue<object>();

    private DateTime lastExecuteDt;


    public ThrottleTimerActor(int element, int second, int maxBust)
    {
        lastExecuteDt = DateTime.Now;

        Context.System
            .Scheduler
            .ScheduleTellRepeatedly(TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(1),
                Self, new Flush(), ActorRefs.NoSender);

#pragma warning disable CS1998
        Receive<SetTarget>(target =>
        {
            consumer = target.Ref;
        });

        Receive<EventCmd>(message =>
        {
            if (eventQueue.Count > maxBust)
            {
                logger.Warning($"ThrottleActor MaxBust : {eventQueue.Count}/{maxBust}");
            }
                
            eventQueue.Enqueue(message);

        });

        Receive<Flush>(message =>
        {
            if (eventQueue.Count > 0)
            {
                var eventData = eventQueue.Dequeue();

                if (consumer != null)
                    consumer.Tell(eventData);
                lastExecuteDt = DateTime.Now;
            }
        });
    }
}
