using ActorLib.Actors.Tools;
using Akka.Actor;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace ActorLibTest.Actors.Tools;

public class ThrottleTimerActorTest : TestKitXunit
{

    IActorRef throttleLimitActor;

    private TimeSpan EpsilonValueForWithins => new TimeSpan(0, 0, 1);


    public ThrottleTimerActorTest(ITestOutputHelper output) : base(output)
    {
    }

    [Theory(DisplayName = "초당 1회 소비제약 -TimerBase")]
    [InlineData(5, 1, false)]
    public void ThrottleTimerTest(int givenTestCount, int givenLimitSeconds, bool isPerformTest)
    {
        var actorSystem = _akkaService.GetActorSystem();

        int expectedCompletedMaxSecond = givenTestCount * givenLimitSeconds + 5;

        // Create ThrottleLimit Actor
        throttleLimitActor = actorSystem.ActorOf(Props.Create(() => new ThrottleTimerActor(1, givenLimitSeconds, 1000)));

        // Connect Throttle -> TestWorkActor(probe)
        var probe = this.CreateTestProbe();
        throttleLimitActor.Tell(new SetTarget(probe));

        // Test IT
        Within(TimeSpan.FromSeconds(expectedCompletedMaxSecond), () => {

            //When : Simultaneous generation of events at unspecified timing
            for (int i = 0; i < givenTestCount; i++)
            {
                throttleLimitActor.Tell(new EventCmd()
                {
                    Message = "test",
                });
            }

            //Then : Safe processing within N seconds limit
            for (int i = 0; i < givenTestCount; i++)
            {
                probe.ExpectMsg<EventCmd>(message =>
                {
                    Assert.Equal("test", message.Message);                        
                });


                if (isPerformTest)
                {
                    _dictionary.Add(_key++, _key);
                    _addCounter.Increment();
                }
                else
                {

                    output.WriteLine($"[{DateTime.Now}] - GTPRequestCmd");
                }
            }
        }, EpsilonValueForWithins);
    }

    [NBenchFact]
    [PerfBenchmark(NumberOfIterations = 2, RunMode = RunMode.Throughput,
    RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
    [CounterThroughputAssertion("TestCounter", MustBe.LessThanOrEqualTo, 1.5d)]
    [CounterTotalAssertion("TestCounter", MustBe.LessThanOrEqualTo, 5)]
    [CounterMeasurement("TestCounter")]
    public void ThrottleTimerTestPerformanceTest()
    {
        ThrottleTimerTest(5, 1, true);
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
