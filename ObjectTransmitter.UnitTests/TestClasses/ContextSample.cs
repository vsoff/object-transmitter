namespace ObjectTransmitter.UnitTests.TestClasses
{
    internal class ContextSample
    {
        public int IntProp { get; set; }
        public double DoubleProp { get; set; }
        public string? StringProp { get; set; }
        public InnerObject InnerObjectProp { get; set; }
    }

    internal class InnerObject
    {
        public int IntProp1 { get; set; }
        public int IntProp2 { get; set; }
    }
}
