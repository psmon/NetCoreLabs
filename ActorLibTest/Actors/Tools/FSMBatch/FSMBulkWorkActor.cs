using ActorLib.Actor.Tools.FSMBatch;
using ActorLib.Persistent;
using Akka.Actor;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;

namespace ActorLibTest.Actors.Tools.FSMBatch;

public class FSMBulkWorkActor : ReceiveActor
{
    private IActorRef? testProbe;
    
    private IDocumentStore _store;
    private TravelReviewRepository _repository;

    private void InitDataBase()
    {
        // RavenDB In-Memory Document Store 설정
        _store = new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:9000" },
            Database = "net-core-labs"
        };
        _store.Initialize();

        _repository = new TravelReviewRepository(_store);
    }

    public FSMBulkWorkActor()
    {
        InitDataBase();

        Receive<IActorRef>(actorRef =>
        {
            testProbe = actorRef;
            testProbe.Tell("done");
        });
        
        Receive<Batch>(msg =>
        {
            using (BulkInsertOperation bulkInsert = _store.BulkInsert())
            {
            }
            
            if (testProbe != null)
            {
                testProbe.Tell(msg);
            }
        });
    }
}