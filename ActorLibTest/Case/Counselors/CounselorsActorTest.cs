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
                // 상담원 생성
                var counselorActor = actorSystem.ActorOf(Props.Create(() => 
                    new CounselorsActor(new CounselorInfo() { Id=1,Name="test1" }, output))
                );

                counselorActor.Tell(new SetCounselorsState() { State = CounselorsState.Online, Skills = new int[] { 1, 2, 3 } });

                ExpectMsg("SetCounselorsState");

                counselorActor.Tell(new CheckTakeTask() { SkillType = 1 });

                ExpectMsg<WishTask>();

                counselorActor.Tell(new AssignTask() { TaskId = 1111 });

                ExpectMsg("I Take Task");

                counselorActor.Tell(new CheckTakeTask() { SkillType = 5 });

                ExpectNoMsg(TimeSpan.FromMilliseconds(500));

                counselorActor.Tell(new SetCounselorsState() { State = CounselorsState.Offline, Skills = new int[] { 1, 2, 3 } });

                ExpectMsg("SetCounselorsState");

                counselorActor.Tell(new CheckTakeTask() { SkillType = 1 });

                ExpectNoMsg(TimeSpan.FromMilliseconds(500));

            });

        }
    }
}
