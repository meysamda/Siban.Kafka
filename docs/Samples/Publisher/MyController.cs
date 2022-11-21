using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siban.Kafka.Abstractions;

namespace Siban.Kafka.Samples.Publisher
{
    [ApiController]
    [Route("api")]
    public class MyController : ControllerBase
    {

        private readonly IPublishMessageBus _messageBus;
        private readonly ILogger<MyController> _logger;

        public MyController(ILogger<MyController> logger, IPublishMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        [HttpGet("1")]
        public async Task<bool> Get()
        {
            await _messageBus.PublishMessageValueAsync("greeting-1", "hello world-1");

            // await _messageBus.PublishMessageValueAsync("greeting-1", "hello world-1", options => {
            //         options.ProducerConfig.ClientId = Dns.GetHostName();
            //         options.ProducerConfig.Acks = Acks.Leader;
            //         options.ProducerConfig.MessageTimeoutMs = 1000;
            //     });

            return true;
        }

        [HttpGet("2")]
        public bool Get2()
        {
            var value = new Greeting { Body = "hello world" };
            _messageBus.PublishMessageValue("greeting-2", value, options => {
                    options.ProducerConfig.ClientId = Dns.GetHostName();
                    options.ProducerConfig.Acks = Acks.Leader;
                    options.ProducerConfig.MessageTimeoutMs = 1000;
                },
                dr => {
                    if (dr.Error.IsError) {
                        System.Console.WriteLine(dr.Error.Reason);
                    }
                });

            return true;
        }

        [HttpGet("3")]
        public bool Get3()
        {
            var message = new Message<string, Greeting> {
                Value = new Greeting { Body = "hello world" }
            };

            _messageBus.PublishMessage("greeting-3", message, options => {
                    options.ProducerConfig.ClientId = Dns.GetHostName();
                    options.ProducerConfig.Acks = Acks.Leader;
                    options.ProducerConfig.MessageTimeoutMs = 1000;
                },
                dr => {
                    if (dr.Error.IsError) {
                        System.Console.WriteLine(dr.Error.Reason);
                    }
                });

            return true;
        }
    }
}
