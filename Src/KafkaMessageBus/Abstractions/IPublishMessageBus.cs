using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IPublishMessageBus
    {
        void Publish<TMessage>(string topic, TMessage message, IProducerOptions<string, TMessage> options = null, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(string topic, TKey key, TMessage message, IProducerOptions<TKey, TMessage> options = null, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TMessage>(string topic, TMessage message, Action<IProducerOptions<string, TMessage>> setupAction, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(string topic, TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> setupAction, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        // -----

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(string topic, TMessage message, IProducerOptions<string, TMessage> options = null)
            where TMessage : IMessage;

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(string topic, TKey key, TMessage message, IProducerOptions<TKey, TMessage> options = null)
            where TMessage : IMessage;

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(string topic, TMessage message, Action<IProducerOptions<string, TMessage>> setupAction)
            where TMessage : IMessage;

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(string topic, TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> setupAction)
            where TMessage : IMessage;
    }
}