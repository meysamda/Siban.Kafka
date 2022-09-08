using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Siban.Kafka.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        Task SubscribeAsync<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, CancellationToken, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);

        Task SubscribeAsync<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<Message<TKey, TMessage>, CancellationToken, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);
    }
}