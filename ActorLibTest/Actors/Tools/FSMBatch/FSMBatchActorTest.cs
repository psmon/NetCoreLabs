using System.Collections.Immutable;
using ActorLib.Actors.Tools.FSMBatch;
using Akka.Actor;
using FluentAssertions;
using Xunit.Abstractions;

namespace ActorLibTest.Actors.Tools.FSMBatch;

public class FSMBatchActorTest : TestKitXunit
{
    public FSMBatchActorTest(ITestOutputHelper output) : base(output)
    {
        // doc : https://getakka.net/articles/actors/finite-state-machine.html
    }

    [Fact]
    public void Simple_finite_state_machine_must_batch_correctly()
    {
        var buncher = Sys.ActorOf(Props.Create<FSMBatchActor>( ));
        buncher.Tell(new SetTarget(TestActor));
        buncher.Tell(new Queue(42));
        buncher.Tell(new Queue(43));
        ExpectMsg<Batch>().Obj.Should().BeEquivalentTo(ImmutableList.Create(42, 43));
        buncher.Tell(new Queue(44));
        buncher.Tell(new Flush());
        buncher.Tell(new Queue(45));
        ExpectMsg<Batch>().Obj.Should().BeEquivalentTo(ImmutableList.Create(44));
        ExpectMsg<Batch>().Obj.Should().BeEquivalentTo(ImmutableList.Create(45));

    }
    
}