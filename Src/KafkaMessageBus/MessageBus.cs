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
            IServiceProvider serviceProvider = null,
            DefaultSerializer defaultSerializer = DefaultSerializer.MicrosoftJsonSerializer,
            DefaultSerializer defaultDeserializer = DefaultSerializer.MicrosoftJsonSerializer,
            ISubscriptionsManager subsManager = null)
        {
            _publishMessageBus = new PublishMessageBus(publishBootstrapServers, defaultSerializer);
            _subscriptionMessageBus = new SubscriptionMessageBus(subscriptionBootstrapServers, serviceProvider, defaultDeserializer, subsManager);
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
            _publishMessageBus.Publish<TMessage>(topic, message, defaultOptionsModifier, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            _publishMessageBus.Publish<TKey, TMessage>(topic, key, message, defaultOptionsModifier, deliveryHandler);
        }

        // ---------

        public void Publish(
            IProducer<string, string> producer,
            string topic,
            string message,
            Action<DeliveryReport<string, string>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(producer, topic, message, deliveryHandler);
        }

        public void Publish<TMessage>(
            IProducer<string, TMessage> producer,
            string topic,
            TMessage message,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(producer, topic, message, deliveryHandler = null);
        }

        public void Publish<TKey, TMessage>(
            IProducer<TKey, TMessage> producer,
            string topic,
            TKey key,
            TMessage message,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            _publishMessageBus.Publish(producer, topic, key, message, deliveryHandler);
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
            return _publishMessageBus.PublishAsync<TMessage>(topic, message, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(topic, key, message, defaultOptionsModifier);
        }

        // ---------

        public Task<DeliveryResult<string, string>> PublishAsync(
            IProducer<string, string> producer,
            string topic,
            string message)
        {
            return _publishMessageBus.PublishAsync(producer, topic, message);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            IProducer<string, TMessage> producer,
            string topic,
            TMessage message)
        {
            return _publishMessageBus.PublishAsync<TMessage>(producer, topic, message);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            IProducer<TKey, TMessage> producer,
            string topic,
            TKey key,
            TMessage message)
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(producer, topic, key, message);
        }

        public IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options)
        {
            return _publishMessageBus.GetProducer(options);
        }

        public IPublishOptions<TKey, TMessage> GetDefaultPublishOptions<TKey, TMessage>(Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _publishMessageBus.GetDefaultPublishOptions(defaultOptionsModifier);
        }

        // ==========

        public Task Subscribe(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _subscriptionMessageBus.Subscribe(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        // ---------

        public Task Subscribe(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            IConsumer<string, string> consumer,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.Subscribe(topics, messageProcessor, consumer, cancellationToken);
        }

        public Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            IConsumer<string, TMessage> consumer,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(topics, messageProcessor, consumer, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            IConsumer<TKey, TMessage> consumer,
            CancellationToken cancellationToken = default)
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(topics, messageProcessor, consumer, cancellationToken);
        }

        // ---------

        public Task Subscribe<TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<string>
        {
            return _subscriptionMessageBus.Subscribe<TMessageProcessor>(topics, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(topics, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(topics, defaultOptionsModifier, cancellationToken);
        }

        // ---------
        
        public Task Subscribe<TMessageProcessor>(
            IEnumerable<string> topics,
            IConsumer<string, string> consumer,
            CancellationToken cancellationToken = default)
            where TMessageProcessor : IMessageProcessor<string>
        {
            return _subscriptionMessageBus.Subscribe<TMessageProcessor>(topics, consumer, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            IConsumer<string, TMessage> consumer,
            CancellationToken cancellationToken = default)
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(topics, consumer, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            IConsumer<TKey, TMessage> consumer,
            CancellationToken cancellationToken = default) where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(topics, consumer, cancellationToken = default);
        }

        public IConsumer<TKey, TMessage> GetConsumer<TKey, TMessage>(ISubscribeOptions<TKey, TMessage> options)
        {
            return _subscriptionMessageBus.GetConsumer(options);
        }

        public ISubscribeOptions<TKey, TMessage> GetDefaultSubscribeOptions<TKey, TMessage>(Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            return _subscriptionMessageBus.GetDefaultSubscribeOptions(defaultOptionsModifier);
        }
    }
}