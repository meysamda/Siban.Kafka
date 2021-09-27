using System;
using System.Text.Json;
using Confluent.Kafka;
using KafkaMessageBus.Abstractions;

namespace KafkaMessageBus
{
    public class DefaultDeserializer<T> : IMessageBusDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull) return default(T);

            try
            {
                var result = JsonSerializer.Deserialize<T>(data);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("unable to deserialize.", ex);
            }
        }
    }
}