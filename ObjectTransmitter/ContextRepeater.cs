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

        public void ConfigureSubscribe(Action<SubscribeConfigurator<T>> configureAction) => throw new NotImplementedException();
    }

    public class SubscribeConfigurator<T> where T : class
    {
        internal SubscribeConfigurator()
        {
        }

        public void Subscribe(Func<T, string> selector, Action<string> callback)
            => throw new NotImplementedException();

        public void Subscribe<TChild>(Func<T, TChild> selector, Action<TChild> callback)
            where TChild : struct
            => throw new NotImplementedException();

        public void Subscribe<TChild>(Func<T, TChild> selector, Action<TChild> callback, Action<SubscribeConfigurator<TChild>> subscribeAction)
            where TChild : class
            => throw new NotImplementedException();
    }
}
