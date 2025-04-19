using ActorLib.Actor.Test;
using Akka.Actor;
using Akka.Routing;
using Akka.TestKit;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace ActorLibTest.Actors.Intro;

public class RoutersTest : TestKitXunit
{        

    public RoutersTest(ITestOutputHelper output) : base(output)
    {
    }

    [Theory(DisplayName = "RoundRobinPoolTest")]
    [InlineData(3,1000)]
    public void RoundRobinPoolTest(int nodeCount, int testCount, bool isPerformTest = false)
    {
        var actorSystem = _akkaService.GetActorSystem();

        TestProbe testProbe = this.CreateTestProbe(actorSystem);

        var props = new RoundRobinPool(nodeCount)                
            .Props(Props.Create(() => new BasicActor()));

        var actor = actorSystem.ActorOf(props);

        for (int i = 0; i < nodeCount; i++)
        {
            actor.Tell(testProbe.Ref);

            testProbe.ExpectMsg("done");
        }

        int cutOff = 3000;

        Within(TimeSpan.FromMilliseconds(cutOff), () =>
        {
            for (int i = 0; i < testCount; i++)
            {
                actor.Tell("hello" + i);
            }

            for (int i = 0; i < testCount; i++)
            {
                testProbe.ExpectMsg("world");
                
                if (isPerformTest)
                {
                    _dictionary.Add(_key++, _key);
                    _addCounter.Increment();
                }
            }
        });
    }

    [Theory(DisplayName = "RandomPoolTest")]
    [InlineData(3, 1000)]
    public void RandomPoolTest(int nodeCount, int testCount, bool isPerformTest = false)
    {
        var actorSystem = _akkaService.GetActorSystem();

        TestProbe testProbe = this.CreateTestProbe(actorSystem);

        var props = new RandomPool(nodeCount)
            .Props(Props.Create(() => new BasicActor()));

        var actor = actorSystem.ActorOf(props);

        int cutOff = 3000;

        Within(TimeSpan.FromMilliseconds(cutOff), () =>
        {
            for (int i = 0; i < testCount; i++)
            {
                actor.Tell("hello" + i);
            }

            for (int i = 0; i < testCount; i++)
            {
                ExpectMsg("world");

                if (isPerformTest)
                {
                    _dictionary.Add(_key++, _key);
                    _addCounter.Increment();
                }
            }
        });
    }

    [NBenchFact]        
    [PerfBenchmark(NumberOfIterations = 3, RunMode = RunMode.Throughput,
    RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
    [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
    [CounterTotalAssertion("TestCounter", MustBe.GreaterThan, 1500.0d)]
    [CounterMeasurement("TestCounter")]
    public void RoundRobinPoolTestPerformanceTest()
    {
        RoundRobinPoolTest(5, 3000, true);
    }

    [PerfSetup]
#pragma warning disable xUnit1013 // Public method should be marked as test
    public void Setup(BenchmarkContext context)
#pragma warning restore xUnit1013 // Public method should be marked as test
    {
        _addCounter = context.GetCounter("TestCounter");
        _key = 0;
    }
}
