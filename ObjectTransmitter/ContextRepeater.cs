using System;

namespace ObjectTransmitter
{
    public class ContextRepeater<T>
        where T : class
    {
        internal ContextRepeater(T context)
        {
            Context = context;
        }

        public T Context { get; }

        public void ApplyChanges(ContextChangesRoot changes) => throw new NotImplementedException();
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
