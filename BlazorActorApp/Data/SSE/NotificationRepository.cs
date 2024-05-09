namespace BlazorActorApp.Data.SSE
{
    public class NotificationRepository : INotificationRepository
    {
        private List<Notification> _notifications = new List<Notification>();

        public NotificationRepository()
        {
            _notifications.Add(new Notification
            {
                Id = "1",
                Message = "This is the first message",
                MessageTime = DateTime.Now
            });

            _notifications.Add(new Notification
            {
                Id = "2",
                Message = "This is the second message",
                MessageTime = DateTime.Now
            });
        }

        public async Task<List<Notification>> GetNotifications()
        {
            return await Task.FromResult(_notifications);
        }

        public async Task<Notification> GetNotification(string Id)
        {
            return await Task.FromResult(_notifications
              .FirstOrDefault(x => x.Id == Id));
        }

        public async Task AddNotification
        (Notification notification)
        {
            _notifications.Add(notification);
        }
    }
}
