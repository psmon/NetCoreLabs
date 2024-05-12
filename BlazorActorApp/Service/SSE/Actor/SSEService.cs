using ActorLib;

using Akka.Actor;

using BlazorActorApp.Data.SSE;

namespace BlazorActorApp.Service.SSE.Actor
{
    public class SSEService
    {
        private AkkaService AkkaService { get; set; }

        public SSEService(AkkaService actorSystem)
        {
            AkkaService = actorSystem;
        }

        private async Task<IActorRef> findUserByIdenty(string actorName)
        {
            IActorRef myActor = AkkaService.GetActor(actorName);
            if (myActor == null)
            {
                myActor = AkkaService.GetActorSystem().ActorOf(Props.Create<SSEUserActor>(actorName));
                AkkaService.AddActor(actorName, myActor);
            }
            return myActor;
        }

        public async Task<object> CheckNotification(string actorName) 
        {
            var myActor = await findUserByIdenty(actorName);
            return await myActor.Ask(new CheckNotification(), TimeSpan.FromSeconds(3));
        }

        public async Task PushNotification(string actorName, Notification noti)
        {
            var myActor = await findUserByIdenty(actorName);
            myActor.Tell(noti);
        }

    }
}
