using ActorLib.Persistent.Actor;
using Akka.Actor;
using Xunit.Abstractions;

namespace ActorLibTest.Persistent.Actor;

public class SalesActorTest : TestKitXunit
{
    public SalesActorTest(ITestOutputHelper output) : base(output)
    {
    }
    
    [Theory(DisplayName = "SalesActor Persistence Test")]
    [InlineData(1_500, 20000)]
    public void TestSalesSimulatorActor(long expectedProfit, int cutoff)
    {
        var actorSystem = _akkaService.GetActorSystem();
        
        var taskCompletion = new TaskCompletionSource<bool>();
        
        var salesActor = actorSystem.ActorOf(Props.Create(() => new SalesActor(expectedProfit,
            taskCompletion)) );
        
        var salesSimulatorActor = actorSystem.ActorOf(Props.Create(() => new SalesSimulatorActor(salesActor)));

        Within(TimeSpan.FromMilliseconds(cutoff), () =>
        {
            taskCompletion.Task.Wait();
        });
    }
    
}