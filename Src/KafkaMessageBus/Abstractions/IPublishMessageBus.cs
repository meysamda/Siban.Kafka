using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IPublishMessageBus
    {        
        void Publish(
            string topic,
            string message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, string>> deliveryHandler = null);

        void Publish<TMessage>(
            string topic,
            TMessage message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null);

        void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null);
        
        // -----

        Task<DeliveryResult<string, string>> PublishAsync(
            string topic,
            string message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null);
        
        Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null);

        Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null);

        // -----

        public IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options);
        public IPublishOptions<TKey, TMessage> GetDefaultPublishOptions<TKey, TMessage>(Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null);
    }
}