using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectTransmitter.Collectors.Collections;
using ObjectTransmitter.UnitTests.TestContexts;
using System;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace ObjectTransmitter.UnitTests.Helpers
{
    public static class TransmitterAssert
    {
        public static void ApplyChangesAndAssert<T>(
               ContextTransmitter<T> transmitter,
               ContextRepeater<T> repeater,
               Action<ContextTransmitter<T>, ContextRepeater<T>> equalityAssert)
            where T : class
        {
            // Getting changes.
            var changes = transmitter.CollectChanges();
            Assert.IsTrue(!transmitter.HasChanges() && changes.ChangedNodes.Count == 0 || changes.ChangedNodes.Count > 0);
            transmitter.ClearChanges();

            // Applying changes.
            repeater.ApplyChanges(changes);

            // Asserting result.
            equalityAssert.Invoke(transmitter, repeater);
        }

        public static void AreContextEquals(
            ContextTransmitter<ISimpleContext> transmitter,
            ContextRepeater<ISimpleContext> repeater)
        {
            Assert.AreEqual(transmitter.Context.IntProp, repeater.Context.IntProp);
            Assert.AreEqual(transmitter.Context.ByteProp, repeater.Context.ByteProp);
            Assert.AreEqual(transmitter.Context.StringProp, repeater.Context.StringProp);
            Assert.AreEqual(transmitter.Context.FloatProp, repeater.Context.FloatProp, float.Epsilon);
            Assert.AreEqual(transmitter.Context.DoubleProp, repeater.Context.DoubleProp, double.Epsilon);
            Assert.AreEqual(transmitter.Context.NullableIntProp, repeater.Context.NullableIntProp);
        }

        public static void AreContextEquals(
            ContextTransmitter<IComplexContext> transmitter,
            ContextRepeater<IComplexContext> repeater)
        {
            Assert.AreEqual(transmitter.Context.Number, repeater.Context.Number);

            if (transmitter.Context.Item != null || repeater.Context.Item != null)
            {
                Assert.IsFalse(transmitter.Context.Item == null || repeater.Context.Item == null);
                Assert.AreEqual(transmitter.Context.Item.Id, repeater.Context.Item.Id);

                if (transmitter.Context.Item.Info != null || repeater.Context.Item.Info != null)
                {
                    Assert.IsFalse(transmitter.Context.Item.Info == null || repeater.Context.Item.Info == null);
                    Assert.AreEqual(transmitter.Context.Item.Info.Description, repeater.Context.Item.Info.Description);
                }
            }
        }

        public static void AreContextEquals(
            ContextTransmitter<IContextWithDictionary> transmitter,
            ContextRepeater<IContextWithDictionary> repeater)
        {
            AreDictionariesEquals(transmitter.Context.CharactersInfoById, repeater.Context.CharactersInfoById,
                (transmitterItem, repeaterItem) =>
                {
                    Assert.AreEqual(transmitterItem.Id, repeaterItem.Id);
                    Assert.AreEqual(transmitterItem.Caption, repeaterItem.Caption);
                    Assert.AreEqual(transmitterItem.Fraction, repeaterItem.Fraction);
                });

            AreDictionariesEquals(transmitter.Context.ExperienceByLevel, repeater.Context.ExperienceByLevel,
                (transmitterItem, repeaterItem) => Assert.AreEqual(transmitterItem, repeaterItem));

            AreDictionariesEquals(transmitter.Context.PlayersById, repeater.Context.PlayersById,
                (transmitterItem, repeaterItem) =>
                {
                    Assert.AreEqual(transmitterItem.Id, repeaterItem.Id);
                    Assert.AreEqual(transmitterItem.Name, repeaterItem.Name);
                    Assert.AreEqual(transmitterItem.Level, repeaterItem.Level);
                });
        }

        public static void AreDictionariesEquals<TKey, TValue>(
            IObservableDictionary<TKey, TValue> dict1,
            IObservableDictionary<TKey, TValue> dict2,
            Action<TValue, TValue> equalityAssert)
        {
            if (dict1 == null && dict2 != null || dict1 != null && dict2 == null)
                Assert.Fail("Dictionaries are not same (one of them is null)");

            if (dict1 == null && dict2 == null)
                return;

            Assert.AreEqual(dict1.Count, dict2.Count);

            foreach (var (id, dict1Item) in dict1)
            {
                Assert.IsTrue(dict2.TryGetValue(id, out var dict2Item));
                if (dict1Item == null && dict2Item == null)
                    continue;

                equalityAssert.Invoke(dict1Item, dict2Item);
            }
        }
    }
}