using System;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Confluent.Kafka;
using System.Threading;
using System.Collections.Generic;

namespace KafkaMessageBus
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

        public void Publish(
            string topic,
            string message,
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, string>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(topic, message, defaultOptionsModifier, deliveryHandler);
        }

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
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(topic, key, message, defaultOptionsModifier, deliveryHandler);
        }

        // ---------

        public Task<DeliveryResult<string, string>> PublishAsync(
            string topic,
            string message,
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishAsync(topic, message, defaultOptionsModifier);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishAsync(topic, message, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishAsync(topic, key, message, defaultOptionsModifier);
        }

        // ---------

        public IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options)
        {
            return _publishMessageBus.GetProducer(options);
        }

        public IPublishOptions<TKey, TMessage> GetDefaultPublishOptions<TKey, TMessage>(Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.GetDefaultPublishOptions(defaultOptionsModifier);
        }

        // ==========

        public Task SubscribeAsync(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.SubscribeAsync(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task SubscribeAsync<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.SubscribeAsync<TMessage>(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task SubscribeAsync<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
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