using System;
using System.Threading.Tasks;
using Siban.Kafka.Abstractions;
using Confluent.Kafka;
using System.Threading;
using System.Collections.Generic;

namespace Siban.Kafka
{
    public class MessageBus : IMessageBus
    {
        private readonly IPublishMessageBus _publishMessageBus;
        private readonly ISubscriptionMessageBus _subscriptionMessageBus;

        public MessageBus(
            IEnumerable<string> publishBootstrapServers,
            IEnumerable<string> subscriptionBootstrapServers,
            DefaultSerializer defaultSerializer = DefaultSerializer.MicrosoftJsonSerializer,
            DefaultSerializer defaultDeserializer = DefaultSerializer.MicrosoftJsonSerializer)
        {
            _publishMessageBus = new PublishMessageBus(publishBootstrapServers, defaultSerializer);
            _subscriptionMessageBus = new SubscriptionMessageBus(subscriptionBootstrapServers, defaultDeserializer);
        }

        // ---------

        public void PublishMessageValue<TValue>(
            string topic,
            TValue value,
            Action<IPublishOptions<string, TValue>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TValue>> deliveryHandler = null)
        {
            _publishMessageBus.PublishMessageValue(topic, value, defaultOptionsModifier, deliveryHandler);
        }

        public void PublishMessage<TKey, TValue>(
            string topic,
            Message<TKey, TValue> message,
            Action<IPublishOptions<TKey, TValue>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        {
            _publishMessageBus.PublishMessage(topic, message, defaultOptionsModifier, deliveryHandler);
        }

        // ---------

        public Task<DeliveryResult<string, TValue>> PublishMessageValueAsync<TValue>(
            string topic,
            TValue value,
            Action<IPublishOptions<string, TValue>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishMessageValueAsync(topic, value, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TValue>> PublishMessageAsync<TKey, TValue>(
            string topic,
            Message<TKey, TValue> message,
            Action<IPublishOptions<TKey, TValue>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishMessageAsync(topic, message, defaultOptionsModifier);
        }

        // ==========

        public Task SubscribeForMessageValueAsync<TValue>(
            IEnumerable<string> topics,
            Func<TValue, Task> handleMethod,
            Action<ISubscribeOptions<string, TValue>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.SubscribeForMessageValueAsync(topics, handleMethod, defaultOptionsModifier, cancellationToken);
        }

        public Task SubscribeForMessageAsync<TKey, TValue>(
            IEnumerable<string> topics,
            Func<Message<TKey, TValue>, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TValue>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.SubscribeForMessageAsync(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public void Unsubscribe<TKey, TValue>(string consumerName)
        {
            _subscriptionMessageBus.Unsubscribe<TKey, TValue>(consumerName);
        }
    }
}