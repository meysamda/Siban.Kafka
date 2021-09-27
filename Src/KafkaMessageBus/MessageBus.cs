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

        public MessageBus(IEnumerable<string> brokers, IServiceProvider serviceProvider, ISubscriptionsManager subsManager = null)
        {
            _publishMessageBus = new PublishMessageBus(brokers);
            _subscriptionMessageBus = new SubscriptionMessageBus(brokers, serviceProvider, subsManager);
        }

        public void Publish<TMessage>(TMessage message, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TMessage>(message, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(TKey key, TMessage message, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TKey, TMessage>(key, message, deliveryHandler);
        }

        public void Publish<TMessage>(TMessage message, IProducerOptions<string, TMessage> options, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TMessage>(message, options, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(TKey key, TMessage message, IProducerOptions<TKey, TMessage> options, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TKey, TMessage>(key, message, options, deliveryHandler);
        }

        public void Publish<TMessage>(TMessage message, Action<IProducerOptions<string, TMessage>> setupAction, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TMessage>(message, setupAction, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> setupAction, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            _publishMessageBus.Publish<TKey, TMessage>(key, message, setupAction, deliveryHandler);
        }

        // ------------

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TMessage>(message);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(key, message);
        }
        
        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message, IProducerOptions<string, TMessage> options)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TMessage>(message, options);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message, IProducerOptions<TKey, TMessage> options)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(key, message, options);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message, Action<IProducerOptions<string, TMessage>> setupAction)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TMessage>(message, setupAction);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> setupAction)
            where TMessage : IMessage
        {
            return _publishMessageBus.PublishAsync<TKey, TMessage>(key, message, setupAction);
        }

        // ------------

        public Task Subscribe<TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(processAction, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(processAction, cancellationToken);
        }

        public Task Subscribe<TMessage>(Func<TMessage, Task> processAction, IConsumerOptions<string, TMessage> options, CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(processAction, options, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(Func<TMessage, Task> processAction, IConsumerOptions<TKey, TMessage> options, CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(processAction, options, cancellationToken);
        }

        public Task Subscribe<TMessage>(Func<TMessage, Task> processAction, Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TMessage>(processAction, setupAction, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(Func<TMessage, Task> processAction, Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage>(processAction, setupAction, cancellationToken);
        }

        // ------------

        public Task Subscribe<TMessage, TMessageProcessor>(CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(IConsumerOptions<string, TMessage> options, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(options, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(IConsumerOptions<TKey, TMessage> options, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(options, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TMessage, TMessageProcessor>(setupAction, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return _subscriptionMessageBus.Subscribe<TKey, TMessage, TMessageProcessor>(setupAction, cancellationToken);
        }
    }
}