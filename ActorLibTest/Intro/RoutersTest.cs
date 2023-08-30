using ActorLib;
using ActorLib.Actors.Test;

using Akka.Actor;
using Akka.Routing;
using Akka.TestKit;
using Akka.TestKit.Xunit2;

using Xunit.Abstractions;

namespace ActorLibTest.Intro
{
    public class RoutersTest : TestKitXunit
    {        

        public RoutersTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory(DisplayName = "RoundRobinPoolTest")]
        [InlineData(3)]
        public void RoundRobinPoolTest(int nodeCount)
        {
            var actorSystem = akkaService.GetActorSystem();

            TestProbe testProbe = this.CreateTestProbe(actorSystem);

            var props = new RoundRobinPool(nodeCount)                
                .Props(Props.Create(() => new BasicActor()));

            var actor = actorSystem.ActorOf(props, "worker");

            for (int i = 0; i < nodeCount; i++)
            {
                actor.Tell(testProbe.Ref);
            }

            int givenTestCount = 1000;

            int givenBlockTimePerTest = 10;

            int cutOff = givenTestCount * givenBlockTimePerTest;

            Within(TimeSpan.FromMilliseconds(cutOff), () =>
            {
                for (int i = 0; i < givenTestCount; i++)
                {
                    actor.Tell("hello" + i);
                }

                for (int i = 0; i < givenTestCount; i++)
                {
                    testProbe.ExpectMsg("world");
                }
            });
        }


    }
}
