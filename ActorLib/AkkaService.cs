using Akka.Actor;

using static Akka.Streams.Attributes;

namespace ActorLib
{
    public class AkkaService
    {
        private ActorSystem actorSystem;

        public void CreateActorSystem(string name)
        {
            if (actorSystem == null)
            {
                actorSystem = ActorSystem.Create(name);
            }
            else
            {
                throw new Exception("The actor system has already been created.");
            }                
        }

        public void FromActorSystem(ActorSystem _actorSystem) 
        {
            if (actorSystem == null)
            {
                actorSystem = _actorSystem;
            }
            else
            {
                throw new Exception("The actor system has already been created.");
            }

        }


        public ActorSystem GetActorSystem()
        {
            if (actorSystem == null)
            {
                actorSystem = ActorSystem.Create("ActorSystem");
            }

            return actorSystem;
        }
    }
}
