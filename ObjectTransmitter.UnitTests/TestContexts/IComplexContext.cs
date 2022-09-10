using ObjectTransmitter.Reflection;

namespace ObjectTransmitter.UnitTests.TestContexts
{
    public interface IComplexContext
    {
        public int Number { get; set; }
        public IItem? Item { get; set; }
    }

    public interface IItem
    {
        public int Id { get; set; }
        public IItemInfo? Info { get; set; }
    }

    public interface IItemInfo
    {
        public string? Description { get; set; }
    }

    internal class ComplexContext : IComplexContext
    {
        public int Number { get; set; }
        public IItem? Item { get; set; }
    }

    internal class Item : IItem
    {
        public int Id { get; set; }
        public IItemInfo? Info { get; set; }
    }

    internal class ItemInfo : IItemInfo
    {
        public string? Description { get; set; }
    }

    internal class ComplexContextRepeaterFactory : RepeaterFactory<IComplexContext, ComplexContext>
    {
        public override ComplexContext CreateRepeater() => new();
    }

    internal class ItemRepeaterFactory : RepeaterFactory<IItem, Item>
    {
        public override Item CreateRepeater() => new();
    }

    internal class ItemInfoRepeaterFactory : RepeaterFactory<IItemInfo, ItemInfo>
    {
        public override ItemInfo CreateRepeater() => new();
    }
}
