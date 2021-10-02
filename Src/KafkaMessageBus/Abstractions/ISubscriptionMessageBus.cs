using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaMessageBus.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        Task Subscribe<TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options = null)
            where TMessage : IMessage;

        Task Subscribe<TKey, TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options = null)
            where TMessage : IMessage;

        Task Subscribe<TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage;

        Task Subscribe<TKey, TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage;

        // -----

        Task Subscribe<TMessage, TMessageProcessor>(IEnumerable<string> topics, CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(IEnumerable<string> topics, CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TMessage, TMessageProcessor>(IEnumerable<string> topics, Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(IEnumerable<string> topics, Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;
    }
}