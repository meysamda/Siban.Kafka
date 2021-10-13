# Introduction 
**KafkaMessageBus** enables you to create a message bus and do publish / subcribe operations simply or in advanced way. it is an abstraction on [Confluent.Kafka](https://github.com/confluentinc/confluent-kafka-dotnet/) which is Confluent's .NET client for [Apache Kafka](kafka.apache.org) and the [Confluent Platform](https://www.confluent.io/product/).

# Referencing
KafkaMessageBus is distributed via NuGet in [KafkaMessageBus](https://www.nuget.org/packages/KafkaMessageBus/) package.

To install KafkaMessageBus from within Visual Studio, search for KafkaMessageBus in the NuGet Package Manager UI, or run the following command in the Package Manager Console:

`Install-Package KafkaMessageBus -Version <last-version>`

To add a reference to a dotnet core project, execute the following at the command line:

`dotnet add package -v <last-version> KafkaMessageBus`

# Usage
Take a look in the [Samples](https://github.com/meysamda/KafkaMessageBus/tree/master/Doc/Samples) directory for example usage. As you can see all you need is to initiate a message bus and then do publish / subscribe operations upon it.

## Publish

Create a new instance from `PublishMessageBus`, then create an new instance from message class inheriting `IMessage` interface and publish it using Publis or PublishAsync methods on a topic. You should use the PublishAsync method if you would like to wait for the result of your publish requests before proceeding. You might typically want to do this in highly concurrent scenarios, for example in the context of handling web requests. Behind the scenes, the client (Confluent.Kafka) will manage optimizing communication with the Kafka brokers for you, batching requests as appropriate.

```c#
using System;
using System.Threading.Tasks;
using KafkaMessageBus;
using Samples.Messages;

namespace Samples.Publisher.Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var bootstrapServers = new string[] { "localhost:9092" };
            var messageBus = new PublishMessageBus(bootstrapServers);

            var message = new TempMessage {
                Body = "Hello world",
                Number = 1,
                MessageId = Guid.NewGuid()
            };

            var result = await messageBus.PublishAsync("test-topic", message);

            result = await messageBus.PublishAsync("test-topic", message, options =>
            {
                options.ProducerConfig.Acks = Confluent.Kafka.Acks.All;
                options.ProducerConfig.BootstrapServers = "some thing different from default bootstrapServers defined in message bus instantiating";
                options.ProducerConfig.MessageTimeoutMs = 50000;
            });
        }
    }
}
```

In .NET web applications (or applications with [Microsoft Hosting Extension](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/6.0.0-rc.2.21480.5)  package installed) you can register PublishMessageBus as a singlton service. Injecting PublishMessageBus into a service inheriting IHostedService, you are able to use Publish / PublishAsync methods to produce your messages on desired topics.

```c#
public static void ConfigureServices(IServiceCollection services)
{            
    var bootstrapServers = new string[] { "localhost:9092" };
    services.AddSubscriptionMessageBus(bootstrapServers);

    services.AddHostedService<SubscriberService>();
}
```

```c#
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using KafkaMessageBus.Abstractions;
using Samples.Messages;

namespace Samples.Publisher.Worker
{
    public class PublisherService : BackgroundService
    {
        private readonly IPublishMessageBus _messageBus;

        public PublisherService(IPublishMessageBus messageBus)
        {
            _messageBus = messageBus;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = new TempMessage
            {
                Body = "Hello world",
                Number = 1,
                MessageId = Guid.NewGuid()
            };

            var result = await _messageBus.PublishAsync("test-topic", message);

            result = await _messageBus.PublishAsync("test-topic", message, options =>
            {
                options.ProducerConfig.Acks = Confluent.Kafka.Acks.All;
                options.ProducerConfig.BootstrapServers = "some thing different from default bootstrapServers defined in message bus registering phase";
                options.ProducerConfig.MessageTimeoutMs = 50000;
            });
        }
    }
}
```

## Subscribe

Create a new instance from `SubscriptionMessageBus`, then subscribe on some topics using Subscribe methods.

```c#
using KafkaMessageBus;
using Samples.Messages;
using System.Threading.Tasks;

namespace Samples.Subscriber.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bootstrapServers = new string[] { "localhost:9092" };
            var messageBus = new SubscriptionMessageBus(bootstrapServers);

            var t1 = messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );

            var t2 = messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                },
                options =>
                {
                    options.ConsumerConfig.GroupId = "new-group-1";
                    options.ConsumerConfig.BootstrapServers = "some thing different from default bootstrapServers defined in message bus registering phase";
                    options.ConsumerConfig.Acks = Confluent.Kafka.Acks.All;
                }
            );

            await Task.WhenAll(t1, t2);
        }
    }
}

```

In .NET web applications (or applications with [Microsoft Hosting Extension](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/6.0.0-rc.2.21480.5)  package installed) you can register SubscriptionMessageBus as a singlton service. Injecting SubscriptionMessageBus into a service inheriting IHostedService, you are able to use Subscribe methods to listen on some topics and handle consumed messages.

```c#
public static void ConfigureServices(IServiceCollection services)
{            
    var bootstrapServers = new string[] { "localhost:9092" };
    services.AddSubscriptionMessageBus(bootstrapServers);

    services.AddHostedService<SubscriberService>();
}
```

```c#
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using KafkaMessageBus.Abstractions;
using Samples.Messages;

namespace Samples.Subscriber.Worker
{
    public class SubscriberService : BackgroundService
    {
        private readonly ISubscriptionMessageBus _messageBus;

        public SubscriberService(ISubscriptionMessageBus messageBus)
        {
            _messageBus = messageBus;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var t1 = _messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );

            var t2 = _messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                },
                options =>
                {
                    options.ConsumerConfig.GroupId = "new-group-1";
                    options.ConsumerConfig.BootstrapServers = "some thing different from default bootstrapServers defined in message bus registering phase";
                    options.ConsumerConfig.Acks = Confluent.Kafka.Acks.All;
                }
            );

            await Task.WhenAll(t1, t2);
        }
    }
}
```

## Publish / Subscribe
In .NET web applications (or applications with [Microsoft Hosting Extension](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/6.0.0-rc.2.21480.5)  package installed) you can register MessageBus as a singlton service. Injecting MessageBus into a service inheriting IHostedService, you can either do publish or subscribe operations.

```c#
public static void ConfigureServices(IServiceCollection services)
{            
    var bootstrapServers = new string[] { "localhost:9092" };
    services.AddMessageBus(bootstrapServers, bootstrapServers);

    services.AddHostedService<SubscriberService>();
    services.AddHostedService<PublisherService>();
}
```