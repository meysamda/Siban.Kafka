using KafkaMessageBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace KafkaMessageBus.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddKafkaMessageBus(this IServiceCollection services, Action<IKafkaMessageBusOptions> setupAction)
        { 
            var options = GetDefaultKafkaMessageBusOptions(services);
            setupAction(options);

            services.AddSingleton<IKafkaMessageBus, KafkaMessageBus>(sp => {
                var messageBus = new KafkaMessageBus(sp, options);
                return messageBus;
            });

            return services;
        }

        private static IKafkaMessageBusOptions GetDefaultKafkaMessageBusOptions(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var options = new DefaultOptions {
                SubsManager = new DefaultSubscriptionsManager(),
                Logger = serviceProvider.GetService<ILogger<KafkaMessageBus>>()
            };

            return options;
        }
    }
}