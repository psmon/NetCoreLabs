namespace ActorLib.Persistent.Model;

public class TravelReview
{
    public string Id { get; set; }
    public string Title { get; set; } // 제목 추가
    public float[] TitleVector { get; set; } // 제목의 벡터 값 추가
    public string Content { get; set; }
    public string Category { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
}