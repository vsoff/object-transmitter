using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.UnitTests.TestClasses;

namespace ObjectTransmitter.UnitTests
{
    [TestClass]
    public class ReflectionTests
    {
        [TestMethod]
        public void CreateTransmitterTest()
        {
            var transmitter = ContextFactory.CreateTransmitter<ContextSample>();
            Assert.IsNotNull(transmitter);
        }
    }
}