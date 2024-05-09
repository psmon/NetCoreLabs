using ActorLib;
using ActorLib.Actors.Test;
using ActorLib.Actors.Tools;

using Akka.Actor;
using Akka.Routing;

using BlazorActorApp.Data;
using BlazorActorApp.Data.Actor;
using BlazorActorApp.Data.SSE;
using BlazorActorApp.Logging;

using Microsoft.AspNetCore.ResponseCompression;

using MudBlazor;
using MudBlazor.Services;

Logger.Configure();
Logger.Log.Info("App starting");


/* Lets log something every second to simulate legitimate log output
System.Timers.Timer timer = new System.Timers.Timer(1000);
timer.Elapsed += (source, e) =>
{
    Logger.Log.Info("tick");
};
timer.Enabled = true;
timer.Start();
*/

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<AkkaService>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddMudServices();
builder.Services.AddMudMarkdownServices();

builder.Services.AddSingleton<INotificationRepository,NotificationRepository>();
builder.Services.AddSingleton<ICustomMessageQueue, CustomMessageQueue>();

builder.Services.AddHostedService<CustomHostedService>();

builder.Services.AddScoped<DebugService>();
builder.Services.AddScoped<JsConsole>();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();

// Akka
var akkaService = app.Services.GetRequiredService<AkkaService>();

// Create ActorSystem
// akka.tcp://default@localhost:9000
var actorSystem = akkaService.CreateActorSystem("default", 9000);

var defaultMonitor = actorSystem.ActorOf(Props.Create<SimpleMonitorActor>());
akkaService.AddActor("defaultMonitor", defaultMonitor);

// Create ActorSystem Onmore for Remote Test
// akka.tcp://default2@localhost:9001
var actorSystem2 = akkaService.CreateActorSystem("default2", 9001);

// Fot Remote Test
actorSystem.ActorOf(Props.Create<BasicActor>(), "someActor");
actorSystem2.ActorOf(Props.Create<BasicActor>(), "someActor");


// Create RoundRobin Router
var roundrobin = actorSystem.ActorOf(Props.Create<BasicActor>().WithRouter(new RoundRobinPool(0)), "roundrobin");
akkaService.AddActor("roundrobin", roundrobin);
var roundrobinMonitor = actorSystem.ActorOf(Props.Create<SimpleMonitorActor>());
akkaService.AddActor("roundrobinMonitor", roundrobinMonitor);


// Create Throttle Actor
var throttlerouter = actorSystem.ActorOf(Props.Create(() => new ThrottleActor(5)));
akkaService.AddActor("throttlerouter", throttlerouter);


// Create BroadCast Router
var broadcast = actorSystem.ActorOf(Props.Create<BasicActor>().WithRouter(new BroadcastPool(0)), "broadcast");
akkaService.AddActor("broadcast", broadcast);
var broadcastMonitor = actorSystem.ActorOf(Props.Create<SimpleMonitorActor>());
akkaService.AddActor("broadcastMonitor", broadcastMonitor);

// Random Router
var random = actorSystem.ActorOf(Props.Create<BasicActor>().WithRouter(new RandomPool(0)), "random");
akkaService.AddActor("random", random);
var randomMonitor = actorSystem.ActorOf(Props.Create<SimpleMonitorActor>());
akkaService.AddActor("randomMonitor", randomMonitor);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapHub<LoggingHub>("/hubs/logging");
    endpoints.MapFallbackToPage("/_Host");
});



app.Run();
