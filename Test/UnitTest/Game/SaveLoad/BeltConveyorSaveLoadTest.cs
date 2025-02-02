using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Block.Blocks.BeltConveyor;
using Core.Item;
using Core.Item.Config;
using NUnit.Framework;

namespace Test.UnitTest.Game.SaveLoad
{
    public class BeltConveyorSaveLoadTest
    {
        [Test]
        public void SaveLoadTest()
        {
            var belt = new VanillaBeltConveyor(1, 10, new ItemStackFactory(new TestItemConfig()), 4, 4000);
            //リフレクションで_inventoryItemsを取得
            var inventoryItemsField =
                typeof(VanillaBeltConveyor).GetField("_inventoryItems", BindingFlags.NonPublic | BindingFlags.Instance);
            var inventoryItems = (List<BeltConveyorInventoryItem>) inventoryItemsField.GetValue(belt);
            //アイテムを設定
            inventoryItems.Add(new BeltConveyorInventoryItem(1, 10, 0));
            inventoryItems.Add(new BeltConveyorInventoryItem(2, 100, 1000));
            inventoryItems.Add(new BeltConveyorInventoryItem(5, 2500, 2000));

            //セーブデータ取得
            var str = belt.GetSaveState();
            Console.WriteLine(str);
            //セーブデータをロード
            var newBelt = new VanillaBeltConveyor(1, 10, str, new ItemStackFactory(new TestItemConfig()), 4, 4000);
            var newInventoryItems = (List<BeltConveyorInventoryItem>) inventoryItemsField.GetValue(newBelt);

            //アイテムが一致するかチェック
            Assert.AreEqual(3, newInventoryItems.Count);
            Assert.AreEqual(1, newInventoryItems[0].ItemId);
            Assert.AreEqual(10, newInventoryItems[0].RemainingTime);
            Assert.AreEqual(0, newInventoryItems[0].LimitTime);
            Assert.AreEqual(2, newInventoryItems[1].ItemId);
            Assert.AreEqual(100, newInventoryItems[1].RemainingTime);
            Assert.AreEqual(1000, newInventoryItems[1].LimitTime);
            Assert.AreEqual(5, newInventoryItems[2].ItemId);
            Assert.AreEqual(2500, newInventoryItems[2].RemainingTime);
            Assert.AreEqual(2000, newInventoryItems[2].LimitTime);
        }
    }
}