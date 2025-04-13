using ActorLib.Persistent.Model;
using Raven.Client.Documents.Indexes;

namespace ActorLib.Persistent;

public class TravelReview_Index : AbstractIndexCreationTask<TravelReview>
{
    public TravelReview_Index()
    {
        Map = reviews => from r in reviews
            select new
            {
                r.Content,
                r.Category,
                r.TitleVector
            };

        // 텍스트 필드 인덱싱 (전체 텍스트 검색용)
        Indexes.Add(x => x.Content, FieldIndexing.Search);
        Indexes.Add(x => x.Category, FieldIndexing.Search);
        
        // 벡터 필드는 따로 분석 인덱싱 불필요, 저장만 필수
        Store(x => x.TitleVector, FieldStorage.Yes);
    }
}