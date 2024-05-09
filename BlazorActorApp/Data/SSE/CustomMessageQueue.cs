using static Akka.IO.Tcp;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace BlazorActorApp.Data.SSE
{
    public class CustomMessageQueue : ICustomMessageQueue
    {
        private ConcurrentDictionary<string, Channel<string>> _concurrentDictionary;

        public CustomMessageQueue()
        {
            _concurrentDictionary = new ConcurrentDictionary<string,
              Channel<string>>();
        }

        public void Register(string id)
        {
            bool success = _concurrentDictionary.TryAdd
              (id, Channel.CreateUnbounded<string>());

            if (!success)
            {
                throw new ArgumentException($"The client Id {id} is already registered");
            }
        }

        public ICollection<string> Keys
        {
            get { return _concurrentDictionary.Keys; }
        }
        public void Deregister(string id)
        {
            _concurrentDictionary.TryRemove(id, out _);
        }

        public async Task EnqueueAsync(Notification notification,
          CancellationToken cancelToken)
        {
            bool success = _concurrentDictionary.TryGetValue(notification.Id,
                out Channel<string> channel
            );

            if (!success)
            {
                throw new ArgumentException($"Error encountered " +
                  $"when adding a new message to the queue.");

            }
            else
            {
                await channel.Writer.WriteAsync(notification.Message,
                  cancelToken);
            }
        }

        public IAsyncEnumerable<string> DequeueAsync(string id,
          CancellationToken cancelToken = default)
        {
            bool success = _concurrentDictionary.TryGetValue(id,
              out Channel<string> channel);

            if (success)
            {
                return channel.Reader.ReadAllAsync(cancelToken);
            }
            else
            {
                throw new ArgumentException($"The client Id {id} isn't registered");
            }
        }

        public async IAsyncEnumerable<string> DequeueAsync(CancellationToken
          cancelToken = default)
        {
            IAsyncEnumerable<string> result;

            foreach (var keyValuePair in _concurrentDictionary)
            {
                await foreach (string str in DequeueAsync(keyValuePair.Key,
                  cancelToken))
                {
                    yield return str;
                }
            }
        }
    }
}
