using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ObjectTransmitter.Reflection;
using ObjectTransmitter.UnitTests.TestClasses;

namespace ObjectTransmitter.UnitTests
{
    [TestClass]
    public class ReflectionTests
    {
        [TestMethod]
        public void CreateContainerTest()
        {
            var container = CreateContainer();
            Assert.IsNotNull(container);
        }

        [TestMethod]
        public void CreateTransmitterTest()
        {
            var container = CreateContainer();
            var contextFactory = new ContextFactory(container);
            var context = contextFactory.CreateTransmitter<IContextSample>();

            context.Context.IntProp = 321;
            context.Context.DoubleProp = 2.3;
            context.Context.StringProp = "sdsdada";
            context.Context.InnerObjectProp = contextFactory.CreateTransmitterPart<IInnerObject>();
            context.Context.InnerObjectProp.IntProp1 = 555;
            context.Context.InnerObjectProp.IntProp2 = 666;

            Assert.AreEqual(true, context.HasChanges());
            var changes = context.CollectChanges();
            Assert.AreNotEqual(0, changes.ChangedNodes.Count);

            context.ClearChanges();
            Assert.AreEqual(false, context.HasChanges());
        }

        private static ObjectTrasmitterContainer CreateContainer()
        {
            var builder = new ObjectTrasmitterContainerBuilder();
            builder.RegisterInteface<IContextSample>();
            builder.RegisterInteface<IInnerObject>();
            return builder.BuildContainer();
        }
    }
}