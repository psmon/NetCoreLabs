using ActorLib.Actors.Tools;

using Akka.Actor;
using Akka.Event;

namespace BlazorActorApp.Data.Actor
{
    public class ActorCountInfoReq
    {
    }

    public class ActorCountInfoRes
    {
        public ActorCountInfoRes(Dictionary<string, int> messageCounts) 
        {
            MessageCounts = messageCounts;
        }

        public Dictionary<string, int> MessageCounts { get; set; }
    }    

    public class SimpleMonitorActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();        

        private Dictionary<string, int> MessageCounts = new Dictionary<string, int>();

        public SimpleMonitorActor()
        {
            ReceiveAsync<ActorCountInfoReq>(async msg =>
            {
                Sender.Tell(new ActorCountInfoRes(MessageCounts));
            });

            ReceiveAsync<Todo>(async msg =>
            {
                string actorName = Sender.Path.Name;

                logger.Info($"ReceiveData {msg.Title} -From:{actorName}");

                if (!MessageCounts.ContainsKey(actorName))
                {
                    MessageCounts[actorName] = 1;
                }
                else
                {
                    MessageCounts[actorName]++;
                }

            });

            ReceiveAsync<string>(async msg =>
            {
                string actorName = Sender.Path.Name;

                logger.Info($"ReceiveData {msg} -From:{actorName}");

                if (msg == "done")
                {
                    // Actore Created
                    MessageCounts[actorName] = 0;
                }
                else
                {
                    if (!MessageCounts.ContainsKey(actorName))
                    {
                        MessageCounts[actorName] = 1;
                    }
                    else
                    {
                        MessageCounts[actorName]++;
                    }
                }
            });
        }
    }

}
