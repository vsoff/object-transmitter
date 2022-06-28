using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        }

        private static ObjectTrasmitterContainer CreateContainer()
        {
            var builder = new ObjectTrasmitterContainerBuilder();
            builder.RegisterInteface<IContextSample>();
            return builder.BuildContainer();
        }
    }
}