using McpServer.Persistent.Model;
using Raven.Client.Documents.Indexes;

namespace McpServer.Persistent;


public class NoteIndex : AbstractIndexCreationTask<NoteDocument>
{
    public NoteIndex()
    {
        Map = notes => from r in notes
            select new
            {
                r.Title,
                r.Content,
                r.Category
            };

        // 텍스트 필드 인덱싱 (전체 텍스트 검색용)
        Indexes.Add(x => x.Title, FieldIndexing.Search);
        Indexes.Add(x => x.Content, FieldIndexing.Search);
        Indexes.Add(x => x.Category, FieldIndexing.Exact);
        
        //Analyzers.Add(x => x.Content, "Lucene.Net.Analysis.Ko.KoreanAnalyzer");
        
    }
}