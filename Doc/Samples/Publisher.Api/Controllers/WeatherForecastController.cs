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
            _messageBus.Publish("greeting-1", "hello world");
            _messageBus.Publish("greeting-1", "hello world", options => { /* modify default options */ });
            await _messageBus.PublishAsync("greeting-1", "hello world");
            await _messageBus.PublishAsync("greeting-1", "hello world", options => { /* modify default options */ });
            
            var message = new Greeting { Body = "hello world" };

            _messageBus.Publish("greeting-2", message);
            _messageBus.Publish("greeting-2", message, options => { /* modify default options */ });
            await _messageBus.PublishAsync("greeting-2", message);
            await _messageBus.PublishAsync("greeting-2", message, options => { /* modify default options */ });

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
