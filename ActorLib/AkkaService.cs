using Akka.Actor;

namespace ActorLib
{
    public class AkkaService
    {
        private ActorSystem actorSystem;

        public void CreateActorSystem(string name)
        {
            actorSystem = ActorSystem.Create(name);
        }

        public void FromActorSystem(ActorSystem _actorSystem) 
        {
            actorSystem = _actorSystem;
        }


        public ActorSystem GetActorSystem()
        {
            return actorSystem;
        }


    }
}
