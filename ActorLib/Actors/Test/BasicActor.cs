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
                logger.Info($"{msg} 를 전송받았습니다.");

                if (msg.Contains("slowCommand"))
                {                    
                    Console.WriteLine($"ReceiveAsync : {msg} {Thread.CurrentThread.Name}-{Thread.CurrentThread.ManagedThreadId} ");

                    await Task.Delay(1000);
                    
                    if (testProbe != null)
                    {
                        testProbe.Tell(msg);
                    }
                }
                else
                {
                    if (Sender != null)
                    {
                        if (msg == "hello")
                        {
                            Sender.Tell("world");
                        }
                        else
                        {
                            Sender.Tell("ok");
                        }
                    }
                }                                
            });
        }
    }
}
