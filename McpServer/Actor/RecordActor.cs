using Akka.Actor;
using Akka.Event;
using McpServer.Actor.Model;
using McpServer.Persistent;
using McpServer.Persistent.Model;

namespace McpServer.Actor;

public class RecordActor : ReceiveActor
{
    private readonly ILoggingAdapter logger = Context.GetLogger();

    private IActorRef? testProbe;
    
    private IActorRef? historyActor;
    
    private readonly NoteRepository noteRepository;
    
    public RecordActor()
    {
        noteRepository = new NoteRepository();
        
        Receive<IActorRef>(actorRef =>
        {
            testProbe = actorRef;

            testProbe.Tell("done-ready");
        });
        
        Receive<SetHistoryActorCommand>(msg =>
        {
            historyActor = msg.HistoryActor;
            
            if (testProbe != null)
            {
                testProbe.Tell("done-set-history");
            }
        });
        
        
        Receive<AddNoteCommand>(msg =>
        {
            var note = new NoteDocument
            {
                Content = msg.Content,
                Category = msg.Category,
                Latitude = msg.Latitude,
                Longitude = msg.Longitude,
                Title = msg.Title,
                TagsEmbeddedAsSingle = msg.TagsEmbeddedAsSingle,
                CreatedAt = DateTime.UtcNow
            };
            
            noteRepository.AddNote(note);
            
            if (testProbe != null)
            {
                testProbe.Tell("done-add");
            }
            
            if(historyActor != null)
            {
                historyActor.Tell(msg);
            }
        });
        
    }
    
}