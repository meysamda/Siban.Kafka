using System;
using System.Text.Json;
using Confluent.Kafka;
using MessagePack;

namespace KafkaMessageBus
{
    public static class Deserializers<T>
    {
        public static IDeserializer<T> MessagePack = new MessagePackDeserializer<T>();
        public static IDeserializer<T> MicrosoftJson = new MicrosoftJsonDeserializer<T>();

        private class MessagePackDeserializer<TInMessagePackDeserializer> : IDeserializer<TInMessagePackDeserializer>
        {
            public TInMessagePackDeserializer Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                if (isNull) return default(TInMessagePackDeserializer);

                try
                {
                    var result = MessagePackSerializer.Deserialize<TInMessagePackDeserializer>(data.ToArray());
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("unable to deserialize.", ex);
                }
            }
        }

        private class MicrosoftJsonDeserializer<TInMicrosoftJsonDeserializer> : IDeserializer<TInMicrosoftJsonDeserializer>
        {
            public TInMicrosoftJsonDeserializer Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                if (isNull) return default(TInMicrosoftJsonDeserializer);

                try
                {
                    var result = JsonSerializer.Deserialize<TInMicrosoftJsonDeserializer>(data);
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("unable to deserialize.", ex);
                }
            }
        }
    }
}