using ObjectTransmitter.Collectors;
using ObjectTransmitter.Reflection;
using System;

namespace ObjectTransmitter
{
    public class ContextRepeater<T>
        where T : class
    {
        private readonly ObjectTrasmitterContainer _container;

        internal ContextRepeater(T context, ObjectTrasmitterContainer container)
        {
            Context = context;
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public T Context { get; }

        public void ApplyChanges(ContextChangesRoot changes)
        {
            if (changes == null) throw new ArgumentNullException(nameof(changes));

            (Context as IRepeater)?.ApplyChanges(changes.ChangedNodes, _container);
        }
    }
}
