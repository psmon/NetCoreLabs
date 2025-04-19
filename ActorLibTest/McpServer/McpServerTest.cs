using System.Text;

using Akka.Actor;
using Akka.TestKit;
using McpServer.Actor;
using McpServer.Actor.Model;
using McpServer.Persistent.Model;
using Raven.Client.Documents;
using Xunit.Abstractions;

namespace ActorLibTest.McpServer;

public class McpServerTest : TestKitXunit
{
    public McpServerTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact(DisplayName = "AddNoteAreOk")]
    public void AddNoteAreOk()
    {
        var actorSystem = _akkaService.GetActorSystem();
        
        TestProbe testProbe = this.CreateTestProbe(actorSystem);
        
        TestProbe testProbeHistory = this.CreateTestProbe(actorSystem);
        
        var recoedActor = actorSystem.ActorOf(Props.Create(() => new RecordActor()));
        
        var historyActor = actorSystem.ActorOf(Props.Create(() => new HistoryActor()));
        
        recoedActor.Tell(testProbe.Ref);

        testProbe.ExpectMsg("done-ready");
        
        historyActor.Tell(testProbeHistory.Ref);
            
        testProbeHistory.ExpectMsg("done-ready");
        
        recoedActor.Tell(new SetHistoryActorCommand()
        {
            HistoryActor = historyActor
        });
        
        testProbe.ExpectMsg("done-set-history");
        
        Within(TimeSpan.FromMilliseconds(3000), () =>
        {
            recoedActor.Tell(new AddNoteCommand()
            {
                Content = "test",
                Category = "test",
                Latitude = 37.7749,
                Longitude = -122.4194,
                Title = "test",
                TagsEmbeddedAsSingle = new RavenVector<float>(new float[] { 1.0f, 2.0f, 3.0f })
            });
            
            testProbe.ExpectMsg("done-add");
            
            recoedActor.Tell(new AddNoteCommand()
            {
                Content = "이 컨텐츠는 한글컨텐츠",
                Category = "자유게시판",
                Latitude = null,
                Longitude = null,
                Title = null,
                TagsEmbeddedAsSingle = new RavenVector<float>(new float[] { 1.0f, 2.0f, 3.0f })
            });
            
            testProbe.ExpectMsg("done-add");    // From RecordActor
            
            historyActor.Tell(new GetNoteHistoryCommand());

            testProbeHistory.ExpectMsg<SearchNoteActorResult>();

        });
    }
    
    [Fact(DisplayName = "SearchNoteAreOk")]
    public void SearchNoteAreOk()
    {
        var actorSystem = _akkaService.GetActorSystem();
        
        TestProbe testProbe = this.CreateTestProbe(actorSystem);
        
        TestProbe testProbeHistory = this.CreateTestProbe(actorSystem);
        
        var searchActor = actorSystem.ActorOf(Props.Create(() => new SearchActor()));
        searchActor.Tell(testProbe.Ref);
        testProbe.ExpectMsg("done-ready");
        
        var historyActor = actorSystem.ActorOf(Props.Create(() => new HistoryActor()));
        historyActor.Tell(testProbeHistory.Ref);
        testProbeHistory.ExpectMsg("done-ready");
        
        searchActor.Tell(new SetHistoryActorCommand()
        {
            HistoryActor = historyActor
        });
        testProbe.ExpectMsg("done-set-history");
        

        Within(TimeSpan.FromMilliseconds(3000), () =>
        {
            searchActor.Tell(new SearchNoteByTextCommand()
            {
                Title = "",
                Content = "",
                Category = "자유게시판"
            });
            
            var result = testProbe.ExpectMsg<SearchNoteActorResult>();
            
            output.WriteLine("============ SearchNoteByTextCommand");
            
            // Output results
            foreach (var note in result.Notes)
            {
                output.WriteLine($"Title: {note.Title}, Content: {note.Content}, Category: {note.Category}" +
                                 $" Latitude: {note.Latitude}, Longitude: {note.Longitude}");
            }
            
            historyActor.Tell(new GetNoteSearchHistoryCommand());
            var resultHistory = testProbeHistory.ExpectMsg<SearchNoteActorResult>();
            
            output.WriteLine("============ GetNoteHistoryCommand");
            
            // Output results
            foreach (var note in resultHistory.Notes)
            {
                output.WriteLine($"Title: {note.Title}, Content: {note.Content}, Category: {note.Category}" +
                                 $" Latitude: {note.Latitude}, Longitude: {note.Longitude}");
            }
            

            Assert.NotNull(result.Notes);
        });
    }
    
    [Fact(DisplayName = "SearchNoteByRadiusAreOk")]
    public void SearchNoteByRadiusAreOk()
    {
        var actorSystem = _akkaService.GetActorSystem();
        
        TestProbe testProbe = this.CreateTestProbe(actorSystem);

        TestProbe testProbeHistory = this.CreateTestProbe(actorSystem);

        var searchActor = actorSystem.ActorOf(Props.Create(() => new SearchActor()));        
        searchActor.Tell(testProbe.Ref);
        testProbe.ExpectMsg("done-ready");

        var historyActor = actorSystem.ActorOf(Props.Create(() => new HistoryActor()));
        historyActor.Tell(testProbeHistory.Ref);
        testProbeHistory.ExpectMsg("done-ready");

        searchActor.Tell(new SetHistoryActorCommand()
        {
            HistoryActor = historyActor
        });
        testProbe.ExpectMsg("done-set-history");


        Within(TimeSpan.FromMilliseconds(3000), () =>
        {
            searchActor.Tell(new SearchNoteByRadiusActorCommand()
            {
                Latitude = 35.2271,
                Longitude = 128.6812,
                Radius = 30000
            });
            
            var result = testProbe.ExpectMsg<SearchNoteActorResult>();

            output.WriteLine("============ SearchNoteByRadiusActorCommand");

            // Output results
            foreach (var note in result.Notes)
            {
                output.WriteLine($"Title: {note.Title}, Content: {note.Content}, Category: {note.Category}" +
                                 $" Latitude: {note.Latitude}, Longitude: {note.Longitude}");
            }

            historyActor.Tell(new GetNoteSearchHistoryCommand());
            var resultHistory = testProbeHistory.ExpectMsg<SearchNoteActorResult>();

            output.WriteLine("============ GetNoteSearchHistoryCommand");

            // Output results
            foreach (var note in resultHistory.Notes)
            {
                output.WriteLine($"Title: {note.Title}, Content: {note.Content}, Category: {note.Category}" +
                                 $" Latitude: {note.Latitude}, Longitude: {note.Longitude}");
            }

            Assert.NotNull(result.Notes);
        });
    }
    
    [Fact(DisplayName = "SearchNoteByVectorAreOk")]
    public void SearchNoteByVectorAreOk()
    {
        var actorSystem = _akkaService.GetActorSystem();
        
        TestProbe testProbe = this.CreateTestProbe(actorSystem);
        
        var searchActor = actorSystem.ActorOf(Props.Create(() => new SearchActor()));
        
        searchActor.Tell(testProbe.Ref);

        testProbe.ExpectMsg("done-ready");

        Within(TimeSpan.FromMilliseconds(3000), () =>
        {
            searchActor.Tell(new SearchNoteByVectorCommand()
            {
                Vector = new float[] { 1.0f, 2.0f, 3.0f },
                TopN = 10
            });
            
            var result = testProbe.ExpectMsg<SearchNoteActorResult>();
            
            // Output results
            foreach (var note in result.Notes)
            {
                output.WriteLine($"Title: {note.Title}, Content: {note.Content}, Category: {note.Category}" +
                                 $" Latitude: {note.Latitude}, Longitude: {note.Longitude}");
            }

            Assert.NotNull(result.Notes);
        });
    }
}