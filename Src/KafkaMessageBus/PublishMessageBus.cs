using System;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Confluent.Kafka;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using KafkaMessageBus.Extenstions;

namespace KafkaMessageBus
{
    public class PublishMessageBus : IPublishMessageBus
    {
        private readonly IEnumerable<string> _bootstrapServers;

        public PublishMessageBus(IEnumerable<string> bootstrapServers)
        {
            _bootstrapServers = bootstrapServers ?? throw new ArgumentNullException(nameof(bootstrapServers));
            if (!bootstrapServers.Any()) throw new ArgumentException("bootstrapServers list is empty", nameof(bootstrapServers));
            _bootstrapServers = bootstrapServers;
        }

        public void Publish<TMessage>(
            string topic,
            TMessage message,
            IPublishOptions<string, TMessage> options = null,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            Publish<string, TMessage>(topic, null, message, options, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options = null,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            options = options ?? GetDefaultPublishOptions<TKey, TMessage>();

            using var producer = GetProducer(options);

            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message
            };
            
            // TODO: there is a bug or unexpected behavior when passing deliveryHandler to 
            // Produce method (either with null or not-null deliveryHandler)

            // deliveryHandler = deliveryHandler ?? DefaultDeliveryReportHandler;
            // producer.Produce(options.Topic, kafkaMessage, deliveryHandler);
            
            producer.Produce(topic, kafkaMessage);
        }

        public void Publish<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            Publish<string, TMessage>(topic, null, message, defaultOptionsModifier, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier,
            Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            var options = GetDefaultPublishOptions<TKey, TMessage>();
            defaultOptionsModifier(options);

            Publish<TKey, TMessage>(topic, key, message, options, deliveryHandler);
        }
        
        // ----------

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            IPublishOptions<string, TMessage> options = null)
            where TMessage : IMessage
        {
            return PublishAsync<string, TMessage>(topic, null, message, options);
        }

        public async Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            IPublishOptions<TKey, TMessage> options = null)
            where TMessage : IMessage
        {
            options = options ?? GetDefaultPublishOptions<TKey, TMessage>();
            using var producer = GetProducer(options);

            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message
            };
            
            var result = await producer.ProduceAsync(topic, kafkaMessage);

            return result;
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(
            string topic,
            TMessage message,
            Action<IPublishOptions<string, TMessage>> defaultOptionsModifier)
            where TMessage : IMessage
        {
            return PublishAsync<string, TMessage>(topic, null, message, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(
            string topic,
            TKey key,
            TMessage message,
            Action<IPublishOptions<TKey, TMessage>> defaultOptionsModifier)
            where TMessage : IMessage
        {
            var options = GetDefaultPublishOptions<TKey, TMessage>();
            defaultOptionsModifier(options);

            return PublishAsync<TKey, TMessage>(topic, key, message, options);
        }

        // ----------        

        private IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IPublishOptions<TKey, TMessage> options)
        where TMessage : IMessage
        {   
            var producer = new ProducerBuilder<TKey, TMessage>(options.ProducerConfig)
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

            return producer;
        }

        private IPublishOptions<TKey, TMessage> GetDefaultPublishOptions<TKey, TMessage>()
        where TMessage : IMessage
        {
            var result = new DefaultPublishOptions<TKey, TMessage> {
                KeySerializer = new DefaultSerializer<TKey>(),
                ValueSerializer = new DefaultSerializer<TMessage>(),
                ProducerConfig = new ProducerConfig 
                {
                    BootstrapServers = _bootstrapServers.GetString(),
                    ClientId = Dns.GetHostName(),
                    Acks = Acks.Leader,
                    MessageTimeoutMs = 1000
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

            return result;
        }

        private void DefaultDeliveryReportHandler<TKey, TMessage>(DeliveryReport<TKey, TMessage> deliveryReport)
        where TMessage : IMessage
        {
            if (deliveryReport.Error.IsError)
            {
                throw new Exception($"producing message on topic {deliveryReport.Topic} failed; reason: {deliveryReport.Error.Reason}, details: {deliveryReport.Error.ToString()}");
            }
        }
    }
}