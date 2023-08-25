using ActorLib;

using Akka.TestKit.Xunit2;

using Xunit.Abstractions;

namespace ActorLibTest.Intro
{
    public class DisPatcherTest : TestKit
    {
        private readonly ITestOutputHelper output;

        private readonly AkkaService akkaService;

        public DisPatcherTest(ITestOutputHelper output) : base()
        {
            this.output = output;
            akkaService = new AkkaService();
            akkaService.FromActorSystem(this.Sys);
        }
    }
}
