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
        private readonly Dictionary<string, object> _consumers;

        public SubscriptionMessageBus(
            IEnumerable<string> bootstrapServers,
            DefaultSerializer defaultDeserializer = DefaultSerializer.MicrosoftJsonSerializer)
        {
            _bootstrapServers = bootstrapServers ?? throw new ArgumentNullException(nameof(bootstrapServers));
            if (!bootstrapServers.Any()) throw new ArgumentException("bootstrapServers list is empty", nameof(bootstrapServers));
            _bootstrapServers = bootstrapServers;

            _defaultDeserializer = defaultDeserializer;
            _consumers = new Dictionary<string, object>();
        }

        public Task SubscribeForMessageValueAsync<TValue>(
            IEnumerable<string> topics,
            Func<TValue, Task> handleMethod,
            Action<ISubscribeOptions<string, TValue>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default)
        {
            var options = GetSubscribeOptions(defaultOptionsModifier);

            return Task.Run(async () =>
            {
                using var consumer = GetConsumer(options);
                consumer.Subscribe(topics);

                while (consumer.Subscription.Any())
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null && !consumeResult.IsPartitionEOF)
                    {
                        await handleMethod(consumeResult.Message.Value);

                        if (options.ConsumerConfig.EnableAutoCommit.HasValue && !options.ConsumerConfig.EnableAutoCommit.Value)
                        {
                            consumer.Commit();
                        }
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

            return Task.Run(async () =>
            {
                using var consumer = GetConsumer(options);
                consumer.Subscribe(topics);

                while (consumer.Subscription.Any())
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null && !consumeResult.IsPartitionEOF)
                    {
                        await handleMethod(consumeResult.Message);

                        if (options.ConsumerConfig.EnableAutoCommit.HasValue && !options.ConsumerConfig.EnableAutoCommit.Value)
                        {
                            consumer.Commit();
                        }
                    }
                }
            }, cancellationToken);
        }

        public void Unsubscribe<TKey, TValue>(string consumerName)
        {
            var name = GetConsumerName<TKey, TValue>(consumerName);
            _consumers.TryGetValue(name, out var consumerObject);

            if (consumerObject != null)
            {
                var consumer = (IConsumer<TKey, TValue>)_consumers[consumerName];
                consumer.Unsubscribe();
            }
        }

        // -----------


        private IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(ISubscribeOptions<TKey, TValue> options = null)
        {
            var consumerName = GetConsumerName<TKey, TValue>(options.ConsumerName);
            _consumers.TryGetValue(consumerName, out object consumerObject);

            IConsumer<TKey, TValue> consumer;
            if (consumerObject != null)
            {
                consumer = (IConsumer<TKey, TValue>)_consumers[consumerName];
            }
            else
            {
                options ??= GetSubscribeOptions<TKey, TValue>();
                var builder = new ConsumerBuilder<TKey, TValue>(options.ConsumerConfig)
                    .SetKeyDeserializer(options.KeyDeserializer)
                    .SetValueDeserializer(options.ValueDeserializer);

                if (options.ErrorHandler != null)
                    builder.SetErrorHandler((consumer, error) => { options.ErrorHandler(error); });

                if (options.LogHandler != null)
                    builder.SetLogHandler((consumer, logMessage) => { options.LogHandler(logMessage); });

                consumer = builder.Build();
                _consumers.Add(consumerName, consumer);
            }

            return consumer;
        }

        private ISubscribeOptions<TKey, TValue> GetSubscribeOptions<TKey, TValue>(Action<ISubscribeOptions<TKey, TValue>> defaultOptionsModifier = null)
        {
            var options = GetSubscribeOptions<TKey, TValue>();

            defaultOptionsModifier?.Invoke(options);

            return options;
        }

        private string GetConsumerName<TKey, TValue>(string consumerName = "default")
        {
            return $"{typeof(TKey).Name}-{typeof(TValue).Name}-{consumerName}";
        }

        private ISubscribeOptions<TKey, TValue> GetSubscribeOptions<TKey, TValue>()
        {
            var options = new SubscribeOptions<TKey, TValue>
            {
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