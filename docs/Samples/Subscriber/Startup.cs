using Microsoft.Extensions.DependencyInjection;

namespace Siban.Kafka.Samples.Subscriber
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {            
            var bootstrapServers = new string[] { "kafka.nsedna.com:32344" };
            services.AddMessageBus(bootstrapServers, bootstrapServers);
            services.AddSingleton<GreetingHandler>();
            services.AddHostedService<SubscriberService>();
        }
    }
}
