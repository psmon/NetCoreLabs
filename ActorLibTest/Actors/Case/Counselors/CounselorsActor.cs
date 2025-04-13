using Akka.Actor;
using Akka.Event;
using Xunit.Abstractions;

namespace ActorLibTest.Actors.Case.Counselors;

public class CounselorsActor : UntypedActor
{
    private ILoggingAdapter log = Context.GetLogger();

    private ITestOutputHelper tlog;

    private CounselorsState counselorsState = CounselorsState.Offline;

    private int[] skills { get; set; } = new int[0];

    private int assignedTaskCount = 0;

    private readonly CounselorInfo counselorInfo;

    private Random Random = new Random();

    public CounselorsActor(CounselorInfo counselorInfo, ITestOutputHelper testOutputHelper)
    {
        this.counselorInfo = counselorInfo;
        tlog = testOutputHelper;
    }

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

                // dummy
                assignedTaskCount = Random.Next(0, 10);

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
            tlog.WriteLine($"received unhanddle message : {message}");
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
                Task.Delay(assignedTaskCount * 100);
                //Sender.Tell("I can help you");

                Sender.Tell(new WishTask() { WishActor = Self });
            }
            else
            {
                //Sender.Tell("I can't help you");
                tlog.WriteLine($"{Self.Path} I can't help you");
            }                
            break;

            case AssignTask assignTask:                
            assignedTaskCount++;
            Sender.Tell("I Take Task");
            break;

            case CompletedTask completedTask:                
            assignedTaskCount--;
            Sender.Tell("I Did Task");
            break;

            case SetCounselorsState counselor:
            if (counselor.State == CounselorsState.Offline)
            {
                tlog.WriteLine("counselors are offline");
                counselorsState = counselor.State;
                skills = counselor.Skills;
                Sender.Tell("SetCounselorsState");
                Become(OffLine);
            }
            break;

            default:
            tlog.WriteLine($"received unhanddle message : {message}");
            break;
        }
    }

    private void OffLine(object message)
    {
        switch (message)
        {
            case CheckTakeTask checkTask:
            //Sender.Tell("I am Not Here..");
            tlog.WriteLine($"{Self.Path} I am Not Here..");
            break;

            case SetCounselorsState counselor:
            if (counselor.State == CounselorsState.Online)
            {
                tlog.WriteLine("counselors are online");
                counselorsState = counselor.State;
                Sender.Tell("SetCounselorsState");
                skills = counselor.Skills;
                Become(Online);
            }
            break;

            default:
            tlog.WriteLine($"received unhanddle message : {message}");
            break;
        }
    }
}
