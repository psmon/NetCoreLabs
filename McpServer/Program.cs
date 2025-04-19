using McpServer.Config;
using McpServer.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var clientMode = args.Contains("--clientMode"); // 실행 인자에서 clientMode 설정

builder.Services.AddSingleton<ActorService>(provider => new ActorService(!clientMode));

builder.Services.AddHostedService<ActorServiceInitializer>();

await builder.Build().RunAsync();