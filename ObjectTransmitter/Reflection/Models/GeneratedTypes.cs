using System;

namespace ObjectTransmitter.Reflection.Models
{
    internal sealed class GeneratedTypes
    {
        public readonly Type InterfaceType;
        public readonly Type TransmitterType;
        public readonly Type RepeaterType;

        public GeneratedTypes(Type interfaceType, Type transmitterType, Type repeaterType)
        {
            InterfaceType = interfaceType;
            TransmitterType = transmitterType;
            RepeaterType = repeaterType;
        }
    }
}
