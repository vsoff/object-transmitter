using ObjectTransmitter.Collectors;
using ObjectTransmitter.Reflection;
using System;

namespace ObjectTransmitter
{
    public class ContextTransmitter<T> where T : class
    {
        private readonly ObjectTrasmitterContainer _container;

        internal ContextTransmitter(T context, ObjectTrasmitterContainer container)
        {
            Context = context;
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public T Context { get; }

        public bool HasChanges() => (Context as ITransmitter)?.HasChanges(_container) ?? false;
        public void ClearChanges() => (Context as ITransmitter)?.ClearChanges(_container);
        public ContextChangesRoot CollectChanges() => new ContextChangesRoot((Context as ITransmitter)?.CollectChanges(_container));
    }
}
