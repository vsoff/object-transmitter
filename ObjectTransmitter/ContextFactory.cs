using System;

namespace ObjectTransmitter
{
    public static class ContextFactory
    {
        public static ContextTransmitter<T> CreateTransmitter<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public static ContextRepeater<T> CreateRepeater<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
