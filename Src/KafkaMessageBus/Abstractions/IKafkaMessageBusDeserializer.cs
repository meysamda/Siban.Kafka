using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IKafkaMessageBusDeserializer<T> : IDeserializer<T>
    {
    }
}
