using ActorLib.Persistent.Model;
using Raven.Client.Documents;

namespace ActorLib.Persistent;

public class MemberRepository
{
    private readonly IDocumentStore _store;

    public MemberRepository(IDocumentStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public void AddMember(Member member)
    {
        using (var session = _store.OpenSession())
        {
            session.Store(member);
            session.SaveChanges();
        }
    }

    public Member GetMemberById(string id)
    {
        using (var session = _store.OpenSession())
        {
            return session.Load<Member>(id);
        }
    }

    public void UpdateMember(Member member)
    {
        using (var session = _store.OpenSession())
        {
            var existingMember = session.Load<Member>(member.Id);
            if (existingMember != null)
            {
                existingMember.Name = member.Name;
                existingMember.Email = member.Email;
                existingMember.Age = member.Age;
                session.SaveChanges();
            }
        }
    }

    public void DeleteMember(string id)
    {
        using (var session = _store.OpenSession())
        {
            var member = session.Load<Member>(id);
            if (member != null)
            {
                session.Delete(member);
                session.SaveChanges();
            }
        }
    }
}