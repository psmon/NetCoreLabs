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
        private readonly BlockingCollection<string> _producerConsumerCollection = new BlockingCollection<string>();

        private readonly ICustomMessageQueue _messageQueue;

        private AkkaService AkkaService { get; set; }

        
        public SSEController(ICustomMessageQueue messageQueue, AkkaService actorSystem )
        {
            AkkaService = actorSystem;        

            for (int i = 0; i < 10; i++)
            {
                //_producerConsumerCollection.Add(string.Format("The product code is: {0}\n", Guid.NewGuid().ToString()));
            }
            _messageQueue = messageQueue;
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

        [HttpGet("subscribe/{id}")]
        public async Task<IActionResult> Subscribe(string id)
        {
            Response.StatusCode = 200;
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            Response.Headers.Add("Content-Type", "text/event-stream");

            try
            {
                Notification notification = new Notification();
                notification.Id = id;

                notification.Message = $"Subscribed to " + $"client {id}";

                _messageQueue.Register(id);
                StreamWriter streamWriter = new StreamWriter(Response.Body);
                await _messageQueue.EnqueueAsync(notification,
                  HttpContext.RequestAborted);

                await foreach (var message in _messageQueue.DequeueAsync
                  (id, HttpContext.RequestAborted))
                {
                    await streamWriter.WriteLineAsync
                      ($"Message received: {message} at {DateTime.Now}");
                    await streamWriter.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                _messageQueue.Deregister(id);
            }

            return Ok();
        }

        [HttpGet("getmessage/{id}")]
        public async Task<IActionResult> GetMessage(string id)
        {
            Response.StatusCode = 200;
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            Response.Headers.Add("Content-Type", "text/event-stream");

            try
            {
                StreamWriter streamWriter = new StreamWriter(Response.Body);
                await foreach (var message in _messageQueue.DequeueAsync(id))
                {
                    await streamWriter.WriteLineAsync(
                      $"Client Id: {id} Message: {message}" +
                      $" Time: {DateTime.Now}");
                    await streamWriter.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                _messageQueue.Deregister(id);
            }

            return Ok();
        }

        [HttpGet("getmessages")]
        public async Task<IActionResult> GetMessages()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            Response.StatusCode = 200;

            try
            {
                StreamWriter streamWriter = new StreamWriter(Response.Body);
                await foreach (var message in _messageQueue.DequeueAsync())
                {
                    await streamWriter.WriteLineAsync($"{DateTime.Now} {message}");
                    await streamWriter.FlushAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult>
        PostMessage([FromBody] Notification notification)
        {
            try
            {
                _messageQueue.Register(notification.Id);
                await _messageQueue.EnqueueAsync(notification,
                  HttpContext.RequestAborted);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
