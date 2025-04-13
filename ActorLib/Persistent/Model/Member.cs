namespace ActorLib.Persistent.Model;

public class Member
{
    public string Id { get; set; } // RavenDB는 기본적으로 Id를 문서 키로 사용
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}