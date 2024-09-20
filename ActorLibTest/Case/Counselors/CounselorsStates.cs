namespace ActorLibTest.Case.Counselors
{
    public class CounselorsStates
    {
        
    }

    public enum CounselorsState
    {
        Offline,
        Online
    }

    public class SetCounselorsState
    {
        public CounselorsState State { get; set; }

        public int[] Skills { get; set; }

    }

    public class CheckTakeTask
    {
        public int SkillType { get; set; }
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
