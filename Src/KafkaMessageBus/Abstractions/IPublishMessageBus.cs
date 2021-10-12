using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IPublishMessageBus
    {
        void Publish<TMessage>(
            string topic,
            TMessage message,
            IPublishOptions<string, TMessage> options = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage;

        // -----

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            IPublishOptions<string, TMessage> options = null)
            where TMessage : IMessage;

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options = null)
            where TMessage : IMessage;

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier)
            where TMessage : IMessage;

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier)
            where TMessage : IMessage;
    }
}