using ActorLib.Actors.Test;

using Akka.Actor;

using Xunit.Abstractions;

namespace ActorLibTest.Intro
{
    public class BasicTest : TestKitXunit
    {                

        public BasicTest(ITestOutputHelper output) : base(output)
        {
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
                    ExpectMsg("ok");
                    output.WriteLine("");
                }

                //ExpectNoMsg();
            });
        }
    }
}
