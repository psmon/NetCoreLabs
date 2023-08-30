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
                    Console.WriteLine($"[{DateTime.Now}] [{Thread.CurrentThread.Name}-{Thread.CurrentThread.ManagedThreadId}]" +
                        $" : {msg}  ");

                    await Task.Delay(10);
                    
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
                            Console.WriteLine($"[{DateTime.Now}] [{Self.Path}]" +
                                $" : {msg}  ");

                            if (testProbe != null)
                            {
                                testProbe.Tell("world");
                            }
                            else
                            {
                                Sender.Tell("world");
                            }
                            
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
