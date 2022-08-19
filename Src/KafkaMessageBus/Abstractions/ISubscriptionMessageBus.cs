using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        Task SubscribeAsync(
            IEnumerable<string> topics,
            Func<string, Dictionary<string, byte[]>, Task> messageProcessor,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);
        
        Task SubscribeAsync<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Dictionary<string, byte[]>, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);

        Task SubscribeAsync<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Dictionary<string, byte[]>, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);

        IConsumer<TKey, TMessage> GetConfluentKafkaConsumer<TKey, TMessage>(ISubscribeOptions<TKey, TMessage> options = null);
        ISubscribeOptions<TKey, TMessage> GetDefaultSubscribeOptions<TKey, TMessage>(Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null);
    }
}