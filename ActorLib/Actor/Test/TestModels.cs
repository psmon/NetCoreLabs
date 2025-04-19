
namespace ActorLib.Actor.Test;

public class DelayCommand
{
    public int Delay { get; set; }

    public string Message { get; set; }
    public DelayCommand(string message, int delay) 
    {
        Delay = delay;
        Message = message;
    }
}

public class MessageCommand
{
    public string Message { get; set; }

    public MessageCommand(string message) 
    { 
        Message = message;
    }
}

public class RemoteCommand
{
    public string Message { get; set; }

    public RemoteCommand(string message)
    {
        Message = message;
    }
}
