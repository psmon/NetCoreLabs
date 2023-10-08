using ActorLib.Actors.Tools;

using Akka.Actor;
using Akka.Event;

using NLog;

namespace ActorLib.Actors.Test
{
    public class BasicActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private IActorRef testProbe;

        public BasicActor()
        {

            ReceiveAsync<IActorRef>(async actorRef =>
            {
                testProbe = actorRef;

                testProbe.Tell("done");
            });

            ReceiveAsync<Todo>(async msg =>
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

            ReceiveAsync<string>(async msg =>
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
            
            ReceiveAsync<MessageCommand>(async msg =>
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

            ReceiveAsync<RemoteCommand>(async msg =>
            {
                logger.Info($"ReceiveRemoteCommand:{msg.Message} Path:{Self.Path}");
            });
        }
    }
}
