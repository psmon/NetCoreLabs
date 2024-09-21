using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

namespace ActorLibTest.Case.Counselors
{
    public class SuperVisorActor : ReceiveActor
    {
        private ILoggingAdapter log = Context.GetLogger();

        private readonly SuperVisorInfo superVisorInfo;

        public SuperVisorActor(SuperVisorInfo superVisorInfo)
        {
            this.superVisorInfo = superVisorInfo;

            Receive<string>(message => {
                log.Info("Received String message: {0}", message);
                Sender.Tell(message);
            });

            Receive<CreateCounselor>(message =>
            {
                log.Info("Received CreateCounselor message: {0}", message.Counselor.Name);

                string uniqueId = $"{message.Counselor.Name}-{message.Counselor.Id}";
                
                // 상담원 생성
                var counselorActor = Context.ActorOf(Props.Create(() =>
                    new CounselorsActor(new CounselorInfo() { Id = 1, Name = "test1" })),
                    uniqueId
                );

                Sender.Tell("CreateCounselor");

            });

            Receive<SetCounselorsState>(message =>
            {
                log.Info("Received SetCounselorsState message: {0}", message.Counselor.Name);

                string uniqueId = $"{message.Counselor.Name}-{message.Counselor.Id}";

                var counselorActor = Context.Child(uniqueId);

                if (counselorActor.IsNobody())
                {
                    log.Warning("Counselor not found: {0}", uniqueId);
                    Sender.Tell("Counselor not found");
                    return;
                }

                counselorActor.Tell(message);

                Sender.Tell("SetCounselorsState");

            });


        }
    }
}
