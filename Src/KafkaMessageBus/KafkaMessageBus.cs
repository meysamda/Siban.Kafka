using System;
using System.Linq;
using KafkaMessageBus.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace KafkaMessageBus
{
    public partial class KafkaMessageBus : IKafkaMessageBus
    {
        private readonly IEnumerable<string> _brokers;
        private readonly IKafkaMessageBusSubscriptionsManager _subsManager;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public KafkaMessageBus(IServiceProvider serviceProvider, IKafkaMessageBusOptions options)
        {
            _serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _brokers = options.Brokers ?? throw new ArgumentException("Brokers is null", "Brokers");
            if (!_brokers.Any()) throw new ArgumentException("Brokers list is empty");

            _subsManager = options.SubsManager ?? throw new ArgumentException("IMessageBusSubscriptionsManager is nul");
            _logger = options.Logger ?? throw new ArgumentException("ILogger is null");
        }
    }
}