using Akka.Actor;
using Akka.Event;
using Akka.Routing;

using Xunit.Abstractions;

namespace ActorLibTest.Case.Counselors
{
    public class SuperVisorActor : ReceiveActor
    {
        private ILoggingAdapter log = Context.GetLogger();

        private ITestOutputHelper tlog;

        private readonly SuperVisorInfo superVisorInfo;

        private IActorRef? routerFirstCompleted;

        private IActorRef? probe;

        public SuperVisorActor(SuperVisorInfo superVisorInfo, ITestOutputHelper testOutputHelper)
        {
            this.superVisorInfo = superVisorInfo;

            tlog = testOutputHelper;

            var within = TimeSpan.FromSeconds(10);

            routerFirstCompleted = Context.ActorOf(Props.Create(() =>
                new CounselorsActor(new CounselorInfo() { Id = 0, Name = "na" } , testOutputHelper))
                .WithRouter(new ScatterGatherFirstCompletedPool(0, within)), "routerFirstCompleted"
            );

            Receive<IActorRef>(message => {
                probe = message;
            });

            Receive<CreateCounselor>(message =>
            {
                tlog.WriteLine("Received CreateCounselor message: {0}", message.Counselor.Name);

                string uniqueId = $"{message.Counselor.Name}-{message.Counselor.Id}";
                
                // 상담원 생성
                var counselorActor = Context.ActorOf(Props.Create(() =>
                    new CounselorsActor(new CounselorInfo() { Id = 1, Name = "test1" }, testOutputHelper)),
                    uniqueId
                );

                var routee = Routee.FromActorRef(counselorActor);
                routerFirstCompleted.Tell(new AddRoutee(routee));

                Sender.Tell("CreateCounselor");

            });

            Receive<SetCounselorsState>(message =>
            {
                tlog.WriteLine("Received SetCounselorsState message: {0}", message.Counselor.Name);

                string uniqueId = $"{message.Counselor.Name}-{message.Counselor.Id}";

                var counselorActor = Context.Child(uniqueId);

                if (counselorActor.IsNobody())
                {
                    tlog.WriteLine("Counselor not found: {0}", uniqueId);
                    Sender.Tell("Counselor not found");
                    return;
                }

                counselorActor.Tell(message);

                Sender.Tell("SetCounselorsState");

            });

            Receive<CheckTakeTask>(message =>
            {
                tlog.WriteLine("Received CheckTakeTask message: {0}", message.SkillType);
                routerFirstCompleted.Tell(message);
            });

            Receive<WishTask>(message =>
            {
                tlog.WriteLine("Received WishTask message: {0}", message.WishActor.Path);
                message.WishActor.Tell(new AssignTask() { TaskId = 1 });
            });

            Receive<string>(message =>
            {
                tlog.WriteLine("Received String message: {0}", message);
                
                if (message == "I Take Task")
                {
                    tlog.WriteLine($"Take Task : {Sender.Path}");

                    if(probe != null)
                    {
                        probe.Tell("SomeOne Take Task");
                    }
                }
            });

        }
    }
}
