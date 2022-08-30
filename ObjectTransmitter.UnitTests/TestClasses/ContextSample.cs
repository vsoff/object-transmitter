using ObjectTransmitter.Reflection;

namespace ObjectTransmitter.UnitTests.TestClasses
{
    public interface IContextSample
    {
        public int IntProp { get; set; }
        public double DoubleProp { get; set; }
        public string? StringProp { get; set; }
        public IInnerObject? InnerObjectProp { get; set; }
    }

    public interface IInnerObject
    {
        public int IntProp1 { get; set; }
        public int IntProp2 { get; set; }
    }

    public class ContextSample : IContextSample
    {
        public ContextSample()
        {

        }

        private int _intProp;
        public int IntProp { get => _intProp; set => _intProp = value; }

        private double _doubleProp;
        public double DoubleProp { get => _doubleProp; set => _doubleProp = value; }

        private string? _stringProp;
        public string? StringProp { get => _stringProp; set => _stringProp = value; }

        private IInnerObject? _innerObjectProp;
        public IInnerObject? InnerObjectProp { get => _innerObjectProp; set => _innerObjectProp = value; }
    }

    public class InnerObject : IInnerObject
    {
        public InnerObject()
        {

        }

        private int _intProp1;
        public int IntProp1 { get => _intProp1; set => _intProp1 = value; }

        private int _intProp2;
        public int IntProp2 { get => _intProp2; set => _intProp2 = value; }
    }

    public class ContextSampleRepeaterFactory : RepeaterFactory<IContextSample, ContextSample>
    {
        public override ContextSample CreateRepeater() => new ContextSample();
    }

    public class InnerObjectRepeaterFactory : RepeaterFactory<IInnerObject, InnerObject>
    {
        public override InnerObject CreateRepeater() => new InnerObject();
    }
}
