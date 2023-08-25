using ActorLib;
using ActorLib.Actors.Test;

using Akka.Actor;
using Akka.TestKit.Xunit2;

using Xunit.Abstractions;

namespace ActorLibTest.Intro
{
    public class BasicTest : TestKit
    {
        private readonly ITestOutputHelper output;

        private readonly AkkaService akkaService;

        public BasicTest(ITestOutputHelper output) : base()
        {
            this.output = output;
            akkaService = new AkkaService();
            akkaService.FromActorSystem(this.Sys);
        }

        [Theory(DisplayName = "Hello에 응당하는 액터테스트")]
        [InlineData(10,100)]
        public void HelloWorldAreOK(int testCount, int cutoff)
        {
            var actorSystem = akkaService.GetActorSystem();

            var basicActor = actorSystem.ActorOf(Props.Create(() => new BasicActor()));

            Within(TimeSpan.FromMilliseconds(cutoff), () =>
            {
                for (int i = 0; i < testCount; i++)
                {
                    basicActor.Tell("hello");                    
                }

                for (int i = 0; i < testCount; i++)
                {
                    ExpectMsg("world");
                }

                //ExpectNoMsg();
            });
        }
    }
}
