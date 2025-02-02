using System;
using Core.Block.BlockFactory;
using Core.Block.Blocks.PowerGenerator;
using Core.Block.Config;
using Core.Block.Config.LoadConfig.Param;
using Core.Item;
using Core.Item.Util;
using Core.Update;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server;

namespace Test.CombinedTest.Core
{
    public class PowerGeneratorTest
    {
        private const int PowerGeneratorId = 5;
        private const int FuelItem1Id = 1;
        private const int FuelItem2Id = 2;

        [Test]
        public void UseFuelTest()
        {
            var (_, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create();
            var blockFactory = serviceProvider.GetService<BlockFactory>();
            var powerGenerator = blockFactory.Create(PowerGeneratorId, 10) as VanillaPowerGenerator;
            var blockConfig = serviceProvider.GetService<IBlockConfig>();
            var generatorConfigParam = blockConfig.GetBlockConfig(PowerGeneratorId).Param as PowerGeneratorConfigParam;
            var itemStackFactory = serviceProvider.GetService<ItemStackFactory>();

            var fuelItem1 = itemStackFactory.Create(generatorConfigParam.FuelSettings[FuelItem1Id].ItemId, 1);
            var fuelItem2 = itemStackFactory.Create(generatorConfigParam.FuelSettings[FuelItem2Id].ItemId, 1);


            //燃料の燃焼時間ループする
            DateTime endTime1 = DateTime.Now.AddMilliseconds(generatorConfigParam.FuelSettings[FuelItem1Id].Time);

            //燃料を挿入
            powerGenerator.InsertItem(fuelItem1);

            //1回目のループ
            GameUpdate.Update();

            //供給電力の確認
            Assert.AreEqual(generatorConfigParam.FuelSettings[FuelItem1Id].Power, powerGenerator.OutputPower());

            //燃料の枯渇までループ
            while (endTime1.AddSeconds(0.1).CompareTo(DateTime.Now) == 1)
            {
                GameUpdate.Update();
            }

            //燃料が枯渇しているか確認
            //リフレクションで現在の燃料を取得
            var fuelItemId = (int) typeof(VanillaPowerGenerator).GetField("_fuelItemId",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(powerGenerator);
            Assert.AreEqual(ItemConst.EmptyItemId, fuelItemId);

            //燃料を2個挿入
            powerGenerator.InsertItem(fuelItem1);
            powerGenerator.InsertItem(fuelItem2);

            //燃料の1個目の枯渇までループ
            endTime1 = DateTime.Now.AddMilliseconds(generatorConfigParam.FuelSettings[FuelItem1Id].Time);
            while (endTime1.AddSeconds(0.1).CompareTo(DateTime.Now) == 1)
            {
                GameUpdate.Update();
            }

            //2個の燃料が入っていることを確認
            fuelItemId = (int) typeof(VanillaPowerGenerator).GetField("_fuelItemId",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(powerGenerator);
            Assert.AreEqual(generatorConfigParam.FuelSettings[FuelItem2Id].ItemId, fuelItemId);

            //燃料の2個目の枯渇までループ
            DateTime endTime2 = DateTime.Now.AddMilliseconds(generatorConfigParam.FuelSettings[FuelItem2Id].Time);
            while (endTime2.AddSeconds(0.1).CompareTo(DateTime.Now) == 1)
            {
                GameUpdate.Update();
            }

            //2個目の燃料が枯渇しているか確認
            fuelItemId = (int) typeof(VanillaPowerGenerator).GetField("_fuelItemId",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(powerGenerator);
            Assert.AreEqual(ItemConst.EmptyItemId, fuelItemId);
        }
    }
}