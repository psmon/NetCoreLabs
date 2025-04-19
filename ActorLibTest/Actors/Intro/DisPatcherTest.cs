﻿
using ActorLib.Actor.Test;
using Akka.Actor;
using Akka.Routing;
using Akka.TestKit;
using Xunit.Abstractions;

namespace ActorLibTest.Actors.Intro;

public class DisPatcherTest : TestKitXunit
{        

    public DisPatcherTest(ITestOutputHelper output) : base(output)
    {            
    }

    [Theory(DisplayName = "Dispatcher - Thread")]
    [InlineData(3, "synchronized-dispatcher")]
    [InlineData(3, "fork-join-dispatcher")]
    [InlineData(3, "custom-dispatcher")]
    [InlineData(3, "custom-task-dispatcher")]
    [InlineData(3, "custom-dedicated-dispatcher")]        
    public void DispatcherTestAreOK(int nodeCount, string disPatcherName)
    {
        var actorSystem = _akkaService.GetActorSystem();

        TestProbe testProbe = this.CreateTestProbe(actorSystem);

        var props = new RoundRobinPool(nodeCount)
            .WithDispatcher(disPatcherName)
            .Props(Props.Create(() => new BasicActor()));

        var actor = actorSystem.ActorOf(props, "worker");

        for (int i = 0; i < nodeCount; i++)
        {
            actor.Tell(testProbe.Ref);

            testProbe.ExpectMsg("done");
        }

        int givenTestCount = 1000;

        int givenBlockTimePerTest = 10;

        int cutOff = givenTestCount * givenBlockTimePerTest;

        Within(TimeSpan.FromMilliseconds(cutOff), () =>
        {
            for (int i = 0; i < givenTestCount; i++)
            {
                actor.Tell( new DelayCommand("slowCommand" + i, 10));
            }

            for (int i = 0; i < givenTestCount; i++)
            {
                string resultMessage = testProbe.ExpectMsg<string>();                    
            }
        });
    }
}
