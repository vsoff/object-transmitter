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
            var type = _container.GetTransmitterType<T>();
            var instance = (T)Activator.CreateInstance(type);
            return new ContextTransmitter<T>(instance);
        }

        public ContextRepeater<T> CreateRepeater<T>() where T : class
        {
            var type = _container.GetRepeaterType<T>();
            var instance = (T)Activator.CreateInstance(type);
            return new ContextRepeater<T>(instance);
        }
    }
}
