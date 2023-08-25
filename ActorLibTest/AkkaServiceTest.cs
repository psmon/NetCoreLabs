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

        private readonly ITestOutputHelper output;

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

        [Theory(DisplayName = "액터시스템 생성후 기본메시지 수행")]
        [InlineData(100)]
        public void CreateSystemAndMessageTestAreOK(int cutoff)
        {
            // 실 서비스코드는 액터시스템을 최초생성한 이후 이용합니다.
            // TeskKit은 메시지를 검사할수있는 기능이 탑재된 ActorSystem을 생성해주기때문에 이용합니다.
            //
            // akkaService.CreateActorSystem("app");
            akkaService.FromActorSystem(this.Sys);

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