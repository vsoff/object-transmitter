using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.UnitTests.TestClasses;

namespace ObjectTransmitter.UnitTests
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        public void CreateTransmitterTest()
        {
            var transmitter = ContextFactory.CreateTransmitter<ContextSample>();
            Assert.IsNotNull(transmitter);
        }

        [TestMethod]
        public void CreateRepeaterTest()
        {
            var repeater = ContextFactory.CreateRepeater<ContextSample>();
            Assert.IsNotNull(repeater);
        }

        [TestMethod]
        public void ApplyChangesEmptyTest()
        {
            // Create contexts.
            var transmitter = ContextFactory.CreateTransmitter<ContextSample>();
            var repeater = ContextFactory.CreateRepeater<ContextSample>();
            Assert.IsNotNull(transmitter);
            Assert.IsNotNull(repeater);

            // Check empty applying.
            var changes = transmitter.CollectChanges();
            Assert.AreEqual(changes.GetChangedNodesCount(), 0);
            repeater.ApplyChanges(changes);
        }

        [TestMethod]
        public void ApplyChangesWithChangesTest()
        {
            // Create contexts.
            var transmitter = ContextFactory.CreateTransmitter<ContextSample>();
            var repeater = ContextFactory.CreateRepeater<ContextSample>();
            Assert.IsNotNull(transmitter);
            Assert.IsNotNull(repeater);

            // Change transmitter values.
            transmitter.Context.IntProp = 123;
            transmitter.Context.DoubleProp = 2.3;

            // Check empty applying.
            var changes = transmitter.CollectChanges();
            Assert.AreEqual(changes.GetChangedNodesCount(), 2);
            repeater.ApplyChanges(changes);

            // Check applyed values.
            Assert.AreEqual(transmitter.Context.IntProp, repeater.Context.IntProp);
            Assert.AreEqual(transmitter.Context.DoubleProp, repeater.Context.DoubleProp);
        }
    }
}