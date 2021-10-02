using KafkaMessageBus;
using KafkaMessageBus.Abstractions;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddSubscriptionMessageBus(this IServiceCollection services, IEnumerable<string> brokers, ISubscriptionsManager subsManager = null)
        { 
            var serviceProvider = services.BuildServiceProvider();

            services.AddSingleton<ISubscriptionMessageBus, SubscriptionMessageBus>(sp => {
                var messageBus = new SubscriptionMessageBus(brokers, serviceProvider, subsManager);
                return messageBus;
            });

            return services;
        }

        public static IServiceCollection AddPublishMessageBus(this IServiceCollection services, string[] brokers)
        { 
            services.AddSingleton<IPublishMessageBus, PublishMessageBus>(sp => {
                var messageBus = new PublishMessageBus(brokers);
                return messageBus;
            });

            return services;
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services, IEnumerable<string> publishBrokers, IEnumerable<string> subscriptionBrokers, ISubscriptionsManager subsManager = null)
        { 
            var serviceProvider = services.BuildServiceProvider();

            services.AddSingleton<IMessageBus, MessageBus>(sp => {
                var messageBus = new MessageBus(publishBrokers, subscriptionBrokers, serviceProvider, subsManager);
                return messageBus;
            });

            return services;
        }
    }
}