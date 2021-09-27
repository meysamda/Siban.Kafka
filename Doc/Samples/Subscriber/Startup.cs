using Microsoft.Extensions.DependencyInjection;

namespace Samples.Subscriber
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {            
            var brokers = new string[] { "localhost:9092" };
            services.AddMessageBus(options => options.Brokers = brokers);
        }
    }
}
