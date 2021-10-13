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
            ISubscriptionsManager subsManager = null)
        {
            _publishMessageBus = new PublishMessageBus(publishBootstrapServers);
            _subscriptionMessageBus = new SubscriptionMessageBus(subscriptionBootstrapServers, serviceProvider, subsManager);
        }

        public void Publish<TMessage>(
            string topic,
            TMessage message,
            IPublishOptions<string, TMessage> options = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TMessage>(topic, message, options, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TKey, TMessage>(topic, key, message, options, deliveryHandler);
        }

        public void Publish<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TMessage>(topic, message, defaultOptionsModifier, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TKey, TMessage>(topic, key, message, defaultOptionsModifier, deliveryHandler);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            IPublishOptions<string, TMessage> options = null)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TMessage>(topic, message, options);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options = null)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(topic, key, message, options);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TMessage>(topic, message, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(topic, key, message, defaultOptionsModifier);
        }

        // ==========

        public Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            ISubscribeOptions<string, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(topics, messageProcessor, options, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            ISubscribeOptions<TKey, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(topics, messageProcessor, options, cancellationToken);
        }

        public Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            ISubscribeOptions<string, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(topics, options, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            ISubscribeOptions<TKey, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(topics, options, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(topics, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(topics, defaultOptionsModifier, cancellationToken);
        }
    }
}