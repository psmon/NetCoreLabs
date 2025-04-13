using ActorLib.Persistent.Model;
using Akka.Actor;
using Akka.Persistence;

namespace ActorLib.Persistent.Actor;

public class SalesActor: ReceivePersistentActor
{
    // The unique actor id
    public override string PersistenceId => "sales-actor";
    
    // The state that will be persisted in SNAPSHOTS
    private SalesActorState _state;
    
    public SalesActor(long expectedProfit, TaskCompletionSource<bool> taskCompletion)
    {
        _state = new SalesActorState
        {
            totalSales = 0
        }; 
        
        // Process a sale:
        Command<Sale>(saleInfo =>
        {
            if (_state.totalSales < expectedProfit)
            {
                // Persist an EVENT to RavenDB
                // ===========================
                
                // The handler function is executed after the EVENT was saved successfully
                Persist(saleInfo, _ =>
                {
                    // Update the latest state in the actor
                    _state.totalSales += saleInfo.Price;

                    ConsoleHelper.WriteToConsole(ConsoleColor.Black,
                        $"Sale was persisted. Phone brand: {saleInfo.Brand}. Price: {saleInfo.Price}");

                    // Store a SNAPSHOT every 5 sale events
                    // ====================================
                    
                    if (LastSequenceNr != 0 && LastSequenceNr % 5 == 0)
                    {
                        SaveSnapshot(_state.totalSales);
                    }
                });
            }
            else if (!taskCompletion.Task.IsCompleted)
            {
                Sender.Tell(new StopSimulate());
                
                ConsoleHelper.WriteToConsole(ConsoleColor.DarkMagenta,
                    $"Sale not persisted: " +
                    $"Total sales have already reached the expected profit of {expectedProfit}");
                
                ConsoleHelper.WriteToConsole(ConsoleColor.DarkMagenta,
                    _state.ToString());
                
                taskCompletion.TrySetResult(true);
            }
        });
        
        // Handle a SNAPSHOT success msg
        Command<SaveSnapshotSuccess>(success =>
        {
            ConsoleHelper.WriteToConsole(ConsoleColor.Blue,
                $"Snapshot saved successfully at sequence number {success.Metadata.SequenceNr}");
            
            // Optionally, delete old snapshots or events here if needed
            // DeleteMessages(success.Metadata.SequenceNr);
        });
        
        // Recover an EVENT
        Recover<Sale>(saleInfo =>
        {
            _state.totalSales += saleInfo.Price;
            
            ConsoleHelper.WriteToConsole(ConsoleColor.DarkGreen,
                $"Event was recovered. Price: {saleInfo.Price}");
        });
        
        // Recover a SNAPSHOT
        Recover<SnapshotOffer>(offer =>
        {
            var salesFromSnapshot = (long) offer.Snapshot;
            _state.totalSales = salesFromSnapshot;
            
            ConsoleHelper.WriteToConsole(ConsoleColor.DarkGreen,
                $"Snapshot was recovered. Total sales from snapshot: {salesFromSnapshot}");
        });
    }
}