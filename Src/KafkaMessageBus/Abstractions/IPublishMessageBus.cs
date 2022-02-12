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
            Action<IPublishOptions<string, string>> defaultOptionsModifier,
            Action<DeliveryReport<string, string>> deliveryHandler = null);

        void Publish<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null);

        void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null);

        // -----

        void Publish(
            IProducer<string, string> producer,
            string topic,
            string message,
            Action<DeliveryReport<string, string>> deliveryHandler = null);

        void Publish<TMessage>(
            IProducer<string, TMessage> producer,
            string topic,
            TMessage message,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null);

        void Publish<TKey, TMessage>(
            IProducer<TKey, TMessage> producer,
            string topic,
            TKey key,
            TMessage message,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null);        

        // -----

        Task<DeliveryResult<string, string>> PublishAsync(
            string topic,
            string message,
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null);
        
        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null);

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null);

        // -----

        Task<DeliveryResult<string, string>> PublishAsync(
            IProducer<string, string> producer,
            string topic,
            string message);

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            IProducer<string, TMessage> producer,
            string topic,
            TMessage message);

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            IProducer<TKey, TMessage> producer,
            string topic,
            TKey key,
            TMessage message);

        public IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options);
        
    }
}