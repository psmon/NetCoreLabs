using ActorLib;
using ActorLib.Actors.Test;

using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.Xunit2;

using Xunit.Abstractions;

namespace ActorLibTest
{
    public class AkkaServiceTest : TestKit
    {
        AkkaService akkaService;

        public readonly ITestOutputHelper output;

        public AkkaServiceTest(ITestOutputHelper output) : base(GetConfig())
        {
            this.output = output;
            akkaService = new AkkaService();
        }

        public static Config GetConfig()
        {
            return ConfigurationFactory.ParseString(@"
                akka {	
	                loglevel = DEBUG
	                loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]                
                }
            ");
        }

        [Theory(DisplayName = "액터시스템을 생성후 기본메시지 수행")]
        [InlineData(100)]
        public void CreateSystemAndMessageTestAreOK(int cutoff)
        {
            akkaService.FromActorSystem(this.Sys);
            // 실 서비스코드는 액터시스템을 최초생성한 이후 이용합니다.
            // akkaService.CreateActorSystem("app");

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