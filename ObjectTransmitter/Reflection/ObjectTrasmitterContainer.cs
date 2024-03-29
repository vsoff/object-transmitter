﻿using ObjectTransmitter.Exceptions;
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
        private readonly RepeaterInstanceFactory _repeaterInstanceFactory;

        internal ObjectTrasmitterContainer(
            ITransportSerializer serializer,
            IEnumerable<GeneratedTypes> generatedTypes,
            IEnumerable<TypeDescription> typeDescriptions,
            IEnumerable<RepeaterFactory> repeaterFactories)
        {
            if (serializer is null) throw new ArgumentNullException(nameof(serializer));
            if (generatedTypes is null) throw new ArgumentNullException(nameof(generatedTypes));
            if (typeDescriptions is null) throw new ArgumentNullException(nameof(typeDescriptions));
            if (repeaterFactories is null) throw new ArgumentNullException(nameof(repeaterFactories));

            var generatedTypeByInterface = generatedTypes.ToDictionary(types => types.InterfaceType, types => types);
            var descriptionByType = new Dictionary<Type, TypeDescription>();
            foreach (var typeDescription in typeDescriptions)
            {
                if (!generatedTypeByInterface.TryGetValue(typeDescription.Type, out var typeGeneratedTypes))
                    throw new ObjectTransmitterException($"Types for `{typeDescription.Type}` not generated");

                descriptionByType[typeGeneratedTypes.InterfaceType] = typeDescription;
                descriptionByType[typeGeneratedTypes.TransmitterType] = typeDescription;

                if (typeDescription.RepeaterType != null)
                    descriptionByType[typeDescription.RepeaterType] = typeDescription;
            }

            var typeByPropertyId = descriptionByType.Values
                .SelectMany(description => description.Properties)
                .GroupBy(prop => prop.PropertyId)
                .ToDictionary(prop => prop.Key, prop => prop.First().Type);

            var repeaterInstanceFactory = new RepeaterInstanceFactory(repeaterFactories);

            _typeByPropertyId = typeByPropertyId;
            _serializer = serializer;
            _generatedTypeByInterface = generatedTypeByInterface;
            _descriptionByType = descriptionByType;
            _repeaterInstanceFactory = repeaterInstanceFactory;
        }

        internal Type GetTransmitterType<TInterface>()
        {
            if (!TryGetTransmitterType<TInterface>(out var transmitterType))
                throw new ObjectTransmitterException($"Transmitter for type `{typeof(TInterface).FullName}` is not found");

            return transmitterType;
        }

        internal bool TryGetTransmitterType<TInterface>(out Type transmitterType)
        {
            var isExists = _generatedTypeByInterface.TryGetValue(typeof(TInterface), out var generatedTypes);
            transmitterType = generatedTypes?.TransmitterType;
            return isExists;
        }
        
        internal bool TryGetDescription(Type type, out TypeDescription typeDescription)
            => _descriptionByType.TryGetValue(type, out typeDescription);

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

            return _serializer.Serialize(value, type);
        }

        internal byte[] Serialize(object value, Type type)
        {
            var transmitterType = _generatedTypeByInterface.TryGetValue(type, out var generatedTypes) ? generatedTypes.TransmitterType : null;
            return _serializer.Serialize(value, transmitterType ?? type);
        }

        internal object Deserialize(byte[] bytes, int propertyId)
        {
            if (!_typeByPropertyId.TryGetValue(propertyId, out var type))
                throw new ObjectTransmitterException($"Type for property with id `{propertyId}` not found");

            return Deserialize(bytes, type);
        }

        internal object Deserialize(byte[] bytes, Type type)
        {
            if (_generatedTypeByInterface.ContainsKey(type))
                throw new ObjectTransmitterException($"Repeater should be instantiated by factory");

            return _serializer.Deserialize(bytes, type);
        }

        internal bool IsRepeaterRegistered(Type interfaceType) => _repeaterInstanceFactory.IsRepeaterRegistered(interfaceType);

        internal object CreateRepeaterInstance(Type interfaceType) => _repeaterInstanceFactory.CreateInstance(interfaceType);
    }
}
