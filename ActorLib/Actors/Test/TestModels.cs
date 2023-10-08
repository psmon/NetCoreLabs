namespace ActorLib.Actors.Test
{
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
}
