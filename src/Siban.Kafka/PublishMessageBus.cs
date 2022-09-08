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

        public void Publish<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
        {
            var options = GetPublishOptions(defaultOptionsModifier);
            var kafkaMessage = new Message<string, TMessage> { Value = message };
            var producer = GetProducer(options);
            if (producer == null)
            {
                var producerName = GetProducerName<string, TMessage>(options.ProducerName);
                _producers.Add(producerName, producer);
            }
            
            producer.Produce(topic, kafkaMessage, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(string topic, Message<TKey, TMessage> message, Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            var options = GetPublishOptions(defaultOptionsModifier);
            var producer = GetProducer(options);
            if (producer == null)
            {
                var producerName = GetProducerName<string, TMessage>(options.ProducerName);
                _producers.Add(producerName, producer);
            }
            
            producer.Produce(topic, message, deliveryHandler);
        }

        // ----------

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null)
        {
            var options = GetPublishOptions(defaultOptionsModifier);
            var kafkaMessage = new Message<string, TMessage> { Value = message };
            var producer = GetProducer(options);
            if (producer == null)
            {
                var producerName = GetProducerName<string, TMessage>(options.ProducerName);
                _producers.Add(producerName, producer);
            }
            
            return producer.ProduceAsync(topic, kafkaMessage);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(string topic, Message<TKey, TMessage> message, Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            var options = GetPublishOptions(defaultOptionsModifier);
            var producer = GetProducer(options);
            if (producer == null)
            {
                var producerName = GetProducerName<TKey, TMessage>(options.ProducerName);
                _producers.Add(producerName, producer);
            }
            
            return producer.ProduceAsync(topic, message);
        }

        // ----------        

        private IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options)
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

        private IPublishOptions<TKey, TMessage> GetPublishOptions<TKey, TMessage>(Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            var options = GetDefaultOptions<TKey, TMessage>();
            defaultOptionsModifier?.Invoke(options);

            return options;
        }
        
        private string GetProducerName<TKey, TMessage>(string producerName)
        {
            if (string.IsNullOrEmpty(producerName))
                producerName = "default";

            return $"{typeof(TKey).Name}-{typeof(TMessage).Name}-{producerName}";
        }

        private IPublishOptions<TKey, TMessage> GetDefaultOptions<TKey, TMessage>()
        {
            return new PublishOptions<TKey, TMessage> {
                KeySerializer = GetDefaultSerializer<TKey>(),
                ValueSerializer = GetDefaultSerializer<TMessage>(),
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