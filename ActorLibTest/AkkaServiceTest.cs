using ActorLib.Actors.Test;
using Akka.Actor;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace ActorLibTest;

public class AkkaServiceTest : TestKitXunit
{

    public AkkaServiceTest(ITestOutputHelper output) : base(output)
    {

    }

    [Theory(DisplayName = "액터시스템 생성후 기본메시지 수행")]
    [InlineData(100)]
    public void CreateSystemAndMessageTestAreOK(int cutoff)
    {
        var actorSystem = _akkaService.GetActorSystem();

        var basicActor = actorSystem.ActorOf(Props.Create(() => new BasicActor()));

        Within(TimeSpan.FromMilliseconds(cutoff), () =>
        {
            basicActor.Tell("hello");
            ExpectMsg("world");
        });

    }


    private void RunTest(int testGroupCount)
    {
        for (var i = 0; i < testGroupCount; i++)
        {
            for (var j = 0; j < 10000; j++)
            {
                var data = new int[100];
                _dataCache.Add(data.ToArray());
            }
                
        }
    }

    [NBenchFact]
    [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
    [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
    public void GarbageCollections_Measurement()
    {
        RunTest(1);
    }

    [NBenchFact]
    [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
    [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.LessThan, 600)]
    [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.LessThan, 300)]
    [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.LessThan, 20)]
    [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.LessThan, 50)]
    public void GarbageCollections_Test()
    {
        RunTest(1);
    }
    
}