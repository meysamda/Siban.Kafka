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

        public void Publish<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(topic, message, defaultOptionsModifier, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            Message<TKey, TMessage> message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(topic, message, defaultOptionsModifier, deliveryHandler);
        }

        // ---------

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishAsync(topic, message, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            Message<TKey, TMessage> message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishAsync(topic, message, defaultOptionsModifier);
        }

        // ==========

        public Task SubscribeAsync<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, CancellationToken, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.SubscribeAsync(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task SubscribeAsync<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<Message<TKey, TMessage>, CancellationToken, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.SubscribeAsync(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }
    }
}