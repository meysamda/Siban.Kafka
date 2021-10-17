using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IPublishMessageBus
    {
        void Publish(
            string topic,
            string message,
            IPublishOptions<string, string> options = null,
            Action<DeliveryReport<string, string>> deliveryHandler = null);

        void Publish<TMessage>(
            string topic,
            TMessage message,
            IPublishOptions<string, TMessage> options = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null);

        void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null);

        // -----
        
        void Publish(
            string topic,
            string message,
            Action<IPublishOptions<string, string>> defaultOptionsModifier,
            Action<DeliveryReport<string, string>> deliveryHandler = null);

        void Publish<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null);

        void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null);

        // -----

        Task<DeliveryResult<string, string>> PublishAsync(
            string topic,
            string message,
            IPublishOptions<string, string> options = null);

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            IPublishOptions<string, TMessage> options = null);

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options = null);

        // -----

        Task<DeliveryResult<string, string>> PublishAsync(
            string topic,
            string message,
            Action<IPublishOptions<string, string>> defaultOptionsModifier);
        
        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier);

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier);
    }
}