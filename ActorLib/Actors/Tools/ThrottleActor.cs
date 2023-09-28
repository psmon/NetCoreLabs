using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace ActorLib.Actors.Tools
{

    public class ThrottleActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private IActorRef? consumer;

        private IActorRef _throttler;

        private readonly IMaterializer _materializer;

        private int _processCouuntPerSec;

        public ThrottleActor(int processCouuntPerSec)
        {
            _materializer = Context.Materializer();

            _processCouuntPerSec = processCouuntPerSec;

            _throttler =
                Source.ActorRef<object>(1000, OverflowStrategy.DropNew)
                      .Throttle(_processCouuntPerSec, TimeSpan.FromSeconds(1), _processCouuntPerSec, ThrottleMode.Shaping)
                      .To(Sink.ActorRef<object>(Self, NotUsed.Instance))
                      .Run(_materializer);

            ReceiveAsync<SetTarget>(async target =>
            {
                consumer = target.Ref;
            });


            ReceiveAsync<TPSInfoReq>(async target =>
            {
                Sender.Tell(_processCouuntPerSec);
            });

            ReceiveAsync<ChangeTPS>(async msg =>
            {
                var oldThrottler = _throttler;

                _processCouuntPerSec = msg.processCouuntPerSec;

                _throttler =
                    Source.ActorRef<object>(1000, OverflowStrategy.DropNew)
                            .Throttle(_processCouuntPerSec, TimeSpan.FromSeconds(1), _processCouuntPerSec, ThrottleMode.Shaping)
                            .To(Sink.ActorRef<object>(Self, NotUsed.Instance))
                            .Run(_materializer);

                oldThrottler.Tell(PoisonPill.Instance);

            });


            ReceiveAsync<TodoQueue>(async msg =>
            {
                _throttler.Tell(new Todo()
                {
                    Id = msg.Todo.Id,
                    Title = msg.Todo.Title
                });
            });

            ReceiveAsync<Todo>(async msg =>
            {
                logger.Info($"{msg.Id} - {msg.Title}");
                // TODO Something

                if (consumer != null)
                {
                    consumer.Tell(msg);
                }
            });
        }
    }
}