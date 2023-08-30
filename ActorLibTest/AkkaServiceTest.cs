using ActorLib.Actors.Test;

using Akka.Actor;

using Xunit.Abstractions;

namespace ActorLibTest
{
    public class AkkaServiceTest : TestKitXunit
    {

        public AkkaServiceTest(ITestOutputHelper output) : base(output)
        {

        }

        [Theory(DisplayName = "액터시스템 생성후 기본메시지 수행")]
        [InlineData(100)]
        public void CreateSystemAndMessageTestAreOK(int cutoff)
        {
            var actorSystem = akkaService.GetActorSystem();

            var basicActor = actorSystem.ActorOf(Props.Create(() => new BasicActor()));

            Within(TimeSpan.FromMilliseconds(cutoff), () =>
            {
                basicActor.Tell("메시지전송");
                ExpectMsg("ok");
            });

        }
    }
}