using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Siban.Kafka.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        Task SubscribeForMessageValueAsync<TValue>(
            IEnumerable<string> topics,
            Func<TValue, Task> handleMethod,
            Action<ISubscribeOptions<string, TValue>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);

        Task SubscribeForMessageAsync<TKey, TValue>(
            IEnumerable<string> topics,
            Func<Message<TKey, TValue>, Task> handleMethod,
            Action<ISubscribeOptions<TKey, TValue>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);
    }
}