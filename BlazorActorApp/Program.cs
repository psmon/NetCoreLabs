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


var app = builder.Build();

// Akka
var akkaService = app.Services.GetRequiredService<AkkaService>();

// Create ActorSystem
var actorSystem = akkaService.CreateActorSystem("BlazorActorSystem");

// Create MonitorActor
var simpleMonitorActor = actorSystem.ActorOf(Props.Create<SimpleMonitorActor>());
akkaService.AddActor("simpleMonitorActor", simpleMonitorActor);

// Create RoundRobin
var roundrobin = actorSystem.ActorOf(Props.Create<BasicActor>().WithRouter(new RoundRobinPool(0)), "roundrobin");
akkaService.AddActor("roundrobin", roundrobin);


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
