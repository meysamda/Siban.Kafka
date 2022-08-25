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
            Headers headers = null,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(topic, message, headers, defaultOptionsModifier, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Headers headers = null,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(topic, key, message, headers, defaultOptionsModifier, deliveryHandler);
        }

        // ---------

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Headers headers = null,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishAsync(topic, message, headers, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Headers headers = null,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishAsync(topic, key, message, headers, defaultOptionsModifier);
        }

        // ---------

        public IProducer<TKey, TMessage> GetConfluentKafkaProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options)
        {
            return _publishMessageBus.GetConfluentKafkaProducer(options);
        }

        public IPublishOptions<TKey, TMessage> GetDefaultPublishOptions<TKey, TMessage>(Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.GetDefaultPublishOptions(defaultOptionsModifier);
        }

        // ==========

        public Task SubscribeAsync<TMessage>(
            IEnumerable<string> topics,
            Func<string, TMessage, Headers, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.SubscribeAsync(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task SubscribeAsync<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TKey, TMessage, Headers, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.SubscribeAsync(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        // ---------

        public IConsumer<TKey, TMessage> GetConfluentKafkaConsumer<TKey, TMessage>(ISubscribeOptions<TKey, TMessage> options)
        {
            return _subscriptionMessageBus.GetConfluentKafkaConsumer(options);
        }

        public ISubscribeOptions<TKey, TMessage> GetDefaultSubscribeOptions<TKey, TMessage>(Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _subscriptionMessageBus.GetDefaultSubscribeOptions(defaultOptionsModifier);
        }
    }
}