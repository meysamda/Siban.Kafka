using System;
using System.Threading.Tasks;
using KafkaMessageBus;
using KafkaMessageBus.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Samples.Publisher.Worker
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
