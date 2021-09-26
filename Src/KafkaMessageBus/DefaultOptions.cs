using System.Collections.Generic;
using KafkaMessageBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace KafkaMessageBus
{
    public class DefaultOptions : IKafkaMessageBusOptions
    {
        public IEnumerable<string> Brokers { get; set; }
        public IKafkaMessageBusSubscriptionsManager SubsManager { get; set; }
        public ILogger Logger { get; set; }
    }
}