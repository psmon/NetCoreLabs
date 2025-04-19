using System.ComponentModel;
using System.Text.Json;
using Akka.Actor;
using McpServer.Actor.Model;
using McpServer.Persistent.Model;
using McpServer.Service;
using ModelContextProtocol.Server;
using Raven.Client.Documents;

namespace McpServer.Tools;

[McpServerToolType]
public static class NoteTool
{
    [McpServerTool, Description("웹노리 노트에 노트를 추가합니다.")]
    public static async Task<string> AddNote(ActorService actorService, 
        [Description("노트의 제목으로 필수값입니다.")] string title,
        [Description("노트의 컨텐츠값으로 필수값입니다.")] string content,
        [Description("노트의 카테고리입니다.")] string? category, 
        [Description("노트에 위치정보가 있다면 latitude를 입력")] double? latitude, 
        [Description("노트에 위치정보가 있다면 longitude 입력")] double? longitude,
        [Description("노트에 임베딩된 값이 있다면 입력")] float[]? tagsEmbeddedAsSingle
        )
    {
        var note = new NoteDocument
        {
            Title = title,
            Category = category,
            Content = content,
            Latitude = latitude,
            Longitude = longitude,
            CreatedAt = DateTime.UtcNow,
            TagsEmbeddedAsSingle = new RavenVector<float>(tagsEmbeddedAsSingle)
        };
        
        actorService.RecordActor.Tell(new AddNoteCommand()
        {
            Title = title,
            Category = category,
            Content = content,
            Latitude = latitude,
            Longitude = longitude,
            TagsEmbeddedAsSingle = new RavenVector<float>(tagsEmbeddedAsSingle)
        }, ActorRefs.NoSender);
        
        return JsonSerializer.Serialize(note);
    }
    
    [McpServerTool, Description("웹노리 노트에서 Text검색을 합니다. 최소 하나값이 필수입니다.")]
    public static async Task<string> SearchNoteByText(ActorService actorService, 
        [Description("웹노리 노트에서 타이틀을 키워드 검색합니다.")] string? title, 
        [Description("웹노리 노트에서 컨텐츠를 키워드 검색합니다.")] string? content, 
        [Description("웹노리 노트에서 카테고리를 키워드 검색합니다.")] string? category)
    {
        var result = await actorService.SearchActor.Ask(new SearchNoteByTextCommand()
        {
            Title = title,
            Content = content,
            Category = category
        }, TimeSpan.FromSeconds(5));
        
        if (result is SearchNoteActorResult searchResult)
        {
            return JsonSerializer.Serialize(searchResult.Notes);
        }
        
        return "Failed to get note history.";
    }
    
    [McpServerTool, Description("웹노리 노트에서 반경검색을 합니다. 모두 필수값입니다.")]
    public static async Task<string> SearchNoteByRadius(ActorService actorService, 
        [Description("This is the latitude of the note ")] double latitude, 
        [Description("This is the longitude of the note ")] double longitude, 
        [Description("This is the radius(m) of the note ")] double radius)
    {
        var result = await actorService.SearchActor.Ask(new SearchNoteByRadiusActorCommand()
        {
            Latitude = latitude,
            Longitude = longitude,
            Radius = radius
        }, TimeSpan.FromSeconds(5));
        
        if (result is SearchNoteActorResult searchResult)
        {
            return JsonSerializer.Serialize(searchResult.Notes);
        }
        
        return "Failed to get note history.";
    }
    
    [McpServerTool, Description("웹노리 노트에서 벡터검색을 합니다. 모두 필수값입니다.")]
    public static async Task<string> SearchNoteByVector(ActorService actorService, 
        [Description("This is the vector of the note  If there is no value, enter null.")] float[] vector, 
        [Description("This is the top N of the note  If there is no value, enter null.")] int topN)
    {
        var result = await actorService.SearchActor.Ask(new SearchNoteByVectorCommand()
        {
            Vector = vector,
            TopN = topN
        }, TimeSpan.FromSeconds(5));
        
        if (result is SearchNoteActorResult searchResult)
        {
            return JsonSerializer.Serialize(searchResult.Notes);
        }
        
        return "Failed to get note history.";
    }
    
    
    [McpServerTool, Description("웹노리 노트에서 최근 추가된 노트를 가져옵니다.")]
    public static async Task<string> GetNoteHistory(ActorService actorService)
    {
        //SearchNoteActorResult
        var result = await actorService.HistoryActor.Ask(new GetNoteHistoryCommand(), TimeSpan.FromSeconds(5));
        
        if (result is SearchNoteActorResult searchResult)
        {
            return JsonSerializer.Serialize(searchResult.Notes);
        }
        
        return "Failed to get note history.";
    }
    
    [McpServerTool, Description("웹노리 노트에서 최근 검색된 노트를 가져옵니다.")]
    public static async Task<string> GetNoteSearchHistory(ActorService actorService)
    {
        //SearchNoteActorResult
        var result = await actorService.HistoryActor.Ask(new GetNoteSearchHistoryCommand(), TimeSpan.FromSeconds(5));
        
        if (result is SearchNoteActorResult searchResult)
        {
            return JsonSerializer.Serialize(searchResult.Notes);
        }
        
        return "Failed to get note history.";
    }

    
}