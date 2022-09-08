using System;
using System.Threading.Tasks;
using Siban.Kafka.Abstractions;
using Confluent.Kafka;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Siban.Kafka
{
    public class SubscriptionMessageBus : ISubscriptionMessageBus
    {
        private readonly IEnumerable<string> _bootstrapServers;
        private readonly DefaultSerializer _defaultDeserializer;

        public SubscriptionMessageBus(
            IEnumerable<string> bootstrapServers,
            DefaultSerializer defaultDeserializer = DefaultSerializer.MicrosoftJsonSerializer)
        {
            _bootstrapServers = bootstrapServers ?? throw new ArgumentNullException(nameof(bootstrapServers));
            if (!bootstrapServers.Any()) throw new ArgumentException("bootstrapServers list is empty", nameof(bootstrapServers));
            _bootstrapServers = bootstrapServers;

            _defaultDeserializer = defaultDeserializer;
        }

        public Task SubscribeAsync<TMessage>(
            IEnumerable<string> topics,
            Func<TMessage, CancellationToken, Task> messageProcessor,
            Action<ISubscribeOptions<string, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            var options = GetSubscribeOptions(defaultOptionsModifier);

            return Task.Run(async () => {
                using var consumer = GetConsumer(options);
                consumer.Subscribe(topics);

                while (true)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null && !consumeResult.IsPartitionEOF)
                    {
                        await messageProcessor(consumeResult.Message.Value, cancellationToken);

                        if (options.ConsumerConfig.EnableAutoCommit.HasValue && !options.ConsumerConfig.EnableAutoCommit.Value)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        public Task SubscribeAsync<TKey, TMessage>(
            IEnumerable<string> topics,
            Func<Message<TKey, TMessage>, CancellationToken, Task> messageProcessor,
            Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            var options = GetSubscribeOptions(defaultOptionsModifier);

            return Task.Run(async () => {
                using var consumer = GetConsumer(options);
                consumer.Subscribe(topics);

                while (true)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null && !consumeResult.IsPartitionEOF)
                    {
                        await messageProcessor(consumeResult.Message, cancellationToken);

                        if (options.ConsumerConfig.EnableAutoCommit.HasValue && !options.ConsumerConfig.EnableAutoCommit.Value)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        // -----------

        private IConsumer<TKey, TMessage> GetConsumer<TKey, TMessage>(ISubscribeOptions<TKey, TMessage> options = null)
        {
            options ??= GetSubscribeOptions<TKey, TMessage>();
            var consumer = new ConsumerBuilder<TKey, TMessage>(options.ConsumerConfig)
                .SetKeyDeserializer(options.KeyDeserializer)
                .SetValueDeserializer(options.ValueDeserializer)
                .SetErrorHandler((consumer, error) => 
                {
                    options.ErrorHandler(error);
                })
                .SetLogHandler((consumer, logMessage) => 
                {
                    options.LogHandler(logMessage);
                })
                .Build();

            return consumer;
        }

        private ISubscribeOptions<TKey, TMessage> GetSubscribeOptions<TKey, TMessage>(Action<ISubscribeOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            var options = GetSubscribeOptions<TKey, TMessage>();

            defaultOptionsModifier?.Invoke(options);

            return options;
        }

        private ISubscribeOptions<TKey, TMessage> GetSubscribeOptions<TKey, TMessage>()
        {
            var messageName = GetMessageName<TMessage>();
            var options = new SubscribeOptions<TKey, TMessage> {
                KeyDeserializer = GetDefaultDeserializer<TKey>(),
                ValueDeserializer = GetDefaultDeserializer<TMessage>(),
                ConsumerConfig = new ConsumerConfig
                {
                    BootstrapServers = _bootstrapServers.ToSepratedString(),
                    GroupId = messageName,
                    EnableAutoCommit = true,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoOffsetStore = true,
                    AllowAutoCreateTopics = true
                }
            };

            return options;
        }

        private IDeserializer<T> GetDefaultDeserializer<T>() 
        {
            return _defaultDeserializer switch
            {
                DefaultSerializer.MicrosoftJsonSerializer => Deserializers<T>.MicrosoftJson,
                DefaultSerializer.MessagePackSerializer => Deserializers<T>.MicrosoftJson,
                _ => Deserializers<T>.MicrosoftJson,
            };
        }

        private string GetMessageName<TMessage>() => typeof(TMessage).Name;
    }
}