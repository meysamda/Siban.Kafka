using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Siban.Kafka.Abstractions
{
    public interface IPublishMessageBus
    {        
        void Publish<TMessage>(
            string topic,
            TMessage message,
            Headers headers = null,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null);

        void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Headers headers = null,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null);
        
        // -----

        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Headers headers = null,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null);

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Headers headers = null,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null);

        // -----

        public IProducer<TKey, TMessage> GetConfluentKafkaProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options);
        public IPublishOptions<TKey, TMessage> GetDefaultPublishOptions<TKey, TMessage>(Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null);
    }
}