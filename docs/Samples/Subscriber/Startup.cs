using Microsoft.Extensions.DependencyInjection;

namespace Siban.Kafka.Samples.Subscriber
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {            
            var bootstrapServers = new string[] { "localhost:9092" };
            services.AddSubscriptionMessageBus(bootstrapServers);
            services.AddSingleton<GreetingMessageProcessor>();
            services.AddHostedService<SubscriberService>();
        }
    }
}
