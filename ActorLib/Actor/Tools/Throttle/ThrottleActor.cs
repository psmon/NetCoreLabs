using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace ActorLib.Actor.Tools.Throttle;

// Stream Base Throttle
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

        Receive<SetTarget>(target =>
        {
            consumer = target.Ref;
        });


        Receive<TPSInfoReq>(target =>
        {
            Sender.Tell(_processCouuntPerSec);
        });

        Receive<ChangeTPS>(msg =>
        {
            var oldThrottler = _throttler;

            logger.Info($"Tps Changed {_processCouuntPerSec} -> {msg.processCouuntPerSec}");

            _processCouuntPerSec = msg.processCouuntPerSec;

            _throttler =
                Source.ActorRef<object>(1000, OverflowStrategy.DropNew)
                        .Throttle(_processCouuntPerSec, TimeSpan.FromSeconds(1), _processCouuntPerSec, ThrottleMode.Shaping)
                        .To(Sink.ActorRef<object>(Self, NotUsed.Instance))
                        .Run(_materializer);

            //oldThrottler.Tell(PoisonPill.Instance);

        });


        Receive<TodoQueue>(msg =>
        {
            _throttler.Tell(new Todo()
            {
                Id = msg.Todo.Id,
                Title = msg.Todo.Title
            });
        });

        Receive<Todo>(msg =>
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