using System;
using System.Reflection;

namespace ObjectTransmitter.Reflection.Models
{
    internal sealed class PropertyDescription
    {
        public readonly int PropertyId;
        public readonly PropertyInfo PropertyInfo;

        public PropertyDescription(int propertyId, PropertyInfo propertyInfo)
        {
            PropertyId = propertyId;
            PropertyInfo = propertyInfo;
        }

        public string Name => PropertyInfo.Name;
        public Type Type => PropertyInfo.PropertyType;
    }
}
