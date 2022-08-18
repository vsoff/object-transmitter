using System;
using System.Collections.Generic;

namespace ObjectTransmitter.Reflection.Models
{
    internal sealed class TypeDescription
    {
        public readonly Type Type;
        public readonly Type RepeaterType;
        public readonly IReadOnlyCollection<PropertyDescription> Properties;

        public TypeDescription(Type type, Type repeaterType, IReadOnlyCollection<PropertyDescription> properties)
        {
            Type = type;
            RepeaterType = repeaterType;
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }
    }
}
