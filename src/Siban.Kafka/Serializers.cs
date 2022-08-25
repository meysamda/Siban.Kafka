using System;
using System.Text.Json;
using Confluent.Kafka;
using MessagePack;

namespace Siban.Kafka
{
    public static class Serializers<T>
    {
        public static ISerializer<T> MessagePack = new MessagePackSerializer<T>();
        public static ISerializer<T> MicrosoftJson = new MicrosoftJsonSerializer<T>();

        private class MessagePackSerializer<TInMessagePackSerializer> : ISerializer<TInMessagePackSerializer>
        {
            public byte[] Serialize(TInMessagePackSerializer data, SerializationContext context)
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
        }

        private class MicrosoftJsonSerializer<TInMicrosoftJsonSerializer> : ISerializer<TInMicrosoftJsonSerializer>
        {
            public byte[] Serialize(TInMicrosoftJsonSerializer data, SerializationContext context)
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
}