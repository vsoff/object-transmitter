using ObjectTransmitter.Exceptions;
using ObjectTransmitter.Reflection.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectTransmitter.Reflection
{
    public sealed class ObjectTrasmitterContainer
    {
        private IReadOnlyDictionary<Type, GeneratedTypes> _generatedTypeByInterface;

        internal ObjectTrasmitterContainer(IEnumerable<GeneratedTypes> generatedTypes)
        {
            _generatedTypeByInterface = generatedTypes.ToDictionary(types => types.InterfaceType, types => types);
        }

        public Type GetTransmitterType<TInterface>() => GetType<TInterface>(x => x.TransmitterType);
        public Type GetRepeaterType<TInterface>() => GetType<TInterface>(x => x.RepeaterType);
        public Type GetContractType<TInterface>() => GetType<TInterface>(x => x.ContractType);

        private Type GetType<TInterface>(Func<GeneratedTypes, Type> selector)
        {
            var interfaceType = typeof(TInterface);
            if (!_generatedTypeByInterface.TryGetValue(interfaceType, out var generatedTypes))
                throw new ObjectTransmitterException($"Type `{interfaceType.FullName}` is not found");

            return selector.Invoke(generatedTypes);
        }
    }
}
