using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IKafkaMessageBus
    {
        // -----

        void Publish<TMessage>(TMessage message, Action<IProducerOptions<string, TMessage>> defaultOptionsModifier = null, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> defaultOptionsModifier = null, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TMessage>(TMessage message, IProducerOptions<string, TMessage> options, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(TKey key, TMessage message, IProducerOptions<TKey, TMessage> options, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        // -----

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message, Action<IProducerOptions<string, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage;

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage;

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message, IProducerOptions<string, TMessage> options)
            where TMessage : IMessage;
            
        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message, IProducerOptions<TKey, TMessage> options)
            where TMessage : IMessage;

        // -----

        Task Subscribe<TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken, Action<IConsumerOptions<string, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage;

        Task Subscribe<TKey, TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken, Action<IConsumerOptions<TKey, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage;

        Task Subscribe<TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options)
            where TMessage : IMessage;

        Task Subscribe<TKey, TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options)
            where TMessage : IMessage;

        // -----

        Task Subscribe<TMessage, TMessageProcessor>(CancellationToken cancellationToken, Action<IConsumerOptions<string, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(CancellationToken cancellationToken, Action<IConsumerOptions<TKey, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TMessage, TMessageProcessor>(CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;

        Task Subscribe<TKey, TMessage, TMessageProcessor>(CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>;
    }
}
