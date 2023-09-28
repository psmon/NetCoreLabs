using Akka.Actor;
using Akka.Event;

namespace ActorLib.Actors.Tools
{
    // Strem을 제어하는 부분은 AkkaStream이 활용된 ThrottleActor 버전을 추천

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
