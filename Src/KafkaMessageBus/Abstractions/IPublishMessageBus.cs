using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IPublishMessageBus
    {
        void Publish<TMessage>(TMessage message, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(TKey key, TMessage message, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TMessage>(TMessage message, IProducerOptions<string, TMessage> options, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(TKey key, TMessage message, IProducerOptions<TKey, TMessage> options, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TMessage>(TMessage message, Action<IProducerOptions<string, TMessage>> setupAction, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> setupAction, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        // -----

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message)
            where TMessage : IMessage;

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message)
            where TMessage : IMessage;

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message, IProducerOptions<string, TMessage> options)
            where TMessage : IMessage;

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message, IProducerOptions<TKey, TMessage> options)
            where TMessage : IMessage;

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message, Action<IProducerOptions<string, TMessage>> setupAction)
            where TMessage : IMessage;

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> setupAction)
            where TMessage : IMessage;
    }
}