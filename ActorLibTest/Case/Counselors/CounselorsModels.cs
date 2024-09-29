using Akka.Actor;

namespace ActorLibTest.Case.Counselors
{
    public class CounselorsModels
    {
        
    }

    public enum CounselorsState
    {
        Offline,
        Online
    }

    public class SuperVisorInfo()
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class CounselorInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class CreateCounselor
    {
        public CounselorInfo Counselor { get; set; }        
    }


    public class SetCounselorsState
    {
        public CounselorInfo Counselor { get; set; }

        public CounselorsState State { get; set; }

        public int[] Skills { get; set; }

    }

    public class CheckTakeTask
    {
        public int SkillType { get; set; }
    }

    public class WishTask
    {
        public required IActorRef WishActor { get; set; }
    }

    public class AssignTask
    {
        public int TaskId { get; set; }
    }

    public class CompletedTask
    {
        public int TaskId { get; set; }
    }

}
