using Microsoft.Extensions.DependencyInjection;

namespace Samples.Publisher.Worker
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {            
            var bootstrapServers = new string[] { "localhost:9092" };
            services.AddPublishMessageBus(bootstrapServers);

            services.AddHostedService<PublisherService>();
        }
    }
}
