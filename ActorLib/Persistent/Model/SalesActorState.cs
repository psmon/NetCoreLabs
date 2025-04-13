namespace ActorLib.Persistent.Model;

// A sale EVENT to be persisted 
public class Sale(long pricePaid, string productBrand)
{
    public long Price { get; set; } = pricePaid;
    public string Brand { get; set; } = productBrand;
}

// MESSAGES for the simulator actor
public class StartSimulate { }
public class StopSimulate { }

// Internal state that will be persisted in a SNAPSHOT 
class SalesActorState
{
    public long totalSales { get; set; }

    public override string ToString()
    {
        return $"[SalesActorState: Total sales are {totalSales}]";
    }
}

public class ConsoleHelper
{
    public static void WriteToConsole(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}