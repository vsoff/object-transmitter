using ObjectTransmitter.Exceptions;
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
        private readonly IDictionary<Type, TypeDescription> _descriptionByType;

        private int _lastPropertyId = 0;
        private ITransportSerializer _serializer;

        public ObjectTrasmitterContainerBuilder()
        {
            _descriptionByType = new Dictionary<Type, TypeDescription>();
            _serializer = new DefaultTransportSerializer();
        }

        public void SetSerializer(ITransportSerializer serializer) => _serializer = serializer;

        public void RegisterInteface<TInterface>() => RegisterInteface(typeof(TInterface));

        public void RegisterInteface(Type type)
        {
            TypeValidator.ThrowIfTypeInvalid(type);

            if (IsRegistered(type))
                throw new ObjectTransmitterException($"Type `{type.FullName}` is already registered");

            var description = CreateDescription(type);
            _descriptionByType[type] = description;
        }

        public ObjectTrasmitterContainer BuildContainer()
        {
            var generatedTypes = new List<GeneratedTypes>();

            foreach (var type in _descriptionByType)
            {
                var transmitterType = ClassGenerator.GenerateTransmitter(type.Value);
                var repeaterType = ClassGenerator.GenerateRepeater(type.Value);
                var contractType = ClassGenerator.GenerateContract(type.Value);

                generatedTypes.Add(new GeneratedTypes(type.Key, transmitterType, repeaterType, contractType));
            }

            return new ObjectTrasmitterContainer(_serializer, generatedTypes, _descriptionByType.Values);
        }

        private bool IsRegistered(Type type) => _descriptionByType.ContainsKey(type);

        private TypeDescription CreateDescription(Type type)
        {
            var properties = type.GetProperties()
                .Select(propertyInfo => new PropertyDescription(Interlocked.Increment(ref _lastPropertyId), propertyInfo))
                .ToList();

            return new TypeDescription(type, properties);
        }
    }
}
