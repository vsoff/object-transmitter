using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.Reflection;
using ObjectTransmitter.UnitTests.TestClasses;

namespace ObjectTransmitter.UnitTests
{
    [TestClass]
    public class ReflectionTests
    {
        [TestMethod]
        public void CreateTransmitterTest()
        {
            var context = ClassGenerator.GenerateClass<IContextSample>();
            context.DoubleProp = 2.3;
            context.IntProp = 321;
            context.StringProp = "sdsdada";
        }
    }
}