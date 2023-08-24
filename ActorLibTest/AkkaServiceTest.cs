using ActorLib;
using ActorLib.Actors.Test;

using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit;
using Akka.TestKit.Xunit2;

using Xunit.Abstractions;

namespace ActorLibTest
{
    public class AkkaServiceTest : TestKit
    {
        AkkaService akkaService;

        TestProbe probe;

        public AkkaServiceTest(ITestOutputHelper output) : base(GetConfig())
        {
            akkaService = new AkkaService();
            akkaService.FromActorSystem(this.Sys);
            probe = this.CreateTestProbe();

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

        [Theory(DisplayName = "���ͽý����� ������ �⺻�޽��� ����")]
        [InlineData(100)]
        public void CreateSystemAndMessageTestAreOK(int cutoff)
        {
            var actorSystem = akkaService.GetActorSystem();

            var basicActor = actorSystem.ActorOf(Props.Create(() => new BasicActor()));

            Within(TimeSpan.FromMilliseconds(cutoff), () =>
            {
                basicActor.Tell("�޽�������");
                ExpectMsg("ok");
            });

        }
    }
}