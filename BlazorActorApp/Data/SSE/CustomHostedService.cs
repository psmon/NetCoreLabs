using System.Text;
using System.Text.Json;

namespace BlazorActorApp.Data.SSE
{
    public sealed class CustomHostedService :
    IHostedService, IAsyncDisposable
    {
        private readonly INotificationRepository _notificationRepository;
        private Timer? _timer;
        public CustomHostedService(INotificationRepository notificationRepository)
            => _notificationRepository = notificationRepository;


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendMessage, null,
              TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }
        private void SendMessage(object? state)
        {            
            using var client = new HttpClient();
            new Uri("http://localhost:8080/" + "api/sse");
            var notifications = _notificationRepository.
              GetNotifications().Result;

            foreach (var notification in notifications)
            {
                if (!notification.IsProcessed)
                {
                    HttpContent body = new StringContent(JsonSerializer.
                      Serialize(notification), Encoding.UTF8, "application/json");
                    var response = client.PostAsync("http://localhost:8080/api/sse/" +
                      "postmessage", body).Result;
                }
            }
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
        }
        public async ValueTask DisposeAsync()
        {
            _timer.Dispose();
        }
    }
}
