using System;

namespace ObjectTransmitter.Reflection.Models
{
    internal sealed class PropertyDescription
    {
        public readonly int PropertyId;
        public readonly string Name;
        public readonly Type Type;

        public PropertyDescription(int propertyId, string propertyName, Type type)
        {
            PropertyId = propertyId;
            Name = propertyName;
            Type = type;
        }
    }
}
