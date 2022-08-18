using System;

namespace ObjectTransmitter.Reflection
{
    public abstract class RepeaterFactory
    {
        public abstract Type RepeaterType { get; }
        public abstract object Create();
    }

    public abstract class RepeaterFactory<T> : RepeaterFactory
    {
        public override Type RepeaterType { get; } = typeof(T);

        public abstract T CreateRepeater();

        public override object Create() => CreateRepeater();
    }
}
