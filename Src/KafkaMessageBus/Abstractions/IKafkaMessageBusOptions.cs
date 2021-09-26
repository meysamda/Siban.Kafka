using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace KafkaMessageBus.Abstractions
{
    public interface IKafkaMessageBusOptions
    {
        IEnumerable<string> Brokers { get; set; }
        IKafkaMessageBusSubscriptionsManager SubsManager { get; set; }
        ILogger Logger { get; set; }
    }
}