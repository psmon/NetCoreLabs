using ActorLib;
using ActorLib.Actors.Test;

using Akka.Actor;
using Akka.Routing;

using BlazorActorApp.Data;
using BlazorActorApp.Data.Actor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<AkkaService>();
builder.Services.AddBlazorBootstrap();


var app = builder.Build();

// Akka
var akkaService = app.Services.GetRequiredService<AkkaService>();

// Create ActorSystem
var actorSystem = akkaService.CreateActorSystem("BlazorActorSystem");

// Create RoundRobin Router
var roundrobin = actorSystem.ActorOf(Props.Create<BasicActor>().WithRouter(new RoundRobinPool(0)), "roundrobin");
akkaService.AddActor("roundrobin", roundrobin);
var roundrobinMonitor = actorSystem.ActorOf(Props.Create<SimpleMonitorActor>());
akkaService.AddActor("roundrobinMonitor", roundrobinMonitor);

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
