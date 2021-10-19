using System;
using Confluent.Kafka;
using MessagePack;

namespace KafkaMessageBus.DefaultSerializers
{
    public class MessagePackSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            if (data == null) return null;

            try
            {
                var result = MessagePackSerializer.Serialize(data);
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
                var result = MessagePackSerializer.Deserialize<T>(data.ToArray());
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("unable to deserialize.", ex);
            }
        }
    }
}