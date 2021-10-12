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

        [HttpGet("publish")]
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
    }
}
