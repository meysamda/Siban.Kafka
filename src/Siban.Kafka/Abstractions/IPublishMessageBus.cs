using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Siban.Kafka.Abstractions
{
    public interface IPublishMessageBus
    {        
        void PublishMessageValue<TValue>(
            string topic,
            TValue value,
            Action<IPublishOptions<string, TValue>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TValue>> deliveryHandler = null);

        void PublishMessage<TKey, TValue>(
            string topic,
            Message<TKey, TValue> message,
            Action<IPublishOptions<TKey, TValue>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);
        
        // -----

        Task<DeliveryResult<string, TValue>> PublishMessageValueAsync<TValue>(
            string topic,
            TValue message,
            Action<IPublishOptions<string, TValue>> defaultOptionsModifier = null);

        Task<DeliveryResult<TKey, TValue>> PublishMessageAsync<TKey, TValue>(
            string topic,
            Message<TKey, TValue> message,
            Action<IPublishOptions<TKey, TValue>> defaultOptionsModifier = null);
    }
}