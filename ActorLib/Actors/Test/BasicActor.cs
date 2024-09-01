using ActorLib.Actors.Tools;

using Akka.Actor;
using Akka.Event;

using NLog;

namespace ActorLib.Actors.Test
{
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

            Receive<RemoteCommand>( msg =>
            {
                logger.Info($"ReceiveRemoteCommand:{msg.Message} Path:{Self.Path}");
            });
        }
    }
}
