using System;
using System.Text.Json;
using Confluent.Kafka;
using KafkaMessageBus.Abstractions;

namespace KafkaMessageBus
{
    public class DefaultSerializer<T> : IKafkaMessageBusSerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            if (data == null) return null;

            try
            {
                var result = JsonSerializer.SerializeToUtf8Bytes(data);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("unable to serialize.", ex);
            }
        }
    }
}