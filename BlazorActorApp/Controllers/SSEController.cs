﻿using System.Text;
using System.Text.Json;

using ActorLib;

using Akka.Actor;
using BlazorActorApp.Data.SSE;
using BlazorActorApp.Service.SSE.Actor;
using Microsoft.AspNetCore.Mvc;

namespace BlazorActorApp.Controllers
{
    [Route("api/sse")]
    public class SSEController : Controller
    {        

        private AkkaService AkkaService { get; set; }

        
        public SSEController(AkkaService actorSystem )
        {
            AkkaService = actorSystem;        
        }

        [HttpGet("message/{identy}")]
        public async Task<ActionResult> GetMessageByActor(string identy)
        {            
            var stringBuilder = new StringBuilder();

            string actorName = $"{identy}-SSE";

            IActorRef myActor = AkkaService.GetActor(actorName);
            if (myActor == null)
            {
                myActor = AkkaService.GetActorSystem().ActorOf(Props.Create<SSEUserActor>(actorName));
                AkkaService.AddActor(actorName, myActor);
            }

            object message = await myActor.Ask(new CheckNotification(), TimeSpan.FromSeconds(3));            

            if (message is Notification)
            {
                var serializedData = JsonSerializer.Serialize(message as Notification);
                stringBuilder.AppendFormat("data: {0}\n\n", serializedData);
                return Content(stringBuilder.ToString(), "text/event-stream");
            }
            else if (message is HeartBeatNotification)
            {
                var serializedData = JsonSerializer.Serialize(new HeartBeatNotification());
                stringBuilder.AppendFormat("data: null", serializedData);
                return Content(stringBuilder.ToString(), "text/event-stream");
            }
            else
            {
                var typeName = message.GetType().Name;                
                stringBuilder.AppendFormat("data: null \n\n");
                return Content(stringBuilder.ToString(), "text/event-stream");
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> notificationTest(string identy, string message)
        {
            try
            {
                string actorName = $"{identy}-Actor";

                IActorRef myActor = AkkaService.GetActor(actorName);
                myActor.Tell(new Notification()
                {
                    Id = identy,
                    IsProcessed = false,
                    Message = message,
                    MessageTime = DateTime.Now,                    
                });
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
