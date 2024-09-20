using Akka.Actor;

using Xunit.Abstractions;

namespace ActorLibTest.Case.Counselors
{
    public class CounselorsActorTest : TestKitXunit
    {
        public CounselorsActorTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory(DisplayName = "CounselorsActorTestAreOK")]
        [InlineData(5, 1)]
        public void CounselorsActorTestAreOK(int counselorCount, int testCount)
        {
            var actorSystem = akkaService.GetActorSystem();

            Within(TimeSpan.FromMilliseconds(3000), () =>
            {
                var counselorActor = actorSystem.ActorOf(Props.Create(() => new CounselorsActor()));

                var probe = this.CreateTestProbe(actorSystem);

                counselorActor.Tell(new SetCounselorsState() { State = CounselorsState.Online, Skills = new int[] { 1,2,3 } }, probe);

                probe.ExpectMsg("SetCounselorsState");

                counselorActor.Tell(new CheckTakeTask() { SkillType = 1 }, probe);

                probe.ExpectMsg("I can help you");

                counselorActor.Tell(new AssignTask() { TaskId = 1111 }, probe);

                probe.ExpectMsg("I Take Task");                

                counselorActor.Tell(new CheckTakeTask() { SkillType = 5 }, probe);

                probe.ExpectMsg("I can't help you");


            });

        }
    }
}
