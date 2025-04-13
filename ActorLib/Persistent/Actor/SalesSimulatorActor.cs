using ActorLib.Persistent.Model;
using Akka.Actor;

namespace ActorLib.Persistent.Actor;

public class SalesSimulatorActor : ReceiveActor
{
    private readonly IActorRef _salesActor;
    private ICancelable scheduler;

    public SalesSimulatorActor(IActorRef salesActor)
    {
        _salesActor = salesActor;

        // Schedule the first sale simulation immediately and then every 2 seconds:
        scheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.Zero, 
            TimeSpan.FromSeconds(2), Self, new StartSimulate(), Self);
        
        Receive<StartSimulate>(HandleStart);
        Receive<StopSimulate>(HandleStop);
    }

    private void HandleStart(StartSimulate message)
    {
        ConsoleHelper.WriteToConsole(ConsoleColor.Black,
            $"About to simulate a sale...");

        Random random = new Random();
        string[] products = { "Apple", "Google", "Nokia", "Xiaomi", "Huawei" };

        var randomBrand = products[random.Next(products.Length)];
        var randomPrice = random.Next(1, 6) * 100; // 100, 200, 300, 400, or 500

        var nextSale = new Sale(randomPrice, randomBrand);
        _salesActor.Tell(nextSale);
    }
    
    private void HandleStop(StopSimulate message)
    {
        scheduler.Cancel();
        ConsoleHelper.WriteToConsole(ConsoleColor.DarkRed,
            "Simulation stopped");
    }
}