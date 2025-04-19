using System.ComponentModel.DataAnnotations;
using Akka.Actor;
using McpServer.Persistent.Model;
using Raven.Client.Documents;

namespace McpServer.Actor.Model;

public class ActorCommand
{
}

public class AddNoteCommand : ActorCommand
{
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    public string? Category { get; set; }
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    public RavenVector<float>? TagsEmbeddedAsSingle { get; set; }
}

public class GetNoteHistoryCommand : ActorCommand
{
}

public class GetNoteSearchHistoryCommand : ActorCommand
{
}

public class SetHistoryActorCommand : ActorCommand
{
    [Required]
    public IActorRef HistoryActor { get; set; }
}

public class SearchNoteByTextCommand : ActorCommand
{
    public string? Title { get; set; }
    
    public string? Content { get; set; }
    
    public string? Category { get; set; }
}

public class SearchNoteByRadiusActorCommand : ActorCommand
{
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public double Radius { get; set; }
}

public class SearchNoteByVectorCommand : ActorCommand
{
    [Required]
    public float[] Vector { get; set; }

    public int TopN { get; set; } = 10;
}

public class SearchNoteActorResult : ActorCommand
{
    [Required]
    public List<NoteDocument> Notes { get; set; }
}

public class SearchErrorResult : ActorCommand
{
    [Required]
    public string ErrorMessage { get; set; }
}



