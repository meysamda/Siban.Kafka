using System;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Samples.Messages;

namespace Samples.Publisher.Api.Controllers
{
    [ApiController]
    [Route("publisher")]
    public class PublisherController : ControllerBase
    {
        private readonly IPublishMessageBus _messageBus;

        public PublisherController(IPublishMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpGet("publish1")]
        public async Task<IActionResult> Publish()
        {
            var message = new TempMessage {
                Body = "Hello world",
                Number = 1,
                MessageId = Guid.NewGuid()
            };

            var result = await _messageBus.PublishAsync("test-topic", message);
            return Ok();
        }

        [HttpGet("publish2")]
        public async Task<IActionResult> Publish2()
        {
            var message = new TempMessage
            {
                Body = "Hello world",
                Number = 1,
                MessageId = Guid.NewGuid()
            };

            var result = await _messageBus.PublishAsync("test-topic", message, options =>
            {
                options.ProducerConfig.Acks = Confluent.Kafka.Acks.All;
                options.ProducerConfig.BootstrapServers = "some thing different from default brokers defined in message bus registering phase";
                options.ProducerConfig.MessageTimeoutMs = 50000;
            });
            return Ok();
        }
    }
}