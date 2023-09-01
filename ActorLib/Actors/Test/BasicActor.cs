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
            });

            ReceiveAsync<string>(async msg =>
            {                
                if (msg.Contains("slowCommand"))
                {                                        
                    await Task.Delay(10);
                    
                    if (testProbe != null)
                    {
                        testProbe.Tell(msg);
                    }
                }
                else
                {                    
                    if (testProbe != null)
                    {
                        testProbe.Tell("world");
                    }
                    else
                    {
                        Sender.Tell("world");
                    }
                }                                
            });
        }
    }
}
