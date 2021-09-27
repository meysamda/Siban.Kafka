using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaMessageBus.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        Task Subscribe<TMessage>(Func<TMessage, Task> messageProcessAction, CancellationToken cancellationToken)
            where TMessage : IMessage;

        Task Subscribe<TKey, TMessage>(Func<TMessage, Task> messageProcessAction, CancellationToken cancellationToken)
            where TMessage : IMessage;

        Task Subscribe<TMessage>(Func<TMessage, Task> messageProcessAction, IConsumerOptions<string, TMessage> options, CancellationToken cancellationToken)
            where TMessage : IMessage;

        Task Subscribe<TKey, TMessage>(Func<TMessage, Task> messageProcessAction, IConsumerOptions<TKey, TMessage> options, CancellationToken cancellationToken)
            where TMessage : IMessage;

        Task Subscribe<TMessage>(Func<TMessage, Task> messageProcessAction, Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage;

        Task Subscribe<TKey, TMessage>(Func<TMessage, Task> messageProcessAction, Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage;

        // -----

        Task Subscribe<TMessage, TMessageProcessor>(CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TMessage, TMessageProcessor>(IConsumerOptions<string, TMessage> options, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(IConsumerOptions<TKey, TMessage> options, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TMessage, TMessageProcessor>(Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;
    }
}