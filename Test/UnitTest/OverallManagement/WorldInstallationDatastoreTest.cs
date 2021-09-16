﻿using System;
using Core;
using Core.Block;
using Core.Block.Machine.util;
using Core.Util;
using industrialization.OverallManagement.DataStore;
using NUnit.Framework;

namespace industrialization_test.UnitTest.OverallManagement
{
    public class WorldBlockDatastoreTest
    {
        [Test]
        public void DataInsertAndOutputTest()
        {
            WorldBlockDatastore.ClearData();
            
            var intId = IntId.NewIntId();
            var i =  NormalMachineFactory.Create(1, intId, new NullIBlockInventory());
            WorldBlockDatastore.AddBlock(i,1,1);
            var output = WorldBlockDatastore.GetBlock(intId);
            Assert.AreEqual(intId, output.GetIntId());

        }

        [Test]
        public void RegisteredDataCoordinateFromFetchTest()
        {
            WorldBlockDatastore.ClearData();
            
            var random = new Random(131513);
            for (int i = 0; i < 10; i++)
            {
                var intId = IntId.NewIntId();
                var ins =  NormalMachineFactory.Create(1, intId, new NullIBlockInventory());

                int x = random.Next(-1000, 1000);
                int y = random.Next(-1000, 1000);
                
                WorldBlockDatastore.AddBlock(ins,x,y);
                var output = WorldBlockDatastore.GetBlock(x,y);
                Assert.AreEqual(intId, output.GetIntId());
            }
        }


        [Test]
        public void AlreadyRegisteredIntIdSecondTimeFailTest()
        {
            WorldBlockDatastore.ClearData();
            
            var intId = IntId.NewIntId();
            var i =  NormalMachineFactory.Create(1, intId, new NullIBlockInventory());
            WorldBlockDatastore.AddBlock(i,1,1);
            
            //座標だけ変えてintIDは同じ
            var i2 =  NormalMachineFactory.Create(1, intId, new NullIBlockInventory());
            Assert.False(WorldBlockDatastore.AddBlock(i2,10,10));
        }

        [Test]
        public void AlreadyCoordinateSecondTimeFailTest()
        {
            WorldBlockDatastore.ClearData();

            var i =  NormalMachineFactory.Create(1, IntId.NewIntId(), new NullIBlockInventory());
            WorldBlockDatastore.AddBlock(i,1,1);
            
            //座標だけ変えてintIDは同じ
            var i2 =  NormalMachineFactory.Create(1, IntId.NewIntId(), new NullIBlockInventory());
            Assert.False(WorldBlockDatastore.AddBlock(i2,1,1));
        }
    }
}