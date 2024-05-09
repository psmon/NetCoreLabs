using Akka.Actor;
using Akka.Streams.Implementation.Fusing;

using BlazorActorApp.Data.SSE;

namespace BlazorActorApp.Data.Actor
{
    public class UserActor : ReceiveActor
    {
        private Queue<Notification> notifications = new Queue<Notification>();
        public UserActor() 
        {
            ReceiveAsync<Notification>(async msg =>
            {
                if (msg.IsProcessed == false)
                {
                    notifications.Enqueue(msg);
                }                
              
            });

            ReceiveAsync<CheckNotification>(async msg =>
            {
                await Task.Delay(1000);

                if (notifications.Count > 0)
                {
                    Sender.Tell(notifications.Dequeue());
                }
                else
                {
                    Sender.Tell(new EmptyNotification());
                }
            });

            //CheckNotification
        }


    }
}
