using ActorLib.Actors.Tools;

using Akka.Actor;

using NBench;
using Pro.NBench.xUnit.XunitExtensions;

using Xunit.Abstractions;

namespace ActorLibTest.tools
{
    public class ThrottleActorTest : TestKitXunit
    {

        IActorRef throttleActor;

        private TimeSpan EpsilonValueForWithins => new TimeSpan(0, 0, 1);


        public ThrottleActorTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory(DisplayName = "수신검증및 n초당 5회 소비제약")]
        [InlineData(100, 5, false)]
        public void ThrottleTest(int givenTestCount, int givenLimitSeconds, bool isPerformTest)
        {
            var actorSystem = akkaService.GetActorSystem();

            int expectedCompletedMaxSecond = givenTestCount * givenLimitSeconds + 5;

            // Create ThrottleActor Actor
            throttleActor = actorSystem.ActorOf(Props.Create(() => new ThrottleActor(givenLimitSeconds)));

            // Connect Throttle -> TestWorkActor(probe)
            var probe = this.CreateTestProbe();
            throttleActor.Tell(new SetTarget(probe));

            // Test IT
            Within(TimeSpan.FromSeconds(expectedCompletedMaxSecond), () => {

                //When : Simultaneous generation of events at unspecified timing
                for (int i = 0; i < givenTestCount; i++)
                {
                    throttleActor.Tell(new TodoQueue()
                    {
                        Todo = new Todo { 
                            Id = i.ToString(),
                            Title = $"ThrottleLimitTest"
                        }
                    });
                }

                //Then : Safe processing within N seconds limit
                for (int i = 0; i < givenTestCount; i++)
                {
                    probe.ExpectMsg<Todo>(message =>
                    {
                        Assert.Equal("ThrottleLimitTest", message.Title);

                        if (isPerformTest)
                        {
                            _dictionary.Add(_key++, _key);
                            _addCounter.Increment();
                        }
                        else
                        {
                            output.WriteLine($"[{DateTime.Now}] - Todo");
                        }
                    });

                }
            }, EpsilonValueForWithins);
        }

        [NBenchFact]
        [PerfBenchmark(NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.LessThanOrEqualTo, 6.0d)]
        [CounterTotalAssertion("TestCounter", MustBe.GreaterThanOrEqualTo, 100)]
        [CounterMeasurement("TestCounter")]
        public void ThrottlePerformanceTest()
        {
            ThrottleTest(100, 5, true);
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
