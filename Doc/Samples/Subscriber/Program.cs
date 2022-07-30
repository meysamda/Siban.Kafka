using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Samples.Subscriber
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .Build()
                .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                    Startup.ConfigureServices(services)
                );

            return hostBuilder;
        }
    }
}
