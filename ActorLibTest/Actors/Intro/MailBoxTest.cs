using System.Text.Json;
using ActorLib.Actors.Test;
using Akka.Actor;
using Akka.TestKit;
using Xunit.Abstractions;

namespace ActorLibTest.Actors.Intro;

public class MailBoxTest : TestKitXunit
{
    public MailBoxTest(ITestOutputHelper output) : base(output)
    {
    }

    [Theory(DisplayName = "메시지 우선순위 MailBox 테스트")]
    [InlineData(7, 3000)]
    public void HelloWorldAreOK(int testCount, int cutoff, bool isPerformTest = false)
    {
        var actorSystem = akkaService.GetActorSystem();

        TestProbe testProbe = this.CreateTestProbe(actorSystem);

        var mailBoxActor = actorSystem.ActorOf(Props.Create(() => new BasicActor()).WithMailbox("my-custom-mailbox") );

        mailBoxActor.Tell(testProbe.Ref);

        testProbe.ExpectMsg("done");


        Within(TimeSpan.FromMilliseconds(cutoff), () =>
        {
            mailBoxActor.Tell(new Issue() { IsBug = true });
            mailBoxActor.Tell(new Issue());
            mailBoxActor.Tell(new Issue() { IsSecurityFlaw = true });
            mailBoxActor.Tell(new Issue() { IsBug = true });
            mailBoxActor.Tell(new Issue() { IsBug = true });
            mailBoxActor.Tell(new Issue() { IsSecurityFlaw = true });
            mailBoxActor.Tell(new Issue());

            for (int i = 0; i < testCount; i++)
            {
                var issue = testProbe.ExpectMsg<Issue>();
                var jsonString = JsonSerializer.Serialize(issue);
                output.WriteLine($"Issue: {jsonString}");


                if (isPerformTest)
                {
                    _dictionary.Add(_key++, _key);
                    _addCounter.Increment();
                }
            }
        });

    }
}
