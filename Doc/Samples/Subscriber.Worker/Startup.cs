using Microsoft.Extensions.DependencyInjection;

namespace Samples.Subscriber.Worker
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {            
            var brokers = new string[] { "localhost:9092" };
            services.AddSubscriptionMessageBus(brokers);

            services.AddHostedService<SubscriberService>();
        }
    }
}
