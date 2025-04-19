using Akka.Actor;
using Akka.Configuration;

using McpServer.Actor;
using McpServer.Actor.Model;

namespace McpServer.Service;

public class ActorService
{
    private readonly ActorSystem actorSystem;
    
    public IActorRef SearchActor { get; set; }
    
    public IActorRef RecordActor { get; set; }
    
    public IActorRef HistoryActor { get; set; }
    
    public ActorService(bool serverMode)
    {
        Console.WriteLine($"ActorService initialized in {(serverMode ? "Server" : "Client")} mode.");

        // HOCON 설정
        var config = ConfigurationFactory.ParseString($@"
            akka {{
                actor {{
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }}
                remote {{
                    dot-netty.tcp {{
                        hostname = ""127.0.0.1""
                        port = {(serverMode ? 5500 : 0)} // 서버 모드일 때만 포트 5500 사용
                    }}
                }}
            }}
        ");

        actorSystem = ActorSystem.Create("MyActorSystem", config);

        if (serverMode)
        {
            // 서버 모드일 때만 작동액터 생성 : MCP Server 
            SearchActor = actorSystem.ActorOf<SearchActor>("search-actor");
            RecordActor = actorSystem.ActorOf<RecordActor>("record-actor");
            HistoryActor = actorSystem.ActorOf<HistoryActor>("history-actor");

            RecordActor.Tell(new SetHistoryActorCommand()
            {
                HistoryActor = HistoryActor
            });

            SearchActor.Tell(new SetHistoryActorCommand()
            {
                HistoryActor = HistoryActor
            });
        }
        else
        {
            // 클라이언트 모드일 때 원격 액터 참조 : MCP Client
            var remoteAddress = "akka.tcp://MyActorSystem@127.0.0.1:5500";
            SearchActor = actorSystem.ActorSelection($"{remoteAddress}/user/search-actor")
                .ResolveOne(TimeSpan.FromSeconds(3)).Result;

            RecordActor = actorSystem.ActorSelection($"{remoteAddress}/user/record-actor")
                .ResolveOne(TimeSpan.FromSeconds(3)).Result;

            HistoryActor = actorSystem.ActorSelection($"{remoteAddress}/user/history-actor")
                .ResolveOne(TimeSpan.FromSeconds(3)).Result;
        }
    }
    
}