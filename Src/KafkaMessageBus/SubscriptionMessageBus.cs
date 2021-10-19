using System;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using KafkaMessageBus.Extenstions;
using KafkaMessageBus.DefaultSerializers;

namespace KafkaMessageBus
{
    public class SubscriptionMessageBus : ISubscriptionMessageBus
    {
        private readonly IEnumerable<string> _bootstrapServers;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly DefaultSerializer _defaultDeserializer;
        private readonly ISubscriptionsManager _subsManager;

        public SubscriptionMessageBus(
            IEnumerable<string> bootstrapServers,
            IServiceProvider serviceProvider = null,
            DefaultSerializer defaultDeserializer = DefaultSerializer.MicrosoftJsonSerializer,
            ISubscriptionsManager subsManager = null)
        {
            _bootstrapServers = bootstrapServers ?? throw new ArgumentNullException(nameof(bootstrapServers));
            if (!bootstrapServers.Any()) throw new ArgumentException("bootstrapServers list is empty", nameof(bootstrapServers));
            _bootstrapServers = bootstrapServers;

            _serviceScopeFactory = serviceProvider?.GetRequiredService<IServiceScopeFactory>();
            _defaultDeserializer = defaultDeserializer;
            _subsManager = subsManager ?? new DefaultSubscriptionsManager();
        }

        public Task Subscribe(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            ISubscribeOptions<string, string> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Subscribe<string>(topics, messageProcessor, options, cancellationToken);
        }

        public Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            ISubscribeOptions<string, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Subscribe<string, TMessage>(topics, messageProcessor, options, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            ISubscribeOptions<TKey, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            options = options ?? GetDefaultSubscribeOptions<TKey, TMessage>();

            return Task.Run(async () => {
                using var consumer = GetConsumer(options);
                consumer.Subscribe(topics);

                _subsManager.AddSubscription<TMessage, Func<TMessage, Task>>();

                while (true)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null && !consumeResult.IsPartitionEOF)
                    {
                        await messageProcessor(consumeResult.Message.Value);

                        if (options.ConsumerConfig.EnableAutoCommit.HasValue && !options.ConsumerConfig.EnableAutoCommit.Value)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        // -----------

        public Task Subscribe(
            IEnumerable<string> topics,
            Func<string, Task> messageProcessor,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Subscribe<string>(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Subscribe<string, TMessage>(topics, messageProcessor, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = GetDefaultSubscribeOptions<TKey, TMessage>();
            defaultOptionsModifier(options);

            return Subscribe<TKey, TMessage>(topics, messageProcessor, options, cancellationToken);
        }

        // -----------

        public Task Subscribe<TMessageProcessor>(
            IEnumerable<string> topics,
            ISubscribeOptions<string, string> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<string>
        {
            return Subscribe<string, TMessageProcessor>(topics, options, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            ISubscribeOptions<string, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return Subscribe<string, TMessage, TMessageProcessor>(topics, options, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            ISubscribeOptions<TKey, TMessage> options = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            if (_serviceScopeFactory == null)
                throw new ArgumentException($"Unable to resolve {typeof(TMessageProcessor).Name}, IServiceProvider is null");

            options = options ?? GetDefaultSubscribeOptions<TKey, TMessage>();
            
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
                        await messageProcessor.Process(consumeResult.Message.Value, cancellationToken);

                        if (options.ConsumerConfig.EnableAutoCommit.HasValue && !options.ConsumerConfig.EnableAutoCommit.Value)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        // -----------

        public Task Subscribe<TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<string, string>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<string>
        {            
            return Subscribe<string, TMessageProcessor>(topics, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>
        {            
            return Subscribe<string, TMessage, TMessageProcessor>(topics, defaultOptionsModifier, cancellationToken);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(
            IEnumerable<string> topics,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessageProcessor : IMessageProcessor<TMessage>
        {  
            var options = GetDefaultSubscribeOptions<TKey, TMessage>();
            defaultOptionsModifier(options);

            return Subscribe<TKey, TMessage, TMessageProcessor>(topics, options, cancellationToken);
        }

        // ---------

        private void Unsubscribe<TMessage, TMessageProcessor>()
        {
            var messageName = _subsManager.GetMessageKey<TMessage>();
            var processorName = typeof(TMessageProcessor).Name;

            _subsManager.RemoveSubscription<TMessage, TMessageProcessor>();
        }

        private IConsumer<TKey, TMessage> GetConsumer<TKey, TMessage>(ISubscribeOptions<TKey, TMessage> options)
        {            
            var consumer = new ConsumerBuilder<TKey, TMessage>(options.ConsumerConfig)
                .SetKeyDeserializer(options.KeyDeserializer)
                .SetValueDeserializer(options.ValueDeserializer)
                .SetErrorHandler((consumer, error) => 
                {
                    consumer.Dispose();
                    options.ErrorHandler(error);
                })
                .SetLogHandler((consumer, logMessage) => 
                {
                    options.LogHandler(logMessage);
                })
                .Build();

            return consumer;
        }

        private ISubscribeOptions<TKey, TMessage> GetDefaultSubscribeOptions<TKey, TMessage>()
        {
            var messageName = _subsManager.GetMessageKey<TMessage>();

            var result = new DefaultSubscribeOptions<TKey, TMessage> {
                KeyDeserializer = GetDefaultDeserializer<TKey>(),
                ValueDeserializer = GetDefaultDeserializer<TMessage>(),
                ConsumerConfig = new ConsumerConfig
                {
                    BootstrapServers = _bootstrapServers.GetString(),
                    GroupId = messageName,
                    EnableAutoCommit = true,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoOffsetStore = true,
                    AllowAutoCreateTopics = true
                },
                ErrorHandler = error => 
                {
                    if (error.IsError)
                    {
                        throw new Exception($"building kafka consumer failed. {error.Reason}");
                    }
                },
                LogHandler = logMessage => 
                {
                    // do some thing with log message
                }
            };

            return result;
        }

        private IDeserializer<T> GetDefaultDeserializer<T>() 
        {
            switch (_defaultDeserializer)
            {
                case DefaultSerializer.MicrosoftJsonSerializer:
                    return new MicrosoftJsonSerializer<T>();
                
                case DefaultSerializer.MessagePackSerializer:
                    return new MessagePackSerializer<T>();

                default:
                    return new MicrosoftJsonSerializer<T>();
            }
        }
    }
}