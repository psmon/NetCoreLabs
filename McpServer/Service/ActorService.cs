using Akka.Actor;
using McpServer.Actor;
using McpServer.Actor.Model;

namespace McpServer.Service;

public class ActorService
{
    private readonly ActorSystem actorSystem;
    
    public IActorRef SearchActor { get; set; }
    
    public IActorRef RecordActor { get; set; }
    
    public IActorRef HistoryActor { get; set; }
    
    public ActorService()
    {
        actorSystem = ActorSystem.Create("MyActorSystem");
        
        SearchActor = actorSystem.ActorOf<SearchActor>("search-actor");
        RecordActor = actorSystem.ActorOf<RecordActor>("record-actor");
        HistoryActor = actorSystem.ActorOf<HistoryActor>("history-actor");
        
        RecordActor.Tell(new SetHistoryActorCommand()
        {
            HistoryActor = HistoryActor
        });
        
        SearchActor.Tell(new SetHistoryActorCommand()
        {
            HistoryActor = HistoryActor
        });
    }
    
}