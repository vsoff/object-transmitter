using System;

namespace ObjectTransmitter.Serialization
{
    public interface ITransportSerializer
    {
        byte[] Serialize(object value, Type type);
        object Deserialize(byte[] bytes, Type type);
    }
}
