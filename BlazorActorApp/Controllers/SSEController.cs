using System.Text;
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

        private SSEService SSEService { get; set; }

        private string PreFix = "sse-";
        
        public SSEController(SSEService sseService)
        {
            SSEService = sseService;        
        }

        [HttpGet("message/{identy}")]
        public async Task<ActionResult> GetMessageByActor(string identy)
        {            
            var stringBuilder = new StringBuilder();
            string actorName = $"{PreFix}{identy}";

            object message = await SSEService.CheckNotification(actorName);

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
        public async Task<IActionResult> PushNotification(string identy, string message)
        {
            try
            {
                string actorName = $"{PreFix}{identy}";
                var noti = new Notification()
                {
                    Id = identy,
                    IsProcessed = false,
                    Message = message,
                    MessageTime = DateTime.Now,
                };

                await SSEService.PushNotification(actorName, noti);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
