using ActorLib.Persistent.Model;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes.Vector;
using Raven.Client.Documents.Linq;

namespace ActorLib.Persistent;

public class TravelReviewRepository
{
    private readonly IDocumentStore _store;

    public TravelReviewRepository(IDocumentStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public void AddReview(TravelReview review)
    {
        using (var session = _store.OpenSession())
        {
            session.Store(review);
            session.SaveChanges();
        }
    }

    public List<TravelReview> SearchReviews(string keyword, double latitude, double longitude, double radiusKm, string category = null)
    {
        using (var session = _store.OpenSession())
        {
            // 명시적으로 변수로 선언
            var keywordValue = keyword;
            var categoryValue = category;

            IRavenQueryable<TravelReview> query = session.Query<TravelReview>();

            if (!string.IsNullOrEmpty(keywordValue))
            {
                query = query.Search(r => r.Content, keywordValue); // 제목 검색 추가
            }

            if (!string.IsNullOrEmpty(categoryValue))
            {
                query = query.Where(r => r.Category == categoryValue); // 제목 검색 추가
            }

            // RavenDB에서 서버 측 필터링 후 클라이언트 측에서 반경 필터링
            var results = query.ToList();

            return results.Where(r =>
                6371 * Math.Acos(
                    Math.Cos(DegToRad(latitude)) * Math.Cos(DegToRad(r.Latitude)) *
                    Math.Cos(DegToRad(r.Longitude) - DegToRad(longitude)) +
                    Math.Sin(DegToRad(latitude)) * Math.Sin(DegToRad(r.Latitude))
                ) <= radiusKm).ToList();
        }
    }
    
    public List<TravelReview> SearchReviewsByRadius(double latitude, double longitude, double radiusKm)
    {
        using (var session = _store.OpenSession())
        {
            return session.Query<TravelReview>()
                .Spatial(
                    r => r.Point(x => x.Latitude, x => x.Longitude),
                    criteria => criteria.WithinRadius(radiusKm, latitude, longitude))
                .ToList();
        }
    }
    
    public List<TravelReview> SearchReviewsByVector(float[] vector, int topN = 5)
    {
        using (var session = _store.OpenSession())
        {
            var results = session.Query<TravelReview>()
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

    private double DegToRad(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}