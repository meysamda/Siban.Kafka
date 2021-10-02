using System;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using KafkaMessageBus.Extenstions;

namespace KafkaMessageBus
{
    public class SubscriptionMessageBus : ISubscriptionMessageBus
    {
        private readonly IEnumerable<string> _brokers;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISubscriptionsManager _subsManager;

        public SubscriptionMessageBus(IEnumerable<string> brokers, IServiceProvider serviceProvider, ISubscriptionsManager subsManager = null)
        {
            _brokers = brokers ?? throw new ArgumentNullException(nameof(brokers));
            if (!brokers.Any()) throw new ArgumentException("Brokers list is empty");
            _brokers = brokers;

            _serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            
            _subsManager = subsManager ?? new DefaultSubscriptionsManager();
        }

        public Task Subscribe<TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options = null)
            where TMessage : IMessage
        {
            return Subscribe<string, TMessage>(topics, messageProcessAction, cancellationToken, options);
        }

        public Task Subscribe<TKey, TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options = null)
            where TMessage : IMessage
        {
            options = options ?? GetDefaultConsumerOptions<TKey, TMessage>();

            return Task.Run(async () => {
                using var consumer = GetConsumer<TKey, TMessage>(options);
                consumer.Subscribe(topics);

                _subsManager.AddSubscription<TMessage, Func<TMessage, Task>>();

                while (true)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null && !consumeResult.IsPartitionEOF)
                    {
                        var expired = consumeResult.Message.Value.MessageExpireDate < DateTime.UtcNow;
                        if (!expired)
                            await messageProcessAction(consumeResult.Message.Value);
                    
                        if (!options.EnableAutoCommit)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        public Task Subscribe<TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            return Subscribe<string, TMessage>(topics, messageProcessAction, setupAction, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(IEnumerable<string> topics, Func<TMessage, Task> messageProcessAction, Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            var options = GetDefaultConsumerOptions<TKey, TMessage>();
            setupAction(options);

            return Subscribe<TKey, TMessage>(topics, messageProcessAction, cancellationToken, options);
        }

        // -----------

        public Task Subscribe<TMessage, TMessageProcessor>(IEnumerable<string> topics, CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return Subscribe<string, TMessage, TMessageProcessor>(topics, cancellationToken, options);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(IEnumerable<string> topics, CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            options = options ?? GetDefaultConsumerOptions<TKey, TMessage>();
            
            return Task.Run(async () => {
                using var consumer = GetConsumer<TKey, TMessage>(options);
                consumer.Subscribe(topics);

                _subsManager.AddSubscription<TMessage, TMessageProcessor>();

                using var scope = _serviceScopeFactory.CreateScope();

                var messageProcessor = scope.ServiceProvider.GetRequiredService<IMessageProcessor<TMessage>>();

                while (true)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null && !consumeResult.IsPartitionEOF)
                    {
                        var expired = consumeResult.Message.Value.MessageExpireDate < DateTime.UtcNow;
                        if (!expired)
                            await messageProcessor.Process(consumeResult.Message.Value, cancellationToken);

                        if (!options.EnableAutoCommit)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(IEnumerable<string> topics, Action<IConsumerOptions<string, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {            
            return Subscribe<string, TMessage, TMessageProcessor>(topics, setupAction, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(IEnumerable<string> topics, Action<IConsumerOptions<TKey, TMessage>> setupAction, CancellationToken cancellationToken)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {  
            var options = GetDefaultConsumerOptions<TKey, TMessage>();
            setupAction(options);

            return Subscribe<TKey, TMessage, TMessageProcessor>(topics, cancellationToken, options);
        }

        // ---------

        private void Unsubscribe<TMessage, TMessageProcessor>()
            where TMessage : IMessage
        {
            var messageName = _subsManager.GetMessageKey<TMessage>();
            var processorName = typeof(TMessageProcessor).Name;

            _subsManager.RemoveSubscription<TMessage, TMessageProcessor>();
        }

        private IConsumer<TKey, TMessage> GetConsumer<TKey, TMessage>(IConsumerOptions<TKey, TMessage> options)
        where TMessage : IMessage
        {            
            var config = GetConsumerConfig<TKey, TMessage>(options);

            var consumer = new ConsumerBuilder<TKey, TMessage>(config)
                .SetKeyDeserializer(options.KeyDeserializer)
                .SetValueDeserializer(options.ValueDeserializer)
                .SetErrorHandler((consumer, error) => {
                    if (error.IsError) {
                        consumer.Dispose();
                        throw new Exception($"building kafka consumer failed. {error.Reason}");
                    }
                })
                .Build();

            return consumer;
        }

        private IConsumerOptions<TKey, TMessage> GetDefaultConsumerOptions<TKey, TMessage>()
        where TMessage : IMessage
        {
            var messageName = _subsManager.GetMessageKey<TMessage>();

            var result = new DefaultConsumerOptions<TKey, TMessage> {
                GroupId = messageName,
                KeyDeserializer = new DefaultDeserializer<TKey>(),
                ValueDeserializer = new DefaultDeserializer<TMessage>(),
                EnableAutoCommit = true,
                AllowAutoCreateTopics = true,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            return result;
        }

        private ConsumerConfig GetConsumerConfig<TKey, TMessage>(IConsumerOptions<TKey, TMessage> options)
        where TMessage : IMessage
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _brokers.GetString(),
                GroupId = options.GroupId,
                EnableAutoCommit = options.EnableAutoCommit,
                AutoOffsetReset = options.AutoOffsetReset,
                EnableAutoOffsetStore = true,
                AllowAutoCreateTopics = true
            };

            return config;
        }
    }
}