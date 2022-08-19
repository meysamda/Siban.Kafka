using System;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Confluent.Kafka;
using System.Collections.Generic;
using System.Linq;
using KafkaMessageBus.Extenstions;

namespace KafkaMessageBus
{
    public class PublishMessageBus : IPublishMessageBus
    {
        private readonly IEnumerable<string> _bootstrapServers;
        private readonly DefaultSerializer _defaultSerializer;
        private readonly Dictionary<string, object> _producers;

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
        
        public void Publish(
            string topic,
            string message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, string>> deliveryHandler = null)
        {
            Publish(topic, null, message, headers, defaultOptionsModifier, deliveryHandler);
        }

        public void Publish<TMessage>(
            string topic,
            TMessage message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
        {
            Publish(topic, null, message, headers, defaultOptionsModifier, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            var options = GetDefaultPublishOptions(defaultOptionsModifier);
            var producer = GetProducer(options);
            if (producer == null)
            {
                var producerName = GetProducerName<TKey, TMessage>(options.ProducerName);
                _producers.Add(producerName, producer);
            }

            var kafkaHeaders = new Headers();
            foreach (var header in headers)
                kafkaHeaders.Add(new Header(header.Key, header.Value));

            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message,
                Headers = kafkaHeaders
            };
            
            producer.Produce(topic, kafkaMessage, deliveryHandler);
        }

        // ----------

        public Task<DeliveryResult<string, string>> PublishAsync(
            string topic,
            string message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null)
        {
            return PublishAsync(topic, null, message, headers, defaultOptionsModifier);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null)
        {
            return PublishAsync(topic, null, message, headers, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Dictionary<string, byte[]> headers = null,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            var options = GetDefaultPublishOptions(defaultOptionsModifier);
            var producer = GetProducer(options);
            if (producer == null)
            {
                var producerName = GetProducerName<TKey, TMessage>(options.ProducerName);
                _producers.Add(producerName, producer);
            }

            var kafkaHeaders = new Headers();
            foreach (var header in headers)
                kafkaHeaders.Add(new Header(header.Key, header.Value));

            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message,
                Headers = kafkaHeaders
            };
            
            return producer.ProduceAsync(topic, kafkaMessage);
        }

        // ----------        

        public IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options)
        {
            var producerName = GetProducerName<TKey, TMessage>(options.ProducerName);
            _producers.TryGetValue(producerName, out object producerObject);

            IProducer<TKey, TMessage> producer;
            if (producerObject != null)
                producer = (IProducer<TKey, TMessage>) _producers[producerName];
            else
            {
                producer = new ProducerBuilder<TKey, TMessage>(options.ProducerConfig)
                    .SetKeySerializer(options.KeySerializer)
                    .SetValueSerializer(options.ValueSerializer)
                    .SetErrorHandler((producer, error) => 
                    {
                        producer.Dispose();
                        options.ErrorHandler(error);
                    })
                    .SetLogHandler((producer, logMessage) => 
                    {
                        options.LogHandler(logMessage);
                    })
                    .Build();
            }

            return producer;
        }

        private string GetProducerName<TKey, TMessage>(string producerName)
        {
            if (string.IsNullOrEmpty(producerName))
                producerName = "default";

            return $"{typeof(TKey).Name}-{typeof(TMessage).Name}-{producerName}";
        }

        public IPublishOptions<TKey, TMessage> GetDefaultPublishOptions<TKey, TMessage>(Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            var options = new DefaultPublishOptions<TKey, TMessage> {
                KeySerializer = GetDefaultSerializer<TKey>(),
                ValueSerializer = GetDefaultSerializer<TMessage>(),
                ProducerConfig = new ProducerConfig 
                {
                    BootstrapServers = _bootstrapServers.GetString()
                },
                ErrorHandler = error => 
                {
                    if (error.IsError)
                    {
                        throw new Exception($"building kafka producer failed. {error.Reason}");
                    }
                },
                LogHandler = logMessage => 
                {
                    // do some thing with log message
                }
            };

            defaultOptionsModifier?.Invoke(options);

            return options;
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