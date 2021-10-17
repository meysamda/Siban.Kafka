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

Create a new instance from `PublishMessageBus`, then send a message using Publish or PublishAsync methods on a topic. You should use the PublishAsync method if you would like to wait for the result of your publish requests before proceeding. You might typically want to do this in highly concurrent scenarios, for example in the context of handling web requests. Behind the scenes, the client (Confluent.Kafka) will manage optimizing communication with the Kafka brokers for you, batching requests as appropriate.

```c#
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

            messageBus.Publish("greeting-1", "hello world");
            messageBus.Publish("greeting-1", "hello world", options => { /* modify default options */ });
            await messageBus.PublishAsync("greeting-1", "hello world");
            await messageBus.PublishAsync("greeting-1", "hello world", options => { /* modify default options */ });
            
            var message = new Greeting { Body = "hello world" };

            messageBus.Publish("greeting-2", message);
            messageBus.Publish("greeting-2", message, options => { /* modify default options */ });
            await messageBus.PublishAsync("greeting-2", message);
            await messageBus.PublishAsync("greeting-2", message, options => { /* modify default options */ });
        }
    }
}
```

In .NET web applications (or applications with [Microsoft Hosting Extension](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/6.0.0-rc.2.21480.5) package installed) you can register PublishMessageBus as a singlton service. Injecting PublishMessageBus into a service, you can use Publish / PublishAsync methods to produce your messages on desired topics.

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
            _messageBus.Publish("greeting-1", "hello world");
            _messageBus.Publish("greeting-1", "hello world", options => { /* modify default options */ });
            await _messageBus.PublishAsync("greeting-1", "hello world");
            await _messageBus.PublishAsync("greeting-1", "hello world", options => { /* modify default options */ });
            
            var message = new Greeting { Body = "hello world" };

            _messageBus.Publish("greeting-2", message);
            _messageBus.Publish("greeting-2", message, options => { /* modify default options */ });
            await _messageBus.PublishAsync("greeting-2", message);
            await _messageBus.PublishAsync("greeting-2", message, options => { /* modify default options */ });
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

            var t1 = messageBus.Subscribe(
                new string[] { "greeting-1" },
                message =>
                {
                    System.Console.WriteLine(message);
                    return Task.CompletedTask;
                }
            );

            var t2 = messageBus.Subscribe(
                new string[] { "greeting-1" },
                message =>
                {
                    System.Console.WriteLine(message);
                    return Task.CompletedTask;
                },
                options => { /*modify default options*/ }
            );

            var t3 = messageBus.Subscribe<Greeting>(
                new string[] { "greeting-2" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );

            var t4 = messageBus.Subscribe<Greeting>(
                new string[] { "greeting-2" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                },
                options => { /*modify default options*/ }
            );

            // ------

            var t5 = messageBus.Subscribe<GreetingMessageProcessor>(new string[] { "greeting-1" });

            var t6 = messageBus.Subscribe<GreetingMessageProcessor>(
                new string[] { "greeting-1" },
                options => { /*modify default options*/ }
            );

            var t7 = messageBus.Subscribe<Greeting, GreetingMessageProcessor>(new string[] { "greeting-2" });

            var t8 = messageBus.Subscribe<Greeting, GreetingMessageProcessor>(
                new string[] { "greeting-2" },
                options => { /*modify default options*/ }
            );

            await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7, t8);
        }
    }
}
```

In .NET web applications (or applications with [Microsoft Hosting Extension](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/6.0.0-rc.2.21480.5) package installed) you can register SubscriptionMessageBus as a singlton service. Injecting SubscriptionMessageBus into a service, you can use Subscribe methods to listen on some topics and handle consumed messages.

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
            var t1 = _messageBus.Subscribe(
                new string[] { "greeting-1" },
                message =>
                {
                    System.Console.WriteLine(message);
                    return Task.CompletedTask;
                }
            );

            var t2 = _messageBus.Subscribe(
                new string[] { "greeting-1" },
                message =>
                {
                    System.Console.WriteLine(message);
                    return Task.CompletedTask;
                },
                options => { /*modify default options*/ }
            );

            var t3 = _messageBus.Subscribe<Greeting>(
                new string[] { "greeting-2" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );

            var t4 = _messageBus.Subscribe<Greeting>(
                new string[] { "greeting-2" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                },
                options => { /*modify default options*/ }
            );

            // ------

            var t5 = _messageBus.Subscribe<GreetingMessageProcessor>(new string[] { "greeting-1" });

            var t6 = _messageBus.Subscribe<GreetingMessageProcessor>(
                new string[] { "greeting-1" },
                options => { /*modify default options*/ }
            );

            var t7 = _messageBus.Subscribe<Greeting, GreetingMessageProcessor>(new string[] { "greeting-2" });

            var t8 = _messageBus.Subscribe<Greeting, GreetingMessageProcessor>(
                new string[] { "greeting-2" },
                options => { /*modify default options*/ }
            );

            await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7, t8);
        }
    }
}
```

## Publish / Subscribe
In .NET web applications (or applications with [Microsoft Hosting Extension](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/6.0.0-rc.2.21480.5) package installed) you can register MessageBus as a singlton service. Injecting MessageBus into a service, you can either do publish or subscribe operations.

```c#
public static void ConfigureServices(IServiceCollection services)
{            
    var bootstrapServers = new string[] { "localhost:9092" };
    services.AddMessageBus(bootstrapServers, bootstrapServers, services);

    services.AddHostedService<SubscriberService>();
    services.AddHostedService<PublisherService>();
}
```