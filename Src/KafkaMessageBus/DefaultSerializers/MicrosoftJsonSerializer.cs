using System;
using System.Text.Json;
using Confluent.Kafka;

namespace KafkaMessageBus.DefaultSerializers
{
    public class MicrosoftJsonSerializer<T> : ISerializer<T>, IDeserializer<T>
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