using ObjectTransmitter.Collectors.Collections;
using ObjectTransmitter.Reflection;

namespace ObjectTransmitter.UnitTests.TestContexts
{
    public interface IContextWithDictionary
    {
        public IObservableDictionary<byte, int>? ExperienceByLevel { get; set; }
        public IObservableDictionary<long, ShortCharacterInfo>? CharactersInfoById { get; set; }
        public IObservableDictionary<int, IPlayerContext>? PlayersById { get; set; }
    }

    internal class RepeaterContextWithDictionary : IContextWithDictionary
    {
        public IObservableDictionary<byte, int>? ExperienceByLevel { get; set; }
        public IObservableDictionary<long, ShortCharacterInfo>? CharactersInfoById { get; set; }
        public IObservableDictionary<int, IPlayerContext>? PlayersById { get; set; }
    }

    public class ShortCharacterInfo
    {
        public long Id { get; set; }
        public string? Caption { get; set; }
        public int Fraction { get; set; }
    }

    public interface IPlayerContext
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Level { get; set; }
    }

    internal class RepeaterPlayerContext : IPlayerContext
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Level { get; set; }
    }

    internal class ContextWithDictionaryRepeaterFactory : RepeaterFactory<IContextWithDictionary, RepeaterContextWithDictionary>
    {
        public override RepeaterContextWithDictionary CreateRepeater() => new RepeaterContextWithDictionary();
    }

    internal class PlayerContextRepeaterFactory : RepeaterFactory<IPlayerContext, RepeaterPlayerContext>
    {
        public override RepeaterPlayerContext CreateRepeater() => new RepeaterPlayerContext();
    }
}