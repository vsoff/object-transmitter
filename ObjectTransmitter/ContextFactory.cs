using ObjectTransmitter.Reflection;
using System;

namespace ObjectTransmitter
{
    public sealed class ContextFactory
    {
        private ObjectTrasmitterContainer _container;

        public ContextFactory(ObjectTrasmitterContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public ContextTransmitter<T> CreateTransmitter<T>() where T : class
        {
            var instance = CreateTransmitterPart<T>();
            return new ContextTransmitter<T>(instance, _container);
        }

        public ContextRepeater<T> CreateRepeater<T>() where T : class
        {
            var instance = CreateRepeaterPart<T>();
            return new ContextRepeater<T>(instance, _container);
        }

        public T CreateTransmitterPart<T>() where T : class
        {
            var type = _container.GetTransmitterType<T>();
            return (T)Activator.CreateInstance(type);
        }

        public T CreateRepeaterPart<T>() where T : class
        {
            var type = _container.GetRepeaterType<T>();
            return (T)Activator.CreateInstance(type);
        }
    }
}
