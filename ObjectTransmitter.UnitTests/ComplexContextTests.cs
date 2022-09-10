using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.Reflection;
using ObjectTransmitter.UnitTests.Helpers;
using ObjectTransmitter.UnitTests.TestContexts;

namespace ObjectTransmitter.UnitTests
{
    [TestClass]
    public class ComplexContextTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ContextFactory _contextFactory;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void Initialize()
        {
            var builder = new ObjectTrasmitterContainerBuilder();
            builder.RegisterInterface(new ComplexContextRepeaterFactory());
            builder.RegisterInterface(new ItemRepeaterFactory());
            builder.RegisterInterface(new ItemInfoRepeaterFactory());
            var container = builder.BuildContainer();
            
            _contextFactory = new ContextFactory(container);
        }

        [TestMethod]
        public void ApplyEmptyTest()
        {
            var transmitter = _contextFactory.CreateTransmitter<IComplexContext>();
            var repeater = _contextFactory.CreateRepeater<IComplexContext>();

            Assert.IsFalse(transmitter.HasChanges());

            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
        }

        [TestMethod]
        public void ApplyWithChangesTest()
        {
            var transmitter = _contextFactory.CreateTransmitter<IComplexContext>();
            var repeater = _contextFactory.CreateRepeater<IComplexContext>();

            transmitter.Context.Number = 123;
            transmitter.Context.Item = _contextFactory.CreateTransmitterPart<IItem>();
            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);

            transmitter.Context.Item.Id = 6663;
            transmitter.Context.Item.Info = _contextFactory.CreateTransmitterPart<IItemInfo>();
            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);

            transmitter.Context.Item.Info.Description = "Item description";
            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);

            transmitter.Context.Item.Info = _contextFactory.CreateTransmitterPart<IItemInfo>();
            transmitter.Context.Item.Info.Description = "Second description";
            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);

            transmitter.Context.Item.Info = null;
            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);

            transmitter.Context.Item = null;
            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
        }
    }
}