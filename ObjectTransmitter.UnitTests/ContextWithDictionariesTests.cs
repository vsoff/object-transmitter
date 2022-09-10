using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.Collectors.Collections;
using ObjectTransmitter.Reflection;
using ObjectTransmitter.Serialization;
using ObjectTransmitter.UnitTests.Helpers;
using ObjectTransmitter.UnitTests.TestContexts;
using System.Linq;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ObjectTransmitter.UnitTests
{
    [TestClass]
    public partial class ContextWithDictionariesTests
    {
        private ContextFactory _contextFactory;
        private ObjectTrasmitterContainer _container;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new ObjectTrasmitterContainerBuilder();
            builder.SetSerializer(new DefaultTransportSerializer());
            builder.RegisterInterface(new ContextWithDictionaryRepeaterFactory());
            builder.RegisterInterface(new PlayerContextRepeaterFactory());
            var container = builder.BuildContainer();

            _contextFactory = new ContextFactory(container);
            _container = container;
        }

        [TestMethod]
        public void EmptySendTest()
        {
            CreateInitializedTransmitterAndRepeater(_container, _contextFactory, out _, out _);
        }

        [TestMethod]
        public void DictionarySetNullTest()
        {
            CreateInitializedTransmitterAndRepeater(_container, _contextFactory, out var transmitter, out var repeater);
            transmitter.Context.CharactersInfoById = null;
            transmitter.Context.ExperienceByLevel = null;
            transmitter.Context.PlayersById = null;

            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
        }

        [TestMethod]
        public void DictionaryAddNullTest()
        {
            CreateInitializedTransmitterAndRepeater(_container, _contextFactory, out var transmitter, out var repeater);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            transmitter.Context.CharactersInfoById.AddOrUpdate(1, null);
            transmitter.Context.PlayersById.AddOrUpdate(1, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
        }

        [TestMethod]
        public void DictionaryRemoveTest()
        {
            CreateInitializedTransmitterAndRepeater(_container, _contextFactory, out var transmitter, out var repeater);

            foreach (var _ in Enumerable.Range(1, 3))
            {
                var characterInfo = new ShortCharacterInfo { Id = 1, Caption = "char1", Fraction = 1 };
                byte level = 44;
                var experience = 45230;
                var playerInfo = _contextFactory.CreateTransmitterPart<IPlayerContext>();
                playerInfo.Id = 3;
                playerInfo.Level = 13;
                playerInfo.Name = "Test player name";

                transmitter.Context.CharactersInfoById.AddOrUpdate(characterInfo.Id, characterInfo);
                transmitter.Context.ExperienceByLevel.AddOrUpdate(level, experience);
                transmitter.Context.PlayersById.AddOrUpdate(playerInfo.Id, playerInfo);

                TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);

                transmitter.Context.CharactersInfoById.Remove(characterInfo.Id);
                transmitter.Context.ExperienceByLevel.Remove(level);
                transmitter.Context.PlayersById.Remove(playerInfo.Id);

                TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
            }
        }

        [TestMethod]
        public void DictionaryUpdateTest()
        {
            CreateInitializedTransmitterAndRepeater(_container, _contextFactory, out var transmitter, out var repeater);

            byte level = 44;
            var experience = 45230;
            var playerInfo = _contextFactory.CreateTransmitterPart<IPlayerContext>();
            playerInfo.Id = 3;
            playerInfo.Level = 13;
            playerInfo.Name = "Test player name";

            transmitter.Context.ExperienceByLevel.AddOrUpdate(level, experience);
            transmitter.Context.PlayersById.AddOrUpdate(playerInfo.Id, playerInfo);

            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);

            foreach (var index in Enumerable.Range(1, 3))
            {
                transmitter.Context.ExperienceByLevel[level] = index * 10;
                transmitter.Context.PlayersById[playerInfo.Id].Level = index;
                transmitter.Context.PlayersById[playerInfo.Id].Name = $"name {index}";

                TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
            }
        }

        [TestMethod]
        public void FilledSendTest()
        {
            CreateInitializedTransmitterAndRepeater(_container, _contextFactory, out var transmitter, out var repeater);

            // Filling transmitter with data.
            transmitter.Context.CharactersInfoById.AddOrUpdate(1, new ShortCharacterInfo { Id = 1, Caption = "char1", Fraction = 1 });
            transmitter.Context.CharactersInfoById.AddOrUpdate(2, new ShortCharacterInfo { Id = 2, Caption = "char2", Fraction = 2 });
            transmitter.Context.ExperienceByLevel.AddOrUpdate(1, 100);
            transmitter.Context.ExperienceByLevel.AddOrUpdate(100, 540000);
            foreach (var id in Enumerable.Range(1, 5))
            {
                var context = _contextFactory.CreateTransmitterPart<IPlayerContext>();
                context.Id = id;
                context.Level = id * 2;
                context.Name = $"Name{id}";

                transmitter.Context.PlayersById.AddOrUpdate(id, context);
            }

            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);
        }

        private static void CreateInitializedTransmitterAndRepeater(
            ObjectTrasmitterContainer container,
            ContextFactory contextFactory,
            out ContextTransmitter<IContextWithDictionary> transmitter,
            out ContextRepeater<IContextWithDictionary> repeater)
        {
            transmitter = contextFactory.CreateTransmitter<IContextWithDictionary>();
            Assert.IsNotNull(transmitter);

            repeater = contextFactory.CreateRepeater<IContextWithDictionary>();
            Assert.IsNotNull(repeater);

            transmitter.Context.PlayersById = new TransmitterObservableDictionary<int, IPlayerContext>(container);
            transmitter.Context.CharactersInfoById = new TransmitterObservableDictionary<long, ShortCharacterInfo>(container);
            transmitter.Context.ExperienceByLevel = new TransmitterObservableDictionary<byte, int>(container);
            Assert.IsNotNull(transmitter.Context.PlayersById);
            Assert.IsNotNull(transmitter.Context.CharactersInfoById);
            Assert.IsNotNull(transmitter.Context.ExperienceByLevel);

            TransmitterAssert.ApplyChangesAndAssert(transmitter, repeater, TransmitterAssert.AreContextEquals);

            Assert.IsNotNull(repeater.Context.PlayersById);
            Assert.IsNotNull(repeater.Context.CharactersInfoById);
            Assert.IsNotNull(repeater.Context.ExperienceByLevel);
        }
    }
}