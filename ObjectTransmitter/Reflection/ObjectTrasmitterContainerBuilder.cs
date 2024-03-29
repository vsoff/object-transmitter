﻿using ObjectTransmitter.Exceptions;
using ObjectTransmitter.Extensions;
using ObjectTransmitter.Reflection.Models;
using ObjectTransmitter.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ObjectTransmitter.Reflection
{
    public sealed class ObjectTrasmitterContainerBuilder
    {
        private readonly IDictionary<Type, RepeaterFactory> _repeaterFactoryByType;
        private readonly IDictionary<Type, TypeDescription> _descriptionByType;

        private int _lastPropertyId = 0;
        private ITransportSerializer _serializer;

        public ObjectTrasmitterContainerBuilder()
        {
            _repeaterFactoryByType = new Dictionary<Type, RepeaterFactory>();
            _descriptionByType = new Dictionary<Type, TypeDescription>();
            _serializer = new DefaultTransportSerializer();
        }

        public void SetSerializer(ITransportSerializer serializer) => _serializer = serializer;

        public void RegisterInterface<TInterface>() => RegisterInterfaceInternal(typeof(TInterface), null, null);
        public void RegisterInterface<TInterface, TRepeater>(RepeaterFactory<TInterface, TRepeater> repeaterFactory) where TRepeater : TInterface
            => RegisterInterfaceInternal(typeof(TInterface), typeof(TRepeater), repeaterFactory);

        private void RegisterInterfaceInternal(Type type, Type repeaterType, RepeaterFactory repeaterFactory)
        {
            TypeValidator.ThrowIfTypeInvalid(type);

            if (IsRegistered(type))
                throw new ObjectTransmitterException($"Type `{type.FullName}` is already registered");

            var description = CreateDescription(type, repeaterType);
            _descriptionByType[type] = description;
            
            if (repeaterFactory != null)
                _repeaterFactoryByType[type] = repeaterFactory;
        }

        public ObjectTrasmitterContainer BuildContainer()
        {
            var generatedTypes = new List<GeneratedTypes>();

            foreach (var type in _descriptionByType)
            {
                var transmitterType = ClassGenerator.GenerateTransmitter(type.Value);

                generatedTypes.Add(new GeneratedTypes(type.Key, transmitterType));
            }

            return new ObjectTrasmitterContainer(_serializer, generatedTypes, _descriptionByType.Values, _repeaterFactoryByType.Values);
        }

        private bool IsRegistered(Type type) => _descriptionByType.ContainsKey(type);

        private TypeDescription CreateDescription(Type type, Type repeaterType)
        {
            var properties = type.GetProperties()
                .Select(propertyInfo => new PropertyDescription(Interlocked.Increment(ref _lastPropertyId), propertyInfo))
                .ToList();

            return new TypeDescription(type, repeaterType, properties);
        }
    }
}
