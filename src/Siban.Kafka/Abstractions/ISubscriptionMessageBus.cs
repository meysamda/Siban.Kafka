using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Siban.Kafka.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        Task SubscribeAsync<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TKey, TMessage, Headers, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);

        IConsumer<TKey, TMessage> GetConfluentKafkaConsumer<TKey, TMessage>(ISubscribeOptions<TKey, TMessage> options = null);
        ISubscribeOptions<TKey, TMessage> GetDefaultSubscribeOptions<TKey, TMessage>(Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null);
    }
}