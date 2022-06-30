using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.Reflection;
using ObjectTransmitter.UnitTests.TestClasses;

namespace ObjectTransmitter.UnitTests
{
    [TestClass]
    public class ContextTests
    {
        private ContextFactory _contextFactory;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new ObjectTrasmitterContainerBuilder();
            builder.RegisterInteface<IContextSample>();
            builder.RegisterInteface<IInnerObject>();
            var container = builder.BuildContainer();

            _contextFactory = new ContextFactory(container);
        }

        [TestMethod]
        public void CreateTransmitterTest()
        {
            var transmitter = _contextFactory.CreateTransmitter<IContextSample>();
            Assert.IsNotNull(transmitter);
        }

        [TestMethod]
        public void CreateRepeaterTest()
        {
            var repeater = _contextFactory.CreateRepeater<IContextSample>();
            Assert.IsNotNull(repeater);
        }

        [TestMethod]
        public void ApplyChangesEmptyTest()
        {
            // Create contexts.
            var transmitter = _contextFactory.CreateTransmitter<IContextSample>();
            var repeater = _contextFactory.CreateRepeater<IContextSample>();
            Assert.IsNotNull(transmitter);
            Assert.IsNotNull(repeater);

            // Check empty applying.
            var changes = transmitter.CollectChanges();
            repeater.ApplyChanges(changes);
        }

        [TestMethod]
        public void ApplyChangesWithChangesTest()
        {
            // Create contexts.
            var transmitter = _contextFactory.CreateTransmitter<IContextSample>();
            var repeater = _contextFactory.CreateRepeater<IContextSample>();
            Assert.IsNotNull(transmitter);
            Assert.IsNotNull(repeater);

            // Change transmitter values.
            transmitter.Context.IntProp = 321;
            transmitter.Context.DoubleProp = 2.3;
            transmitter.Context.StringProp = "sdsdada";
            transmitter.Context.InnerObjectProp = _contextFactory.CreateTransmitterPart<IInnerObject>();
            transmitter.Context.InnerObjectProp.IntProp1 = 555;
            transmitter.Context.InnerObjectProp.IntProp2 = 666;

            // Change values.
            var changes = transmitter.CollectChanges();
            repeater.ApplyChanges(changes);
            transmitter.ClearChanges();

            // Check applyed values.
            Assert.AreEqual(transmitter.Context.IntProp, repeater.Context.IntProp);
            Assert.AreEqual(transmitter.Context.DoubleProp, repeater.Context.DoubleProp);

            // Change transmitter values (2).
            transmitter.Context.InnerObjectProp.IntProp1 = 1000;
            transmitter.Context.InnerObjectProp.IntProp2 = 1001;

            // Change values (2).
            var changes2 = transmitter.CollectChanges();
            repeater.ApplyChanges(changes2);
            transmitter.ClearChanges();

            // Check applyed values (2).
            Assert.AreEqual(transmitter.Context.IntProp, repeater.Context.IntProp);
            Assert.AreEqual(transmitter.Context.DoubleProp, repeater.Context.DoubleProp);
        }

        [TestMethod]
        public void RepeaterSubscriptionConfigureTest()
        {
            var repeater = _contextFactory.CreateRepeater<IContextSample>();

            var intChanged = false;
            var doubleChanged = false;
            var stringChanged = false;
            var innerObjectChanged = false;
            var int1Changed = false;
            var int2Changed = false;

            repeater.ConfigureSubscribe(root =>
            {
                root.Subscribe(context => context.IntProp, intValue => intChanged = true);
                root.Subscribe(context => context.DoubleProp, doubleValue => doubleChanged = true);
                root.Subscribe(context => context.StringProp, stringValue => stringChanged = true);
                root.Subscribe(context => context.InnerObjectProp, 
                    innerObjectValue => innerObjectChanged = true,
                    subscriber =>
                    {
                        subscriber.Subscribe(context => context.IntProp1, intValue => int1Changed = true);
                        subscriber.Subscribe(context => context.IntProp2, intValue => int2Changed = true);
                    });
            });
        }
    }
}