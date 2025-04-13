using ActorLib.Persistent.Model;
using Raven.Client.Documents.Indexes;

namespace ActorLib.Persistent;

public class TravelReviewIndex : AbstractIndexCreationTask<TravelReview>
{
    public TravelReviewIndex()
    {
        Map = reviews => from r in reviews
            select new
            {
                r.Content,
                r.Category
            };

        // 텍스트 필드 인덱싱 (전체 텍스트 검색용)
        Indexes.Add(x => x.Content, FieldIndexing.Search);
        Indexes.Add(x => x.Category, FieldIndexing.Search);
        
    }
}