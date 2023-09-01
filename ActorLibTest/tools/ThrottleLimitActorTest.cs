using ActorLib.Actors.Tools;

using Akka.Actor;

using NBench;
using Pro.NBench.xUnit.XunitExtensions;

using Xunit.Abstractions;

namespace ActorLibTest.tools
{
    public class ThrottleLimitActorTest : TestKitXunit
    {

        IActorRef throttleLimitActor;

        private TimeSpan EpsilonValueForWithins => new TimeSpan(0, 0, 1);


        public ThrottleLimitActorTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory(DisplayName = "테스트 n초당 1회 호출제약")]
        [InlineData(5, 1, false)]
        public void ThrottleLimitTest(int givenTestCount, int givenLimitSeconds, bool isPerformTest)
        {
            var actorSystem = akkaService.GetActorSystem();

            int expectedCompletedMaxSecond = givenTestCount * givenLimitSeconds + 5;

            // Create ThrottleLimit Actor
            throttleLimitActor = actorSystem.ActorOf(Props.Create(() => new ThrottleLimitActor(1, givenLimitSeconds, 1000)));

            actorSystem
               .Scheduler
               .ScheduleTellRepeatedly(TimeSpan.FromSeconds(0),
                         TimeSpan.FromSeconds(1),
                         throttleLimitActor, new Flush(), ActorRefs.NoSender);

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

                    output.WriteLine($"[{DateTime.Now}] - GTPRequestCmd");

                    if (isPerformTest)
                    {
                        _dictionary.Add(_key++, _key);
                        _addCounter.Increment();
                    }
                }
            }, EpsilonValueForWithins);
        }

        [NBenchFact]
        [PerfBenchmark(NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.LessThanOrEqualTo, 1.0d)]
        [CounterTotalAssertion("TestCounter", MustBe.LessThanOrEqualTo, 1)]
        [CounterMeasurement("TestCounter")]
        public void ThrottleLimitPerformanceTest()
        {
            ThrottleLimitTest(1, 1, true);
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
}
