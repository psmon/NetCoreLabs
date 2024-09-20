using Akka.Actor;
using Akka.Event;

using NLog;


namespace ActorLibTest.Case.Counselors
{
    public class CounselorsActor : UntypedActor
    {
        private ILoggingAdapter log = Context.GetLogger();

        private CounselorsState counselorsState = CounselorsState.Offline;

        private int[] skills { get; set; } = new int[0];

        private int assignedTaskCount = 0;

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SetCounselorsState counselor:
                if(counselor.State == CounselorsState.Online)
                {
                    log.Info("counselors are online");
                    counselorsState = counselor.State;
                    Sender.Tell("SetCounselorsState");
                    skills = counselor.Skills;

                    Become(Online);
                    
                    
                }
                else if (counselor.State == CounselorsState.Offline)
                {
                    log.Info("counselors are offline");
                    counselorsState = counselor.State;
                    skills = counselor.Skills;
                    Sender.Tell("SetCounselorsState");

                    Become(OffLine);
                    
                }
                break;
                default:
                    log.Info("received unknown message");
                break;
            }
        }

        private void Online(object message)
        {
            switch (message)
            {
                case CheckTakeTask checkTask:
                if (skills.Contains(checkTask.SkillType))
                {
                    Sender.Tell("I can help you");
                }
                else
                {
                    Sender.Tell("I can't help you");
                }                
                break;
                case AssignTask assignTask:
                Sender.Tell("I Take Task");
                assignedTaskCount++;
                break;
                case CompletedTask completedTask:
                assignedTaskCount--;
                break;
            }
        }

        private void OffLine(object message)
        {
            switch (message)
            {
                case CheckTakeTask checkTask:
                Sender.Tell("I am Not Here..");
                break;
            }
        }

    }
}
