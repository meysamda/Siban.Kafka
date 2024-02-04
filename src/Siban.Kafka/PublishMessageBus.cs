using System;
using System.Threading.Tasks;
using Siban.Kafka.Abstractions;
using Confluent.Kafka;
using System.Collections.Generic;
using System.Linq;
namespace Siban.Kafka
{
    public class PublishMessageBus : IPublishMessageBus
    {
        private readonly IEnumerable<string> _bootstrapServers;
        private readonly DefaultSerializer _defaultSerializer;
        private readonly Dictionary<string, object> _producers;
        private static readonly object LockObject = new object();

        public PublishMessageBus(
            IEnumerable<string> bootstrapServers,
            DefaultSerializer defaultSerializer = DefaultSerializer.MicrosoftJsonSerializer)
        {
            _bootstrapServers = bootstrapServers ?? throw new ArgumentNullException(nameof(bootstrapServers));
            if (!bootstrapServers.Any()) throw new ArgumentException("bootstrapServers list is empty", nameof(bootstrapServers));
            _bootstrapServers = bootstrapServers;

            _defaultSerializer = defaultSerializer;
            _producers = new Dictionary<string, object>();
        }

        // ----------

        public void PublishMessageValue<TValue>(
            string topic,
            TValue value,
            Action<IPublishOptions<string, TValue>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TValue>> deliveryHandler = null)
        {
            var options = GetPublishOptions(defaultOptionsModifier);
            var message = new Message<string, TValue> { Value = value };
            var producer = GetProducer(options);

            producer.Produce(topic, message, deliveryHandler);
        }

        public void PublishMessage<TKey, TValue>(
            string topic,
            Message<TKey, TValue> message,
            Action<IPublishOptions<TKey, TValue>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        {
            var options = GetPublishOptions(defaultOptionsModifier);
            var producer = GetProducer(options);

            producer.Produce(topic, message, deliveryHandler);
        }

        // ----------

        public Task<DeliveryResult<string, TValue>> PublishMessageValueAsync<TValue>(
            string topic,
            TValue value,
            Action<IPublishOptions<string, TValue>> defaultOptionsModifier = null)
        {
            var options = GetPublishOptions(defaultOptionsModifier);
            var message = new Message<string, TValue> { Value = value };
            var producer = GetProducer(options);

            return producer.ProduceAsync(topic, message);
        }

        public Task<DeliveryResult<TKey, TValue>> PublishMessageAsync<TKey, TValue>(
            string topic,
            Message<TKey, TValue> message,
            Action<IPublishOptions<TKey, TValue>> defaultOptionsModifier = null)
        {
            var options = GetPublishOptions(defaultOptionsModifier);
            var producer = GetProducer(options);

            return producer.ProduceAsync(topic, message);
        }

        // ----------        

        private IProducer<TKey, TValue> GetProducer<TKey, TValue>(IPublishOptions<TKey, TValue> options)
        {
            var producerName = GetProducerName<TKey, TValue>(options.ProducerName);
            _producers.TryGetValue(producerName, out object producerObject);

            IProducer<TKey, TValue> producer;
            if (producerObject != null)
            {
                producer = (IProducer<TKey, TValue>)_producers[producerName];
            }
            else
            {
                var builder = new ProducerBuilder<TKey, TValue>(options.ProducerConfig)
                    .SetKeySerializer(options.KeySerializer)
                    .SetValueSerializer(options.ValueSerializer);

                if (options.ErrorHandler != null)
                    builder.SetErrorHandler((consumer, error) => { options.ErrorHandler(error); });

                if (options.LogHandler != null)
                    builder.SetLogHandler((consumer, logMessage) => { options.LogHandler(logMessage); });

                producer = builder.Build();
                AddProducer(producerName, producer);
            }

            return producer;
        }

        private void AddProducer<TKey, TValue>(string producerName, IProducer<TKey, TValue> producer)
        {
            lock (LockObject)
            {
                if (!_producers.ContainsKey(producerName))
                {
                    _producers.Add(producerName, producer);
                }
            }
        }

        private IPublishOptions<TKey, TValue> GetPublishOptions<TKey, TValue>(Action<IPublishOptions<TKey, TValue>> defaultOptionsModifier = null)
        {
            var options = GetDefaultOptions<TKey, TValue>();
            defaultOptionsModifier?.Invoke(options);

            return options;
        }

        private string GetProducerName<TKey, TValue>(string producerName)
        {
            if (string.IsNullOrEmpty(producerName))
                producerName = "default";

            return $"{typeof(TKey).Name}-{typeof(TValue).Name}-{producerName}";
        }

        private IPublishOptions<TKey, TValue> GetDefaultOptions<TKey, TValue>()
        {
            return new PublishOptions<TKey, TValue>
            {
                KeySerializer = GetDefaultSerializer<TKey>(),
                ValueSerializer = GetDefaultSerializer<TValue>(),
                ProducerConfig = new ProducerConfig
                {
                    BootstrapServers = _bootstrapServers.ToSepratedString()
                }
            };
        }

        private ISerializer<T> GetDefaultSerializer<T>()
        {
            return _defaultSerializer switch
            {
                DefaultSerializer.MicrosoftJsonSerializer => Serializers<T>.MicrosoftJson,
                DefaultSerializer.MessagePackSerializer => Serializers<T>.MessagePack,
                _ => Serializers<T>.MicrosoftJson,
            };
        }
    }
}