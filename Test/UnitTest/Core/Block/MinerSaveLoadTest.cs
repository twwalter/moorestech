using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Block.BlockFactory;
using Core.Block.Blocks.Miner;
using Core.Item;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server;

namespace Test.UnitTest.Core.Block
{
    public class MinerSaveLoadTest
    {
        private int MinerId = 6;
        
        [Test]
        public void SaveLoadTest()
        {
            var (_, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create();
            var blockFactory = serviceProvider.GetService<BlockFactory>();
            var itemStackFactory = serviceProvider.GetService<ItemStackFactory>();

            var originalMiner = blockFactory.Create(MinerId, 1);
            double originalRemainingMillSecond = 350;
            
            var outputSlot = 
                (List<IItemStack>)typeof(VanillaMiner).
                    GetField("_outputSlot", BindingFlags.Instance | BindingFlags.NonPublic).
                    GetValue(originalMiner);
            outputSlot[0] = itemStackFactory.Create(1,1);
            outputSlot[2] = itemStackFactory.Create(4,1);
            typeof(VanillaMiner).
                GetField("_remainingMillSecond", BindingFlags.Instance | BindingFlags.NonPublic).
                SetValue(originalMiner,originalRemainingMillSecond);

            var json = originalMiner.GetSaveState();
            Console.WriteLine(json);
            
            
            var loadedMiner = blockFactory.Load(MinerId, 1,json);
            var loadedOutputSlot = 
                (List<IItemStack>)typeof(VanillaMiner).
                    GetField("_outputSlot", BindingFlags.Instance | BindingFlags.NonPublic).
                    GetValue(loadedMiner);
            var loadedRemainingMillSecond = 
                (double)typeof(VanillaMiner).
                    GetField("_remainingMillSecond", BindingFlags.Instance | BindingFlags.NonPublic).
                    GetValue(loadedMiner);
            
            Assert.AreEqual(outputSlot[0],loadedOutputSlot[0]);
            Assert.AreEqual(outputSlot[1],loadedOutputSlot[1]);
            Assert.AreEqual(outputSlot[2],loadedOutputSlot[2]);
            Assert.AreEqual(originalRemainingMillSecond,loadedRemainingMillSecond);
        }
    }
}