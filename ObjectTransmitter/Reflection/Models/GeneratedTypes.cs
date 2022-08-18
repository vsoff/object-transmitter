using System;

namespace ObjectTransmitter.Reflection.Models
{
    internal sealed class GeneratedTypes
    {
        public readonly Type InterfaceType;
        public readonly Type TransmitterType;

        public GeneratedTypes(Type interfaceType, Type transmitterType)
        {
            InterfaceType = interfaceType;
            TransmitterType = transmitterType;
        }
    }
}
