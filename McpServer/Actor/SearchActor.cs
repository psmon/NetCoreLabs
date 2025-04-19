using Akka.Actor;
using Akka.Event;
using McpServer.Actor.Model;
using McpServer.Persistent;

namespace McpServer.Actor;

public class SearchActor : ReceiveActor
{
    private readonly ILoggingAdapter _logger = Context.GetLogger();

    private IActorRef? testProbe;
    
    private readonly NoteRepository noteRepository;
    
    private IActorRef? historyActor;
    
    public SearchActor()
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

        Receive<SearchNoteByTextCommand>(command =>
        {
            _logger.Info($"SearchNoteByTextCommand: {command.Title}, {command.Content}, {command.Category}");
            
            var notes = noteRepository.SearchByText(command.Title, command.Content, command.Category);
            
            _logger.Info($"SearchNoteByTextCommand: {notes.Count} notes found");
            
            Sender.Tell(new SearchNoteActorResult()
            {
                Notes = notes
            });
            
            if (testProbe != null)
            {
                testProbe.Tell(new SearchNoteActorResult()
                {
                    Notes = notes
                });
            }
            
            if(historyActor != null)
            {
                historyActor.Tell(notes);
            }
            
        });
        
        Receive<SearchNoteByRadiusActorCommand>(command =>
        {
            var notes = noteRepository.SearchByRadius(command.Latitude, command.Longitude, command.Radius);
            
            Sender.Tell(new SearchNoteActorResult()
            {
                Notes = notes
            });
            
            if (testProbe != null)
            {
                testProbe.Tell(new SearchNoteActorResult()
                {
                    Notes = notes
                });
            }
            
            if(historyActor != null)
            {
                historyActor.Tell(notes);
            }
            
        });
        
        Receive<SearchNoteByVectorCommand>(command =>
        {
            var notes = noteRepository.SearchByVector(command.Vector, command.TopN);
            
            Sender.Tell(new SearchNoteActorResult()
            {
                Notes = notes
            });
            
            if (testProbe != null)
            {
                testProbe.Tell(new SearchNoteActorResult()
                {
                    Notes = notes
                });
            }
            
            if(historyActor != null)
            {
                historyActor.Tell(notes);
            }
        });
    }
}