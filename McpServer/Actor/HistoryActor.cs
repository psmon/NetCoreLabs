using Akka.Actor;
using Akka.Event;
using McpServer.Actor.Model;
using McpServer.Persistent.Model;

namespace McpServer.Actor;

public class HistoryActor : ReceiveActor
{
    private readonly ILoggingAdapter logger = Context.GetLogger();

    private IActorRef? testProbe;
    
    private Queue<NoteDocument> noteQueue;
    
    private Queue<NoteDocument> noteSearchQueue;
    
    
    private void EnqueueNote(NoteDocument note)
    {
        noteQueue.Enqueue(note);
        if(noteQueue.Count > 10)
        {
            noteQueue.Dequeue();
        }
    }
    
    private void EnqueueSearchNote(NoteDocument note)
    {
        noteSearchQueue.Enqueue(note);
        if(noteSearchQueue.Count > 50)
        {
            noteSearchQueue.Dequeue();
        }
    }

    public HistoryActor()
    {
        noteQueue = new Queue<NoteDocument>();
        
        noteSearchQueue = new Queue<NoteDocument>();
        
        Receive<IActorRef>(actorRef =>
        {
            testProbe = actorRef;

            testProbe.Tell("done-ready");
        });
        
        Receive<AddNoteCommand>(msg =>
        {
            EnqueueNote(new NoteDocument()
            {
                Content = msg.Content,
                Category = msg.Category,
                Latitude = msg.Latitude,
                Longitude = msg.Longitude,
                Title = msg.Title,
                TagsEmbeddedAsSingle = msg.TagsEmbeddedAsSingle,
                CreatedAt = DateTime.UtcNow
            });
        });
        
        Receive<List<NoteDocument>>(notes =>
        {
            foreach (var note in notes)
            {
                EnqueueSearchNote(note);
            }
        });
        
        
        Receive<GetNoteHistoryCommand>(msg =>
        {
            // Handle GetNoteHistoryCommand
            if (testProbe != null)
            {
                testProbe.Tell(new SearchNoteActorResult()
                {
                    Notes = noteQueue.ToList()
                });
            }
            
            Sender.Tell(new SearchNoteActorResult()
            {
                Notes = noteQueue.ToList()
            });
            
        });
        
        Receive<GetNoteSearchHistoryCommand>(msg =>
        {
            logger.Info("try GetNoteSearchHistoryCommand");


            if (testProbe != null)
            {
                testProbe.Tell(new SearchNoteActorResult()
                {
                    Notes = noteSearchQueue.ToList()
                });
            }
            
            Sender.Tell(new SearchNoteActorResult()
            {
                Notes = noteSearchQueue.ToList()
            });

            logger.Info($"SearchNoteActorResult {noteSearchQueue.Count}");

        });
        
    }
    
}