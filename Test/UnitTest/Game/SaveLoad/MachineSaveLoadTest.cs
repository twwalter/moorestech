using System;
using System.Reflection;
using Core.Block.BlockFactory;
using Core.Block.Blocks.Machine;
using Core.Block.Blocks.Machine.Inventory;
using Core.Block.Blocks.Machine.InventoryController;
using Core.Block.RecipeConfig;
using Core.Item;
using Core.Item.Config;
using Core.Update;
using Game.Save.Json;
using Game.World.Interface.DataStore;
using NUnit.Framework;
using PlayerInventory;
using PlayerInventory.Event;
using Test.Module.TestConfig;
using World.DataStore;
using World.Event;

namespace Test.UnitTest.Game.SaveLoad
{
    public class MachineSaveLoadTest
    {
        //インベントリのあるブロックを追加した時のテスト
        //レシピやブロックが変わった時はテストコードを修正してください
        [Test]
        public void InventoryBlockTest()
        {
            //機械の追加
            var (itemStackFactory, blockFactory, worldBlockDatastore, _, assembleSaveJsonText,_) =
                CreateBlockTestModule();
            var machine = (VanillaMachine) blockFactory.Create(2, 10);
            worldBlockDatastore.AddBlock(machine, 0, 0, BlockDirection.North);


            //レシピ用のアイテムを追加
            machine.InsertItem(itemStackFactory.Create(1, 3));
            machine.InsertItem(itemStackFactory.Create(2, 1));
            //処理を開始
            GameUpdate.Update();
            //別のアイテムを追加
            machine.InsertItem(itemStackFactory.Create(5, 6));
            machine.InsertItem(itemStackFactory.Create(2, 4));

            //リフレクションで機械の状態を設定
            //機械のレシピの残り時間設定
            var vanillaMachineRunProcess = (VanillaMachineRunProcess) typeof(VanillaMachine)
                .GetField("_vanillaMachineRunProcess", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(machine);
            typeof(VanillaMachineRunProcess)
                .GetField("_remainingMillSecond", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(vanillaMachineRunProcess, 300);

            //機械のアウトプットスロットの設定
            var _vanillaMachineInventory = (VanillaMachineInventory) typeof(VanillaMachine)
                .GetField("_vanillaMachineInventory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(machine);

            var outputInventory = (VanillaMachineOutputInventory) typeof(VanillaMachineInventory)
                .GetField("_vanillaMachineOutputInventory", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(_vanillaMachineInventory);

            outputInventory.SetItem(1, itemStackFactory.Create(1, 1));
            outputInventory.SetItem(2, itemStackFactory.Create(3, 2));

            //レシピIDを取得
            var recipeId = vanillaMachineRunProcess.RecipeDataId;

            var json = assembleSaveJsonText.AssembleSaveJson();
            Console.WriteLine(json);
            //配置したブロックを削除
            worldBlockDatastore.AddBlock(blockFactory.Create(0, 0), 0, 0, BlockDirection.North);


            //ロードした時に機械の状態が正しいことを確認
            var (_, _, loadWorldBlockDatastore, _, _,loadJsonFile) = CreateBlockTestModule();
            loadJsonFile.Load(json);

            var loadMachine = (VanillaMachine) loadWorldBlockDatastore.GetBlock(0, 0);
            Console.WriteLine(machine.GetHashCode());
            Console.WriteLine(loadMachine.GetHashCode());
            //ブロックID、intIDが同じであることを確認
            Assert.AreEqual(machine.GetBlockId(), loadMachine.GetBlockId());
            Assert.AreEqual(machine.GetIntId(), loadMachine.GetIntId());


            //機械のレシピの残り時間のチェック
            var loadVanillaMachineRunProcess = (VanillaMachineRunProcess) typeof(VanillaMachine)
                .GetField("_vanillaMachineRunProcess", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(loadMachine);
            Assert.AreEqual((double) 300, (double) loadVanillaMachineRunProcess.RemainingMillSecond);
            //レシピIDのチェック
            Assert.AreEqual(recipeId, loadVanillaMachineRunProcess.RecipeDataId);
            //機械のステータスのチェック
            Assert.AreEqual(ProcessState.Processing, loadVanillaMachineRunProcess.State);


            var loadMachineInventory = (VanillaMachineInventory) typeof(VanillaMachine)
                .GetField("_vanillaMachineInventory", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(loadMachine);
            //インプットスロットのチェック
            var inputInventoryField = (VanillaMachineInputInventory) typeof(VanillaMachineInventory)
                .GetField("_vanillaMachineInputInventory", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(loadMachineInventory);
            Assert.AreEqual(itemStackFactory.Create(5, 6), inputInventoryField.InputSlot[0]);
            Assert.AreEqual(itemStackFactory.Create(2, 4), inputInventoryField.InputSlot[1]);

            //アウトプットスロットのチェック
            var outputInventoryField = (VanillaMachineOutputInventory) typeof(VanillaMachineInventory)
                .GetField("_vanillaMachineOutputInventory", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(loadMachineInventory);
            Assert.AreEqual(itemStackFactory.CreatEmpty(), outputInventoryField.OutputSlot[0]);
            Assert.AreEqual(itemStackFactory.Create(1, 1), outputInventoryField.OutputSlot[1]);
            Assert.AreEqual(itemStackFactory.Create(3, 2), outputInventoryField.OutputSlot[2]);
        }

        private (ItemStackFactory, BlockFactory, WorldBlockDatastore, PlayerInventoryDataStore, AssembleSaveJsonText,LoadJsonFile)
            CreateBlockTestModule()
        {
            var itemFactory = new ItemStackFactory(new TestItemConfig());
            var blockFactory = new BlockFactory(new AllMachineBlockConfig(),
                new VanillaIBlockTemplates(new TestMachineRecipeConfig(itemFactory), itemFactory));
            var worldBlockDatastore =
                new WorldBlockDatastore(new BlockPlaceEvent(), blockFactory, new BlockRemoveEvent());
            var playerInventoryDataStore = new PlayerInventoryDataStore(new PlayerInventoryUpdateEvent(), itemFactory);
            var assembleSaveJsonText = new AssembleSaveJsonText(playerInventoryDataStore, worldBlockDatastore);
            var loadJsonText = new LoadJsonFile(new SaveJsonFileName(""), worldBlockDatastore,playerInventoryDataStore);

            return (itemFactory, blockFactory, worldBlockDatastore, playerInventoryDataStore, assembleSaveJsonText,loadJsonText);
        }
    }
}