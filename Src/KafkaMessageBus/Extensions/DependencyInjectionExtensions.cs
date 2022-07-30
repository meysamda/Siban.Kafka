using KafkaMessageBus;
using KafkaMessageBus.Abstractions;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddSubscriptionMessageBus(
            this IServiceCollection services,
            IEnumerable<string> bootstrapServers,
            DefaultSerializer defaultDeserializer = DefaultSerializer.MicrosoftJsonSerializer)
        { 
            services.AddSingleton<ISubscriptionMessageBus, SubscriptionMessageBus>(sp => {
                var messageBus = new SubscriptionMessageBus(bootstrapServers, defaultDeserializer);
                return messageBus;
            });

            return services;
        }

        public static IServiceCollection AddPublishMessageBus(
            this IServiceCollection services,
            string[] bootstrapServers,
            DefaultSerializer defaultSerializer = DefaultSerializer.MicrosoftJsonSerializer)
        { 
            services.AddSingleton<IPublishMessageBus, PublishMessageBus>(sp => {
                var messageBus = new PublishMessageBus(bootstrapServers, defaultSerializer);
                return messageBus;
            });

            return services;
        }

        public static IServiceCollection AddMessageBus(
            this IServiceCollection services,
            IEnumerable<string> publishBootstrapServers,
            IEnumerable<string> subscriptionBootstrapServers,
            DefaultSerializer defaultSerializer = DefaultSerializer.MicrosoftJsonSerializer,
            DefaultSerializer defaultDeserializer = DefaultSerializer.MicrosoftJsonSerializer)
        { 
            services.AddSingleton<IMessageBus, MessageBus>(sp => {
                var messageBus = new MessageBus(publishBootstrapServers, subscriptionBootstrapServers, defaultSerializer, defaultDeserializer);
                return messageBus;
            });

            return services;
        }
    }
}