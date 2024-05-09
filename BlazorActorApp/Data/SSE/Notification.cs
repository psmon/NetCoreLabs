namespace BlazorActorApp.Data.SSE
{
    public class Notification
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime MessageTime { get; set; } = DateTime.Now;

        public bool IsProcessed { get; set; }
    }

    public interface INotificationRepository
    {
        public Task<List<Notification>> GetNotifications();
        public Task<Notification> GetNotification(string Id);
        public Task AddNotification(Notification notification);
    }
}
