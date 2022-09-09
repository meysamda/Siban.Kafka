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

        public Task SubscribeForMessageValueAsync<TValue>(
            IEnumerable<string> topics,
            Func<TValue, Task> handleMethod,
            Action<ISubscribeOptions<string, TValue>> defaultOptionsModifier = null,
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
                        await handleMethod(consumeResult.Message.Value);

                        if (options.ConsumerConfig.EnableAutoCommit.HasValue && !options.ConsumerConfig.EnableAutoCommit.Value)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        public Task SubscribeForMessageAsync<TKey, TValue>(
            IEnumerable<string> topics,
            Func<Message<TKey, TValue>, Task> handleMethod,
            Action<ISubscribeOptions<TKey, TValue>> defaultOptionsModifier = null,
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
                        await handleMethod(consumeResult.Message);

                        if (options.ConsumerConfig.EnableAutoCommit.HasValue && !options.ConsumerConfig.EnableAutoCommit.Value)
                            consumer.Commit();
                    }
                }
            }, cancellationToken);
        }

        // -----------

        private IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(ISubscribeOptions<TKey, TValue> options = null)
        {
            options ??= GetSubscribeOptions<TKey, TValue>();
            var consumer = new ConsumerBuilder<TKey, TValue>(options.ConsumerConfig)
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

        private ISubscribeOptions<TKey, TValue> GetSubscribeOptions<TKey, TValue>(Action<ISubscribeOptions<TKey, TValue>> defaultOptionsModifier = null)
        {
            var options = GetSubscribeOptions<TKey, TValue>();

            defaultOptionsModifier?.Invoke(options);

            return options;
        }

        private ISubscribeOptions<TKey, TValue> GetSubscribeOptions<TKey, TValue>()
        {
            var options = new SubscribeOptions<TKey, TValue> {
                KeyDeserializer = GetDefaultDeserializer<TKey>(),
                ValueDeserializer = GetDefaultDeserializer<TValue>(),
                ConsumerConfig = new ConsumerConfig
                {
                    BootstrapServers = _bootstrapServers.ToSepratedString(),
                    GroupId = GetTypeName<TValue>(),
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

        private string GetTypeName<T>() => typeof(T).Name;
    }
}