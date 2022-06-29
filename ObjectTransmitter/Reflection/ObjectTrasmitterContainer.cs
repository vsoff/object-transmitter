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
        private IReadOnlyDictionary<Type, TypeDescription> _descriptionByType;

        internal ObjectTrasmitterContainer(
            IEnumerable<GeneratedTypes> generatedTypes,
            IEnumerable<TypeDescription> typeDescriptions)
        {
            if (generatedTypes is null) throw new ArgumentNullException(nameof(generatedTypes));
            if (typeDescriptions is null) throw new ArgumentNullException(nameof(typeDescriptions));

            var generatedTypeByInterface = generatedTypes.ToDictionary(types => types.InterfaceType, types => types);
            var descriptionByType = new Dictionary<Type, TypeDescription>();
            foreach (var typeDescription in typeDescriptions)
            {
                if (!generatedTypeByInterface.TryGetValue(typeDescription.Type, out var typeGeneratedTypes))
                    throw new ObjectTransmitterException($"Types for `{typeDescription.Type}` not generated");

                descriptionByType[typeGeneratedTypes.InterfaceType] = typeDescription;
                descriptionByType[typeGeneratedTypes.ContractType] = typeDescription;
                descriptionByType[typeGeneratedTypes.TransmitterType] = typeDescription;
                descriptionByType[typeGeneratedTypes.RepeaterType] = typeDescription;
            }

            _generatedTypeByInterface = generatedTypeByInterface;
            _descriptionByType = descriptionByType;
        }

        internal Type GetTransmitterType<TInterface>() => GetType<TInterface>(x => x.TransmitterType);
        internal Type GetRepeaterType<TInterface>() => GetType<TInterface>(x => x.RepeaterType);
        internal Type GetContractType<TInterface>() => GetType<TInterface>(x => x.ContractType);
        
        internal TypeDescription GetDescription(Type type)
        {
            if (!_descriptionByType.TryGetValue(type, out var description))
                throw new ObjectTransmitterException($"Type `{type.FullName}` is not registered");

            return description;
        }

        private Type GetType<TInterface>(Func<GeneratedTypes, Type> selector)
        {
            var interfaceType = typeof(TInterface);
            if (!_generatedTypeByInterface.TryGetValue(interfaceType, out var generatedTypes))
                throw new ObjectTransmitterException($"Type `{interfaceType.FullName}` is not found");

            return selector.Invoke(generatedTypes);
        }
    }
}
