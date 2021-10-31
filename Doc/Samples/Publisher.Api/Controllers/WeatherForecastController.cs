using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaMessageBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Samples.Messages;

namespace Samples.Publisher.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IPublishMessageBus _messageBus;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IPublishMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _messageBus.Publish("greeting-1", "hello world-1", options => {
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

            var result = await _messageBus.PublishAsync("greeting-1", "hello world-2", options => {
                    options.ProducerConfig.ClientId = Dns.GetHostName();
                    options.ProducerConfig.Acks = Acks.Leader;
                    options.ProducerConfig.MessageTimeoutMs = 1000;
                });

            var producerConfig = new ProducerConfig {
                BootstrapServers = "localhost:9092"
            };
            var producer = new ProducerBuilder<string, string>(producerConfig)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            _messageBus.Publish(producer, "greeting-1", "hello world-3", dr => { 
                if (dr.Error.IsError) {
                    System.Console.WriteLine(dr.Error.Reason);
                    throw new System.Exception($"error, {dr.Error.Reason}");
                }
            });

            result = await _messageBus.PublishAsync(producer, "greeting-1", "hello world-4");

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
