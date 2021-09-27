using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IMessageBusDeserializer<T> : IDeserializer<T>
    {
    }
}
