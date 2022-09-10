using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.Reflection;
using ObjectTransmitter.UnitTests.Helpers;
using ObjectTransmitter.UnitTests.TestContexts;
using System.Linq;

namespace ObjectTransmitter.UnitTests
{

    [TestClass]
    public class SimpleContextTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ContextFactory _contextFactory;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void Initialize()
        {
            var builder = new ObjectTrasmitterContainerBuilder();
            builder.RegisterInterface(new SimpleContextRepeaterFactory());
            var container = builder.BuildContainer();
            
            _contextFactory = new ContextFactory(container);
        }

        [TestMethod]
        public void CreateTransmitterTest()
        {
            var transmitter = _contextFactory.CreateTransmitter<ISimpleContext>();
            Assert.IsNotNull(transmitter);
        }

        [TestMethod]
        public void CreateRepeaterTest()
        {
            var repeater = _contextFactory.CreateRepeater<ISimpleContext>();
            Assert.IsNotNull(repeater);
        }

        [TestMethod]
        public void ApplyEmptyTest()
        {
            var transmitter = _contextFactory.CreateTransmitter<ISimpleContext>();
            var repeater = _contextFactory.CreateRepeater<ISimpleContext>();

            Assert.IsFalse(transmitter.HasChanges());

            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
        }

        [TestMethod]
        public void ApplyWithChangesTest()
        {
            var transmitter = _contextFactory.CreateTransmitter<ISimpleContext>();
            var repeater = _contextFactory.CreateRepeater<ISimpleContext>();

            // Changing values.
            foreach (var index in Enumerable.Range(1, 3))
            {
                transmitter.Context.IntProp = 1004 + index;
                transmitter.Context.ByteProp = (byte)(75 + index);
                transmitter.Context.StringProp = $"Index: {index}";
                transmitter.Context.FloatProp = 133.4f + index;
                transmitter.Context.DoubleProp = 562.13 + index;
                transmitter.Context.NullableIntProp = index % 2 == 0 ? null : 1337 + index;

                TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
            }
        }
    }
}