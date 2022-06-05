using System;

namespace ObjectTransmitter
{
    public class ContextRepeater<T> where T : class
    {
        internal ContextRepeater(T context)
        {
            Context = context;
        }

        public T Context { get; }

        public void ApplyChanges(ContextChangesRoot changes) => throw new NotImplementedException();
    }
}
