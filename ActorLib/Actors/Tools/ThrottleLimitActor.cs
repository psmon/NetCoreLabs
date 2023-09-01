using Akka.Actor;
using Akka.Event;

namespace ActorLib.Actors.Tools
{
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

    public class ThrottleLimitActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private IActorRef? consumer;

        private List<object> eventQueue = new List<object>();

        private DateTime lastExecuteDt;


        public ThrottleLimitActor(int element, int second, int maxBust)
        {
            lastExecuteDt = DateTime.Now;

#pragma warning disable CS1998
            ReceiveAsync<SetTarget>(async target =>
            {
                consumer = target.Ref;
            });

            ReceiveAsync<EventCmd>(async message =>
            {
                if (eventQueue.Count > maxBust)
                {
                    logger.Warning($"ThrottleActor MaxBust : {eventQueue.Count}/{maxBust}");
                }
                eventQueue.Add(message);
            });

            ReceiveAsync<Flush>(async message =>
            {
                TimeSpan timeSpan = DateTime.Now - lastExecuteDt;

                if (eventQueue.Count > 0 && timeSpan.TotalMilliseconds > 1)
                {
                    eventQueue.ForEach(obj =>
                    {
                        if (consumer != null)
                            consumer.Tell(obj);

                        Task.Delay(TimeSpan.FromSeconds(second)).Wait();
                    });

                    eventQueue.Clear();
                    lastExecuteDt = DateTime.Now;
                }
            });
        }
    }
}
