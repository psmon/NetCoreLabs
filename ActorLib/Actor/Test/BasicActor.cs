using ActorLib.Actor.Tools.Throttle;
using Akka.Actor;
using Akka.Event;

namespace ActorLib.Actor.Test;

public class BasicActor : ReceiveActor
{
    private readonly ILoggingAdapter logger = Context.GetLogger();

    private IActorRef? testProbe;

    public BasicActor()
    {

        Receive<IActorRef>(actorRef =>
        {
            testProbe = actorRef;

            testProbe.Tell("done");
        });

        Receive<Todo>(msg =>
        {
            if (testProbe != null)
            {
                testProbe.Tell(msg);
            }
        });
        
        ReceiveAsync<DelayCommand>(async msg =>
        {
            await Task.Delay(msg.Delay);

            if (testProbe != null)
            {
                testProbe.Tell("world");
            }
            else
            {
                Sender.Tell("world");
            }

        });

        Receive<string>(msg =>
        {
            if (testProbe != null)
            {
                testProbe.Tell("world");
            }
            else
            {
                Sender.Tell("world");
            }
        });

        Receive<MessageCommand>(msg =>
        {
            if (testProbe != null)
            {
                testProbe.Tell(msg.Message);
            }
            else
            {
                Sender.Tell(msg.Message);
            }
        });

        Receive<Issue>(msg =>
        {
            logger.Info($"ReceiveIssue:{msg.ToJsonString()}");

            if (testProbe != null)
            {
                // 우선순위 역전 테스트를 위해 200ms 지연
                Task.Delay(200).Wait();
                testProbe.Tell(msg);                    
            }
            else
            {
                Sender.Tell(msg);
            }
        });

        Receive<RemoteCommand>( msg =>
        {
            logger.Info($"ReceiveRemoteCommand:{msg.Message} Path:{Self.Path}");
        });
    }

    protected override void PreStart()
    {
        // 액터가 생성될 때 실행할 코드
        logger.Info("BasicActor is starting.");
        base.PreStart();
    }

    protected override void PostStop()
    {
        // 액터가 종료될 때 실행할 코드
        logger.Info("BasicActor is stopping.");
        base.PostStop();
    }
}
