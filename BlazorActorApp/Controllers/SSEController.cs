using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

using BlazorActorApp.Data.SSE;

using Microsoft.AspNetCore.Mvc;

namespace BlazorActorApp.Controllers
{
    [Route("api/sse")]
    public class SSEController : Controller
    {
        private readonly BlockingCollection<string> _producerConsumerCollection = new BlockingCollection<string>();

        private readonly ICustomMessageQueue _messageQueue;

        public SSEController(ICustomMessageQueue messageQueue)
        {
            for (int i = 0; i < 10; i++)
            {
                _producerConsumerCollection.Add(string.Format("The product code is: {0}\n", Guid.NewGuid().ToString()));
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

        [HttpGet("message")]
        public ActionResult GetMessage()
        {
            var result = string.Empty;
            var stringBuilder = new StringBuilder();

            if (_producerConsumerCollection.TryTake(out result,
              TimeSpan.FromMilliseconds(1000)))
            {
                var serializedData = JsonSerializer.Serialize(
                  new { message = result });
                stringBuilder.AppendFormat("data: {0}\n\n", serializedData);
            }

            return Content(stringBuilder.ToString(), "text/event-stream");
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

        [HttpGet("noti-test")]
        public async Task<IActionResult> notificationTest()
        {
            try
            {
                Notification notification = new Notification()
                {
                    Id = "a",
                    IsProcessed = true,
                    Message = "test",
                    MessageTime = DateTime.Now,                    
                };

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
