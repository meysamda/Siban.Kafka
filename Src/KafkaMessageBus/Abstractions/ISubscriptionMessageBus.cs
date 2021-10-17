using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaMessageBus.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        Task Subscribe(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            ISubscribeOptions<string, string> options = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            ISubscribeOptions<string, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            ISubscribeOptions<TKey, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken));

        // -----

        Task Subscribe(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken));
        
        Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken));

        Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken));

        // -----

        Task Subscribe<TMessageProcessor>(
            IEnumerable<string> topics,
            ISubscribeOptions<string, string> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<string>;

        Task Subscribe<TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            ISubscribeOptions<string, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            ISubscribeOptions<TKey, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>;

        // -----

        Task Subscribe<TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<string>;
        
        Task Subscribe<TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>;
    }
}