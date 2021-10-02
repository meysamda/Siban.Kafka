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

        public MessageBus(IEnumerable<string> publishBrokers, IEnumerable<string> subscriptionBrokers, IServiceProvider serviceProvider, ISubscriptionsManager subsManager = null)
        {
            _publishMessageBus = new PublishMessageBus(publishBrokers);
            _subscriptionMessageBus = new SubscriptionMessageBus(subscriptionBrokers, serviceProvider, subsManager);
        }

        public void Publish<TMessage>(string topic, TMessage message, IProducerOptions<string, TMessage> options = null, Action<DeliveryReport<string, TMessage>> deliveryHandler = null) where TMessage : IMessage
        {
            _publishMessageBus.Publish<TMessage>(topic, message, options, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(string topic, TKey key, TMessage message, IProducerOptions<TKey, TMessage> options = null, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null) where TMessage : IMessage
        {
            _publishMessageBus.Publish<TKey, TMessage>(topic, key, message, options, deliveryHandler);
        }

        public void Publish<TMessage>(string topic, TMessage message, Action<IProducerOptions<string, TMessage>> setupAction, Action<DeliveryReport<string, TMessage>> deliveryHandler = null) where TMessage : IMessage
        {
            _publishMessageBus.Publish<TMessage>(topic, message, setupAction, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(string topic, TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> setupAction, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null) where TMessage : IMessage
        {
            _publishMessageBus.Publish<TKey, TMessage>(topic, key, message, setupAction, deliveryHandler);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(string topic, TMessage message, IProducerOptions<string, TMessage> options = null) where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TMessage>(topic, message, options);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(string topic, TKey key, TMessage message, IProducerOptions<TKey, TMessage> options = null) where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(topic, key, message, options);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(string topic, TMessage message, Action<IProducerOptions<string, TMessage>> setupAction) where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TMessage>(topic, message, setupAction);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(string topic, TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> setupAction) where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(topic, key, message, setupAction);
        }

        // ==========

        public Task Subscribe<TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options = null) where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(topics, messageProcessAction, cancellationToken, options);
        }

        public Task Subscribe<TKey, TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options = null) where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(topics, messageProcessAction, cancellationToken, options);
        }

        public Task Subscribe<TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken) where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(topics, messageProcessAction, setupAction, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken) where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(topics, messageProcessAction, setupAction, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(IEnumerable<string> topics, CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(topics, cancellationToken, options);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(IEnumerable<string> topics, CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(topics, cancellationToken, options);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(IEnumerable<string> topics, Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(topics, setupAction, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(IEnumerable<string> topics, Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(topics, setupAction, cancellationToken);
        }
    }
}