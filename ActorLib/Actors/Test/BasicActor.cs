using Akka.Actor;
using Akka.Event;

namespace ActorLib.Actors.Test
{
    public class BasicActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        public BasicActor()
        {
            ReceiveAsync<string>(async msg =>
            {
                logger.Info($"{msg} 를 전송받았습니다.");

                if (msg == "slowCommand")
                {
                    await Task.Delay(1000);
                }

                if (Sender != null)
                {
                    Sender.Tell("ok");
                }
            });
        }
    }
}
