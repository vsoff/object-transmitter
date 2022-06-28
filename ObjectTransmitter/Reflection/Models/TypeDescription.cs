using System;
using System.Collections.Generic;

namespace ObjectTransmitter.Reflection.Models
{
    internal sealed class TypeDescription
    {
        public readonly Type Type;
        public readonly IReadOnlyCollection<PropertyDescription> Properties;

        public TypeDescription(Type type, IReadOnlyCollection<PropertyDescription> properties)
        {
            Type = type;
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }
    }
}
