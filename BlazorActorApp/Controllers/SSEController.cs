using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

using ActorLib;

using Akka.Actor;
using Akka.Streams.Implementation.Fusing;

using BlazorActorApp.Data.Actor;
using BlazorActorApp.Data.SSE;
using BlazorActorApp.Logging;

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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }

        [HttpGet("message/{identy}")]
        public async Task<ActionResult> GetMessageByActor(string identy)
        {            
            var stringBuilder = new StringBuilder();

            string actorName = $"{identy}-Actor";

            IActorRef myActor = AkkaService.GetActor(actorName);
            if (myActor == null)
            {
                myActor = AkkaService.GetActorSystem().ActorOf(Props.Create<UserActor>());
                AkkaService.AddActor(actorName, myActor);

                myActor.Tell(new Notification()
                {
                    Id = identy,
                    MessageTime = DateTime.Now,
                    Message = "웰컴메시지.. sent by sse",
                    IsProcessed = false
                });
            }

            object message = await myActor.Ask(new CheckNotification(), TimeSpan.FromSeconds(3));            

            if (message is Notification)
            {
                var serializedData = JsonSerializer.Serialize(message as Notification);
                stringBuilder.AppendFormat("data: {0}\n\n", serializedData);
                return Content(stringBuilder.ToString(), "text/event-stream");
            }
            else if (message is EmptyNotification)
            {
                var serializedData = JsonSerializer.Serialize(new EmptyNotification());
                stringBuilder.AppendFormat("data: null", serializedData);
                return Content(stringBuilder.ToString(), "text/event-stream");
            }
            else
            {
                var typeName = message.GetType().Name;
                int x = 99;
                stringBuilder.AppendFormat("data: null \n\n");
                return Content(stringBuilder.ToString(), "text/event-stream");
            }
        }

        [HttpGet("noti-test/{identy}")]
        public async Task<IActionResult> notificationTest(string identy)
        {
            try
            {
                string actorName = $"{identy}-Actor";

                IActorRef myActor = AkkaService.GetActor(actorName);
                myActor.Tell(new Notification()
                {
                    Id = identy,
                    IsProcessed = false,
                    Message = "SSE TEST 메시지",
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
