using Raven.Client.Documents;

namespace McpServer.Persistent.Model;

public class NoteDocument
{
    public string Id { get; set; } // RavenDB는 기본적으로 Id를 문서 키로 사용
    
    public string? Title { get; set; }
    
    public string? Category { get; set; }
    
    public string? Content { get; set; }
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    public RavenVector<float>? TagsEmbeddedAsSingle { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
}