using ActorLib.Actors.Test;

using Akka.Actor;
using Akka.Routing;
using Akka.TestKit;

using Xunit.Abstractions;

namespace ActorLibTest.Intro
{
    public class DisPatcherTest : TestKitXunit
    {
        TestProbe testProbe;
        

        public DisPatcherTest(ITestOutputHelper output) : base(output)
        {            
        }

        [Theory(DisplayName = "Dispatcher - Thread")]
        [InlineData(3, "synchronized-dispatcher")]
        [InlineData(3, "fork-join-dispatcher")]
        [InlineData(3, "custom-dispatcher")]
        [InlineData(3, "custom-task-dispatcher")]
        public void DispatcherTest(int nodeCount, string disPatcherName)
        {
            var actorSystem = akkaService.GetActorSystem();

            testProbe = this.CreateTestProbe(actorSystem);

            var props = new RoundRobinPool(nodeCount)
                .WithDispatcher(disPatcherName)
                .Props(Props.Create(() => new BasicActor()));

            var actor = actorSystem.ActorOf(props, "worker");

            for (int i = 0; i < nodeCount; i++)
            {
                actor.Tell(testProbe.Ref);
            }

            int givenTestCount = 10;

            int expectedCutOff = givenTestCount * 1000;

            Within(TimeSpan.FromMilliseconds(expectedCutOff), () =>
            {
                for (int i = 0; i < givenTestCount; i++)
                {
                    actor.Tell("slowCommand" +  i);
                }

                for (int i = 0; i < givenTestCount; i++)
                {
                    string resultMessage = testProbe.ExpectMsg<string>();                    
                }
            });
        }
    }
}
