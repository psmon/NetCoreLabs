using Akka.Actor;

using Xunit.Abstractions;

namespace ActorLibTest.Case.Counselors
{
    public class CounselorsActorTest : TestKitXunit
    {
        public CounselorsActorTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(DisplayName = "CounselorsActorTestAreOK")]        
        public void CounselorsActorTestAreOK()
        {
            var actorSystem = akkaService.GetActorSystem();

            Within(TimeSpan.FromMilliseconds(10000), () =>
            {
                var counselorActor = actorSystem.ActorOf(Props.Create(() => new CounselorsActor()));

                counselorActor.Tell(new SetCounselorsState() { State = CounselorsState.Online, Skills = new int[] { 1, 2, 3 } });

                ExpectMsg("SetCounselorsState");

                counselorActor.Tell(new CheckTakeTask() { SkillType = 1 });

                ExpectMsg("I can help you");

                counselorActor.Tell(new AssignTask() { TaskId = 1111 });

                ExpectMsg("I Take Task");

                counselorActor.Tell(new CheckTakeTask() { SkillType = 5 });

                ExpectMsg("I can't help you");

                counselorActor.Tell(new SetCounselorsState() { State = CounselorsState.Offline, Skills = new int[] { 1, 2, 3 } });

                ExpectMsg("SetCounselorsState");

                counselorActor.Tell(new CheckTakeTask() { SkillType = 1 });

                ExpectMsg("I am Not Here..");
            });

        }
    }
}
