# Introduction 
**Siban.Kafka** is an abstraction on top of [Confluent.Kafka](https://github.com/confluentinc/confluent-kafka-dotnet/) which is Confluent's .NET client for [Apache Kafka](kafka.apache.org) and the [Confluent Platform](https://www.confluent.io/product/). It enables you to create a message bus and do publish / subcribe operations simply or in advanced way.

# Referencing
Siban.Kafka is distributed via NuGet in [Siban.Kafka](https://www.nuget.org/packages/Siban.Kafka/) package.

To install Siban.Kafka from within Visual Studio, search for Siban.Kafka in the NuGet Package Manager UI, or run the following command in the Package Manager Console:

`Install-Package Siban.Kafka -Version <last-version>`

To add a reference to a dotnet core project, execute the following at the command line:

`dotnet add package -v <last-version> Siban.Kafka`

# Usage
Take a look in the [Samples](https://github.com/meysamda/Siban.Kafka/tree/master/docs/Samples) directory for example usage. As you can see all you need is to initiate a message bus and then do publish / subscribe operations upon it.

## Publish

Create a new instance from `PublishMessageBus`, then send a message using Publish methods on a topic. You should use the Async method if you would like to wait for the result of your publish requests before proceeding. You might typically want to do this in highly concurrent scenarios, for example in the context of handling web requests. Behind the scenes, the client (Confluent.Kafka) will manage optimizing communication with the Kafka brokers for you, batching requests as appropriate.

```c#
[HttpGet("1")]
public bool Get()
{
    _messageBus.PublishMessageValue("greeting-1", "hello world-1", options => {
            options.ProducerConfig.ClientId = Dns.GetHostName();
            options.ProducerConfig.Acks = Acks.Leader;
            options.ProducerConfig.MessageTimeoutMs = 1000;
        },
        dr => {
            if (dr.Error.IsError) {
                System.Console.WriteLine(dr.Error.Reason);
            }
        });

    return true;
}

[HttpGet("2")]
public bool Get2()
{
    var value = new Greeting { Body = "hello world" };
    _messageBus.PublishMessageValue("greeting-2", value, options => {
            options.ProducerConfig.ClientId = Dns.GetHostName();
            options.ProducerConfig.Acks = Acks.Leader;
            options.ProducerConfig.MessageTimeoutMs = 1000;
        },
        dr => {
            if (dr.Error.IsError) {
                System.Console.WriteLine(dr.Error.Reason);
            }
        });

    return true;
}

[HttpGet("3")]
public bool Get3()
{
    var message = new Message<string, Greeting> {
        Value = new Greeting { Body = "hello world" }
    };

    _messageBus.PublishMessage("greeting-3", message, options => {
            options.ProducerConfig.ClientId = Dns.GetHostName();
            options.ProducerConfig.Acks = Acks.Leader;
            options.ProducerConfig.MessageTimeoutMs = 1000;
        },
        dr => {
            if (dr.Error.IsError) {
                System.Console.WriteLine(dr.Error.Reason);
            }
        });

    return true;
}
```

In .NET web applications (or applications with [Microsoft Hosting Extension](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/6.0.0-rc.2.21480.5) package installed) you can register PublishMessageBus as a singlton service. Injecting PublishMessageBus into a service, you can use Publish methods to produce your messages on desired topics.

```c#
public static void ConfigureServices(IServiceCollection services)
{            
    var bootstrapServers = new string[] { "localhost:9092" };
    services.AddPublishMessageBus(bootstrapServers);
}
```

## Subscribe

Create a new instance from `SubscriptionMessageBus`, then subscribe on some topics using Subscribe methods.

```c#
var t1 = _messageBus.SubscribeForMessageValueAsync<string>(
    new [] { "greeting-1" },
    (value) => _handler.Handle(value, cancellationToken),
    options => {
        options.ConsumerConfig.GroupId = "greeting";
        options.ConsumerConfig.AllowAutoCreateTopics = true;
        options.ConsumerConfig.EnableAutoCommit = false;
        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest;
    },
    cancellationToken
);

var t2 = _messageBus.SubscribeForMessageValueAsync<Greeting>(
    new [] { "greeting-2" },
    (value) => _handler.Handle(value, cancellationToken),
    options => {
        options.ConsumerConfig.GroupId = "greeting";
        options.ConsumerConfig.AllowAutoCreateTopics = true;
        options.ConsumerConfig.EnableAutoCommit = false;
        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest;
    },
    cancellationToken
);

var t3 = _messageBus.SubscribeForMessageAsync<string, Greeting>(
    new [] { "greeting-3" },
    (message) => _handler.Handle(message, cancellationToken),
    options => {
        options.ConsumerConfig.GroupId = "greeting";
        options.ConsumerConfig.AllowAutoCreateTopics = true;
        options.ConsumerConfig.EnableAutoCommit = false;
        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest;
    },
    cancellationToken
);

await Task.WhenAll(t1, t2, t3);
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

## Publish / Subscribe
In .NET web applications (or applications with [Microsoft Hosting Extension](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/6.0.0-rc.2.21480.5) package installed) you can register MessageBus as a singlton service. Injecting MessageBus into a service, you can either do publish or subscribe operations.

```c#
public static void ConfigureServices(IServiceCollection services)
{            
    var bootstrapServers = new string[] { "localhost:9092" };
    services.AddMessageBus(bootstrapServers, bootstrapServers);

    services.AddHostedService<SubscriberService>();
}
```
