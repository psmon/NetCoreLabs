using Akka.Actor;
using Akka.TestKit;

using BlazorActorApp.Data.SSE;
using BlazorActorApp.Service.SSE.Actor;

using NBench;

using Pro.NBench.xUnit.XunitExtensions;

using Xunit.Abstractions;

namespace ActorLibTest.Intro
{
    public class SSETest : TestKitXunit
    {                

        public SSETest(ITestOutputHelper output) : base(output)
        {
        }        

        [Theory(DisplayName = "SSEUserActor 액터테스트")]
        [InlineData(10,3000)]
        public void SSEUserActorAreOK(int testCount, int cutoff, bool isPerformTest = false)
        {            
            var actorSystem = akkaService.GetActorSystem();

            TestProbe testProbe = this.CreateTestProbe(actorSystem);

            string uuid = Guid.NewGuid().ToString();

            var basicActor = actorSystem.ActorOf(Props.Create(() => new SSEUserActor(uuid)));

            basicActor.Tell(testProbe.Ref);

            testProbe.ExpectMsg("done");

            Within(TimeSpan.FromMilliseconds(cutoff), () =>
            {
                
                for (int i = 0; i < testCount; i++)
                {
                    string message = $"test-{i}";
                    basicActor.Tell(new Notification()
                    {
                        Message = message,
                    });                    
                }

                for (int i = 0; i < testCount; i++)
                {
                    basicActor.Tell(new CheckNotification());
                }

                for (int i = 0; i < testCount; i++)
                {                                                            
                    var message = testProbe.ExpectMsg<Notification>();
                    if(!isPerformTest)
                        output.WriteLine($"Message:{message.Message}");


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
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10.0d)]
        [CounterTotalAssertion("TestCounter", MustBe.GreaterThan, 100.0d)]
        [CounterMeasurement("TestCounter")]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void SSEUserActorPerformanceTest()
        {
            int testUser = 100;
            int testEventPerUser = 10;
            for (int i = 0; i < testUser; i++) 
            {
                SSEUserActorAreOK(testEventPerUser, 3000, true);
            }
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
