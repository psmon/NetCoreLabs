using ActorLib.Persistent;
using ActorLib.Persistent.Model;
using Raven.Client.Documents;
using Xunit.Abstractions;

namespace ActorLibTest.Persistent;

public class MemberRepositoryTest : TestKitXunit
{
    private readonly IDocumentStore _store;
    private readonly MemberRepository _repository;
    
    public MemberRepositoryTest(ITestOutputHelper output) : base(output)
    {
        // RavenDB 임베디드 서버 초기화
        _store = new DocumentStore
        {
            Urls = new[] { "http://localhost:9000" }, // 로컬 RavenDB URL
            Database = "net-core-labs"
        };
        _store.Initialize();

        // MemberRepository 초기화
        _repository = new MemberRepository(_store);
    }
    
    [Fact]
    public void AddMember_ShouldAddMemberSuccessfully()
    {
        // Arrange
        var member = new Member
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 30
        };

        // Act
        _repository.AddMember(member);

        // Assert
        var retrievedMember = _repository.GetMemberById(member.Id);
        Assert.NotNull(retrievedMember);
        Assert.Equal("John Doe", retrievedMember.Name);
    }

    [Fact]
    public void UpdateMember_ShouldUpdateMemberSuccessfully()
    {
        // Arrange
        var member = new Member
        {
            Name = "Jane Doe",
            Email = "jane.doe@example.com",
            Age = 25
        };
        _repository.AddMember(member);

        // Act
        member.Age = 26;
        _repository.UpdateMember(member);

        // Assert
        var updatedMember = _repository.GetMemberById(member.Id);
        Assert.NotNull(updatedMember);
        Assert.Equal(26, updatedMember.Age);
    }

    [Fact]
    public void DeleteMember_ShouldDeleteMemberSuccessfully()
    {
        // Arrange
        var member = new Member
        {
            Name = "Mark Smith",
            Email = "mark.smith@example.com",
            Age = 40
        };
        _repository.AddMember(member);

        // Act
        _repository.DeleteMember(member.Id);

        // Assert
        var deletedMember = _repository.GetMemberById(member.Id);
        Assert.Null(deletedMember);
    }
    
}