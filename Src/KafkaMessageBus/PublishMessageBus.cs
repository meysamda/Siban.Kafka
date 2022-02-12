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
        private Dictionary<string, object> _producers;

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
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, string>> deliveryHandler = null)
        {
            var options = GetDefaultPublishOptions<string, string>(defaultOptionsModifier);
            Publish<string, string>(topic, null, message, options, deliveryHandler);
        }

        public void Publish<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
        {
            var options = GetDefaultPublishOptions<string, TMessage>(defaultOptionsModifier);
            Publish<string, TMessage>(topic, null, message, options, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            var options = GetDefaultPublishOptions<TKey, TMessage>(defaultOptionsModifier);
            Publish<TKey, TMessage>(topic, key, message, options, deliveryHandler);
        }

        private void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            var producer = GetProducer(options);
            if (producer == null)
            {
                var producerName = GetProducerName<TKey, TMessage>(options.ProducerName);
                _producers.Add(producerName, producer);
            }

            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message
            };
            
            producer.Produce(topic, kafkaMessage, deliveryHandler);
        }

        // ----------

        public void Publish(
            IProducer<string, string> producer,
            string topic,
            string message,
            Action<DeliveryReport<string, string>> deliveryHandler = null)
        {
            Publish<string, string>(producer, topic, null, message, deliveryHandler);
        }

        public void Publish<TMessage>(
            IProducer<string, TMessage> producer,
            string topic,
            TMessage message,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
        {
            Publish<string, TMessage>(producer, topic, null, message, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            IProducer<TKey, TMessage> producer,
            string topic,
            TKey key,
            TMessage message,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
        {
            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message
            };
            
            producer.Produce(topic, kafkaMessage, deliveryHandler);
        }

        // ----------

        public Task<DeliveryResult<string, string>> PublishAsync(
            string topic,
            string message,
            Action<IPublishOptions<string, string>> defaultOptionsModifier = null)
        {
            var options = GetDefaultPublishOptions<string, string>(defaultOptionsModifier);
            return PublishAsync<string, string>(topic, null, message, options);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier = null)
        {
            var options = GetDefaultPublishOptions<string, TMessage>(defaultOptionsModifier);
            return PublishAsync<string, TMessage>(topic, null, message, options);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
        {
            var options = GetDefaultPublishOptions<TKey, TMessage>(defaultOptionsModifier);
            return PublishAsync<TKey, TMessage>(topic, key, message, options);
        }

        private Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options)
        {
            var producer = GetProducer(options);
            if (producer == null)
            {
                var producerName = GetProducerName<TKey, TMessage>(options.ProducerName);
                _producers.Add(producerName, producer);
            }
            
            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message
            };
            
            return producer.ProduceAsync(topic, kafkaMessage);
        }

        // ----------   

        public Task<DeliveryResult<string, string>> PublishAsync(
            IProducer<string, string> producer,
            string topic,
            string message)
        {
            return PublishAsync<string, string>(producer, topic, null, message);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            IProducer<string, TMessage> producer,
            string topic,
            TMessage message)
        {
            return PublishAsync<string, TMessage>(producer, topic, null, message);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            IProducer<TKey, TMessage> producer,
            string topic,
            TKey key,
            TMessage message)
        {
            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message
            };
            
            return producer.ProduceAsync(topic, kafkaMessage);
        }

        // ----------        

        public IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options)
        {   
            object producerObject;
            var producerName = GetProducerName<TKey, TMessage>(options.ProducerName);
            _producers.TryGetValue(producerName, out producerObject);

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

        private IPublishOptions<TKey, TMessage> GetDefaultPublishOptions<TKey, TMessage>(Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier = null)
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

            if (defaultOptionsModifier != null)
                defaultOptionsModifier(options);

            return options;
        }

        private void DefaultDeliveryReportHandler<TKey, TMessage>(DeliveryReport<TKey, TMessage> deliveryReport)
        {
            if (deliveryReport.Error.IsError)
            {
                throw new Exception($"producing message on topic {deliveryReport.Topic} failed; reason: {deliveryReport.Error.Reason}, details: {deliveryReport.Error.ToString()}");
            }
        }

        private ISerializer<T> GetDefaultSerializer<T>() 
        {
            switch (_defaultSerializer)
            {
                case DefaultSerializer.MicrosoftJsonSerializer:
                    return Serializers<T>.MicrosoftJson;
                
                case DefaultSerializer.MessagePackSerializer:
                    return Serializers<T>.MessagePack;

                default:
                    return Serializers<T>.MicrosoftJson;
            }
        }
    }
}