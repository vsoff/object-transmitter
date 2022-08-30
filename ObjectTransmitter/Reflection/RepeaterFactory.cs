using System;

namespace ObjectTransmitter.Reflection
{
    public abstract class RepeaterFactory
    {
        public abstract Type RepeaterInterfaceType { get; }
        public abstract Type RepeaterType { get; }
        public abstract object Create();
    }

    public abstract class RepeaterFactory<TInterface, TType> : RepeaterFactory
    {
        public override Type RepeaterInterfaceType { get; } = typeof(TInterface);
        public override Type RepeaterType { get; } = typeof(TType);

        public abstract TType CreateRepeater();

        public override object Create() => CreateRepeater();
    }
}
