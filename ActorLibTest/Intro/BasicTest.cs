using ActorLib.Actors.Test;

using Akka.Actor;
using Akka.TestKit;

using NBench;
using Pro.NBench.xUnit.XunitExtensions;

using Xunit.Abstractions;

namespace ActorLibTest.Intro
{
    public class BasicTest : TestKitXunit
    {                

        public BasicTest(ITestOutputHelper output) : base(output)
        {
        }        

        [Theory(DisplayName = "Hello에 응당하는 액터테스트")]
        [InlineData(10,3000)]
        public void HelloWorldAreOK(int testCount, int cutoff, bool isPerformTest = false)
        {
            
            var actorSystem = akkaService.GetActorSystem();

            TestProbe testProbe = this.CreateTestProbe(actorSystem);

            var basicActor = actorSystem.ActorOf(Props.Create(() => new BasicActor()));

            basicActor.Tell(testProbe.Ref);

            testProbe.ExpectMsg("done");

            Within(TimeSpan.FromMilliseconds(cutoff), () =>
            {
                for (int i = 0; i < testCount; i++)
                {
                    basicActor.Tell("hello");                    
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

        [NBenchFact]
        // Perfectly valid counter setup
        [PerfBenchmark(NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        [CounterTotalAssertion("TestCounter", MustBe.GreaterThan, 1500.0d)]
        [CounterMeasurement("TestCounter")]
        public void HelloWorldPerformanceTest()
        {
            HelloWorldAreOK(100, 3000, true);
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
