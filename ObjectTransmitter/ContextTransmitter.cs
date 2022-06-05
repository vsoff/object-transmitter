using System;

namespace ObjectTransmitter
{
    public class ContextTransmitter<T> where T : class
    {
        internal ContextTransmitter(T context)
        {
            Context = context;
        }

        public T Context { get; }

        public bool HasChanges() => throw new NotImplementedException();
        public void ClearChanges() => throw new NotImplementedException();
        public ContextChangesRoot CollectChanges() => throw new NotImplementedException();
    }
}
