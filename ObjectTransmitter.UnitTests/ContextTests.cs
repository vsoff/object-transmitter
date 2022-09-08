using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.Collectors.Collections;
using ObjectTransmitter.Reflection;
using ObjectTransmitter.UnitTests.TestClasses;
using System;

namespace ObjectTransmitter.UnitTests
{
    [TestClass]
    public class ContextTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ContextFactory _contextFactory;
        private ObjectTrasmitterContainer _container;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void Initialize()
        {
            var builder = new ObjectTrasmitterContainerBuilder();
            builder.RegisterInterface<IContextSample, ContextSample>(new ContextSampleRepeaterFactory());
            builder.RegisterInterface<IInnerObject, InnerObject>(new InnerObjectRepeaterFactory());
            var container = builder.BuildContainer();
            
            _contextFactory = new ContextFactory(container);
            _container = container;
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
            transmitter.Context.IntsMap = new TransmitterObservableDictionary<Guid, int>(_container);
            transmitter.Context.ObjectsMap = new TransmitterObservableDictionary<Guid, IInnerObject>(_container);
            Guid intItem1Key = new Guid("106d17c9-a108-44dc-a447-dc1fbe8cb9c0");
            Guid intItem2Key = new Guid("2cd122dc-6d22-4c97-a639-82a3318c84ee");
            Guid objItem1Key = new Guid("e534fa0e-a7ab-42c0-8348-af8ecbb805be");
            Guid objItem2Key = new Guid("4c4996a6-996b-4834-9160-e9202f4d2cf8");
            transmitter.Context.IntsMap.AddOrUpdate(intItem1Key, 5);
            transmitter.Context.IntsMap.AddOrUpdate(intItem2Key, 2);
            transmitter.Context.ObjectsMap.AddOrUpdate(objItem1Key, _contextFactory.CreateTransmitterPart<IInnerObject>());
            transmitter.Context.ObjectsMap.AddOrUpdate(objItem2Key, _contextFactory.CreateTransmitterPart<IInnerObject>());

            // Change values.
            var changes = transmitter.CollectChanges();
            repeater.ApplyChanges(changes);
            transmitter.ClearChanges();

            // Check applyed values.
            Assert.AreEqual(transmitter.Context.IntProp, repeater.Context.IntProp);
            Assert.AreEqual(transmitter.Context.DoubleProp, repeater.Context.DoubleProp);
            Assert.AreEqual(transmitter.Context.IntsMap[intItem1Key], repeater.Context.IntsMap[intItem1Key]);
            Assert.AreEqual(transmitter.Context.IntsMap[intItem2Key], repeater.Context.IntsMap[intItem2Key]);
            Assert.AreEqual(transmitter.Context.ObjectsMap[objItem1Key].IntProp1, repeater.Context.ObjectsMap[objItem1Key].IntProp1);
            Assert.AreEqual(transmitter.Context.ObjectsMap[objItem2Key].IntProp1, repeater.Context.ObjectsMap[objItem2Key].IntProp1);

            // Change transmitter values (2).
            transmitter.Context.InnerObjectProp.IntProp1 = 1000;
            transmitter.Context.InnerObjectProp.IntProp2 = 1001;
            transmitter.Context.IntsMap[intItem1Key] = 6667;
            transmitter.Context.IntsMap[intItem2Key] = 6669;
            transmitter.Context.ObjectsMap[objItem1Key].IntProp1 = 999;
            transmitter.Context.ObjectsMap[objItem2Key].IntProp1 = 1999;

            // Change values (2).
            var changes2 = transmitter.CollectChanges();
            repeater.ApplyChanges(changes2);
            transmitter.ClearChanges();

            // Check applyed values (2).
            Assert.AreEqual(transmitter.Context.IntProp, repeater.Context.IntProp);
            Assert.AreEqual(transmitter.Context.DoubleProp, repeater.Context.DoubleProp);
            Assert.AreEqual(transmitter.Context.IntsMap[intItem1Key], repeater.Context.IntsMap[intItem1Key]);
            Assert.AreEqual(transmitter.Context.IntsMap[intItem2Key], repeater.Context.IntsMap[intItem2Key]);
            Assert.AreEqual(transmitter.Context.ObjectsMap[objItem1Key].IntProp1, repeater.Context.ObjectsMap[objItem1Key].IntProp1);
            Assert.AreEqual(transmitter.Context.ObjectsMap[objItem2Key].IntProp1, repeater.Context.ObjectsMap[objItem2Key].IntProp1);
        }
    }
}