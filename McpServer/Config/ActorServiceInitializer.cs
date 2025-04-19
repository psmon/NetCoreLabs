using McpServer.Service;
using Microsoft.Extensions.Hosting;

namespace McpServer.Config;

public class ActorServiceInitializer : IHostedService
{
    private readonly ActorService _actorService;

    public ActorServiceInitializer(ActorService actorService)
    {
        _actorService = actorService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // ActorService 초기화 로직
        // 필요 시 추가 설정 가능
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // ActorService 종료 로직 (필요 시)
        return Task.CompletedTask;
    }
}