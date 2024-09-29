using Akka.Actor;

using Xunit.Abstractions;

namespace ActorLibTest.Case.Counselors
{
    public class SuperVisorActorTest : TestKitXunit
    {
        public SuperVisorActorTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(DisplayName = "SuperVisorActorTestAreOK")]
        public void SuperVisorActorTestAreOK()
        {
            var actorSystem = akkaService.GetActorSystem();

            Within(TimeSpan.FromMilliseconds(10000), () =>
            {
                // 상담원 생성
                var superVisorActor = actorSystem.ActorOf(Props.Create(() =>
                    new SuperVisorActor(new SuperVisorInfo() { Id = 1, Name = "webnori" }, output))
                );

                var probe = CreateTestProbe(actorSystem);
                superVisorActor.Tell(probe.Ref);


                superVisorActor.Tell(new CreateCounselor() { Counselor = new CounselorInfo() { Id = 1, Name = "test1" } });

                ExpectMsg("CreateCounselor");

                superVisorActor.Tell(new SetCounselorsState() { Counselor = new CounselorInfo()
                {
                    Id = 1,
                    Name = "test1" },
                    State = CounselorsState.Online,
                    Skills = new int[] { 1, 2, 3 } }
                );

                ExpectMsg("SetCounselorsState");

                superVisorActor.Tell(new SetCounselorsState()
                {
                    Counselor = new CounselorInfo() { Id = 2, Name = "test2" },
                    State = CounselorsState.Online,
                    Skills = new int[] { 1, 2, 3 }
                });

                ExpectMsg("Counselor not found");

                superVisorActor.Tell(new CreateCounselor() { Counselor = new CounselorInfo() { Id = 2, Name = "test2" } });

                ExpectMsg("CreateCounselor");

                superVisorActor.Tell(new SetCounselorsState()
                {
                    Counselor = new CounselorInfo()
                    {
                        Id = 2,
                        Name = "test2"
                    },
                    State = CounselorsState.Online,
                    Skills = new int[] { 1, 2, 3 }
                });

                ExpectMsg("SetCounselorsState");

                superVisorActor.Tell(new CheckTakeTask() { SkillType = 1 });

                probe.ExpectMsg("SomeOne Take Task");


            }); // end of within

        }
    }
}
