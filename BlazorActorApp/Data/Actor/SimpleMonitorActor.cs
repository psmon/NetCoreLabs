using ActorLib.Actors.Test;
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

        private Queue<Issue> Issues = new Queue<Issue>();

        public SimpleMonitorActor()
        {
            Receive<ActorCountInfoReq>(msg =>
            {
                Sender.Tell(new ActorCountInfoRes(MessageCounts));
            });

            Receive<Todo>(msg =>
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

            Receive<Issue>(msg => {
                Issues.Enqueue(msg);
            });

            Receive<ExpectIssue>(msg => {
                if (Issues.Count > 0)
                {
                    var issue = Issues.Dequeue();
                    Sender.Tell(issue);
                }
                else
                {
                    Sender.Tell(new NoIssue());
                }
            });

            Receive<string>(msg =>
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
