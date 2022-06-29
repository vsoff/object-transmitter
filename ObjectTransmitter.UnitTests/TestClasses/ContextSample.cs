namespace ObjectTransmitter.UnitTests.TestClasses
{
    public interface IContextSample
    {
        public int IntProp { get; set; }
        public double DoubleProp { get; set; }
        public string? StringProp { get; set; }
        public IInnerObject InnerObjectProp { get; set; }
    }

    public interface IInnerObject
    {
        public int IntProp1 { get; set; }
        public int IntProp2 { get; set; }
    }
}
