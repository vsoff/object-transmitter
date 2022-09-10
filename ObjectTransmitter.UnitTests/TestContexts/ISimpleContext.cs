using ObjectTransmitter.Reflection;

namespace ObjectTransmitter.UnitTests.TestContexts
{
    public interface ISimpleContext
    {
        public int IntProp { get; set; }
        public float FloatProp { get; set; }
        public double DoubleProp { get; set; }
        public byte ByteProp { get; set; }
        public string? StringProp { get; set; }
        public int? NullableIntProp { get; set; }
    }

    public class RepeaterSimpleContext : ISimpleContext
    {
        public int IntProp { get; set; }
        public float FloatProp { get; set; }
        public double DoubleProp { get; set; }
        public byte ByteProp { get; set; }
        public string? StringProp { get; set; }
        public int? NullableIntProp { get; set; }
    }

    public class SimpleContextRepeaterFactory : RepeaterFactory<ISimpleContext, RepeaterSimpleContext>
    {
        public override RepeaterSimpleContext CreateRepeater() => new();
    }
}
