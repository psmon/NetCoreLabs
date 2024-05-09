namespace BlazorActorApp.Data.SSE
{
    public interface ICustomMessageQueue
    {
        void Register(string id);
        void Deregister(string id);
        ICollection<string> Keys { get; }
        IAsyncEnumerable<string>
          DequeueAsync(string id, CancellationToken cancelToken = default);
        IAsyncEnumerable<string>
          DequeueAsync(CancellationToken cancelToken = default);
        Task EnqueueAsync(Notification notification, CancellationToken cancelToken);
    }
}
