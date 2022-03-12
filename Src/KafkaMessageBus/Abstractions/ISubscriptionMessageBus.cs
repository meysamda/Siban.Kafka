using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        Task Subscribe(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken));
        
        Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken));

        // -----

        Task Subscribe(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            IConsumer<string, string> consumer,
            CancellationToken cancellationToken = default(CancellationToken));

        Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            IConsumer<string, TMessage> consumer,
            CancellationToken cancellationToken = default(CancellationToken));

        Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            IConsumer<TKey, TMessage> consumer,
            CancellationToken cancellationToken = default(CancellationToken));

        IConsumer<TKey, TMessage> GetConsumer<TKey, TMessage>(ISubscribeOptions<TKey, TMessage> options);
        ISubscribeOptions<TKey, TMessage> GetDefaultSubscribeOptions<TKey, TMessage>(Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null);
    }
}