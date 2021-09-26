using System;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KafkaMessageBus.Extensions;
using System.Threading;

namespace KafkaMessageBus
{
    public partial class KafkaMessageBus : IKafkaMessageBus
    {
        public Task Subscribe<TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken, Action<IConsumerOptions<string, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage
        {
            return Subscribe<string, TMessage>(processAction, cancellationToken, defaultOptionsModifier);
        }

        public Task Subscribe<TKey, TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken, Action<IConsumerOptions<TKey, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage
        {
            var defaultOptions = GetDefaultConsumerOptions<TKey, TMessage>();
            if (defaultOptionsModifier != null)
                defaultOptionsModifier(defaultOptions);

            return Subscribe<TKey, TMessage>(processAction, cancellationToken, defaultOptions);
        }

        public Task Subscribe<TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options)
            where TMessage : IMessage
        {
            return Subscribe<string, TMessage>(processAction, cancellationToken, options);
        }

        public Task Subscribe<TKey, TMessage>(Func<TMessage, Task> processAction, CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options)
            where TMessage : IMessage
        {
            return Task.Run(async () => {
                using var consumer = GetConsumer<TKey, TMessage>(options);
                consumer.Subscribe(options.Topics);

                _subsManager.AddSubscription<TMessage, Func<TMessage, Task>>();

                while (true)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null && !consumeResult.IsPartitionEOF)
                    {
                        var expired = consumeResult.Message.Value.MessageExpireDate < DateTime.UtcNow;
                        if (!expired)
                            await processAction(consumeResult.Message.Value);
                    
                        if (!options.EnableAutoCommit)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(CancellationToken cancellationToken, Action<IConsumerOptions<string, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {            
            return Subscribe<string, TMessage, TMessageProcessor>(cancellationToken, defaultOptionsModifier);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(CancellationToken cancellationToken, Action<IConsumerOptions<TKey, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {  
            var defaultOptions = GetDefaultConsumerOptions<TKey, TMessage>();
            if (defaultOptionsModifier != null)
                defaultOptionsModifier(defaultOptions);

            return Subscribe<TKey, TMessage, TMessageProcessor>(cancellationToken, defaultOptions);
        }

        public Task Subscribe<TMessage, TMessageProcessor>(CancellationToken cancellationToken, IConsumerOptions<string, TMessage> options)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return Subscribe<string, TMessage, TMessageProcessor>(cancellationToken, options);
        }

        public Task Subscribe<TKey, TMessage, TMessageProcessor>(CancellationToken cancellationToken, IConsumerOptions<TKey, TMessage> options)
            where TMessage : IMessage
            where TMessageProcessor : IMessageProcessor<TMessage>
        {
            return Task.Run(async () => {
                using var consumer = GetConsumer<TKey, TMessage>(options);
                consumer.Subscribe(options.Topics);

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

        private void Unsubscribe<TMessage, TMessageProcessor>()
            where TMessage : IMessage
        {
            var messageName = _subsManager.GetMessageKey<TMessage>();
            var processorName = typeof(TMessageProcessor).Name;

            try
            {
                _subsManager.RemoveSubscription<TMessage, TMessageProcessor>();
                _logger.LogWarning($"message processor {processorName} unsubscribed from message {messageName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"unsubscribing message processor {processorName} from message {messageName} failed.");
                throw ex;
            }
        }

        // ---------

        private IConsumer<TKey, TMessage> GetConsumer<TKey, TMessage>(IConsumerOptions<TKey, TMessage> options)
        where TMessage : IMessage
        {            
            var config = GetConsumerConfig<TKey, TMessage>(options);

            var consumer = new ConsumerBuilder<TKey, TMessage>(config)
                .SetKeyDeserializer(options.KeyDeserializer)
                .SetValueDeserializer(options.ValueDeserializer)
                .SetErrorHandler((consumer, error) => {
                    if (error.IsError) {
                        _logger.LogError($"building kafka consumer failed. {error.Reason}");
                        consumer.Dispose();
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
                Topics = new [] { messageName },
                GroupId = messageName,
                TimeOutMilliseconds = 1000,
                KeyDeserializer = new DefaultDeserializer<TKey>(),
                ValueDeserializer = new DefaultDeserializer<TMessage>()
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