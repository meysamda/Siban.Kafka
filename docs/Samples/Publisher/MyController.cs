using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private readonly IMessageBus _messageBus;
        private readonly ILogger<MyController> _logger;

        public MyController(ILogger<MyController> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        [HttpGet]
        public bool Get()
        {
            _messageBus.Publish("greeting-1", "hello world-1", null, options => {
                    options.ProducerConfig.ClientId = Dns.GetHostName();
                    options.ProducerConfig.Acks = Acks.Leader;
                    options.ProducerConfig.MessageTimeoutMs = 1000;
                },
                dr => {
                    if (dr.Error.IsError) {
                        System.Console.WriteLine(dr.Error.Reason);
                        throw new System.Exception($"error, {dr.Error.Reason}");
                    }
                });

            return true;
        }
    }
}
