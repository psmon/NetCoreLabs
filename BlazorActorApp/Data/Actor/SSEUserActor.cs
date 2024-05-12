using Akka.Actor;

using BlazorActorApp.Data.SSE;

namespace BlazorActorApp.Data.Actor
{
    public class SSEUserActor : ReceiveActor
    {
        private Queue<Notification> notifications = new Queue<Notification>();

        private string IdentyValue { get; set; }

        private IActorRef testProbe;
        public SSEUserActor(string identyValue) 
        {
            IdentyValue = identyValue;

            notifications.Enqueue(new Notification()
            {
                Id = IdentyValue,
                Message = $"[{IdentyValue}] 웰컴메시지... by sse",
                MessageTime = DateTime.Now,
            });

            ReceiveAsync<IActorRef>(async actorRef =>
            {
                testProbe = actorRef;
                testProbe.Tell("done");
            });

            ReceiveAsync<Notification>(async msg =>
            {
                if (msg.IsProcessed == false)
                {
                    notifications.Enqueue(msg);
                }                
              
            });

            ReceiveAsync<HeatBeatNotification>(async msg =>
            {
                if (testProbe != null)
                {
                    testProbe.Tell(new HeatBeatNotification());
                }
                else
                {
                    Sender.Tell(new HeatBeatNotification());
                }
            });

            ReceiveAsync<CheckNotification>(async msg =>
            {                
                if (notifications.Count > 0)
                {
                    if (testProbe != null)
                    {
                        testProbe.Tell(notifications.Dequeue());
                    }
                    else
                    {
                        Sender.Tell(notifications.Dequeue());
                    }                    
                }
                else
                {
                    HeatBeatNotification heatBeatNotification = new HeatBeatNotification();

                    Task.Delay(1000).ContinueWith(tr => heatBeatNotification)
                        .PipeTo(Self, Sender);
                }
            });
        }
    }
}
