using Akka.Actor;
using Akka.Configuration;

namespace ActorLib
{
    public class AkkaService
    {        
        private Dictionary<string, ActorSystem> actorSystems = new Dictionary<string, ActorSystem>();

        private Dictionary<string, IActorRef> actors = new Dictionary<string, IActorRef>();

        public ActorSystem CreateActorSystem(string name, int port = 0)
        {
            if (!actorSystems.ContainsKey(name))
            {
                if (port == 0)
                {
                    // Load from App.config
                    actorSystems[name] = ActorSystem.Create(name);
                }
                else
                {
                    // Note :
                    // 포트가 지정된경우 리모트로 작동되며 다양한 Actor 작동을 커스텀 설정합니다.
                    string akkaConfig = @"
                    akka {
                        loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]	
	                    actor {
                            provider = remote	
	                    }
	
                        remote {
                            dot-netty.tcp {
                                port = $port
                                hostname = ""127.0.0.1""
                            }
                        }	
                    }
                    my-custom-mailbox {
                        mailbox-type : ""ActorLib.Actor.Test.IssueTrackerMailbox, ActorLib""
                    }
                    ";

                    akkaConfig = akkaConfig.Replace("$port", port.ToString());
                    var config = ConfigurationFactory.ParseString(akkaConfig);
                    actorSystems[name] = ActorSystem.Create(name, config);
                }                
            }
            else
            {
                throw new Exception($"{name} actorsystem has already been created. ");
            }

            return actorSystems[name];
        }

        public void SetDeafaultSystem(ActorSystem _actorSystem) 
        {
            if (actorSystems.Count == 0)
            {
                actorSystems["default"] = _actorSystem;
            }
            else
            {
                throw new Exception("The actor system has already been created.");
            }
        }

        public ActorSystem GetActorSystem(string name = "default")
        {
            ActorSystem firstOrDefault = null;

            if (!actorSystems.ContainsKey(name))
            {
                if (string.IsNullOrEmpty(name))
                {
                    firstOrDefault = CreateActorSystem("ActorSystem");
                }
                else
                {
                    firstOrDefault = CreateActorSystem(name);
                }
            }
            else
            {
                firstOrDefault = actorSystems[name];
            }
            
            return firstOrDefault;
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
            if (actors.ContainsKey(name))
            {
                return actors[name];
            }
            else
            {
                return null;
            }            
        }
    }
}
