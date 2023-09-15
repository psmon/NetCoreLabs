﻿using Akka.Actor;

using static Akka.Streams.Attributes;

namespace ActorLib
{
    public class AkkaService
    {
        private ActorSystem actorSystem;

        private Dictionary<string, IActorRef> actors = new Dictionary<string, IActorRef>();

        public ActorSystem CreateActorSystem(string name)
        {
            if (actorSystem == null)
            {
                actorSystem = ActorSystem.Create(name);
            }
            else
            {
                throw new Exception("The actor system has already been created.");
            }

            return actorSystem;
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

        public void AddActor(string name, IActorRef actor) 
        {
            if (!actors.ContainsKey(name))
            {
                actors[name] = actor;
            }
        }

        public IActorRef GetActor(string name) 
        { 
            return actors[name];
        }
    }
}
