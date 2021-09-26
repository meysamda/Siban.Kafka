using System;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using KafkaMessageBus.Extensions;
using System.Net;

namespace KafkaMessageBus
{
    public partial class KafkaMessageBus : IKafkaMessageBus
    {
        public void Publish<TMessage>(TMessage message, Action<IProducerOptions<string, TMessage>> defaultOptionsModifier = null, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            Publish<string, TMessage>(null, message, defaultOptionsModifier, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> defaultOptionsModifier = null, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            var options = GetDefaultProducerOptions<TKey, TMessage>(message);
            if (defaultOptionsModifier != null)
                defaultOptionsModifier(options);

            Publish<TKey, TMessage>(key, message, options, deliveryHandler);
        }

        public void Publish<TMessage>(TMessage message, IProducerOptions<string, TMessage> options, Action<DeliveryReport<string, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            Publish<string, TMessage>(null, message, options, deliveryHandler);
        }

        public void Publish<TKey, TMessage>(TKey key, TMessage message, IProducerOptions<TKey, TMessage> options, Action<DeliveryReport<TKey, TMessage>> deliveryHandler = null)
            where TMessage : IMessage
        {
            using var producer = GetProducer(options);

            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message
            };
            
            // TODO: there is a bug or unexpected behavior when passing deliveryHandler to 
            // Produce method (either with null or not-null deliveryHandler)

            // deliveryHandler = deliveryHandler ?? DefaultDeliveryReportHandler;
            // producer.Produce(options.Topic, kafkaMessage, deliveryHandler);
            
            producer.Produce(options.Topic, kafkaMessage);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message, Action<IProducerOptions<string, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage
        {
            return PublishAsync<string, TMessage>(null, message, defaultOptionsModifier);
        }

        public Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message, Action<IProducerOptions<TKey, TMessage>> defaultOptionsModifier = null)
            where TMessage : IMessage
        {
            var options = GetDefaultProducerOptions<TKey, TMessage>(message);
            if (defaultOptionsModifier != null)
                defaultOptionsModifier(options);

            return PublishAsync<TKey, TMessage>(key, message, options);
        }

        public Task<DeliveryResult<string, TMessage>> PublishAsync<TMessage>(TMessage message, IProducerOptions<string, TMessage> options)
            where TMessage : IMessage
        {
            return PublishAsync<string, TMessage>(null, message, options);
        }

        public async Task<DeliveryResult<TKey, TMessage>> PublishAsync<TKey, TMessage>(TKey key, TMessage message, IProducerOptions<TKey, TMessage> options)
            where TMessage : IMessage
        {
            using var producer = GetProducer(options);

            var kafkaMessage = new Message<TKey, TMessage> {
                Key = key,
                Value = message
            };
            
            var result = await producer.ProduceAsync(options.Topic, kafkaMessage);

            return result;
        }

        // ----------        

        private IProducer<TKey, TMessage> GetProducer<TKey, TMessage>(IProducerOptions<TKey, TMessage> options)
        where TMessage : IMessage
        {            
            var config = GetProducerConfig<TKey, TMessage>(options);

            var producer = new ProducerBuilder<TKey, TMessage>(config)
                .SetKeySerializer(options.KeySerializer)
                .SetValueSerializer(options.ValueSerializer)
                .SetErrorHandler((producer, error) => {
                    if (error.IsError) _logger.LogError($"building kafka producer failed. {error.Reason}");
                })
                .Build();

            return producer;
        }

        private IProducerOptions<TKey, TMessage> GetDefaultProducerOptions<TKey, TMessage>(TMessage message)
        where TMessage : IMessage
        {
            var result = new DefaultProduceOptions<TKey, TMessage> {
                Topic = message.GetType().Name,
                KeySerializer = new DefaultSerializer<TKey>(),
                ValueSerializer = new DefaultSerializer<TMessage>()
            };

            return result;
        }

        private void DefaultDeliveryReportHandler<TKey, TMessage>(DeliveryReport<TKey, TMessage> deliveryReport)
        where TMessage : IMessage
        {
            if (deliveryReport.Error.IsError)
            {
                _logger.LogError($"producing message on topic {deliveryReport.Topic} failed; reason: {deliveryReport.Error.Reason}, details: {deliveryReport.Error.ToString()}");
                throw new Exception($"producing message on topic {deliveryReport.Topic} failed; reason: {deliveryReport.Error.Reason}, details: {deliveryReport.Error.ToString()}");
            }
        }

        private ProducerConfig GetProducerConfig<TKey, TMessage>(IProducerOptions<TKey, TMessage> options)
        where TMessage : IMessage
        {
            var config = new ProducerConfig 
            {
                BootstrapServers = _brokers.GetString(),
                ClientId = Dns.GetHostName(),
                // EnableDeliveryReports = false
            };

            return config;
        }
    }
}