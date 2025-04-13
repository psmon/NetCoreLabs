using Raven.Client.Documents;

namespace ActorLib.Persistent.Model;

public class TravelReview
{
    public string Id { get; set; }
    public string Title { get; set; } // 제목 추가
    
    // This field will hold numerical vector data - Not quantized
    public RavenVector<float> TagsEmbeddedAsSingle { get; set; }
    
    // This field will hold numerical vector data - Quantized to Int8
    public sbyte[][] TagsEmbeddedAsInt8 { get; set; }
    
    // This field will hold numerical vector data - Encoded in Base64 format
    public List<string> TagsEmbeddedAsBase64 { get; set; }
    
    public string Content { get; set; }
    public string Category { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
}