using ObjectTransmitter.Exceptions;
using ObjectTransmitter.Reflection.Models;
using ObjectTransmitter.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectTransmitter.Reflection
{
    public sealed class ObjectTrasmitterContainer
    {
        private readonly IReadOnlyDictionary<Type, GeneratedTypes> _generatedTypeByInterface;
        private readonly IReadOnlyDictionary<Type, TypeDescription> _descriptionByType;
        private readonly IReadOnlyDictionary<int, Type> _typeByPropertyId;
        private readonly ITransportSerializer _serializer;

        internal ObjectTrasmitterContainer(
            ITransportSerializer serializer,
            IEnumerable<GeneratedTypes> generatedTypes,
            IEnumerable<TypeDescription> typeDescriptions)
        {
            if (serializer is null) throw new ArgumentNullException(nameof(serializer));
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

            var typeByPropertyId = descriptionByType.Values
                .SelectMany(description => description.Properties)
                .GroupBy(prop => prop.PropertyId)
                .ToDictionary(prop => prop.Key, prop => prop.First().Type);

            _typeByPropertyId = typeByPropertyId;
            _serializer = serializer;
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

        internal byte[] Serialize(object value, int propertyId)
        {
            if (!_typeByPropertyId.TryGetValue(propertyId, out var type))
                throw new ObjectTransmitterException($"Type for property with id `{propertyId}` not found");

            var transmitterType = _generatedTypeByInterface.TryGetValue(type, out var generatedTypes) ? generatedTypes.TransmitterType : null;

            return _serializer.Serialize(value, transmitterType ?? type);
        }

        internal object Deserialize(byte[] bytes, int propertyId)
        {
            if (!_typeByPropertyId.TryGetValue(propertyId, out var type))
                throw new ObjectTransmitterException($"Type for property with id `{propertyId}` not found");

            var repeaterType = _generatedTypeByInterface.TryGetValue(type, out var generatedTypes) ? generatedTypes.RepeaterType : null;

            return _serializer.Deserialize(bytes, repeaterType ?? type);
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
