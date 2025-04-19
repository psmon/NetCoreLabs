using McpServer.Persistent.Model;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes.Vector;
using Raven.Client.Documents.Linq;

namespace McpServer.Persistent;

public class NoteRepository
{
    private readonly IDocumentStore _store;
    
    
    public NoteRepository()
    {
        _store = new DocumentStore
        {
            Urls = new[] { "http://localhost:9000" },
            Database = "net-core-labs"
        };
        _store.Initialize();
        
        new NoteIndex().Execute(_store);
    }
    
    public void AddNote(NoteDocument note)
    {
        using (var session = _store.OpenSession())
        {
            session.Store(note);
            session.SaveChanges();
        }
    }
    
    public List<NoteDocument> SearchByText(string title,string content, string category)
    {
        using (var session = _store.OpenSession())
        {
            // 명시적으로 변수로 선언
            var titleValue = title;
            var contentValue = content;
            var categoryValue = category;

            IRavenQueryable<NoteDocument> query = session.Query<NoteDocument>();
            
            if (!string.IsNullOrEmpty(titleValue))
            {
                query = query.Search(r => r.Title, titleValue);         // 제목 풀텍스트 검색
            }

            if (!string.IsNullOrEmpty(contentValue))
            {
                query = query.Search(r => r.Content, contentValue);     // 컨텐츠 풀텍스트 검색
            }

            if (!string.IsNullOrEmpty(categoryValue))
            {
                query = query.Where(r => r.Category == categoryValue); // 카테고리 필터
            }
            
            var results = query.ToList();
            
            return results;
        }
    }
    
    public List<NoteDocument> SearchByRadius(double latitude, double longitude, double radiusKm)
    {
        using (var session = _store.OpenSession())
        {
            return session.Query<NoteDocument>()
                .Spatial(
                    r => r.Point(x => x.Latitude, x => x.Longitude),
                    criteria => criteria.WithinRadius(radiusKm, latitude, longitude))
                .ToList();
        }
    }
    
    public List<NoteDocument> SearchByVector(float[] vector, int topN = 5)
    {
        using (var session = _store.OpenSession())
        {
            var results = session.Query<NoteDocument>()
                .VectorSearch(
                    field => field.WithEmbedding(x => x.TagsEmbeddedAsSingle, VectorEmbeddingType.Single),
                    queryVector => queryVector.ByEmbedding(new RavenVector<float>(vector)),
                    0.85f,
                    topN)
                .Customize(x => x.WaitForNonStaleResults())
                .ToList();

            return results;
        }
    }
    
}